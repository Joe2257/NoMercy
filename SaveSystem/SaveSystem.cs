using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

//This script convert the variables that need to be saved into to binary or reverse it when the game need to be loaded.
public static class SaveSystem
{
    public static void SaveSharedData(SharedVariables sharVar)
    {

        BinaryFormatter formatter = new BinaryFormatter();
        string path               = Application.persistentDataPath + "/savegame.data";
        FileStream stream         = new FileStream(path, FileMode.Create);

        GameData data = new GameData(sharVar);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static GameData LoadGameData()
    {
        string path = Application.persistentDataPath + "/savegame.data";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream         = new FileStream(path, FileMode.Open);

            GameData data = formatter.Deserialize(stream) as GameData;

            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Save file not found or is corrupted.");
            return null;
        }
    }

}
