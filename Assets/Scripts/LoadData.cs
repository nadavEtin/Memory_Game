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

    private void InitBinarySerializer()
    {
        if (bSerializer == null)
            bSerializer = new BinarySerializer();
    }

    //checks for existing data
    public bool SavedDataExists()
    {
        bool result = false;

        if(LoadIntData("lineAmount") != 0 && LoadIntData("columnAmount") != 0)
            result = true;

        return result;
    }

    public int LoadIntData(string key)
    {
        int val = 0;

        if(binary)
        {
            InitBinarySerializer();
            if (File.Exists(bSerializer.path + key))
            {
                bSerializer.stream = new FileStream(bSerializer.path + key, FileMode.Open);
                val = (int)bSerializer.formatter.Deserialize(bSerializer.stream);
                bSerializer.stream.Close();
            }
        }
        else if (localMemory && SaveData.localSavedData != null && SaveData.localSavedData.intData.ContainsKey(key))
             val = SaveData.localSavedData.intData[key];
        else
            val = PlayerPrefs.GetInt(key, 0); 

        return val;
    }

    public bool LoadBoolData(string key)
    {
        bool val;

        if (SaveData.localSavedData != null && SaveData.localSavedData.boolData.ContainsKey(key))
            val = SaveData.localSavedData.boolData[key];
        else
            val = PlayerPrefs.GetInt(key, 0) == 0 ? false : true;

        return val;
    }

    public float LoadFloatData(string key)
    {
        float val = 0;

        if (SaveData.localSavedData != null && SaveData.localSavedData.floatData.ContainsKey(key))
            val = SaveData.localSavedData.floatData[key];
        else
            val = PlayerPrefs.GetFloat(key, 0);

        return val;
    }

    public string LoadStringData(string key)
    {
        string val = "";

        if (SaveData.localSavedData != null && SaveData.localSavedData.stringData.ContainsKey(key))
            val = SaveData.localSavedData.stringData[key];
        else
            val = PlayerPrefs.GetString(key, "");

        return val;
    }

    public List<BaseCardDataSrlzd> LoadBaseCardListData(string fileName)
    {
        List<BaseCardDataSrlzd> cards = new List<BaseCardDataSrlzd>();

        if (binary)
        {
            InitBinarySerializer();
            if (File.Exists(bSerializer.path + fileName))
            {
                bSerializer.stream = new FileStream(bSerializer.path + fileName, FileMode.Open);
                cards = bSerializer.formatter.Deserialize(bSerializer.stream) as List<BaseCardDataSrlzd>;
                bSerializer.stream.Close();
            }
            else
            {
                Debug.LogError("Wrong binary file name: " + fileName);
            }
        }

        return cards;
    }
}
