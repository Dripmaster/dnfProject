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
    float initPos = 130f;
    float targetpos = -160f;
    Text cText;
    int comboValue;
    float currentPos;
    bool isPlay;
    private void Awake()
    {
        sr = GetComponent<CanvasGroup>();
        c = sr.alpha;
        mytrans = GetComponent<RectTransform>();
        cText = GetComponentInChildren<Text>();
        comboValue = 0;
    }

    private void OnEnable()
    {
        c = 1;
        sr.alpha = c;
        isPlay = false;
        cText.text = comboValue+"";
    }
    public void setComboValue(int v) {
        comboValue = v;
        c = 1;
        sr.alpha = c;
        mytrans.anchoredPosition = new Vector2(currentPos, mytrans.anchoredPosition.y);
        cText.text = comboValue + "";
        if(!isPlay)
            StartCoroutine(fadeout());
        currentPos = initPos;
    }
    IEnumerator fadeout() {
        isPlay = true;
        yield return StartCoroutine(slideIn());
        do
        {
            if (currentPos == initPos)
            {
                yield return StartCoroutine(slideIn());
            }
            c -= Time.deltaTime / 2;
            sr.alpha = c;
            yield return null;
        } while (c >= 0);
            mytrans.anchoredPosition = new Vector2(initPos, mytrans.anchoredPosition.y);
        isPlay = false;
    }
    IEnumerator slideIn()
    {
        do
        {
            currentPos -= Time.deltaTime*1000;
            mytrans.anchoredPosition = new Vector2(currentPos,mytrans.anchoredPosition.y);
            yield return null;
        } while (currentPos >= -155);
        mytrans.anchoredPosition = new Vector2(targetpos, mytrans.anchoredPosition.y);
    }
}
