using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class playerDataManager : MonoBehaviour
{
    public static playerDataManager instance=null;
    playerInven inven;
    playerProgress progress;

    [System.Serializable]
    public class playerInven
    {
        public List<item> playerInventory = new List<item>();
        public int gold = 0;
        public void addItem(int type) {
            bool need = true;
            foreach (var i in playerInventory)
            {
                if (i.id == type)
                {
                    i.count++;
                    need = false;
                    break;
                }
            }
            if (need) {
                playerInventory.Add(new item(type,1));
            }
        }
        public int getItem(int type,int count = 0) {
            int value = 0;
            foreach (var i in playerInventory)
            {
                if (i.id == type)
                {
                    i.count -= count;
                    value = i.count;
                    break;

                }
                
            }
            return value;
        }
    }
    [System.Serializable]
    public class playerProgress {
        public bool tutorialClear = false;
        public List<int> floorProgress = new List<int>();
        public playerProgress() {
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
        loadInventory();
        loadProgress();
    }
    void SaveProgress()
    {//NOTICE : progress변경이 있을때마다 호출!
        string str = JsonUtility.ToJson(progress);
        File.WriteAllText(Application.dataPath + "/Data/progressData.json", str);
    }
    void loadProgress() {
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
    void saveInventory()
    {//NOTICE : 인벤토리 변경이 있을때마다 호출 !
        string str = JsonUtility.ToJson(inven);
        File.WriteAllText(Application.dataPath + "/Data/invenData.json", str);
    }
    void loadInventory() {
        string jsonString = null;
        try
        {
            jsonString = File.ReadAllText(Application.dataPath + "/Data/invenData.json");
        }
        catch
        { 

        }
        if(jsonString!=null)
        inven = JsonUtility.FromJson<playerInven>(jsonString);
        if (inven == null) {
            inven = new playerInven();
            saveInventory();
        }
    }
    public void addGold(int goldSize) {
        inven.gold += goldSize;
        saveInventory();
    }
    public void addItem(itemType type) {
        inven.addItem((int)type);
        saveInventory();
    }
    public bool popGold(int goldSize,bool chcek = true) {
        if (inven.gold < goldSize)
            return false;
        else {
            if (!chcek)
            {
                inven.gold -= goldSize;
                saveInventory();
            }
            return true;
        }
    }
    public bool popItem(itemType type, int count,bool chcek = true) {
        if (inven.getItem((int)type) < count)
        {
            return false;
        }
        else {
            if (!chcek) {
                inven.getItem((int)type,count);
                saveInventory();
            }
            return true;
        }
    }
    public bool hasTutoClear() {
        return progress.tutorialClear;
    }
    public int getMapProgress(int mapNum) {
        return progress.floorProgress[mapNum];
    }
    public void setTutoClear(bool value) {
        progress.tutorialClear = value;
        SaveProgress();
    }
    public void setMapProgress(int mapNum,int value) {
        progress.floorProgress[mapNum] = value;
        SaveProgress();
    }
}
