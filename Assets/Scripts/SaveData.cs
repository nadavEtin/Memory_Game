using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class LocalData
{
    public Dictionary<string, int> intData;
    public Dictionary<string, int[]> intArrayData;
    public Dictionary<string, string> stringData;
    public Dictionary<string, bool> boolData;
    public Dictionary<string, float> floatData;

    public LocalData()
    {
        intData = new Dictionary<string, int>();
        stringData = new Dictionary<string, string>();
        boolData = new Dictionary<string, bool>();
        floatData = new Dictionary<string, float>();

        intArrayData = new Dictionary<string, int[]>();
    }
}

public class SaveData : MonoBehaviour
{
    public static SaveData Instance;
    public static LocalData localSavedData;

    [Header("Save to:")]
    public bool localMemory;
    public bool playerPrefs;
    public bool binary;

    private BinarySerializer bSerializer;

    private void Start()
    {
        if (Instance == null)
            Instance = this;
    }

    public void SaveGameData(string name, List<BaseCardDataSrlzd> cards)
    {
        if(binary)
            SaveBinaryData(name, cards);
    }

    public void SaveButtonClicked()
    {
        //clear old data before saving
        localSavedData = new LocalData();
        PlayerPrefs.DeleteAll();
    }

    public void SaveGameData(string name, int data)
    {
        if (localMemory)
            localSavedData.intData.Add(name, data);
        else if (playerPrefs)
            PlayerPrefs.SetInt(name, data);
        else if (binary)
            SaveBinaryData(name, data);
    }

    public void SaveGameData(string name, float data)
    {
        if (localMemory)
            localSavedData.floatData.Add(name, data);
        else if (binary)
            SaveBinaryData(name, data);
        else if (playerPrefs)
            PlayerPrefs.SetFloat(name, data);
    }

    public void SaveGameData(string name, string data)
    {
        if (localMemory)
            localSavedData.stringData.Add(name, data);
        else if (binary)
            SaveBinaryData(name, data);
        else if (playerPrefs)
            PlayerPrefs.SetString(name, data);
    }

    public void SaveGameData(string name, bool data)
    {
        if (localMemory)
            localSavedData.boolData.Add(name, data);
        else if (playerPrefs)
            PlayerPrefs.SetInt(name, data ? 1 : 0);
        else if (binary)
            SaveBinaryData(name, data);
    }

    private void SaveBinaryData(string name, object data)
    {
        if (bSerializer == null)
            bSerializer = new BinarySerializer();
        bSerializer.SetPath(name);
        bSerializer.stream = new FileStream(bSerializer.path, FileMode.Create);
        bSerializer.formatter.Serialize(bSerializer.stream, data);
        bSerializer.CloseStream();
    }
}
