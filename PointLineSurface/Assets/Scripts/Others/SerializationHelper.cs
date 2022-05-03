using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class SerializationHelper
{
    public static bool Save(string saveName, object data)
    {
        BinaryFormatter formatter = GetBinaryFormatter();
        if(!Directory.Exists(Application.persistentDataPath + "/saves/"))
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/saves/");
        }

        string path = Application.persistentDataPath + "/saves/" + saveName + ".save";
        FileStream file = File.Create(path);
        formatter.Serialize(file, data);
        file.Close();
        return true;
    }

    public static object Load(string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        BinaryFormatter formattter = GetBinaryFormatter();
        FileStream file = File.Open(path, FileMode.Open); ;

        try
        {
            object data = formattter.Deserialize(file);
            file.Close();
            return data;
        }
        catch
        {
            Debug.Log($"Failed to load file at {path}");
            file.Close();
            return null;
        }
    }

    public static BinaryFormatter GetBinaryFormatter()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        return formatter;
    }
}
