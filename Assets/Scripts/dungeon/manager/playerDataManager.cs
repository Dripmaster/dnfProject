using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class playerDataManager : MonoBehaviour
{
    public static playerDataManager instance = null;
    playerInven inven;
    playerProgress progress;
    potionInfo hpPotion;
    potionInfo cleanPotion;

    item currendEquip;
    int mapLevel = 1;
    [System.Serializable]
    public class playerInven
    {
        public List<item> playerInventory = new List<item>();
        public int gold = 0;
        public void addItem(int type, int count = 1)
        {
            bool need = true;
            if (type < (int)itemType.sword)
            {
                foreach (var i in playerInventory)
                {
                    if (i.type == type)
                    {
                        i.count += count;
                        need = false;
                        break;
                    }
                }
            }
            if (need)
            {
                playerInventory.Add(new item(playerInventory.Count, 1, type));
            }
        }
        public item getEquip()
        {
            item r = null;
            foreach (var i in playerInventory)
            {
                if (i.type >= (int)itemType.sword)
                {
                    r = i;
                    break;
                }
            }
            return r;
        }
        public item getItemById(int id) {
            item r = null;
            foreach (var i in playerInventory)
            {
                if (i.id == id)
                {
                    r = i;
                    break;
                }
            }
            return r;
        }
        public int getItem(int type, int count = 0)
        {
            int value = 0;
            foreach (var i in playerInventory)
            {
                if (i.type == type)
                {
                    i.count -= count;
                    value = i.count;
                    break;
                }
            }
            return value;
        }
        public void DestroyItem(item i)
        {
            playerInventory.Remove(i);
        }
        public item clearInven(int equip_id) {
            item i = null;
            for (int index = playerInventory.Count - 1; index >= 0; index--)
            {
                if (playerInventory[index].count == 0) {
                    playerInventory.RemoveAt(index);
                }
            }
            int c = 0;
            foreach (var in_item in playerInventory)
            {
                if (in_item.id == equip_id)
                    i = in_item;
                in_item.id = c++;
            }
            return i;
        }
    }
    [System.Serializable]
    public class playerProgress
    {
        public bool tutorialClear = false;
        public List<int> floorProgress = new List<int>();
        public playerProgress()
        {
            for (int i = 0; i < 10; i++)
            {
                floorProgress.Add(0);
            }
        }
    }
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        currendEquip = null;
        loadInventory();
        loadProgress();
    }
    void SaveProgress()
    {//NOTICE : progress변경이 있을때마다 호출!
        string str = JsonUtility.ToJson(progress);
        File.WriteAllText(Application.dataPath + "/Data/progressData.json", str);
    }
    void loadProgress()
    {
        string jsonString = null;
        try
        {
            jsonString = File.ReadAllText(Application.dataPath + "/Data/progressData.json");
        }
        catch
        {

        }
        if (jsonString != null)
            progress = JsonUtility.FromJson<playerProgress>(jsonString);
        if (progress == null)
        {
            progress = new playerProgress();
            SaveProgress();
        }
    }
    public void setPotionInfo(potionInfo p, int v)
    {
        if (v == 0)
        {
            hpPotion = p;
            hpPotion.setItemCount((inven.getItem((int)itemType.healPotion, 0)));
        }
        else
        {
            cleanPotion = p;
            cleanPotion.setItemCount((inven.getItem((int)itemType.clearPotion, 0)));

        }

    }
    void saveInventory()
    {//NOTICE : 인벤토리 변경이 있을때마다 호출 !
        string str = JsonUtility.ToJson(inven);
        File.WriteAllText(Application.dataPath + "/Data/invenData.json", str);
    }
    void loadInventory()
    {
        string jsonString = null;
        try
        {
            jsonString = File.ReadAllText(Application.dataPath + "/Data/invenData.json");
        }
        catch
        {
        }
        if (jsonString != null)
        {
            inven = JsonUtility.FromJson<playerInven>(jsonString);
            int equip_id = PlayerPrefs.GetInt("equip", -1);
            if (equip_id != -1)
                currendEquip = inven.getItemById(equip_id);
            else {
                currendEquip = inven.getEquip();
            }
        }
        if (inven == null)
        {
            inven = new playerInven();
            inven.addItem((int)itemType.bigSword);
            inven.addItem((int)itemType.sword);
            inven.addItem((int)itemType.hammer);
            inven.addItem((int)itemType.healPotion,5);
            inven.addItem((int)itemType.clearPotion,5);
            saveInventory();
        }
    }
    public void addGold(int goldSize)
    {
        inven.gold += goldSize;
        saveInventory();
    }
    public void addItem(itemType type, int count = 1)
    {
        inven.addItem((int)type, count);
        saveInventory();
    }
    public bool popGold(int goldSize, bool chcek = true)
    {
        if (inven.gold < goldSize)
            return false;
        else
        {
            if (!chcek)
            {
                inven.gold -= goldSize;
                saveInventory();
            }
            return true;
        }
    }
    public int getItemCount(itemType type) {
        return inven.getItem((int)type);
    }
    public bool popItem(itemType type, int count, bool chcek = true)
    {
        if (inven.getItem((int)type) < count)
        {
            return false;
        }
        else
        {
            if (!chcek)
            {
                inven.getItem((int)type, count);
                saveInventory();
                if (type == itemType.clearPotion)
                {
                    cleanPotion.setItemCount(inven.getItem((int)type));
                }
                else if (type == itemType.healPotion)
                { 
                hpPotion.setItemCount(inven.getItem((int)type));
                }
                if (currendEquip != null)
                {
                    setEquip( inven.clearInven(currendEquip.id));               
                }
            }
            return true;
        }
    }
    public void destroyItem(item i)
    {
        inven.DestroyItem(i);
        if (currendEquip != null)
        {
            setEquip(inven.clearInven(currendEquip.id));
        }
    }

    public playerInven getInventory() {
        return inven;
    }
    public bool hasTutoClear()
    {
        return progress.tutorialClear;
    }
    public int getMapProgress(mapType mapNum)
    {
        return progress.floorProgress[(int)mapNum];
    }
    public void setTutoClear(bool value)
    {
        progress.tutorialClear = value;
        SaveProgress();
    }
    public void setMapProgress(mapType mapNum, int value)
    {
        progress.floorProgress[(int)mapNum] = value;
        SaveProgress();
    }
    public float showAtkPoint(item waepon) {
        int weaponLevel = 0;
        if (waepon == null) {
            return 0;
        }
        foreach (var i in waepon.upgradeList)
        {
            weaponLevel += i;
        }
        return weaponLevel + (weaponLevel /10) *5;
     }
    public void setEquip(item weapon) {
        currendEquip = weapon;
        PlayerPrefs.SetInt("equip",weapon.id);
    }
    public item getEquip() {
        return currendEquip;
    }

    public void setMap(int m) {
        mapLevel = m;
    }
    public int getMap() {
        return mapLevel;
    }
}
