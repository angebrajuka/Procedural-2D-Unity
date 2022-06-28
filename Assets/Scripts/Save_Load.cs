using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

public static class Save_Load
{
    static string DirectoryPath()
    {
        return Application.persistentDataPath + "/saves/";
    }

    static string Path(int slot)
    {
        return DirectoryPath()+"slot"+slot+".sav";
    }

    public static bool GetSaveInfo(int slot, out DateTime dateTime, out string saveName)
    {
        string filePath = Path(slot);
        string metaPath = filePath+".meta";
        if(File.Exists(filePath) && File.Exists(metaPath))
        {
            dateTime = File.GetLastWriteTime(filePath);
            var streamReader = new StreamReader(File.OpenRead(metaPath));
            saveName = streamReader.ReadLine();
            return true;
        }
        dateTime = default(DateTime);
        saveName = default(string);
        return false;
    }

    public static void Save(int slot)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        Directory.CreateDirectory(DirectoryPath());
        string filePath = Path(slot);
        FileStream stream = new FileStream(filePath, FileMode.Create);

        SaveData data = new SaveData();
        formatter.Serialize(stream, data);
        stream.Close();

        // stream = new FileStream(filePath+".meta", FileMode.Create);
        // stream.Write();
    }

    public static void Load(int slot)
    {
        if(!TryLoad(slot)) {
            Debug.LogError("file not found at '"+Path(slot)+"'");
            Application.Quit();
        }
    }

    public static bool TryLoad(int slot) {
        string filePath = Path(slot);
        try {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(filePath, FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;
            data.Load();
            stream.Close();

            return true;
        }
        catch {};
        return false;
    }
}
