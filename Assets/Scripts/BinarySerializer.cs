using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class BinarySerializer
{
    public string path;
    public BinaryFormatter formatter;
    public FileStream stream;

    public BinarySerializer()
    {
        path = Application.persistentDataPath + "/";
        formatter = new BinaryFormatter();
    }

    public void SetPath(string name)
    {
        path = Application.persistentDataPath + "/" + name;
    }

    public void CloseStream()
    {
        stream.Close();
    }
}

[System.Serializable]
public class BaseCardDataSrlzd
{
    public string name;
    public int pairNum;
    public bool imageShowing;
    public bool matchComplete;
}
