using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.Events;


//class created to handle randomization and undoing that randomization
public class randomize : MonoBehaviour
{

    DressManager dm;

    //persistent listener for Randomize button to randomize character
    public void random()
    {
        dm = transform.root.GetComponent<DressManager>();
        StartCoroutine(randHelper());
    }

    IEnumerator randHelper()
    {


        if (dm != null)
        {
            dm.cpa.gameObject.SetActive(false);
            int length = UnityEngine.Random.Range(0, itemType.GetNames(typeof(itemType)).Length - 10);
            length /= 2;
            HashSet<int> enums = new HashSet<int>();

            enums.Add((int)itemType.brows);
            enums.Add((int)itemType.hair);
            enums.Add((int)itemType.bangs);
            enums.Add((int)itemType.eyes);
            enums.Add((int)itemType.bg);
            for (int i = 0; i < length; i++)
            {
                enums.Add(UnityEngine.Random.Range(5, (int)itemType.particles));
            }
            enums.Add((int)itemType.skin);


            List<UndoInfo> uiArr = new List<UndoInfo>();


            //check if two items that don't go well together are both in enums; if so, randomly remove one
            if (enums.Contains((int)itemType.ear) && enums.Contains((int)itemType.sidehorn))
            {
                enums.Remove((UnityEngine.Random.value > 0.5f) ? (int)itemType.ear : (int)itemType.sidehorn);
            }
            if (enums.Contains((int)itemType.bow) && enums.Contains((int)itemType.choker))
            {
                enums.Remove((UnityEngine.Random.value > 0.5f) ? (int)itemType.bow : (int)itemType.choker);
            }
            if (enums.Contains((int)itemType.bubble) && enums.Contains((int)itemType.msk))
            {
                enums.Remove((UnityEngine.Random.value > 0.5f) ? (int)itemType.bubble : (int)itemType.msk);
            }
            if (enums.Contains((int)itemType.glasses) && enums.Contains((int)itemType.eyepatch))
            {
                enums.Remove((UnityEngine.Random.value > 0.5f) ? (int)itemType.glasses : (int)itemType.eyepatch);
            }
            if (enums.Contains((int)itemType.scar) && enums.Contains((int)itemType.glasses))
            {
                enums.Remove((UnityEngine.Random.value > 0.5f) ? (int)itemType.scar : (int)itemType.glasses);
            }
            if (enums.Contains((int)itemType.hearts) && enums.Contains((int)itemType.flower))
            {
                enums.Remove((UnityEngine.Random.value > 0.5f) ? (int)itemType.hearts : (int)itemType.flower);
            }
            if (enums.Contains((int)itemType.sidehorn) && enums.Contains((int)itemType.unicorn))
            {
                enums.Remove((UnityEngine.Random.value > 0.5f) ? (int)itemType.sidehorn : (int)itemType.unicorn);
            }
            if (enums.Contains((int)itemType.sidehorn) && enums.Contains((int)itemType.flower))
            {
                enums.Remove((UnityEngine.Random.value > 0.5f) ? (int)itemType.sidehorn : (int)itemType.flower);
            }
            if (enums.Contains((int)itemType.flower) && enums.Contains((int)itemType.ear))
            {
                enums.Remove((UnityEngine.Random.value > 0.5f) ? (int)itemType.flower : (int)itemType.ear);
            }
            if (enums.Contains((int)itemType.blood) && enums.Contains((int)itemType.blood2))
            {
                enums.Remove((UnityEngine.Random.value > 0.5f) ? (int)itemType.blood : (int)itemType.blood2);
            }
            if (enums.Contains((int)itemType.freckles) && enums.Contains((int)itemType.starfreckles))
            {
                enums.Remove((UnityEngine.Random.value > 0.5f) ? (int)itemType.freckles : (int)itemType.starfreckles);
            }


            Color bg = dm.fm.bg.color;

            HashSet<string> s = new HashSet<string>(new string[]{
           "b_","bh","e_", "eb", "l_", "n_", "bg", "t_", "bl",
        });
            Button check = dm.transform.GetChild(1).GetChild(1).GetComponent<Button>();

            foreach (var i in Enum.GetValues(typeof(itemType)))
            {
                itemType it = (itemType)i;
                Sprite newT = getRandomSprite(it);
                string key = (newT != null) ? newT.name.Substring(0, 2) : "";

                if (newT != null && !s.Contains(key) && dm.fm.XtraStuff.ContainsKey(key))
                {
                    if (dm.fm.XtraStuff[key] != null)
                    {
                        dm.fm.XtraStuff[key].gameObject.SetActive(false);
                        //make remove undoable
                        if (dm.fm.XtraStuff.ContainsKey(key + "2"))
                        {
                            dm.fm.XtraStuff[key + "2"].gameObject.SetActive(false);
                            dm.x.onClick.AddListener(() =>
                            {
                                dm.fm.XtraStuff[key].gameObject.SetActive(true);
                                dm.fm.XtraStuff[key + "2"].gameObject.SetActive(true);
                            });
                        }
                        else
                        {
                            dm.x.onClick.AddListener(() =>
                            {
                                dm.fm.XtraStuff[key].gameObject.SetActive(true);
                            });
                        }


                        check.onClick.AddListener(() =>
                        {
                            dm.fm.Remove(key);
                        });
                    }
                }
                else if (enums.Contains((int)i))
                {
                    
                    uiArr.Add(dm.fm.faceSet(newT, it));
                    //if heart..
                    if (it == itemType.hearts && GameObject.FindGameObjectWithTag("Finish").transform.Find("hart") == null) dm.setUpParticles(dm.xtra[19], 1, true);
                    // else if (it == itemType.particles) dm.setUpParticles(uiArr[count].before, 0);
                }

            }

            dm.colorPick(uiArr);
            yield return null;
            check.onClick.AddListener(() => { check.onClick.RemoveAllListeners(); });
            dm.x.onClick.AddListener(() => { check.onClick.RemoveAllListeners(); });
            RandomizeColors(uiArr, bg);

        }
    }

    //pass in Color bg to ensure color before faceSet
    void RandomizeColors(List<UndoInfo> uiArr, Color bg)
    {
        HashSet<string> s = new HashSet<string>(new string[]{
         "eyepatch","bg","iris","head", "nose",  "blush","sx","sl1", "bangs", "hair", "eye", "Se", "se1", "se4", "se5", "glasses_hart", "glasses_circle", "se6"
        });

        Color origSkin = dm.fm.skin[1].color,
       origHair = dm.fm.hair.color,
       leftI = dm.fm.leftE[1].color, rightI = dm.fm.rightE[1].color,
       bgCam = Camera.main.backgroundColor, lips = dm.fm.lips.color;

        Color32[] c = getRandColor(UnityEngine.Random.Range(0, 26));
        Color rand = c[UnityEngine.Random.Range(0, c.Length)];
        foreach (UndoInfo ui in uiArr)
        {
            if (ui.set != null && !s.Contains(ui.set.gameObject.name) && ui.set.sprite != null && !s.Contains(ui.set.sprite.name))
            {

                ui.set.color = rand;
                if (ui.set2 != null) ui.set2.color = rand;

                if (ui.set.sprite.name == "hart")
                {
                    var main = GameObject.FindGameObjectWithTag("Finish").transform.Find("hart").GetComponent<ParticleSystem>().main;
                    main.startColor = new ParticleSystem.MinMaxGradient(rand,
                    new Color(rand.r, rand.g, rand.b, 0.2f));
                }
            }
            rand = c[UnityEngine.Random.Range(0, c.Length)];
        }



        Color newSkin = c[UnityEngine.Random.Range(0, 2)];
        dm.changeSkin(newSkin);
        if (dm.fm.lips.color.r < 180 && (dm.fm.lips.color.g < 180 || dm.fm.lips.color.b < 180)
        && UnityEngine.Random.value > 0.25f
        )
        {

            dm.fm.lips.color = new Color(newSkin.r + 0.2f, newSkin.g + 0.1f, newSkin.b + 0.1f, 1);
            dm.x.onClick.AddListener(() =>
            {
                dm.fm.lips.color = lips;
            });
        }
        Color32 hair = c[UnityEngine.Random.Range(0, c.Length)];

        //change hair color to 2nd color
        dm.fm.leftE[2].color = hair;
        dm.fm.rightE[2].color = hair;
        dm.fm.bangs.color = hair;
        dm.fm.hair.color = hair;




        dm.fm.leftE[1].color = c[UnityEngine.Random.Range(0, c.Length / 2)];
        dm.fm.rightE[1].color = c[UnityEngine.Random.Range(0, c.Length / 2)];

        RandomizeBackground((UnityEngine.Random.value > 0.3f) ? c[UnityEngine.Random.Range(0, c.Length)] : new Color32(255, 255, 255, 255), c[UnityEngine.Random.Range(0, c.Length)]);

        dm.x.onClick.AddListener(() =>
        {
            dm.changeSkin(origSkin);
            dm.fm.leftE[2].color = origHair;
            dm.fm.rightE[2].color = origHair;
            dm.fm.bangs.color = origHair;
            dm.fm.hair.color = origHair;
            dm.fm.leftE[1].color = leftI;
            dm.fm.rightE[1].color = rightI;
            dm.fm.bg.color = bg;
            Camera.main.backgroundColor = bgCam;
        });



    }

    void RandomizeBackground(Color32 one, Color32 two)
    {
        Camera.main.backgroundColor = one;
        if (dm.fm.bg.sprite != null && (dm.fm.bg.sprite.name == "bgleaves" || dm.fm.bg.sprite.name == "bgsky"))
        {
            //change alpha
            dm.fm.bg.color = new Color(1, 1, 1, 1 - HSBColor.FromColor(one).b);
        }
        else
        {
            dm.fm.bg.color = two;
        }
    }
    Color32[] getRandColor(int num)
    {
        //switch to returning array based on num (swiytch case)
        switch (num)
        {
            case 0:
                return new Color32[] { new Color32(228, 240, 245, 255), new Color32(158, 153, 153, 255), new Color32(70, 62, 74, 255), new Color32(193, 212, 227, 255), new Color32(131, 153, 181, 255), new Color32(68, 79, 107, 255), };
            case 1: return new Color32[] { new Color32(176, 128, 106, 255), new Color32(241, 233, 216, 255), new Color32(208, 211, 170, 255), new Color32(176, 186, 141, 255), new Color32(238, 209, 121, 255), new Color32(108, 88, 91, 255) };
            case 2: return new Color32[] { new Color32(245, 234, 234, 255), new Color32(236, 217, 229, 255), new Color32(219, 236, 222, 255), new Color32(205, 233, 218, 255), new Color32(228, 210, 226, 255), new Color32(239, 249, 229, 255), };
            case 3: return new Color32[] { new Color32(191, 173, 154, 255), new Color32(215, 213, 216, 255), new Color32(165, 192, 170, 255), new Color32(104, 146, 128, 255), new Color32(59, 114, 104, 255), new Color32(28, 56, 52, 255), };
            case 4: return new Color32[] { new Color32(237, 234, 229, 255), new Color32(244, 226, 182, 255), new Color32(192, 223, 223, 255), new Color32(219, 99, 116, 255), new Color32(137, 151, 163, 255), new Color32(159, 203, 214, 255), };
            case 5: return new Color32[] { new Color32(235, 219, 181, 255), new Color32(197, 176, 151, 255), new Color32(224, 180, 128, 255), new Color32(65, 166, 210, 255), new Color32(183, 227, 240, 255), new Color32(219, 228, 227, 255), new Color32(65, 84, 89, 255), };
            case 6: return new Color32[] { new Color32(254, 219, 203, 255), new Color32(206, 190, 195, 255), new Color32(205, 195, 230, 255), new Color32(134, 185, 198, 255), new Color32(29, 77, 171, 255), new Color32(164, 88, 151, 255), new Color32(57, 44, 90, 255) };
            case 7: return new Color32[] { new Color32(253, 239, 239, 255), new Color32(210, 188, 171, 255), new Color32(229, 206, 214, 255), new Color32(250, 230, 235, 255), new Color32(154, 172, 130, 255), new Color32(85, 114, 86, 255), new Color32(43, 64, 61, 255) };
            case 8: return new Color32[] { new Color32(240, 216, 187, 255), new Color32(102, 76, 62, 255), new Color32(217, 143, 94, 255), new Color32(55, 55, 55, 255), new Color32(110, 182, 190, 255), new Color32(56, 114, 119, 255), };
            case 9: return new Color32[] { new Color32(219, 206, 205, 255), new Color32(236, 214, 220, 255), new Color32(206, 207, 220, 255), new Color32(174, 190, 209, 255), new Color32(106, 125, 122, 255), new Color32(207, 108, 87, 255), new Color32(75, 69, 70, 255), };
            case 10: return new Color32[] { new Color32(242, 230, 225, 255), new Color32(247, 243, 239, 255), new Color32(78, 163, 163, 255), new Color32(55, 120, 150, 255), new Color32(182, 176, 189, 255), new Color32(43, 92, 122, 255) };
            case 11: return new Color32[] { new Color32(216, 198, 159, 255), new Color32(245, 245, 245, 255), new Color32(242, 235, 167, 255), new Color32(227, 227, 227, 255), new Color32(227, 181, 29, 255), new Color32(105, 105, 86, 255), new Color32(199, 199, 201, 255) };
            case 12: return new Color32[] { new Color32(254, 230, 225, 255), new Color32(206, 158, 163, 255), new Color32(254, 183, 207, 255), new Color32(224, 109, 137, 255), new Color32(35, 43, 57, 255), new Color32(108, 252, 253, 255), new Color32(185, 238, 244, 255), };
            case 13: return new Color32[] { new Color32(254, 230, 225, 255), new Color32(206, 158, 163, 255), new Color32(254, 183, 207, 255), new Color32(224, 109, 137, 255), new Color32(35, 43, 57, 255), new Color32(108, 252, 253, 255), new Color32(185, 238, 244, 255), };
            case 14: return new Color32[] { new Color32(208, 167, 146, 255), new Color32(229, 203, 188, 255), new Color32(206, 203, 206, 255), new Color32(236, 225, 218, 255), new Color32(230, 232, 228, 255), new Color32(127, 125, 126, 255) };
            case 15: return new Color32[] { new Color32(238, 246, 235, 255), new Color32(203, 183, 142, 255), new Color32(221, 212, 155, 255), new Color32(238, 238, 206, 255), new Color32(226, 112, 131, 255), new Color32(216, 177, 214, 255), new Color32(185, 225, 238, 255) };
            case 16: return new Color32[] { new Color32(234, 229, 219, 255), new Color32(208, 167, 146, 255), new Color32(175, 205, 199, 255), new Color32(99, 153, 152, 255), new Color32(43, 62, 55, 255), new Color32(100, 40, 48, 255), new Color32(237, 73, 90, 255) };
            case 17: return new Color32[] { new Color32(201, 178, 154, 255), new Color32(128, 98, 86, 255), new Color32(235, 228, 221, 255), new Color32(69, 52, 63, 255), new Color32(86, 96, 112, 255), new Color32(188, 203, 209, 255) };
            case 18: return new Color32[] { new Color32(255, 255, 255, 255), new Color32(233, 233, 233, 255), new Color32(26, 25, 26, 255), new Color32(76, 92, 106, 255), new Color32(149, 167, 176, 255), new Color32(206, 206, 206, 255), new Color32(222, 223, 223, 255) };
            case 19: return new Color32[] { new Color32(209, 192, 175, 255), new Color32(97, 87, 83, 255), new Color32(227, 223, 218, 255), new Color32(120, 104, 120, 255), new Color32(170, 191, 188, 255), new Color32(211, 216, 232, 255) };
            case 20: return new Color32[] { new Color32(182, 168, 155, 255), new Color32(244, 243, 241, 255), new Color32(236, 227, 227, 255), new Color32(212, 193, 196, 255), new Color32(42, 43, 44, 255), new Color32(225, 223, 211, 255) };
            case 21: return new Color32[] { new Color32(186, 181, 181, 255), new Color32(231, 227, 222, 255), new Color32(208, 206, 199, 255), new Color32(249, 249, 245, 255), new Color32(242, 242, 235, 255), new Color32(233, 234, 238, 255) };
            case 22: return new Color32[] { new Color32(168, 151, 143, 255), new Color32(102, 76, 77, 255), new Color32(37, 31, 48, 255), new Color32(202, 202, 204, 255), new Color32(227, 227, 232, 255), new Color32(125, 44, 56, 255) };
            case 23: return new Color32[] { new Color32(168, 151, 143, 255), new Color32(102, 76, 77, 255), new Color32(37, 31, 48, 255), new Color32(202, 202, 204, 255), new Color32(227, 227, 232, 255), new Color32(125, 44, 56, 255) };
            case 24: return new Color32[] { new Color32(168, 151, 143, 255), new Color32(100, 73, 71, 255), new Color32(178, 172, 226, 255), new Color32(138, 116, 182, 255), new Color32(83, 58, 90, 255), new Color32(47, 47, 47, 255), };

        }
        return new Color32[] { new Color32(247, 232, 228, 255), new Color32(113, 107, 110, 255), new Color32(253, 254, 240, 255), new Color32(130, 144, 157, 255), new Color32(181, 193, 204, 255), new Color32(226, 229, 234, 255) };

    }

    Sprite getRandomSprite(itemType it)
    {
        switch (it)
        {
            case itemType.eye:
                return dm.eye[UnityEngine.Random.Range(0, dm.eye.Length)];
            case itemType.bow:
                return dm.clothes[4];
            case itemType.choker:
                return dm.clothes[UnityEngine.Random.Range(5, 7)];
            case itemType.glasses:
                return dm.clothes[UnityEngine.Random.Range(7, dm.clothes.Length)];
            case itemType.freckles:
                return dm.xtra[1];
            case itemType.starfreckles:
                return dm.xtra[2];
            case itemType.lippiercing:
                return dm.xtra[3];
            case itemType.sl1:
                return dm.xtra[4];
            case itemType.tears:
                return dm.xtra[5];
            case itemType.bubble:
                return dm.xtra[6];
            case itemType.ear:
                return dm.xtra[UnityEngine.Random.Range(7, 9)];
            case itemType.sidehorn:
                return dm.xtra[UnityEngine.Random.Range(9, 11)];
            case itemType.unicorn:
                return dm.xtra[11];
            case itemType.blood2:
                return dm.xtra[12];
            case itemType.blood:
                return dm.xtra[13];
            case itemType.hairstrand:
                return dm.xtra[14];
            case itemType.hearts:
                return dm.xtra[15];
            case itemType.flower:
                return dm.xtra[16];
            case itemType.eyes:
                return dm.eyes[UnityEngine.Random.Range(0, dm.eyes.Length)];
            case itemType.lips:
                return dm.lips[UnityEngine.Random.Range(0, dm.lips.Length)];
            case itemType.nose:
                return dm.nose[UnityEngine.Random.Range(0, dm.nose.Length)];
            case itemType.clothes:
                return dm.clothes[UnityEngine.Random.Range(0, 4)];
            case itemType.bg:
                int rand = UnityEngine.Random.Range(0, dm.bg.Length + 1);
                if (rand == dm.bg.Length) return null;
                return dm.bg[rand];
            case itemType.brows:
                return dm.brows[UnityEngine.Random.Range(0, dm.brows.Length)];
            case itemType.hair:
                return dm.hair[UnityEngine.Random.Range(0, dm.hair.Length)];
            case itemType.bangs:
                return dm.bangs[UnityEngine.Random.Range(0, dm.bangs.Length)];
            case itemType.eyepatch:
                return dm.xtra[21];
            case itemType.hdphones:
                return dm.xtra[24];
            case itemType.msk:
                return dm.xtra[22];
            case itemType.scar:
                return dm.xtra[23];
            case itemType.blush:
                return dm.xtra[0];
            case itemType.particles:
                return dm.xtra[UnityEngine.Random.Range(17, 19)];

        }
        return null;
    }
}
