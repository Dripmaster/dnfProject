using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public abstract class FileData
{
    protected string filePath;

    ///<summary>
    ///Read file and encode in UTF8
    ///</summary>
    public static string ReadFile(string filePath)
    {
        string path = Application.dataPath + "/" + filePath;
        string data = File.ReadAllText(path, Encoding.UTF8);
        return data;
    }

    protected FileData(string filePath)
    {
        this.filePath = filePath;
    }

    ///<summary>
    ///Load file at filePath
    ///</summary>
    abstract public void Load();


}