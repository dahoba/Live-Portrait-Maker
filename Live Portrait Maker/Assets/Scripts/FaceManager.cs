using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

using UnityEngine.SceneManagement;


//Manager runs throughout the whole lifetime of the program
//Manages 2.5D facial animation, looking mechanic, and switching character Image sprites
public class FaceManager : MonoBehaviour
{

    public Image bg;
    /*
    Summary of skin array:
	0: nose
	1: head
	2: eye
	3: eye
	4: body
	5: eyelid 
	6: blush
    7: eyelid
	 */
    public Image[] skin;

    public Image clothes, hair, bangs, lips;
    /*
 Summary of Eye arrays:
 0 eyewhite
 1 iris
 2 brow
  */
    public List<Image> leftE, rightE;

    //sprites used for blinking
    public Sprite half, blink;


    //holds references to removable clothing/wearable items
    public Dictionary<string, Image> XtraStuff;

    public event Action OnSingleTap;
    public event Action OnDoubleTap;

    //defines the maximum time between two taps to make it double tap
    private float tapThreshold = 0.3f;
    public Action updateDelegate;
    private float tapTimer = 0.0f;
    private bool tap = false;

    //defines where the character should look toward, from the from vector
    Vector2 towards, from;

    Coroutine AnimateRoutine;

    public float HorzEye, VertEye, VertNose;

    public void UnloadDressUp()
    {
        StartCoroutine(unloadDressHelper());
    }

    IEnumerator unloadDressHelper()
    {

        yield return new WaitForSeconds(0.1f);
        setUpDelegates();
        setUpListeners();
    }


    public void Start()
    {
        setUpDelegates();
        XtraStuff = new Dictionary<string, Image>(StringComparer.Ordinal);
        Application.targetFrameRate = 27;
        XtraStuff.Add("b_", bangs);
        XtraStuff.Add("bh", hair);
        XtraStuff.Add("e_", skin[2]);
        XtraStuff.Add("eb", leftE[2]);
        XtraStuff.Add("l_", lips);
        XtraStuff.Add("n_", skin[0]);
        XtraStuff.Add("bg", bg);
        XtraStuff.Add("bl", skin[6]);

        UnityEngine.Random.InitState(System.Environment.TickCount);

        from = new Vector2(UnityEngine.Random.Range(0, Screen.width),
             UnityEngine.Random.Range(0, Screen.height));
        towards = new Vector2(UnityEngine.Random.Range(0, Screen.width),
             UnityEngine.Random.Range(0, Screen.height));


        AnimateRoutine = StartCoroutine(animate());
        //if player's first time playing, begin tutorial
        if (PlayerPrefs.GetInt("intro") != 1)
        {
            Intro();
        }
        else
        {
            Destroy(transform.GetChild(2).gameObject);
            setUpListeners();
        }
    }

    IEnumerator animate()
    {
        yield return null;
        RectTransform shine = (RectTransform)bangs.transform.GetChild(0);

        while (AnimateRoutine != null)
        {
            float time = UnityEngine.Random.Range(1.4f, 4f);
            towards = new Vector2(UnityEngine.Random.Range(0, Screen.width),
             UnityEngine.Random.Range(0, Screen.height));
            Vector2 to = towards;

            for (float t = 0; t < time; t += Time.deltaTime)
            {
                float tt = t / time;
                tt = tt * tt * (3f - 2f * tt);
                towards = Vector2.Lerp(from, to, tt);
                setFaceAtAngle(towards, shine);
                yield return null;
            }

            from = to;
            if (updateDelegate != null && UnityEngine.Random.value > 0.5f)
            {
                StartCoroutine(blinkAnimate());
            }
        }
    }
    //DO NOT CALL THIS WHILE IN SHOP MODE
    IEnumerator blinkAnimate()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0f, 0.9f));
        if (updateDelegate == null) yield break;
        Sprite s = skin[2].sprite;
        if (s == blink) yield break;
        skin[2].sprite = half;
        skin[3].sprite = half;
        yield return null;
        skin[2].sprite = blink;
        skin[3].sprite = blink;
        yield return new WaitForSeconds(0.15f);
        skin[2].sprite = half;
        skin[3].sprite = half;
        yield return null;
        skin[2].sprite = s;
        skin[3].sprite = s;
    }
    IEnumerator look()
    {
        RectTransform shine = (RectTransform)bangs.transform.GetChild(0);
        float time = UnityEngine.Random.Range(0.4f, 1f);
        from = towards;

        Vector2 to =
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
          Input.mousePosition;
#elif UNITY_IOS || UNITY_ANDROID
Input.GetTouch(0).position;
#endif
        for (float t = 0; t < time; t += Time.deltaTime)
        {
            float tt = t / time;
            tt = tt * tt * (3f - 2f * tt);
            towards = Vector2.Lerp(from, to, tt);
            setFaceAtAngle(towards, shine);
            yield return null;
        }
        yield return new WaitForSeconds(0.7f);
        from = to;
        if (updateDelegate != null && UnityEngine.Random.value > 0.7f)
        {
            StartCoroutine(blinkAnimate());
        }
        AnimateRoutine = StartCoroutine(animate());
    }

    void setFaceAtAngle(Vector2 towards, RectTransform shine)
    {

        //hair: pos y from 928.7 to 940.7 to 952.7
        hair.rectTransform.anchoredPosition = new Vector2(turnRatio(towards.x, Screen.width, 12, -12), turnRatio(towards.y, Screen.height, 1123.59f, 1061.59f));
        //bangs: rot x from 9.94 to 0
        bangs.rectTransform.eulerAngles = new Vector3(turnRatio(towards.y, Screen.height, 9.94f, 0), 0, 0);
        bangs.rectTransform.anchoredPosition = new Vector2(turnRatio(towards.x, Screen.width, -12, 12), bangs.rectTransform.anchoredPosition.y);

        //change head rot x from 8.99 to 0
        skin[1].rectTransform.eulerAngles = new Vector3(turnRatio(towards.y, Screen.height, 10, 0), 0, turnRatio(towards.x, Screen.width, -0.95f, 0.95f));
        //eyeright poy -32 to -25 to -18 (+VertEye); rox 0 to 15
        rightE[0].rectTransform.anchoredPosition = new Vector2(turnRatio(towards.x, Screen.width, 169.54f + HorzEye, 175.54f + HorzEye), turnRatio(towards.y, Screen.height, -35 + VertEye, -18 + VertEye));
        rightE[0].rectTransform.eulerAngles = new Vector3(turnRatio(towards.y, Screen.height, 0, 15), turnRatio(towards.x, Screen.width, -15, 15), 0);
        //X -25.1f, 11.8f
        //Y 1.8f, 21.6f
        //RY -18.6f, 18.6f
        //left is like right, but flipped
        rightE[1].rectTransform.anchoredPosition = new Vector2(turnRatio(towards.x, Screen.width, -24.1f, 10.8f), turnRatio(towards.y, Screen.height, 3.8f, 18.6f));
        rightE[1].rectTransform.eulerAngles = new Vector3(0, turnRatio(towards.x, Screen.width, -18.6f, 18.6f), 0);
        rightE[2].rectTransform.anchoredPosition = new Vector2(rightE[2].rectTransform.anchoredPosition.x, turnRatio(towards.y, Screen.height, 99.5f, 114.6f));
        //eyeleft poy -32 to -25 to -18(+VertEye); rox 0 to 15
        leftE[0].rectTransform.anchoredPosition = new Vector2(turnRatio(towards.x, Screen.width, -180.1f - HorzEye, -174.1f - HorzEye), turnRatio(towards.y, Screen.height, -35 + VertEye, -18 + VertEye));
        leftE[0].rectTransform.eulerAngles = new Vector3(turnRatio(towards.y, Screen.height, 0, 15), turnRatio(towards.x, Screen.width, 195, 165), 0);

        leftE[1].rectTransform.anchoredPosition = new Vector2(turnRatio(towards.x, Screen.width, 10.8f, -24.1f), turnRatio(towards.y, Screen.height, 3.8f, 18.6f));
        leftE[1].rectTransform.eulerAngles = new Vector3(0, turnRatio(towards.x, Screen.width, 18.6f, -18.6f), 0);
        leftE[2].rectTransform.anchoredPosition = new Vector2(leftE[2].rectTransform.anchoredPosition.x, turnRatio(towards.y, Screen.height, 99.5f, 114.6f));

        //lips poy -300.5 to -295.5 to -290.5; rox 18 to 0
        //lips pox -10 to 10; roy -15 to 15
        lips.rectTransform.anchoredPosition = new Vector2(turnRatio(towards.x, Screen.width, -5, 5), turnRatio(towards.y, Screen.height, -310.5f, -285.5f));
        lips.rectTransform.eulerAngles = new Vector3(turnRatio(towards.y, Screen.height, 18, 0), turnRatio(towards.x, Screen.width, -15, 15), 0);

        //nose poy from -140.8f, -128.8f; rox from 0 to 16.9
        //nose pox from -7.68 to 0 to 7.68 (+vertnose); roy from -18, 18
        skin[0].rectTransform.anchoredPosition = new Vector2(turnRatio(towards.x, Screen.width, -7.68f, 7.68f), turnRatio(towards.y, Screen.height, -142.8f + VertNose, -125.8f + VertNose));
        skin[0].rectTransform.eulerAngles = new Vector3(turnRatio(towards.y, Screen.height, 0, 16.9f), turnRatio(towards.x, Screen.width, 5, -5), 0);
        shine.anchoredPosition = new Vector2(turnRatio(towards.x, Screen.width, 10, -10), shine.anchoredPosition.y);
    }

    float turnRatio(float a, float max, float min2, float max2)
    {
        return a * ((max2 - min2) / max) + min2;
    }


    public void ChangeLook()
    {
        OnSingleTap += () =>
        {
            StopCoroutine(AnimateRoutine);
            AnimateRoutine = StartCoroutine(look());
        };
    }
    void Intro()
    {
        PlayerPrefs.SetInt("intro", 1);
        Intro i = transform.GetChild(2).GetComponent<Intro>();
        i.Init(this);
    }

    public void setUpDelegates()
    {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
        updateDelegate = UpdateEditor;
#elif UNITY_IOS || UNITY_ANDROID
        updateDelegate = UpdateMobile;
#endif
    }

    void setUpListeners()
    {
        OnDoubleTap += DressUp;
        ChangeLook();
    }

    public void setUpDressListener()
    {
        OnDoubleTap += DressUp;
    }

    void removeListeners()
    {

        OnDoubleTap -= DressUp;
        OnSingleTap = null;
    }

    void DressUp()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Additive);
        updateDelegate = null;
        removeListeners();
    }
    private void Update()
    {
        if (updateDelegate != null) { updateDelegate(); }

    }
    private void OnDestroy()
    {
        OnSingleTap = null;
        OnDoubleTap = null;
    }

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
    private void UpdateEditor()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Time.time < this.tapTimer + this.tapThreshold)
            {
                if (OnDoubleTap != null) { OnDoubleTap(); }
                this.tap = false;
                return;
            }
            this.tap = true;
            this.tapTimer = Time.time;
        }
        if (this.tap == true && Time.time > this.tapTimer + this.tapThreshold)
        {
            this.tap = false;
            if (OnSingleTap != null) { OnSingleTap(); }
        }
    }
#elif UNITY_IOS || UNITY_ANDROID
    private void UpdateMobile ()
    {
        Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                if(t.tapCount == 2)
                {
                    if(OnDoubleTap != null){ OnDoubleTap();}
                }
                if(t.tapCount == 1)
                {
                    if(OnSingleTap != null) { 
                        OnSingleTap(); 
    
                        }
                }
            }
    }

#endif

    //Remove the image mapped to key - used when undoing and removing
    public void Remove(string key)
    {
        Image x;
        Debug.Log("removing " + key + "..");
        XtraStuff.TryGetValue(key, out x);
        if (x != null)
        {
            if (x.name != "clothes" && x.name != "blush")
            {
                Destroy(x.gameObject);
                //check if second
                if (XtraStuff.ContainsKey(key + "2"))
                {
                    Destroy(XtraStuff[key + "2"].gameObject);
                    XtraStuff.Remove(key + "2");
                }
                //check if hart
                else if (key == "ha")
                {
                    Transform ps = GameObject.FindGameObjectWithTag("Finish").transform.Find("hartic");
                    if (ps != null) Destroy(ps.gameObject);
                }

            }
            else
            {
                x.gameObject.SetActive(false);
            }
        }
        else if (key=="pa")
        {
            Transform ps = GameObject.FindGameObjectWithTag("Finish").transform.Find("particle_snow");
            if (ps != null) Destroy(ps.gameObject);
        }
        else if (key=="pA")
        {
            Transform ps = GameObject.FindGameObjectWithTag("Finish").transform.Find("pArticle_sparkle");
            if (ps != null) Destroy(ps.gameObject);
        }
        else if (key == "GX")
        {
            Destroy(Camera.main.gameObject.GetComponent<Glitch>());
        }
        XtraStuff.Remove(key);

    }
    public UndoInfo faceSet(Sprite newThang, itemType it)
    {
        //key is the first 2 chars. if 2nd, first 2 chars+"2"
        UndoInfo hm;
        hm.set = null;
        hm.set2 = null;
        hm.before = null;
        hm.beforeC = Color.white;
        string key = "";
        if (newThang != null)
            key = newThang.name.Substring(0, 2);

        switch (it)
        {
            case itemType.eye:
                switch (newThang.name)
                {
                    case "iris":
                        hm.set = leftE[1];
                        hm.set2 = rightE[1];
                        hm.before = hm.set.sprite;
                        break;
                    case "whites":
                        hm.set = leftE[0];
                        hm.set2 = rightE[0];
                        hm.before = hm.set.sprite;
                        break;
                    case "se4":
                    case "se5":
                    case "se1":
                    case "se6":
                        if (!XtraStuff.ContainsKey(key))
                        {

                            hm.set = newImgAt(key, new Vector2(-3f, 7.783465f),
                            leftE[2],null,1);
                            hm.set2 = newImgAt(key, new Vector2(-3f, 7.783465f),
                            rightE[2],null,1);

                        }
                        else
                        {
                            hm.set = XtraStuff[key];
                            hm.set2 = XtraStuff[key + "2"];
                            hm.before = hm.set.sprite;
                        }
                        break;
                    case "Se":
                        if (!XtraStuff.ContainsKey(key))
                        {
                            hm.set = newImgAt(key, new Vector2(9.5f, 0.5f),
                            leftE[2],null,1);
                            hm.set2 = newImgAt(key, new Vector2(-28.2f, 0.5f),
                            rightE[2],null,1);
                        }
                        else
                        {
                            hm.set = XtraStuff[key];
                            hm.set2 = XtraStuff[key + "2"];
                            hm.before = hm.set.sprite;
                        }
                        break;

                    case "xe":
                    case "ye":
                        if (!XtraStuff.ContainsKey(key))
                        {
                            hm.set = newImgAt(key, new Vector2(-4.5f, -5.4f),
                            leftE[2], leftE[1].transform,1);
                            hm.set2 = newImgAt(key, new Vector2(-4.5f, -5.4f),
                            rightE[2], rightE[1].transform,1);
                        }
                        else
                        {
                            hm.set = XtraStuff[key];
                            hm.set2 = XtraStuff[key + "2"];
                            hm.before = hm.set.sprite;
                        }
                        Material add = Resources.Load<Material>("Additive");
                        hm.set.material = add;
                        hm.set2.material = add;
                        break;

                }

                break;
            case itemType.bow:
                setHm(new Vector2(10.378f, 453f), clothes, null, clothes.transform.parent.childCount,
                key, ref hm);
                break;

            case itemType.choker:
                int index;
                if (XtraStuff.ContainsKey("bo"))
                {
                    index = XtraStuff["bo"].transform.GetSiblingIndex();
                }
                else
                {
                    index = 0;
                }
                switch (newThang.name)
                {
                    case "choker":
                        setHm(new Vector2(10.37823f, 531f), clothes, null, index, key, ref hm);
                        break;
                    case "chokerbow":
                        setHm(new Vector2(10.37823f, 12.9f), clothes, null, index, key, ref hm);
                        break;
                }
                break;
            case itemType.glasses:
                setHm(new Vector2(-7.5f, 112.5f), skin[6], null, 0,
                key, ref hm);
                break;
            case itemType.freckles:
                setHm(new Vector2(0f, -44.24f), skin[6], null, skin[0].transform.GetSiblingIndex()+1,
                key, ref hm);
                break;
            case itemType.starfreckles:
                setHm(new Vector2(0f, -68), skin[6], null, bangs.rectTransform.GetSiblingIndex(),
                key, ref hm);
                break;
            case itemType.lippiercing:
                setHm(new Vector2(0f, 32.9f), skin[6], lips.rectTransform, 4,
                key, ref hm);
                break;
            case itemType.sl1:
                setHm(new Vector2(0.55f, 11.1f), skin[6], lips.rectTransform, 0,
                key, ref hm);
                hm.set.material = Resources.Load<Material>("Additive");
                break;
            case itemType.tears:
                setHm(new Vector2(29.7f, -39.86f), rightE[2], null, rightE[0].transform.childCount,
                key, ref hm);
                break;
            case itemType.bubble:
                setHm(new Vector2(-6.151662f, -234f), skin[6], null, 0,
                key, ref hm);
                break;
            case itemType.ear:
                int index2;
                if (XtraStuff.ContainsKey("ha"))
                {
                    index2 = XtraStuff["ha"].transform.GetSiblingIndex();
                }
                else if (XtraStuff.ContainsKey("fl"))
                {
                    index2 = XtraStuff["fl"].transform.GetSiblingIndex();
                }
                else if (XtraStuff.ContainsKey("he"))
                {
                    index2 = XtraStuff["he"].transform.GetSiblingIndex();
                }
                else
                {
                    index2 = 0;
                }
                setHmTwice(new Vector2(-328.1f, 609.5f), new Vector2(329.3f, 609.5f),
               skin[6], null, index2, key, ref hm);
                break;
            case itemType.sidehorn:
                int index3;
                if (XtraStuff.ContainsKey("ha"))
                {
                    index3 = XtraStuff["ha"].transform.GetSiblingIndex();
                }
                else if (XtraStuff.ContainsKey("fl"))
                {
                    index3 = XtraStuff["fl"].transform.GetSiblingIndex();
                }
                else
                {
                    index3 = 0;
                }
                setHmTwice(new Vector2(-328.1f, 596.8f), new Vector2(329.3012f, 596.8f),
               skin[6], null, index3, key, ref hm);
                break;
            case itemType.eyepatch:
                setHm(new Vector2(-225.1f, 52.7f), rightE[2], null, 0,
                key, ref hm);
                break;
            case itemType.hdphones:
                setHmTwice(new Vector2(-238.808f, 513.5f), new Vector2(231f, 513.5f),
               skin[6], null, bangs.transform.GetSiblingIndex(), key, ref hm);
                break;
            case itemType.msk:
                int index5;
                if (XtraStuff.ContainsKey("hd"))
                {
                    index5 = XtraStuff["hd"].transform.GetSiblingIndex();
                }
                else
                {
                    index5 = bangs.transform.GetSiblingIndex();
                }
                setHmTwice(new Vector2(-173.0009f, -140f), new Vector2(162.4f, -140f),
                   skin[6], null, index5, key, ref hm);
                break;
            case itemType.scar:
                setHm(new Vector2(-7.828147f, 16.29871f), rightE[2], null, 0,
                key, ref hm);
                break;
            case itemType.unicorn:
                setHm(new Vector2(44.5f, 607.314f), skin[6], null, 0,
                key, ref hm);
                break;
            case itemType.blood2:
                setHm(new Vector2(24.1f, -30.4f), skin[6], skin[0].rectTransform, 0,
                key, ref hm);
                break;
            case itemType.blood:
                setHm(new Vector2(34f, -99.21933f), skin[6], null, 0,
                key, ref hm);
                break;
            case itemType.hairstrand:
                setHm(new Vector2(15f, 629.59f), skin[6], null, 1,
                key, ref hm);
                break;
            case itemType.hearts:
                setHm(new Vector2(6f, 585.7f), skin[6], null, 0,
                key, ref hm);
                break;
            case itemType.flower:
                int index4;
                if (XtraStuff.ContainsKey("ha"))
                {
                    index4 = XtraStuff["ha"].transform.GetSiblingIndex();
                }
                else
                {
                    index4 = 0;
                }
                setHm(new Vector2(0f, 518.5f), skin[6], null, index4,
                key, ref hm);
                break;
            case itemType.eyes:
                hm.set = skin[2];
                hm.set2 = skin[3];
                hm.before = hm.set.sprite;
                break;
            case itemType.lips:
                hm.set = lips;
                hm.before = hm.set.sprite;
                break;
            case itemType.nose:
                hm.set = skin[0];
                hm.before = hm.set.sprite;
                break;
            case itemType.clothes:
                hm.set = clothes;
                clothes.gameObject.SetActive(true);
                XtraStuff[key] = clothes;
                hm.before = hm.set.sprite;
                break;
            case itemType.bg:
                hm.set = bg;
                if (newThang == null)
                    hm.set.color=Color.clear;
                else
                {
                    hm.beforeC = hm.set.color;
                    hm.set.color = Color.white;
                }
                hm.before = hm.set.sprite;
                break;
            case itemType.brows:
                hm.set = leftE[2];
                hm.set2 = rightE[2];
                hm.before = hm.set.sprite;
                break;
            case itemType.eyelid:
                hm.set = skin[5];
                hm.set2 = skin[7];
                hm.before = hm.set.sprite;
                break;
            case itemType.hair:
                hm.set = hair;
                hm.before = hm.set.sprite;
                break;
            case itemType.bangs:
                hm.set = bangs;
                hm.before = hm.set.sprite;
                break;
            case itemType.skin:
                hm.set = skin[1];
                hm.before = hm.set.sprite;
                break;
            case itemType.blush:
                hm.set = skin[6];
                hm.set.gameObject.SetActive(true);
                XtraStuff[key] = skin[6];
                hm.before = hm.set.sprite;
                break;
            case itemType.particles:
                hm.before = newThang;
                XtraStuff[key] = null;
                break;

            case itemType.WaterScript:
                setHm(Vector2.zero, bg, bg.transform.parent, 0, "wd", ref hm);
                if (hm.set.gameObject.GetComponent<WaterScript>() == null)
                    hm.set.gameObject.AddComponent<WaterScript>();
                hm.set.sprite=null;
                hm.set.gameObject.name = "wd";
                break;
            case itemType.WaterfallScript:
                setHm(Vector2.zero, bg, bg.transform.parent, 0, "wf", ref hm);
                WaterfallScript wf = hm.set.gameObject.GetComponent<WaterfallScript>();
                if (wf == null)
                    hm.set.gameObject.AddComponent<WaterfallScript>();
                else   
                    hm.beforeC=wf.LightColor;
                hm.set.sprite=null;
                hm.set.gameObject.name = "wf";
                break;
            case itemType.glitch:
                if (!XtraStuff.ContainsKey("GX"))
                {
                    Camera.main.gameObject.AddComponent<Glitch>().colorDrift = 0.25f;
                    XtraStuff["GX"] = null;
                }
                hm.before=newThang;
                break;

        }

        if (hm.set != null)
        {
            if (hm.set != bg && it != itemType.WaterfallScript) hm.beforeC = hm.set.color;
            if (newThang != null)
            {
                hm.set.sprite = newThang;
                //we don't want to resize if bg is pattern, or if user has adjusted size of nose
                if (it != itemType.bg && it != itemType.nose)
                    hm.set.SetNativeSize();
                if (hm.set2 != null)
                {
                    hm.set2.sprite = newThang;
                    hm.set2.SetNativeSize();
                }
            }
        }
        return hm;
    }

    void setHmTwice(Vector2 placement, Vector2 placement2, Image dup, Transform parent, int index, string key, ref UndoInfo hm)
    {
        if (!XtraStuff.ContainsKey(key))
        {
            hm.set = newImgAt(key, placement, dup, parent, index);
            hm.set2 = newImgAt(key, placement2, dup, parent, index);
            hm.set.rectTransform.localRotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }
        else
        {
            hm.set = XtraStuff[key];
            hm.set2 = XtraStuff[key + "2"];
            hm.before = hm.set.sprite;
        }
    }
    void setHm(Vector2 placement, Image dup, Transform parent, int index, string key, ref UndoInfo hm)
    {
        if (!XtraStuff.ContainsKey(key))
        {
            hm.set = newImgAt(key, placement,
            dup, parent, index);
        }
        else
        {
            hm.set = XtraStuff[key];
            hm.before = hm.set.sprite;
            hm.set.rectTransform.anchoredPosition = placement;
        }
    }

    Image newImgAt(string key, Vector2 pos, Image dup, Transform parent = null, int index = 0)
    {
        Image ret = Instantiate(dup, parent == null ? dup.transform.parent : parent, false).GetComponent<Image>();
        if (index > 0)
        {
            ret.rectTransform.SetSiblingIndex(index);
        }
        ret.rectTransform.anchoredPosition = pos;
        ret.rectTransform.localRotation = Quaternion.Euler(Vector3.zero);

        if (XtraStuff.ContainsKey(key))
        {
            XtraStuff[key + "2"] = ret;
        }
        else
        {
            XtraStuff[key] = ret;


        }
        ret.gameObject.SetActive(true);
        ret.color = Color.white;
        return ret;
    }

}
