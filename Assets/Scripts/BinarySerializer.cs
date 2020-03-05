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
        path = System.Environment.SpecialFolder.DesktopDirectory + "memory.game";
        //path = "C:/Users/Nadav/Desktop/moon_active/memory.game";
        formatter = new BinaryFormatter();
        //stream = new FileStream(path, FileMode.OpenOrCreate);
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
