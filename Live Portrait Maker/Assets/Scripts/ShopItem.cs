using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;



public class ShopItem : MonoBehaviour
{

    // Use this for initialization
    public void SetItem(Sprite smth, itemType i,
   DressManager dm, Func<Sprite, itemType, UndoInfo> faceSet,
  bool equipped)
    {
        helper(GetComponent<Image>(), smth, i, dm, faceSet, equipped);
    }

    public void SetItem(Sprite smth, itemType i,
        DressManager dm, Func<Sprite, itemType, UndoInfo> faceSet,
  bool equipped, Color change)
    {
        Image im = GetComponent<Image>();
        helper(im, smth, i, dm, faceSet, equipped);
        im.color = change;
    }

    void helper(Image im, Sprite smth, itemType i,
      DressManager dm, Func<Sprite, itemType, UndoInfo> faceSet,
bool equipped)
    {
        ColorPicker cpa = dm.cpa;
        im.sprite = smth;
        im.color = Color.white;
        Button b = GetComponent<Button>();
        b.onClick.RemoveAllListeners();
        b.onClick.AddListener(() =>
        {

            UndoInfo fs = faceSet(smth, i);
            if (i == itemType.glitch || i == itemType.eyes || i == itemType.nose || i == itemType.eyelid)
            {
                cpa.gameObject.SetActive(false);
                GameObject g = cpa.transform.parent.GetChild(4).gameObject;
                g.SetActive(true);
                Slider one = g.transform.GetChild(0).GetComponent<Slider>();
                Slider two = g.transform.GetChild(1).GetComponent<Slider>();
                one.onValueChanged.RemoveAllListeners();
                two.onValueChanged.RemoveAllListeners();

                dm.x.onClick.AddListener(() =>
                {
                    g.SetActive(false);
                });

                if (i == itemType.eyes)
                {
                    float ratio = 10;


                    RectTransform eye1 = (RectTransform)fs.set.rectTransform.parent;
                    RectTransform eye2 = (RectTransform)fs.set2.rectTransform.parent;

                    one.value = dm.fm.HorzEye / -ratio;
                    two.value = dm.fm.VertEye / ratio;

                    float origX = dm.fm.HorzEye;
                    float origY = dm.fm.VertEye;
                    two.onValueChanged.AddListener((float val) =>
                    {
                        // eye1.anchoredPosition = new Vector2(eye1.anchoredPosition.x, -25 + ratio * val);
                        // eye2.anchoredPosition = new Vector2(eye2.anchoredPosition.x, -25 + ratio * val);
                        dm.fm.VertEye = ratio * val;
                    });
                    one.onValueChanged.AddListener((float val) =>
                    {
                        dm.fm.HorzEye = ratio * val;
                    });

                    dm.x.onClick.AddListener(() =>
                   {
                       dm.fm.VertEye = origY;
                       dm.fm.HorzEye = origX;

                   });
                }
                else if (i == itemType.nose)
                {
                    float ratio = 10;
                    float ratio2 = 20f;

                    one.transform.GetChild(2).eulerAngles = new Vector3(0, 0, 45);

                    one.value = (fs.set.rectTransform.sizeDelta.y - 304) / ratio2;
                    two.value = dm.fm.VertNose / ratio;

                    Vector2 origSize = fs.set.rectTransform.sizeDelta;
                    float VertNose = dm.fm.VertNose;

                    dm.x.onClick.AddListener(() =>
                    {
                        one.transform.GetChild(2).eulerAngles = Vector3.zero;
                    });

                    dm.x.onClick.AddListener(() =>
                    {


                        fs.set.rectTransform.sizeDelta = origSize;

                        dm.fm.VertNose = VertNose;

                    });

                    one.onValueChanged.AddListener((float val) =>
                    {
                        fs.set.rectTransform.sizeDelta = new Vector2(349 + ratio2 * val, 304 + ratio2 * val);
                    });
                    two.onValueChanged.AddListener((float val) =>
                    {
                        dm.fm.VertNose = ratio * val;
                    });
                }
                else if (i == itemType.glitch)
                {
                    float ratio = 0.4f;
                    float ratio2 = 0.2f;
                    Glitch ag = Camera.main.GetComponent<Glitch>();

                    one.value = ag.colorDrift;
                    two.value = ag.verticalJump / ratio2;

                    one.onValueChanged.AddListener((float val) =>
                    {
                        ag.colorDrift = val;
                        ag.scanLineJitter = val * ratio;
                    });
                    two.onValueChanged.AddListener((float val) =>
                    {
                        ag.verticalJump = val * ratio2;
                    });

                    dm.x.onClick.AddListener(() =>
                    {
                        Destroy(ag);
                        dm.fm.XtraStuff.Remove("GX");
                    });


                }
                else if (i == itemType.eyelid)
                {
                    float ratio = 10f;
                    float ratio2 = 14f;
                    // 20- 48

                    Image oneI = one.transform.GetChild(2).GetComponent<Image>();
                    Sprite before = oneI.sprite;
                    oneI.sprite = Resources.Load<Sprite>("random");
                    oneI.rectTransform.sizeDelta = Vector2.one * 64;
                    float origY = fs.set.rectTransform.anchoredPosition.y;
                    one.value = fs.set.rectTransform.eulerAngles.z / ratio;
                    two.value = (fs.set.rectTransform.anchoredPosition.y - 34f) / ratio2;

                    dm.x.onClick.AddListener(() =>
                    {
                        oneI.sprite = before;
                        oneI.rectTransform.sizeDelta = Vector2.one * 80.99f;
                        fs.set.rectTransform.eulerAngles = Vector3.zero;
                        fs.set2.rectTransform.eulerAngles = Vector3.zero;

                        fs.set.rectTransform.anchoredPosition = new Vector2(fs.set.rectTransform.anchoredPosition.x, origY);

                        fs.set2.rectTransform.anchoredPosition = new Vector2(fs.set2.rectTransform.anchoredPosition.x, origY);

                    });


                    one.onValueChanged.AddListener((float val) =>
                    {
                        fs.set.rectTransform.localRotation = Quaternion.Euler(0, 0, ratio * val);
                        fs.set2.rectTransform.localRotation = Quaternion.Euler(0, 0, ratio * val);
                    });
                    two.onValueChanged.AddListener((float val) =>
                    {
                        fs.set.rectTransform.anchoredPosition = new Vector2(fs.set.rectTransform.anchoredPosition.x, 34f + ratio2 * val);
                        fs.set2.rectTransform.anchoredPosition = new Vector2(fs.set2.rectTransform.anchoredPosition.x, 34f + ratio2 * val);
                    });



                }
            }
            else if (fs.set != null && fs.set.gameObject.name == "iris")
            {
                Transform left = cpa.transform.parent.GetChild(5);
                left.gameObject.SetActive(true);
                Transform right = cpa.transform.parent.GetChild(6);
                right.gameObject.SetActive(true);
                cpa.Color = fs.set.color;
                dm.x.onClick.AddListener(() =>
                {
                    left.gameObject.SetActive(false);
                    right.gameObject.SetActive(false);
                });
                cpa.gameObject.SetActive(true);
                cpa.Reset();


            }
            else
            {
                if (fs.set != null)
                {
                    if (!equipped)
                    {
                        if (i == itemType.WaterScript)
                        {
                            dm.x.onClick.AddListener(() =>
                                {
                                    dm.fm.XtraStuff.Remove("wd");
                                    Destroy(dm.fm.transform.Find("wd").gameObject);
                                });
                        }
                        else if (i == itemType.WaterfallScript)
                        {
                            dm.x.onClick.AddListener(() =>
                                {
                                    dm.fm.XtraStuff.Remove("wf");
                                    Destroy(dm.fm.transform.Find("wf").gameObject);
                                });
                        }
                    }


                    if (i == itemType.bg && fs.set.sprite == null) cpa.Color = Camera.main.backgroundColor;
                    else cpa.Color = fs.set.color;
                    cpa.Reset();
                }
                else
                {
                    if (i == itemType.particles && transform.GetChild(0).gameObject.activeSelf)
                    {
                        cpa.Color = GameObject.FindGameObjectWithTag("Finish").transform.Find(smth.name).GetComponent<ParticleSystem>().main.startColor.colorMin;
                        cpa.Reset();
                    }
                    else
                    {
                        dm.x.onClick.AddListener(() =>
                        {
                            string key = smth.name.Substring(0, 2);
                            dm.fm.Remove(key);
                        });
                    }

                }

                cpa.gameObject.SetActive(true);

            }
            dm.colorPick(fs, transform.GetChild(0).gameObject);


        });



        if (equipped)
        {

            transform.GetChild(0).gameObject.SetActive(true);

        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        if (i == itemType.WaterScript)
        {
            WaterScript wd = gameObject.AddComponent<WaterScript>();
            dm.Switch += () => { Destroy(wd); };

        }
        else if (i == itemType.WaterfallScript)
        {
            WaterfallScript wd = gameObject.AddComponent<WaterfallScript>();
            dm.Switch += () => { Destroy(wd); };


        }
        else if (i == itemType.glitch)
        {
            im.type = Image.Type.Sliced;
            dm.Switch += () => { im.type = Image.Type.Simple; };

        }
        gameObject.SetActive(true);
    }


}
