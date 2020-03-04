using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadData : MonoBehaviour
{
    public static LoadData Instance;

    void Start()
    {
        if (Instance == null)
            Instance = this;
    }

    //tests for existing data
    public bool SavedDataExists()
    {
        bool result = true;

        if(LoadIntData("lineAmount") == 0 || LoadIntData("columnAmount") == 0)
            result = false;

        return result;
    }

    public int LoadIntData(string key)
    {
        int val;
        if (SaveData.localSavedData != null && SaveData.localSavedData.intData.ContainsKey(key))
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
}
