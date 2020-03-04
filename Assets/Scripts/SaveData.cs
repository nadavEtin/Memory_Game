using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalData
{
    public Dictionary<string, int> intData;
    public Dictionary<string, string> stringData;
    public Dictionary<string, bool> boolData;
    public Dictionary<string, float> floatData;

    public LocalData()
    {
        intData = new Dictionary<string, int>();
        stringData = new Dictionary<string, string>();
        boolData = new Dictionary<string, bool>();
        floatData = new Dictionary<string, float>();
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
        if(saveInMemory == true)
            localSavedData = new LocalData();
        if (saveToPlayerPrefs == true)
            PlayerPrefs.DeleteAll();
    }

    public void SaveGameData(string name, int data)
    {
        if(saveToPlayerPrefs == true)
        {
            PlayerPrefs.SetInt(name, data);
        }

        if(saveInMemory == true)
        {
            localSavedData.intData.Add(name, data);
        }
    }

    public void SaveGameData(string name, float data)
    {
        if (saveToPlayerPrefs == true)
        {
            PlayerPrefs.SetFloat(name, data);
        }


        if (saveInMemory == true)
        {
            localSavedData.floatData.Add(name, data);
        }
    }

    public void SaveGameData(string name, string data)
    {
        if (saveToPlayerPrefs == true)
        {
            PlayerPrefs.SetString(name, data);
        }


        if (saveInMemory == true)
        {
            localSavedData.stringData.Add(name, data);
        }
    }

    public void SaveGameData(string name, bool data)
    {
        if (saveToPlayerPrefs == true)
        {
            PlayerPrefs.SetInt(name, data ? 1 : 0);
        }


        if (saveInMemory == true)
        {
            localSavedData.boolData.Add(name, data);
        }
    }
}
