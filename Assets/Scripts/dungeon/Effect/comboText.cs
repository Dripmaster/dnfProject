using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class comboText : MonoBehaviour
{
    CanvasGroup sr;
    float c;
    RectTransform mytrans;
    float initPos = 158.33f;
    float targetPos = -158.33f;
    private void Awake()
    {
        sr = GetComponent<CanvasGroup>();
        c = sr.alpha;
        mytrans = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        c = 1;
        sr.alpha = c;
        StartCoroutine(fadeout());
    }

    IEnumerator fadeout() {
        yield return StartCoroutine(slideIn());
        do
        {
            c -= Time.deltaTime;
            sr.alpha = c;
            yield return null;
        } while (c>=0);
        mytrans.anchoredPosition = new Vector2(initPos,mytrans.anchoredPosition.y);

    }
    IEnumerator slideIn()
    {
        float currentPos = mytrans.anchoredPosition.x;
        print(currentPos);
        do
        {
            currentPos -= Time.deltaTime*1000;
            mytrans.anchoredPosition = new Vector2(currentPos,mytrans.anchoredPosition.y);
            yield return null;
        } while (currentPos >= -158);
    }
}
