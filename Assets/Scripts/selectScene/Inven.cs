using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Inven : MonoBehaviour
{
    public GameObject content;
    public Sprite[] itemSprites;
    public GameObject itemSlot;
    GameObject canvas;
    playerDataManager.playerInven inven;
    Action<item, Sprite> OnSelect;

    void Start()
    {
        canvas = transform.Find("canvas").gameObject;
        inven = playerDataManager.instance.getInventory();

    }

    // Update is called once per frame
    void Update()
    {

    }

    void InitInven()
    {
        int indexCount = 0;
        foreach (item item in inven.playerInventory)
        {
            if (item.count == 0) continue;

            int colNum = indexCount % 4;
            if (indexCount >= content.transform.childCount) // 동적으로 인벤토리 늘리기
            {
                RectTransform contentRect = content.GetComponent<RectTransform>();
                contentRect.sizeDelta = new Vector2(contentRect.sizeDelta.x, contentRect.sizeDelta.y + 100);
                for (int i = 0; i < 4; i++)
                {
                    GameObject newItemRow = Instantiate(itemSlot, content.transform);
                    newItemRow.transform.localScale = Vector3.one;
                }
            }
            GameObject itemCell = content.transform.GetChild(indexCount).gameObject;
            itemCell.tag = "etcItem";
            if (item.type == (int)itemType.bigSword || item.type == (int)itemType.sword || item.type == (int)itemType.hammer)
                itemCell.tag = "weaponItem";
            if (item.type == (int)itemType.darkMat || item.type == (int)itemType.fireMat || item.type == (int)itemType.glowMat
                   || item.type == (int)itemType.grassMat || item.type == (int)itemType.waterMat)
                itemCell.tag = "materialItem";
            itemCell.GetComponent<Button>().interactable = true;
            GameObject icon = itemCell.transform.Find("Image").gameObject;
            icon.SetActive(true);
            icon.GetComponent<Image>().sprite = itemSprites[item.type - 1];
            icon.transform.Find("Text").GetComponent<Text>().text = item.count.ToString();

            itemCell.GetComponent<Button>().onClick.AddListener(() => OnClickItem(item, icon.GetComponent<Image>().sprite));


            indexCount++;
        }
    }
    public void OpenInven(Action<item, Sprite> call, int type = 0)
    {
        OnSelect = call;
        InitInven();
        foreach (Transform child in content.transform)
        {
            if (child.tag == "Untagged") continue;

            Button btn = child.GetComponent<Button>();
            btn.interactable = true;

            ColorBlock newColor = btn.colors;
            newColor.disabledColor = new Color(200, 200, 200, 0.5f);
            btn.colors = newColor;

            Image itemImg = btn.transform.Find("Image").GetComponent<Image>();
            itemImg.color = new Color(255, 255, 255, 1f);

            string itemTag = "weaponItem";
            if (type == 0) continue;
            if (type == 1) // only weapon
                itemTag = "weaponItem";
            if (type == 2) // only weapon
                itemTag = "materialItem";

            if (child.tag != itemTag)
            {
                newColor.disabledColor = new Color(0, 0, 0, 0.4f);
                btn.colors = newColor;

                itemImg.color = new Color(0.5f, 0.5f, 0.5f, 0.4f);
                btn.interactable = false;
            }
        }
        canvas.SetActive(true);
    }
    public void CloseInven()
    {
        canvas.SetActive(false);
    }
    void OnClickItem(item i, Sprite s)
    {
        OnSelect.Invoke(i, s);
        CloseInven();
    }
}

