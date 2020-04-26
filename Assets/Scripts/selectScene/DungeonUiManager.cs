using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonUiManager : MonoBehaviour
{
    GameObject canvas;
    GameObject bg;
    GameObject dungeonUi;

    bool isOpen = false;
    bool doingChange = false;

    mapType mapType;

    // Start is called before the first frame update
    void Awake()
    {

        canvas = transform.Find("canvas").gameObject;
        bg = canvas.transform.Find("bg").gameObject;
        dungeonUi = canvas.transform.Find("ui").gameObject;
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
            doingChange &= bgImage.color.a < 1;

            RectTransform dungeonTrans = dungeonUi.GetComponent<RectTransform>();
            if (dungeonTrans.anchoredPosition.y > 0)
            {
                Vector3 pos = dungeonTrans.anchoredPosition;
                pos.y -= (pos.y * 5f + 100f) * Time.deltaTime;
                dungeonTrans.anchoredPosition = pos;
            }
        }
    }

    void InitImage()
    {

        doingChange = true;
        Image bgImage = bg.GetComponent<Image>();
        Color bgColor = bgImage.color;
        bgColor.a = 0;
        bgImage.color = bgColor;

        CanvasGroup dungeonGroup = dungeonUi.GetComponent<CanvasGroup>();
        RectTransform dungeonTransform = dungeonGroup.GetComponent<RectTransform>();
        Vector3 dungeonPos = dungeonTransform.anchoredPosition;
        dungeonPos.y = 720;
        dungeonTransform.anchoredPosition = dungeonPos;


        dungeonUi.transform.Find("names").transform.Find("grass").gameObject.SetActive(false);
        dungeonUi.transform.Find("names").transform.Find("grass_1").gameObject.SetActive(false);
        dungeonUi.transform.Find("names").transform.Find("fire").gameObject.SetActive(false);
        dungeonUi.transform.Find("names").transform.Find("fire_1").gameObject.SetActive(false);
        dungeonUi.transform.Find("names").transform.Find("water").gameObject.SetActive(false);
        dungeonUi.transform.Find("names").transform.Find("water_1").gameObject.SetActive(false);
        dungeonUi.transform.Find("names").transform.Find("dark").gameObject.SetActive(false);
        dungeonUi.transform.Find("names").transform.Find("glory").gameObject.SetActive(false);

        switch (mapType)
        {
            case mapType.grass:
                dungeonUi.transform.Find("names").transform.Find("grass").gameObject.SetActive(true);
                break;
            case mapType.miniGrass:
                dungeonUi.transform.Find("names").transform.Find("grass_1").gameObject.SetActive(true);
                break;
            case mapType.fire:
                dungeonUi.transform.Find("names").transform.Find("fire").gameObject.SetActive(true);
                break;
            case mapType.miniFire:
                dungeonUi.transform.Find("names").transform.Find("fire_1").gameObject.SetActive(true);
                break;
            case mapType.water:
                dungeonUi.transform.Find("names").transform.Find("water").gameObject.SetActive(true);
                break;
            case mapType.miniWater:
                dungeonUi.transform.Find("names").transform.Find("water_1").gameObject.SetActive(true);
                break;
            case mapType.dark:
                dungeonUi.transform.Find("names").transform.Find("dark").gameObject.SetActive(true);
                break;
            case mapType.glow:
                dungeonUi.transform.Find("names").transform.Find("glory").gameObject.SetActive(true);
                break;
        }
        dungeonUi.transform.Find("bestFloor").GetComponent<Text>().text = playerDataManager.instance.getMapProgress(mapType).ToString();

        if (playerDataManager.instance.getMapProgress(mapType) >= 10)
            dungeonUi.transform.Find("clear").gameObject.SetActive(true);
        else
            dungeonUi.transform.Find("clear").gameObject.SetActive(false);
    }

    public void OpenDungeonUi(mapType mapType)
    {
        this.mapType = mapType;
        canvas.SetActive(true);
        isOpen = true;
        InitImage();
    }
    public void OnClickBg()
    {
        if (!doingChange)
        {
            isOpen = false;
            canvas.SetActive(false);
        }

    }
}
