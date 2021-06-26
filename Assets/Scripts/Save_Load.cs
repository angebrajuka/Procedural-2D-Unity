using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public static class Save_Load
{
    static string Path(int slot)
    {
        return Application.persistentDataPath + "/saves/slot"+slot+".sav";
    }

    public static void Save(int slot)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string filePath = Path(slot);
        FileStream stream = new FileStream(filePath, FileMode.Create);

        SaveData data = new SaveData();
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static void Load(int slot)
    {
        string filePath = Path(slot);
        if(File.Exists(filePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(filePath, FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;
            stream.Close();
        }
        else
        {
            Debug.LogError("file not found at '"+filePath+"'");
        }
    }
}
