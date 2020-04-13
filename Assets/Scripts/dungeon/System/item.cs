using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class item
{
    public int id;
    public int count;
    public item(int _id, int _count) {
        id = _id;
        count = _count;
    }
}
