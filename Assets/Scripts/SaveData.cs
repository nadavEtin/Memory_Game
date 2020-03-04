using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

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
    //OPTIONAL: ADD BINARY SERIALIZATION FOR SAVING

    [SerializeField]
    private bool saveToPlayerPrefs;
    [SerializeField]
    private bool saveInMemory;

    private void Start()
    {
        if (Instance == null)
            Instance = this;
    }

    public void SaveButtonClicked()
    {
        //clear old data before saving
        if(saveInMemory)
            localSavedData = new LocalData();
        if (saveToPlayerPrefs)
            PlayerPrefs.DeleteAll();
    }

    public void SaveGameData(string name, int data)
    {
        if(saveToPlayerPrefs)
            PlayerPrefs.SetInt(name, data);

        if(saveInMemory)
            localSavedData.intData.Add(name, data);
    }

    public void SaveGameData(string name, float data)
    {
        if (saveToPlayerPrefs)
            PlayerPrefs.SetFloat(name, data);

        if (saveInMemory)
            localSavedData.floatData.Add(name, data);
    }

    public void SaveGameData(string name, string data)
    {
        if (saveToPlayerPrefs)
            PlayerPrefs.SetString(name, data);

        if (saveInMemory)
            localSavedData.stringData.Add(name, data);
    }

    public void SaveGameData(string name, bool data)
    {
        if (saveToPlayerPrefs)
            PlayerPrefs.SetInt(name, data ? 1 : 0);

        if (saveInMemory)
            localSavedData.boolData.Add(name, data);
    }
}
