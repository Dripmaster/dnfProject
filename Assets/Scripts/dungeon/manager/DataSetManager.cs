using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataSetManager : MonoBehaviour
{
    public static DataSetManager instance = null;
    CSVData FSMData = null;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        dataLoad();
    }
    void dataLoad() {
        FSMData = new CSVData("ModuleData/Csharp.mon");
        FSMData.Load();
    }
    public float loadFSMData(int dataType,int fsmType) {
        float f = 0;
        try
        {
            f = float.Parse(FSMData[dataType, fsmType]);
        }
        catch {
            Debug.Log(fsmType+" : "+FSMData[dataType, fsmType]);  
        };
        return f;
    } 
}
