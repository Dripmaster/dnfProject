using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum UiType
{
    TOWN = 0,
    SHOP,
    BUILD,
}

public class TownUiManager : MonoBehaviour
{
    GameObject canvas;
    GameObject bg;
    GameObject townButton;
    GameObject townBuild;

    UiType uiType = UiType.TOWN;

    bool isOpen = false;
    bool doingChange = false;

    [HideInInspector]
    public item selectWeapon;
    [HideInInspector]
    public item selectMaterial;

    [HideInInspector]
    public bool isSelectWeapon;
    [HideInInspector]
    public bool isSelectMaterial;


    // Start is called before the first frame update
    void Awake()
    {
        canvas = transform.Find("canvas").gameObject;
        bg = canvas.transform.Find("bg").gameObject;
        townButton = canvas.transform.Find("button").gameObject;
        townBuild = canvas.transform.Find("build").gameObject;
    }
    // Update is called once per frame
    void Update()
    {
        if (isOpen)
        {
            switch (uiType)
            {
                case UiType.TOWN:
                    Image bgImage = bg.GetComponent<Image>();
                    if (bgImage.color.a < 1)
                    {
                        Color color = bgImage.color;
                        color.a += 4 * Time.deltaTime;
                        bgImage.color = color;
                    }
                    doingChange &= bgImage.color.a < 1;

                    CanvasGroup buttonUi = townButton.GetComponent<CanvasGroup>();
                    if (buttonUi.alpha < 1)
                    {
                        buttonUi.alpha += 1 * Time.deltaTime;
                    }

                    RectTransform buttonTrans = townButton.GetComponent<RectTransform>();
                    if (buttonTrans.anchoredPosition.y > 0)
                    {
                        Vector3 pos = buttonTrans.anchoredPosition;
                        pos.y -= 30 * 2f * Time.deltaTime;
                        buttonTrans.anchoredPosition = pos;
                    }
                    break;
                case UiType.BUILD:
                    CanvasGroup buildUi = townBuild.GetComponent<CanvasGroup>();
                    if (buildUi.alpha < 1)
                    {
                        buildUi.alpha += 3 * Time.deltaTime;
                    }
                    doingChange &= buildUi.alpha < 1;

                    RectTransform buildTrans = townBuild.GetComponent<RectTransform>();
                    if (buildTrans.anchoredPosition.y > 0)
                    {
                        Vector3 pos = buildTrans.anchoredPosition;
                        pos.y -= 20 * 4 * Time.deltaTime;
                        buildTrans.anchoredPosition = pos;
                    }
                    break;
            }
        }
    }
    void InitImage()
    {

        townButton.gameObject.SetActive(false);
        townBuild.gameObject.SetActive(false);
        doingChange = true;

        switch (uiType)
        {
            case UiType.TOWN:
                townButton.gameObject.SetActive(true);

                Image bgImage = bg.GetComponent<Image>();
                Color bgColor = bgImage.color;
                bgColor.a = 0;
                bgImage.color = bgColor;

                CanvasGroup buttonUi = townButton.GetComponent<CanvasGroup>();
                buttonUi.alpha = 0;
                RectTransform buttonTransform = townButton.GetComponent<RectTransform>();
                Vector3 buttonPos = buttonTransform.anchoredPosition;
                buttonPos.y = 30;
                buttonTransform.anchoredPosition = buttonPos;

                break;
            case UiType.SHOP:
                break;
            case UiType.BUILD:
                townBuild.gameObject.SetActive(true);

                CanvasGroup buildUi = townBuild.GetComponent<CanvasGroup>();
                buildUi.alpha = 0.2f;
                RectTransform buildTransform = townBuild.GetComponent<RectTransform>();
                Vector3 buildPos = buildTransform.anchoredPosition;
                buildPos.y = 20;
                buildTransform.anchoredPosition = buildPos;

                break;
        }

    }
    public void OpenTownUi()
    {
        canvas.SetActive(true);
        uiType = UiType.TOWN;
        isOpen = true;
        InitImage();
    }
    public void OnClickBg()
    {
        if (!doingChange)
        {
            switch (uiType)
            {
                case UiType.TOWN:
                    isOpen = false;
                    canvas.SetActive(false);

                    break;
                case UiType.SHOP:
                    break;
                case UiType.BUILD:
                    uiType = UiType.TOWN;
                    townButton.SetActive(true);
                    townBuild.SetActive(false);
                    ClearSelect();
                    break;
            }
        }
    }
    public void ClearSelect()
    {
        selectWeapon = null;
        selectMaterial = null;
        isSelectWeapon = false;
        isSelectMaterial = false;
        townBuild.transform.Find("weaponBtn").transform.Find("Image").GetComponent<Image>().sprite = null;
        townBuild.transform.Find("weaponBtn").transform.Find("Image").gameObject.SetActive(false);
        townBuild.transform.Find("materialBtn").transform.Find("Image").GetComponent<Image>().sprite = null;
        townBuild.transform.Find("materialBtn").transform.Find("Image").gameObject.SetActive(false);
    }
    public void ChangeType(int type)
    {
        if (!doingChange)
        {

            uiType = (UiType)type;
            InitImage();
        }
    }
    public void OpenTownInven(int type)
    {
        GameObject.Find("invenUi").GetComponent<Inven>().OpenInven((item, sprite) =>
        {
            GameObject selectItem;
            if (type == 1)
            {
                selectWeapon = item;
                isSelectWeapon = true;
                selectItem = townBuild.transform.Find("weaponBtn").gameObject;
            }
            else
            {
                selectMaterial = item;
                isSelectMaterial = true;
                selectItem = townBuild.transform.Find("materialBtn").gameObject;
            }
            selectItem.transform.Find("Image").GetComponent<Image>().sprite = sprite;
            selectItem.transform.Find("Image").gameObject.SetActive(true);

        }, type);
    }
}
