using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonUiManager : MonoBehaviour
{
    GameObject canvas;
    GameObject bg;
    GameObject dungeonUi;
    Text potion1;
    Text potion2;

    bool isOpen = false;
    bool doingChange = false;

    mapType selectMapType;
    item selectEquip;

    // Start is called before the first frame update
    void Awake()
    {
        canvas = transform.Find("canvas").gameObject;
        bg = canvas.transform.Find("bg").gameObject;
        dungeonUi = canvas.transform.Find("ui").gameObject;
        potion1 = transform.Find("canvas/ui/potion1/Text").GetComponent<Text>();
        potion2 = transform.Find("canvas/ui/potion2/Text").GetComponent<Text>();

        if (playerDataManager.instance.getEquip() == null)
        {
            foreach (item a in playerDataManager.instance.getInventory().playerInventory)
            {
                if (a.type >= (int)itemType.sword)
                {
                    selectEquip = a;
                    playerDataManager.instance.setEquip(a);
                    break;
                }
            }
        }
        else {
            selectEquip = playerDataManager.instance.getEquip();
        }
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

        switch (selectMapType)
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
        dungeonUi.transform.Find("bestFloor").GetComponent<Text>().text = playerDataManager.instance.getMapProgress(selectMapType).ToString();

        if (playerDataManager.instance.getMapProgress(selectMapType) >= 10)
            dungeonUi.transform.Find("clear").gameObject.SetActive(true);
        else
            dungeonUi.transform.Find("clear").gameObject.SetActive(false);



        dungeonUi.transform.Find("sword").gameObject.SetActive(false);
        dungeonUi.transform.Find("bigsword").gameObject.SetActive(false);
        dungeonUi.transform.Find("hammer").gameObject.SetActive(false);
        if (playerDataManager.instance.getEquip().type == (int)itemType.sword)
        {
            dungeonUi.transform.Find("selectEquip").transform.Find("sword").gameObject.SetActive(true);
            dungeonUi.transform.Find("sword").gameObject.SetActive(true);
        }
        if (playerDataManager.instance.getEquip().type == (int)itemType.hammer)
        {
            dungeonUi.transform.Find("selectEquip").transform.Find("hammer").gameObject.SetActive(true);
            dungeonUi.transform.Find("hammer").gameObject.SetActive(true);
        }
        if (playerDataManager.instance.getEquip().type == (int)itemType.bigSword)
        {
            dungeonUi.transform.Find("selectEquip").transform.Find("bigsword").gameObject.SetActive(true);
            dungeonUi.transform.Find("bigsword").gameObject.SetActive(true);
        }
        potion1.text = playerDataManager.instance.getItemCount(itemType.healPotion).ToString();
        potion2.text = playerDataManager.instance.getItemCount(itemType.clearPotion).ToString();

    }

    public void OpenDungeonUi(mapType map)
    {
        selectMapType = map;
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

    public void OnClickPlay()
    {
        playerDataManager.instance.setMap((int)selectMapType);
        playerDataManager.instance.setEquip(selectEquip);
        SceneChangeManager sceneChangeManager = GameObject.Find("SceneManager").GetComponent<SceneChangeManager>();
        sceneChangeManager.ChangeScene(2, 0, 1f);
    }

    public void OpenDungeonInven()
    {
        GameObject.Find("invenUi").GetComponent<Inven>().OpenInven((item, sprite) =>
        {
            GameObject selectItem;
            selectEquip = item;
            selectItem = dungeonUi.transform.Find("selectEquip").gameObject;
            string weaponString = "sword";

            selectItem.transform.Find("sword").gameObject.SetActive(false);
            selectItem.transform.Find("bigsword").gameObject.SetActive(false);
            selectItem.transform.Find("hammer").gameObject.SetActive(false);
            dungeonUi.transform.Find("sword").gameObject.SetActive(false);
            dungeonUi.transform.Find("bigsword").gameObject.SetActive(false);
            dungeonUi.transform.Find("hammer").gameObject.SetActive(false);
            switch (item.type)
            {
                case (int)itemType.sword:
                    weaponString = "sword";
                    dungeonUi.transform.Find("sword").gameObject.SetActive(true);
                    break;
                case (int)itemType.hammer:
                    weaponString = "hammer";
                    dungeonUi.transform.Find("hammer").gameObject.SetActive(true);
                    break;
                case (int)itemType.bigSword:
                    weaponString = "bigsword";
                    dungeonUi.transform.Find("bigsword").gameObject.SetActive(true);
                    break;
            }

            selectItem.transform.Find(weaponString).gameObject.SetActive(true);
            playerDataManager.instance.setEquip(selectEquip);
        }, 1);
    }
}
