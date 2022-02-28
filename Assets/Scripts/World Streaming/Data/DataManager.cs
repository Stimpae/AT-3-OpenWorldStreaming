using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public static class DataManager
{
    public static void SaveData(out string path, object data, string name )
    {
        path = Application.persistentDataPath + "/" + name + ".dat";
        FileStream file;
        if (File.Exists(path))
            file = File.OpenWrite(path);

        else file = File.Create(path);
    
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, data);
        file.Close();
    }
    
    public static object LoadData(string path)
    {
        string destination = path;
        FileStream file;

        if (File.Exists(destination)) file = File.OpenRead(destination);
        else
        {
            Debug.LogError("File not found");
            return null;
        }
        BinaryFormatter bf = new BinaryFormatter();
        var data = bf.Deserialize(file);
        file.Close();
        return data;
    }
    
}
