using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LoadData : MonoBehaviour
{
    public static LoadData Instance;

    [Header("Load from:")]
    public bool localMemory;
    public bool playerPrefs;
    public bool binary;

    private BinarySerializer bSerializer;

    void Start()
    {
        if (Instance == null)
            Instance = this;
    }

    //checks for existing data
    public bool SavedDataExists()
    {
        bool result = false;

        if(LoadIntData("line.Amount") != 0 && LoadIntData("column.Amount") != 0)
            result = true;

        return result;
    }

    public int LoadIntData(string key)
    {
        int val = 0;

        if(binary)
            val = LoadBinaryData(key) == null ? 0 : (int)LoadBinaryData(key);
        else if (localMemory && SaveData.localSavedData != null && SaveData.localSavedData.intData.ContainsKey(key))
             val = SaveData.localSavedData.intData[key];
        else
            val = PlayerPrefs.GetInt(key, 0); 

        return val;
    }

    public bool LoadBoolData(string key)
    {
        bool val;

        if (binary)
            val = LoadBinaryData(key) == null ? false : (bool)LoadBinaryData(key);
        else if (SaveData.localSavedData != null && SaveData.localSavedData.boolData.ContainsKey(key))
            val = SaveData.localSavedData.boolData[key];
        else
            val = PlayerPrefs.GetInt(key, 0) == 0 ? false : true;

        return val;
    }

    public float LoadFloatData(string key)
    {
        float val = 0;

        if (binary)
            val = LoadBinaryData(key) == null ? 0 : (float)LoadBinaryData(key);

        else if (SaveData.localSavedData != null && SaveData.localSavedData.floatData.ContainsKey(key))
            val = SaveData.localSavedData.floatData[key];
        else
            val = PlayerPrefs.GetFloat(key, 0);

        return val;
    }

    public string LoadStringData(string key)
    {
        string val = "";

        if (binary)
            val = LoadBinaryData(key) == null ? "" : (string)LoadBinaryData(key);
        else if (SaveData.localSavedData != null && SaveData.localSavedData.stringData.ContainsKey(key))
            val = SaveData.localSavedData.stringData[key];
        else
            val = PlayerPrefs.GetString(key, "");

        return val;
    }

    public List<BaseCardDataSrlzd> LoadBaseCardListData(string fileName)
    {
        List<BaseCardDataSrlzd> cards = null;

        if (binary)
            cards = LoadBinaryData(fileName) == null ? null : LoadBinaryData(fileName) as List<BaseCardDataSrlzd>;
        if (cards == null)
            cards = new List<BaseCardDataSrlzd>();
        return cards;
    }

    private object LoadBinaryData(string name)
    {
        object returnVal = null;

        if (bSerializer == null)
            bSerializer = new BinarySerializer();

        bSerializer.SetPath(name);
        if (File.Exists(bSerializer.path))
        {
            
            bSerializer.stream = new FileStream(bSerializer.path, FileMode.Open);
            returnVal = bSerializer.formatter.Deserialize(bSerializer.stream);
            bSerializer.stream.Close();
        }
        else
        {
            Debug.LogError("Wrong binary file name: " + name);
        }

        return returnVal;
    }
}