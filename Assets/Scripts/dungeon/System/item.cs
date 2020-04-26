using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class item
{
    public int id;
    public int type;
    public int count;
    public List<int> upgradeList;
    public List<bool> hasSkillList;
    public item(int _id, int _count, int t) {
        id = _id;
        type = t;
        count = _count;
        if (type >= (int)itemType.sword) {
            upgradeList = new List<int>();
            upgradeList.Add(0);
            upgradeList.Add(0);
            upgradeList.Add(0);
            upgradeList.Add(0);
            upgradeList.Add(0);
            hasSkillList = new List<bool>();
            hasSkillList.Add(false);
            hasSkillList.Add(false);
            hasSkillList.Add(false);
            hasSkillList.Add(false);
            hasSkillList.Add(false);
        }
    }
}
