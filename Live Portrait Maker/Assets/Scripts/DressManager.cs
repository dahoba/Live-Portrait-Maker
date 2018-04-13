using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

public enum itemType
{
    brows,
    hair,
    bangs,

    eyes,
    bg,

    eye,

    lips,
    nose,

    clothes,
    blush,
    bow,
    choker,
    glasses,

    freckles,
    starfreckles,
    lippiercing,
    sl1,
    tears,
    bubble,
    eyepatch,
    hdphones,
    msk,
    scar,
    ear,
    sidehorn,
    hearts,
    blood2,
    blood,
    flower,
    hairstrand,
    unicorn,
    particles,
    WaterScript,
    WaterfallScript,
    glitch, eyelid, skin,

}

public struct UndoInfo
{
    public Image set, set2;
    public Sprite before;
    public Color beforeC;
}


//Manager runs when shop is open
//Manages how shop is implemented and how shop connects to character
public class DressManager : MonoBehaviour
{


    List<ShopItem> items;
    public Sprite[] eye, eyes, brows, lips, nose, bangs, hair, clothes, bg, xtra;

    public ColorPicker cpa;

    RectTransform actualContent;
    [HideInInspector]
    public FaceManager fm;

    bool check = false;

    UnityAction welcome;

    //Switch to a new tab in the shop
    public event Action Switch;

    public Button x;

    public void Start()
    {
        fm = GameObject.FindGameObjectWithTag("Player").GetComponent<FaceManager>();
        items = new List<ShopItem>();
        actualContent = (RectTransform)transform.GetChild(0).GetChild(1).GetChild(0).GetChild(0);
        items.Add(actualContent.GetChild(0).GetComponent<ShopItem>());

        dressUp();
    }


    private void Update()
    {
        if (check)
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButtonDown(0) &&
            !EventSystem.current.IsPointerOverGameObject())
            {
#elif UNITY_IOS || UNITY_ANDROID
       if(Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began
       && !EventSystem.current.IsPointerOverGameObject(Input.touches[0].fingerId)){
#endif
                CanvasGroup du = transform.GetChild(0).GetComponent<CanvasGroup>();
                if (du.interactable)
                {
                    TurnOffEnd(du);
                }
            }
        }
    }

    //fade off turn
    void TurnOff(CanvasGroup turn)
    {
        check = false;
        turn.interactable = false;
        turn.blocksRaycasts = false;
        LeanTween.value(gameObject, (float val) =>
        {
            turn.alpha = val;
        }, 1, 0, 0.3f).setEase(LeanTweenType.easeOutCubic);

    }

    //fade off turn, then exit shop
    public void TurnOffEnd(CanvasGroup turn)
    {
        check = false;
        turn.interactable = false;
        turn.blocksRaycasts = false;
        LeanTween.value(gameObject, (float val) =>
        {
            turn.alpha = val;
        }, 1, 0, 0.4f).setEase(LeanTweenType.easeOutCubic).setOnComplete(() =>
        {
            fm.UnloadDressUp();
            SceneManager.UnloadSceneAsync(1);
        });

    }

    //fade in turn
    void TurnOn(CanvasGroup turn)
    {
        LeanTween.value(gameObject, (float val) =>
        {
            turn.alpha = val;
        }, 0, 1, 0.25f).setEase(LeanTweenType.easeOutCubic).setOnComplete(() =>
        {
            turn.interactable = true;
            turn.blocksRaycasts = true;
            check = true;
        });
    }

    public void dressUp()
    {
        CanvasGroup du = transform.GetChild(0).GetComponent<CanvasGroup>();
        TurnOn(du);

        Button[] butts = transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponentsInChildren<Button>();
        welcome = delegate () { TurnOffWelcome(butts); };
        foreach (Button b in butts)
        {
            b.onClick.AddListener(welcome);
        }
    }

    void TurnOffWelcome(Button[] butts)
    {
        foreach (Button b in butts)
        {
            b.onClick.RemoveListener(welcome);
        }
        Destroy(transform.GetChild(0).GetChild(1).GetChild(2).gameObject);
    }

    public void colorPick(List<UndoInfo> uiArr)
    {
        CanvasGroup du = transform.GetChild(0).GetComponent<CanvasGroup>();
        CanvasGroup cp = transform.GetChild(1).GetComponent<CanvasGroup>();
        TurnOff(du);

        Color bg = Camera.main.backgroundColor;

        x.onClick.AddListener(() =>
        {
            foreach (UndoInfo ui in uiArr)
            {
                //if (ui.beforeC == null) break;
                if (ui.set != null && ui.set.gameObject.name == "bg")
                {
                    UndoChangeBg(ui, bg);
                }
                else
                {
                    UndoChanges(ui);
                }
            }
            TurnOff(cp);
            TurnOn(du);
            x.onClick.RemoveAllListeners();
        });

        LeanTween.value(gameObject, (float val) =>
        {
            cp.alpha = val;
        }, 0, 1, 0.4f).setEase(LeanTweenType.easeInCubic).setOnComplete(() =>
        {
            cp.interactable = true;
            cp.blocksRaycasts = true;
        });
    }


    public void colorPick(UndoInfo ui, GameObject equipped)
    {
        CanvasGroup du = transform.GetChild(0).GetComponent<CanvasGroup>();
        CanvasGroup cp = transform.GetChild(1).GetComponent<CanvasGroup>();
        TurnOff(du);

        Color bg = Camera.main.backgroundColor;
        x.onClick.AddListener(() =>
        {
            if (ui.set != null && ui.set.gameObject.name == "bg")
            {
                UndoChangeBg(ui, bg);
            }
            else
            {
                UndoChanges(ui);
            }
            if (equipped)
            {
                transform.GetChild(1).GetChild(3).gameObject.SetActive(false);
            }
            TurnOff(cp);
            TurnOn(du);
            x.onClick.RemoveAllListeners();
        });

        setCPAListeners(ui);
        HashSet<string> s = new HashSet<string>(new string[]{
            "hair",
            "bangs",
            "eye",
            "brow",
            "lips",
            "nose",
            "bg",
            "head"
        });
        if (equipped.activeSelf && (ui.set == null || ui.set != null && !s.Contains(ui.set.gameObject.name)))
        {
            Button remove = transform.GetChild(1).GetChild(3).GetComponent<Button>();
            remove.gameObject.SetActive(true);
            remove.onClick.AddListener(() =>
            {
                equipped.SetActive(false);
                string key = "EMPTY";
                if (ui.set != null)
                    if (ui.set.sprite != null)
                        key = ui.set.sprite.name.Substring(0, 2);
                    else
                        key = ui.set.gameObject.name.Substring(0, 2);
                else if (ui.before != null) key = ui.before.name.Substring(0, 2);
                if (key != "EMPTY")
                {
                    fm.Remove(key);
                }
                x.onClick.RemoveAllListeners();
                TurnOff(cp);
                TurnOn(du);
                remove.transform.parent.GetChild(4).gameObject.SetActive(false);
                remove.gameObject.SetActive(false);
                remove.onClick.RemoveAllListeners();

            });
        }

        LeanTween.value(gameObject, (float val) =>
        {
            cp.alpha = val;
        }, 0, 1, 0.4f).setEase(LeanTweenType.easeInCubic).setOnComplete(() =>
        {
            cp.interactable = true;
            cp.blocksRaycasts = true;
        });
    }

    void UndoChanges(UndoInfo ui)
    {

        if (ui.set != null && ui.before != null)
        {
            if (ui.set.gameObject.name == "head")
            {
                changeSkin(ui.beforeC);
            }
            else
            {
                UndoHelper(ui);
            }
        }
        else
        {
            if (ui.set != null)
            {
                if (ui.set.sprite != null)
                    fm.Remove(ui.set.sprite.name.Substring(0, 2));
                else
                {
                    if (ui.set.gameObject.name == "wf")
                        ui.set.GetComponent<WaterfallScript>().LightColor = ui.beforeC;
                    else
                        ui.set.color = ui.beforeC;
                }
            }
            else if (ui.before != null && (ui.before.name != "GX" && !ui.before.name.StartsWith("pa") && !ui.before.name.StartsWith("pA")))
            {
                string key = ui.before.name.Substring(0, 2);
                fm.Remove(key);
            }
        }
    }

    void UndoHelper(UndoInfo ui)
    {
        ui.set.sprite = ui.before;
        ui.set.color = ui.beforeC;
        ui.set.SetNativeSize();
        if (ui.set2 != null)
        {
            ui.set2.sprite = ui.before;
            ui.set2.color = ui.beforeC;
            ui.set2.SetNativeSize();
        }
    }


    public void changeSkin(Color change)
    {
        int i = 0;
        fm.skin[i++].color = new Color(change.r * 1.2f, change.g * 1.2f, change.b * 1.2f);
        for (; i < fm.skin.Length; i++)
        {
            fm.skin[i].color = change;
        }
    }


    void UndoChangeBg(UndoInfo ui, Color bg)
    {

        ui.set.sprite = ui.before;
        ui.set.color = ui.beforeC;
        Camera.main.backgroundColor = bg;

    }

    void setCPAListeners(UndoInfo ui)
    {
        cpa.clearUpdateColor();

        if (ui.set != null)
        {

            if (ui.set.gameObject.name == "bg")
            {
                if (ui.set.sprite != null && ui.set.sprite.name[2] != 'p')
                {
                    cpa.UpdateColorAction += () => { ui.set.color = new Color(1, 1, 1, 1 - cpa.B); };
                }
                cpa.UpdateColorAction += () => { Camera.main.backgroundColor = cpa.Color; };
            }
            else if (ui.set.gameObject.name == "head")
            {
                cpa.UpdateColorAction += () =>
                {
                    changeSkin(cpa.Color);
                };
            }
            else if (ui.set.gameObject.name == "wf")
            {
                WaterfallScript Waterfall = ui.set.GetComponent<WaterfallScript>();
                cpa.UpdateColorAction += () =>
                {
                    Waterfall.LightColor = cpa.Color;
                };
            }

            else
            {

                cpa.UpdateColorAction += () => { ui.set.color = cpa.Color; };


                if (ui.set2 != null)
                {
                    cpa.UpdateColorAction += () => { ui.set2.color = cpa.Color; };
                }

                if (ui.set.sprite != null && ui.set.sprite.name == "hart")
                {
                    setUpParticles(xtra[19], 1);
                }

                if (ui.set.gameObject.name == "bangs")
                {
                    Image shine = ui.set.transform.GetChild(0).GetComponent<Image>();
                    cpa.UpdateColorAction += () =>
                    {
                        shine.color = new Color(shine.color.r, shine.color.g, shine.color.b,
Mathf.Lerp(1, 0.35f, cpa.B));
                        ;
                    };
                }
            }
        }
        else if (ui.before != null && (ui.before.name.StartsWith("pa") || ui.before.name.StartsWith("pA")))
        {
            setUpParticles(ui.before, 0);
        }
    }


    public void setUpParticles(Sprite particle, int index, bool random = false)
    {
        if (isEquippedParticle(particle))
        {
            var main2 = GameObject.FindGameObjectWithTag("Finish").transform.Find(particle.name).GetComponent<ParticleSystem>().main;
            Color original = main2.startColor.colorMin;
            cpa.UpdateColorAction += () =>
          {
              main2.startColor = new ParticleSystem.MinMaxGradient(cpa.Color,
              new Color(cpa.Color.r, cpa.Color.g, cpa.Color.b, 0.2f));
          };
            x.onClick.AddListener(() =>
            {
                main2.startColor = new ParticleSystem.MinMaxGradient(original,
    new Color(original.r, original.g, original.b, 0.2f));
            });
            return;
        }
        GameObject go = new GameObject();

        go.transform.SetParent(GameObject.FindGameObjectWithTag("Finish").transform, false);
        ParticleSystem ps = go.AddComponent<ParticleSystem>();
        go.name = particle.name;

        if (index < go.transform.parent.childCount) go.transform.SetSiblingIndex(index);
        var main = ps.main;

        if (!random)
        {
            cpa.UpdateColorAction += () =>
            {
                main.startColor = new ParticleSystem.MinMaxGradient(cpa.Color,
                new Color(cpa.Color.r, cpa.Color.g, cpa.Color.b, 0.2f));
            };
            cpa.Color = Color.white; cpa.Reset(); cpa.UpdateColor();
        }
        ParticleSystemRenderer psr = ps.GetComponent<ParticleSystemRenderer>();
        Material new_;
        ParticleSystem.EmissionModule em = ps.emission;
        ParticleSystem.ShapeModule sm = ps.shape;

        switch (particle.name)
        {

            case "pArticle_sparkle":
                new_ = Resources.Load<Material>("Star");
                //start size, start speed, shape, lifetime, emission rate, 
                go.transform.localPosition = new Vector3(0, 0, 0);

                main.startLifetime = 5;
                main.maxParticles = 15;
                main.startSpeed = new ParticleSystem.MinMaxCurve(0.05f, 0.6f);
                main.startSize = new ParticleSystem.MinMaxCurve(0.2f, 0.8f);

                em.rateOverTime = new ParticleSystem.MinMaxCurve(3, 6);
                sm.angle = 15.53f;
                sm.radius = 2.8f;
                sm.radiusThickness = 0.7f;
                sm.rotation = new Vector3(-10, 0, 0);

                ParticleSystem.ColorOverLifetimeModule c = ps.colorOverLifetime;
                c.enabled = true;
                Gradient ourGradient = new Gradient();
                ourGradient.SetKeys(
                      new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(0, 0.0f), new GradientAlphaKey(1, 0.89f),
                        new GradientAlphaKey(0, 1f) }
                    );
                c.color = new ParticleSystem.MinMaxGradient(ourGradient);
                //transform pos,
                break;
            case "hartic":

                new_ = Resources.Load<Material>("Hart");

                go.transform.localPosition = new Vector3(0, Screen.height / 3.5f, 0);
                main.startLifetime = 3;
                main.maxParticles = 8;
                main.startSpeed = new ParticleSystem.MinMaxCurve(0.1f, 0.4f);
                main.startSize = new ParticleSystem.MinMaxCurve(0.3f, 0.9f);
                main.startRotation = new ParticleSystem.MinMaxCurve(-0.45f, 0.45f);
                em.rateOverTime = 3;

                sm.shapeType = ParticleSystemShapeType.SingleSidedEdge;
                sm.radius = 2.473689f;

                ParticleSystem.ColorOverLifetimeModule cc = ps.colorOverLifetime;
                cc.enabled = true;
                Gradient ourGradient2 = new Gradient();
                ourGradient2.SetKeys(
                      new GradientColorKey[] { new GradientColorKey(Color.white, 0.0f) },
                    new GradientAlphaKey[] { new GradientAlphaKey(0, 0.4f), new GradientAlphaKey(1, 0.89f),
                        new GradientAlphaKey(0, 1f) }
                    );
                cc.color = new ParticleSystem.MinMaxGradient(ourGradient2);
                ParticleSystem.SizeOverLifetimeModule sol = ps.sizeOverLifetime;
                sol.enabled = true;

                break;

            case "particle_snow":
                new_ = Resources.Load<Material>("Snow");
                go.transform.localPosition = new Vector3(0, Screen.height / 2 + 10, 0);
                sm.shapeType = ParticleSystemShapeType.BoxEdge;
                sm.scale = new Vector3(4.5f, 0.045964f, 1);

                main.gravityModifier = 0.01f;
                main.startSize = new ParticleSystem.MinMaxCurve(UnityEngine.Random.Range(0.2f, 0.4f),
                UnityEngine.Random.Range(0.45f, 0.6f));

                main.startLifetime = 15.5f;
                main.maxParticles = 40;
                main.startSpeed = new ParticleSystem.MinMaxCurve(0.1f, 2);


                em.rateOverTime = 3;

                ParticleSystem.VelocityOverLifetimeModule vl = ps.velocityOverLifetime;
                vl.enabled = true;
                vl.x = new ParticleSystem.MinMaxCurve(-0.2f, 0.2f);
                break;
            default:
                new_ = new Material(Shader.Find("Particles/Alpha Blended"));
                break;

        }
        new_.mainTexture = particle.texture;
        psr.material = new_;
    }

    //helper function for setting up shop tabs
    void DeactivateExtra(int i)
    {
        for (; i < items.Count; i++)
        {
            items[i].gameObject.SetActive(false);
        }
    }
    //helper function for setting up shop tabs
    bool isEquipped(Sprite r)
    {
        string key = r.name.Substring(0, 2);
        return fm.XtraStuff.ContainsKey(key) && (fm.XtraStuff[key].sprite == r);
    }
    //helper function for setting up shop tabs
    bool isEquipped(string key)
    {
        return fm.XtraStuff.ContainsKey(key);
    }
    //helper function for setting up shop tabs
    bool isEquippedParticle(Sprite r)
    {
        string key = r.name.Substring(0, 2);
        return fm.XtraStuff.ContainsKey(key) && GameObject.FindGameObjectWithTag("Finish").transform.Find(r.name) != null;
    }
    //helper function for setting up shop tabs
    void button_helper(Sprite[] arr, itemType it)
    {
        ShopItem next = items[0];
        int i = 0;
        for (; i < arr.Length; i++)
        {
            next = getnext(i, next);
            next.SetItem(arr[i], it, this, fm.faceSet, isEquipped(arr[i]));

        }
        DeactivateExtra(i);
    }
    //helper function for setting up shop tabs
    void button_helper(Sprite[] arr, itemType it, int i, int len)
    {
        ShopItem next = items[0];
        for (; i < len; i++)
        {
            next = getnext(i, next);
            next.SetItem(arr[i], it, this, fm.faceSet, isEquipped(arr[i]));
        }
    }
    //helper function for setting up shop tabs
    void button_helper(Sprite[] arr, itemType it, int i, int start, int len)
    {
        int diff = i + (len - start);
        ShopItem next = items[0];
        for (; i < diff; i++, start++)
        {

            next = getnext(i, next);
            next.SetItem(arr[start], it, this, fm.faceSet, isEquipped(arr[start]));
        }
    }
    //helper function for setting up shop tabs
    void button_helper_particles(Sprite[] arr, itemType it, int i, int start, int len)
    {
        int diff = i + (len - start);
        ShopItem next = items[0];
        for (; i < diff; i++, start++)
        {

            next = getnext(i, next);
            next.SetItem(arr[start], it, this, fm.faceSet, isEquippedParticle(arr[start]));
        }
    }

    //helper function for setting up shop tabs
    void button_helper(Sprite[] arr, itemType it, int i, ShopItem next)
    {
        for (; i < arr.Length; i++)
        {
            next = getnext(i, next);
            next.SetItem(arr[i], it, this, fm.faceSet, isEquipped(arr[i]));

        }
        DeactivateExtra(i);
    }


    //helper function for setting up shop tabs
    ShopItem getnext(int i, ShopItem next)
    {
        if (items.Count > i)
        {
            next = items[i];
        }
        else
        {
            next = Instantiate(next.gameObject, next.transform.parent, false).GetComponent<ShopItem>();
            items.Add(next);
        }
        return next;
    }
    //helper function for setting up shop tabs
    void switching()
    {
        if (Switch != null)
        {
            Switch();
            Switch = null;
        }
        actualContent.anchoredPosition = new Vector2(actualContent.anchoredPosition.x, 0);
    }
    public void eye_()
    {
        switching();
        button_helper(eye, itemType.eye, 0, 2);
        button_helper(eye, itemType.eyelid, 2, 3);
        button_helper(eye, itemType.eye, 3, items[0]);

    }
    public void eyes_()
    {
        switching();
        button_helper(eyes, itemType.eyes);
    }

    public void brows_()
    {
        switching();
        button_helper(brows, itemType.brows);
    }
    public void lips_()
    {
        switching();
        button_helper(lips, itemType.lips);
        button_helper(xtra, itemType.sl1, 5, 4, 5);
    }

    public void nose_()
    {
        switching();
        button_helper(nose, itemType.nose);

    }
    public void skin_()
    {
        switching();
        ShopItem next = items[0];
        button_helper(xtra, itemType.blush, 0, 1);
        button_helper(xtra, itemType.freckles, 1, 2);
        button_helper(xtra, itemType.scar, 2, 23, 24);
        next = getnext(3, next);
        next.SetItem(null, itemType.skin, this, fm.faceSet, false, fm.skin[1].color);
        next.transform.SetAsFirstSibling();
        DeactivateExtra(4);
    }

    public void bangs_()
    {
        switching();
        button_helper(bangs, itemType.bangs);

    }
    public void hair_()
    {
        switching();
        button_helper(hair, itemType.hair);

    }

    public void clothes_()
    {
        switching();
        button_helper(clothes, itemType.clothes, 0, 4);
        button_helper(clothes, itemType.bow, 4, 5);
        button_helper(clothes, itemType.choker, 5, 7);
        button_helper(clothes, itemType.glasses, 7, items[0]);
        button_helper(xtra, itemType.eyepatch, 9, 21, 22);
        button_helper(xtra, itemType.msk, 10, 22, 23);
    }

    public void bg_()
    {
        switching();
        ShopItem next = items[0];
        button_helper(bg, itemType.bg, 0, next);

        next = getnext(bg.Length, next);
        next.SetItem(null, itemType.bg, this, fm.faceSet, false, Camera.main.backgroundColor);
        next.transform.SetAsFirstSibling();

    }
    public void xtra_()
    {
        switching();

        int index = 0;

        button_helper(xtra, itemType.starfreckles, index++, 2, 3);
        button_helper(xtra, itemType.hairstrand, index++, 14, 15);
        button_helper(xtra, itemType.bubble, index++, 6, 7);
        button_helper(xtra, itemType.hdphones, index++, 24, 25);
        button_helper(xtra, itemType.ear, index++, 7, 9);
        index++;
        button_helper(xtra, itemType.sidehorn, index++, 9, 11);
        index++;
        button_helper(xtra, itemType.unicorn, index++, 11, 12);
        button_helper(xtra, itemType.hearts, index++, 15, 16);
        button_helper(xtra, itemType.flower, index++, 16, 17);
        button_helper(xtra, itemType.lippiercing, index++, 3, 4);
        button_helper(xtra, itemType.blood2, index++, 12, 13);
        button_helper(xtra, itemType.blood, index++, 13, 14);
        button_helper(xtra, itemType.tears, index++, 5, 6);
        button_helper_particles(xtra, itemType.particles, index++, 17, 19);
        index++;

        //WaterScript deluxe
        ShopItem next = getnext(index++, items[0]);
        bool e = isEquipped("wd");
        next.SetItem(null, itemType.WaterScript, this, fm.faceSet, e, e ? fm.XtraStuff["wd"].color : Color.white);
        //WaterfallScript
        next = getnext(index++, items[0]);
        e = isEquipped("wf");
        next.SetItem(null, itemType.WaterfallScript, this, fm.faceSet, e, e ? fm.XtraStuff["wf"].GetComponent<WaterfallScript>().LightColor : Color.white);
        //glitch
        next = getnext(index++, items[0]);
        e = isEquipped("GX");
        next.SetItem(xtra[20], itemType.glitch, this, fm.faceSet, e, Color.white);



    }
}
