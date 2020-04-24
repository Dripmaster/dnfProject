using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TownUiManager : MonoBehaviour
{
    GameObject canvas;
    GameObject bg;
    GameObject shopBtn;
    GameObject smithyBtn;

    bool isOpen = false;

    // Start is called before the first frame update
    void Awake()
    {
        canvas = transform.Find("canvas").gameObject;
        bg = canvas.transform.Find("bg").gameObject;
        shopBtn = canvas.transform.Find("shopBtn").gameObject;
        smithyBtn = canvas.transform.Find("smithyBtn").gameObject;
    }
    // Update is called once per frame
    void Update()
    {
        if (isOpen)
        {
            Image bgImage = bg.GetComponent<Image>();
            if (bgImage.color.a < 1)
            {
                Color color = bgImage.color;
                color.a += 4 * Time.deltaTime;
                bgImage.color = color;
            }

            Image shopBtnImage = shopBtn.GetComponent<Image>();
            if (shopBtnImage.color.a < 1)
            {
                Color color = shopBtnImage.color;
                color.a += 1 * Time.deltaTime;
                shopBtnImage.color = color;
            }

            Image smithyBtnImage = smithyBtn.GetComponent<Image>();
            if (smithyBtnImage.color.a < 1)
            {
                Color color = smithyBtnImage.color;
                color.a += 1 * Time.deltaTime;
                smithyBtnImage.color = color;
            }

            RectTransform shopBtnTrans = shopBtn.GetComponent<RectTransform>();
            if (shopBtnTrans.anchoredPosition.y > 0)
            {
                Vector3 pos = shopBtnTrans.anchoredPosition;
                pos.y -= 30 * 2f * Time.deltaTime;
                shopBtnTrans.anchoredPosition = pos;
            }

            RectTransform smithyBtnTrans = smithyBtn.GetComponent<RectTransform>();
            if (smithyBtnTrans.anchoredPosition.y > 0)
            {
                Vector3 pos = smithyBtnTrans.anchoredPosition;
                pos.y -= 30 * 2f * Time.deltaTime;
                smithyBtnTrans.anchoredPosition = pos;
            }
        }
    }
    void InitImage()
    {
        Image bgImage = bg.GetComponent<Image>();
        Color bgColor = bgImage.color;
        bgColor.a = 0;
        bgImage.color = bgColor;

        Image shopBtnImage = shopBtn.GetComponent<Image>();
        Color shopBtnColor = shopBtnImage.color;
        shopBtnColor.a = 0;
        shopBtnImage.color = shopBtnColor;
        RectTransform shopBtnRectTransform = shopBtn.GetComponent<RectTransform>();
        Vector3 shopBtnPos = shopBtnRectTransform.anchoredPosition;
        shopBtnPos.y = 30;
        shopBtnRectTransform.anchoredPosition = shopBtnPos;


        Image smithyBtnImage = smithyBtn.GetComponent<Image>();
        Color smithyBtnColor = smithyBtnImage.color;
        smithyBtnColor.a = 0;
        smithyBtnImage.color = smithyBtnColor;
        RectTransform smithyBtnRectTransform = smithyBtn.GetComponent<RectTransform>();
        Vector3 smithyBtnPos = smithyBtnRectTransform.anchoredPosition;
        smithyBtnPos.y = 30;
        smithyBtnRectTransform.anchoredPosition = smithyBtnPos;
    }
    public void OpenTownUi()
    {
        InitImage();
        isOpen = true;
        canvas.SetActive(true);
    }
    public void CloseTownUi()
    {
        if (bg.GetComponent<Image>().color.a >= 1)
        {

            isOpen = false;
            canvas.SetActive(false);
        }
    }
}
