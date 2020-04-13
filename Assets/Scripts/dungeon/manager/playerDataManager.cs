using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class playerDataManager : MonoBehaviour
{
    public static playerDataManager instance;
    playerInven inven;

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
    }

    private void Awake()
    {
        instance = this;
        inven = new playerInven();
        loadInventory();
    }
    private void OnEnable()
    {
        StartCoroutine(saveLoop());
    }
    void saveInventory()
    {
        string str = JsonUtility.ToJson(inven);
        File.WriteAllText(Application.dataPath + "/Data/invenData.json", str);
    }

    void loadInventory() {
        string jsonString = File.ReadAllText(Application.dataPath + "/Data/invenData.json");
        if(jsonString!=null)
        inven = JsonUtility.FromJson<playerInven>(jsonString);
        if (inven == null) {
            inven = new playerInven();
        }
    }
    public void addGold() {
        inven.gold += 10;
    }
    public void addItem(int type) {
        inven.addItem(type);
    
    }
    IEnumerator saveLoop() {
        //!TODO : 1초마다 말고 더 좋은방법으로 ㄱ
        WaitForSeconds wfs = new WaitForSeconds(1);
        do
        {
            saveInventory();
            yield return wfs;
        } while (true);
    }
}
