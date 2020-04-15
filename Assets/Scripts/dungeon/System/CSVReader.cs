using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// Class that contains a CSV data as 2d array
/// Load() should be done before anything
/// </summary>
public sealed class CSVData : FileData
{
    string[,] data;
    int rowCount;
    int colCount;
    Dictionary<string, int> fields = new Dictionary<string, int>();
    Dictionary<string, int> keys = new Dictionary<string, int>();

    public CSVData(string filePath) : base(filePath)
    {
    }

    public string this[string key, string field]
    {
        get { return data[keys[key], fields[field]]; }
    }
    public string this[string key, int field]
    {
        get { return data[keys[key], field]; }
    }
    public string this[int key, int field]
    {
        get { return data[key, field]; }
    }
    public int FieldCount { get { return colCount; } }
    public int KeyCount { get { return rowCount; } }
    public int GetFieldIndex(string field)
    {
        return fields[field];
    }

    /// <summary>
    /// Copies a row of the 2d array. Caching recommended.
    /// </summary>
    public string[] this[int key]
    {
        get
        {
            string[] row = new string[colCount];
            for (int i = 0; i < colCount; i++)
                row[i] = data[key, i];
            return row;
        }
    }
    /// <summary>
    /// Copies a row of the 2d array. Caching recommended.
    /// </summary>
    public string[] this[string key]
    {
        get
        {
            string[] row = new string[colCount];
            for (int i = 0; i < colCount; i++)
                row[i] = data[keys[key], i];
            return row;
        }
    }

    override public void Load()
    {
        string rawText = FileData.ReadFile(filePath);
        //Count row/col
        string[] lines = rawText.Substring(0, rawText.Length - 1).Split('\n');
        
        string[] fieldData = lines[0].Split(',');
        rowCount = lines.Length;
        colCount = fieldData.Length;

        //Get Fields
        for (int i = 0; i < colCount; ++i)
        {
            fields.Add(fieldData[i].TrimEnd('\r'), i);
        }

        //Fill Data
        data = new string[rowCount, colCount];
        for (int i = 0; i < rowCount; ++i)
        {
            string[] line = lines[i].Split(',');
            //Get Keys
            keys.Add(line[0], i);
            for (int j = 0; j < colCount; ++j)
            {
                data[i, j] = line[j].TrimEnd('\r');
            }
        }
    }
}