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

    private void InitBinarySerializer()
    {
        if (bSerializer == null)
            bSerializer = new BinarySerializer();
    }

    public void SaveGameData(string name, List<BaseCardDataSrlzd> cards)
    {
        if(binary)
        {
            InitBinarySerializer();
            bSerializer.path += name;
            bSerializer.stream = new FileStream(bSerializer.path, FileMode.Create);
            bSerializer.formatter.Serialize(bSerializer.stream, cards);
            bSerializer.CloseStream();
        }
    }

    public void SaveButtonClicked()
    {
        //clear old data before saving
        localSavedData = new LocalData();
        PlayerPrefs.DeleteAll();
    }

    public void SaveGameData(string name, int data)
    {
        if(binary)
        {
            InitBinarySerializer();
            bSerializer.path += name;
            bSerializer.stream = new FileStream(bSerializer.path, FileMode.Create);
            bSerializer.formatter.Serialize(bSerializer.stream, data);
            bSerializer.CloseStream();
        }

        if(playerPrefs)
            PlayerPrefs.SetInt(name, data);

        if(localMemory)
            localSavedData.intData.Add(name, data);
    }

    public void SaveGameData(string name, float data)
    {
        if (playerPrefs)
            PlayerPrefs.SetFloat(name, data);

        if (localMemory)
            localSavedData.floatData.Add(name, data);
    }

    public void SaveGameData(string name, string data)
    {
        if (playerPrefs)
            PlayerPrefs.SetString(name, data);

        if (localMemory)
            localSavedData.stringData.Add(name, data);
    }

    public void SaveGameData(string name, bool data)
    {
        if (playerPrefs)
            PlayerPrefs.SetInt(name, data ? 1 : 0);

        if (localMemory)
            localSavedData.boolData.Add(name, data);
    }
}
