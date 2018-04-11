using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class Intro : MonoBehaviour
{


    public Image face, speechBbl, pointer;
    public TextMeshProUGUI speech;

    Queue<Action> nextActions;

    FaceManager d;

    public void Init(FaceManager fm)
    {
        d = fm;
        d.OnSingleTap += Change;
        gameObject.SetActive(true);
        happy();
        nextActions = new Queue<Action>();
        nextActions.Enqueue(hello);
        nextActions.Enqueue(tapOnce);
        nextActions.Enqueue(tap);
        nextActions.Enqueue(tapTwice);
        nextActions.Enqueue(tapT);
        nextActions.Enqueue(thatsIt);
        Change();
    }

    public void Change()
    {

        if (nextActions.Count == 0)
        {
            d.OnSingleTap -= Change;
            GameObject game = GameObject.FindGameObjectWithTag("Respawn");
            if (game != null) d.updateDelegate = null;
            LeanTween.value(speechBbl.gameObject, (float val) =>
       {
           speechBbl.rectTransform.localScale = Vector3.one * val;
           speechBbl.rectTransform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(-90, 0, val));
       }, 1f, 0.3f, 0.3f).setEaseInQuart().setOnComplete(() =>
                {
                    LeanTween.cancel(face.gameObject);
                    Destroy(gameObject);
                }
                );
        }
        else
        {
            nextActions.Dequeue()();
        }
    }


    void hello()
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        speech.text = "hello hello~";

        //rescale + rotate
        LeanTween.value(speechBbl.gameObject, (float val) =>
        {
            speechBbl.rectTransform.localScale = Vector3.one * val;
            speechBbl.rectTransform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(-90, 0, val));
            cg.alpha = val;
        }, 0.3f, 1f, 0.6f).setEaseOutQuart().setOnComplete(() =>
            {
                StartCoroutine(bounce());
            });
    }

    void tapOnce()
    {

        //rescale + rotate
        LeanTween.value(speechBbl.gameObject, (float val) =>
        {
            speechBbl.rectTransform.localScale = Vector3.one * val;
            speechBbl.rectTransform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(-90, 0, val));
        }, 1f, 0.3f, 0.3f).setEaseInQuart().setOnComplete(() =>
        {
            speech.text = "tap once to look";
            LeanTween.value(speechBbl.gameObject, (float val) =>
            {

                speechBbl.rectTransform.localScale = Vector3.one * val;
                speechBbl.rectTransform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(-90, 0, val));
            }, 0.3f, 1f, 0.6f).setEaseOutQuart().setOnComplete(() =>
            {
                StartCoroutine(bounce());
            });
        });
    }
    void tap()
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        LeanTween.value(speechBbl.gameObject, (float val) =>
      {
          speechBbl.rectTransform.localScale = Vector3.one * val;
          speechBbl.rectTransform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(-90, 0, val));
          cg.alpha = val;
      }, 1f, 0f, 0.4f).setEaseOutQuart().setOnComplete(() =>
      {
          d.ChangeLook();
          cg.alpha = 0;
          pointer.gameObject.SetActive(true);
          face.gameObject.SetActive(false);
          speechBbl.gameObject.SetActive(false);
          cg.alpha = 1;
          Image sparks = pointer.transform.GetChild(0).GetComponent<Image>();
          LeanTween.value(pointer.gameObject, (float val) =>
      {
          //change width from 267.6 to 240
          //change x rotation from 0 to -17.79
          //change sparks fill from 0 to 1
          pointer.rectTransform.eulerAngles = new Vector3(val * -17.79f, 0, 0);
          pointer.rectTransform.sizeDelta = new Vector2(267.6f - 27.6f * val, pointer.rectTransform.sizeDelta.y);
          sparks.fillAmount = val;
      }, 0, 1, 0.7f).setEaseInOutQuart().setDelay(0.3f).setLoopPingPong();
      });
    }

    void tapTwice()
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        cg.alpha = 0;
        LeanTween.cancel(pointer.gameObject);
        pointer.gameObject.SetActive(false);
        face.gameObject.SetActive(true);
        speechBbl.gameObject.SetActive(true);
        speech.text = "tap twice to change things up!";
        //rescale + rotate

        LeanTween.value(speechBbl.gameObject, (float val) =>
        {
            cg.alpha = val;
            speechBbl.rectTransform.localScale = Vector3.one * val;
            speechBbl.rectTransform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(-90, 0, val));
        }, 0.3f, 1f, 0.4f).setDelay(1).setEaseOutQuart().setOnComplete(() =>
        {
            StartCoroutine(bounce());
        });

    }

    void tapT()
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        LeanTween.value(speechBbl.gameObject, (float val) =>
      {
          speechBbl.rectTransform.localScale = Vector3.one * val;
          speechBbl.rectTransform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(-90, 0, val));
          cg.alpha = val;
      }, 1f, 0, 0.5f).setEaseOutQuart().setOnComplete(() =>
      {
          cg.alpha = 0;
          pointer.gameObject.SetActive(true);
          face.gameObject.SetActive(false);
          speechBbl.gameObject.SetActive(false);

          Image sparks = pointer.transform.GetChild(0).GetComponent<Image>();
          sparks.fillAmount = 0;
          cg.alpha = 1;
          StartCoroutine(taptwice(sparks));

          d.setUpDressListener();
          d.OnDoubleTap += Change;
          d.OnSingleTap -= Change;
      });

    }

    IEnumerator taptwice(Image sparks)
    {

        while (pointer.gameObject.activeSelf == true)
        {
            LeanTween.value(pointer.gameObject, (float val) =>
        {
            pointer.rectTransform.eulerAngles = new Vector3(val * -17.79f, 0, 0);
            pointer.rectTransform.sizeDelta = new Vector2(267.6f - 27.6f * val, pointer.rectTransform.sizeDelta.y);
            sparks.fillAmount = val;
        }, 0, 1, 0.6f).setEaseInOutQuart();
            yield return new WaitForSeconds(0.6f);

            LeanTween.value(pointer.gameObject, (float val) =>
            {
                pointer.rectTransform.eulerAngles = new Vector3(val * -17.79f, 0, 0);
                pointer.rectTransform.sizeDelta = new Vector2(267.6f - 27.6f * val, pointer.rectTransform.sizeDelta.y);
                sparks.fillAmount = val * 0.7f;
            }, 1, 0.5f, 0.05f).setEaseInQuart();
            yield return new WaitForSeconds(0.05f);

            LeanTween.value(pointer.gameObject, (float val) =>
            {
                pointer.rectTransform.eulerAngles = new Vector3(val * -17.79f, 0, 0);
                pointer.rectTransform.sizeDelta = new Vector2(267.6f - 27.6f * val, pointer.rectTransform.sizeDelta.y);
                sparks.fillAmount = val * 0.7f;
            }, 0.5f, 1, 0.1f).setEaseOutQuart();
            yield return new WaitForSeconds(0.1f);

            LeanTween.value(pointer.gameObject, (float val) =>
            {
                pointer.rectTransform.eulerAngles = new Vector3(val * -17.79f, 0, 0);
                pointer.rectTransform.sizeDelta = new Vector2(267.6f - 27.6f * val, pointer.rectTransform.sizeDelta.y);
                sparks.fillAmount = val;
            }, 1, 0, 1f).setEaseInOutQuart();
            yield return new WaitForSeconds(1f);
        }
    }

    void thatsIt()
    {

        CanvasGroup cg = GetComponent<CanvasGroup>();
        cg.alpha = 0;
        LeanTween.cancel(pointer.gameObject);
        pointer.gameObject.SetActive(false);
        face.gameObject.SetActive(true);
        speechBbl.gameObject.SetActive(true);
        speech.text = "that's it; have fun! :D";
        d.OnDoubleTap -= Change;


        //rescale + rotate
        StartCoroutine(wait(cg));



    }

    IEnumerator wait(CanvasGroup cg)
    {
        yield return new WaitForSeconds(0.1f);
        transform.SetParent(GameObject.FindGameObjectWithTag("Respawn").transform, false);
        d.OnSingleTap += Change;
        d.setUpDelegates();

        LeanTween.value(speechBbl.gameObject, (float val) =>
        {
            cg.alpha = val;
            speechBbl.rectTransform.localScale = Vector3.one * val;
            speechBbl.rectTransform.eulerAngles = new Vector3(0, 0, Mathf.Lerp(-90, 0, val));
        }, 0.3f, 1f, 0.6f).setEaseOutQuart().setOnComplete(() =>
        {

            StartCoroutine(bounce());
        });

    }




    void happy()
    {
        float orig = face.rectTransform.eulerAngles.z;
        LeanTween.value(face.gameObject, (float val) =>
        {
            face.rectTransform.eulerAngles = new Vector3(0, 0, val);
        }, orig, -4.67f, 0.9f).setEaseOutSine().setOnComplete(() =>
        {
            LeanTween.value(face.gameObject, (float val) =>
            {
                face.rectTransform.eulerAngles = new Vector3(0, 0, val);
            }, -4.67f, 8.48f, 0.9f).setEaseOutSine().setLoopPingPong();
        });
    }

    IEnumerator bounce()
    {

        LeanTween.cancel(face.gameObject);
        yield return null;
        LeanTween.value(face.gameObject, (float val) =>
        {
            face.rectTransform.anchoredPosition = new Vector2(face.rectTransform.anchoredPosition.x, val);
        }, 24.5f, 70f, 0.5f).setEaseOutBack().setOnComplete(() =>
        {
            LeanTween.value(face.gameObject, (float val) =>
            {
                face.rectTransform.anchoredPosition = new Vector2(face.rectTransform.anchoredPosition.x, val);
            }, 70f, 24.5f, 0.8f).setEaseOutBounce().setOnComplete(() =>
             {
                 happy();
             });


        });

    }
}
