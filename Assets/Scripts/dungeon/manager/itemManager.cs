using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class itemManager : MonoBehaviour
{
    
    public static itemManager instance;
    List<itemBase> itemList;
    public GameObject goldItemPrefab;
    public Sprite[] effectImage;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        itemList = new List<itemBase>();
    }
    public void itemGenerate(Vector2 pos,itemType type) {
        itemBase item = null;

        foreach (itemBase e in itemList)
        {
            if (e.gameObject.activeInHierarchy == false)
            {
                item = e;
                e.transform.position = pos;
                e.transform.rotation = Quaternion.identity;
                break;
            }
        }
        if (item == null)
        {
            item = Instantiate(goldItemPrefab, pos, Quaternion.identity).GetComponent<itemBase>();
            item.transform.SetParent(LevelManager.instance.getCurrentMap().transform);
            itemList.Add(item);
        }
        
        item.gameObject.SetActive(true);
        item.setAnim(type);
    }
    public void itemEvent(itemType type) {
        //TODO itemHandle필요!!
        switch (type) {
            case itemType.gold:
                EffectManager.getEffect(playerFSM.instance.transform.position).setImage(effectImage[0]);
                playerDataManager.instance.addGold(10);
                break;
            case itemType.darkMat:
            case itemType.fireMat:
            case itemType.glowMat:
            case itemType.grassMat:
            case itemType.waterMat:
                playerDataManager.instance.addItem(type);
                break;
            default:break;
        
        }
    
    }
}
