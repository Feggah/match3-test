using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveGame(GameManager game)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/game.match3";
        FileStream stream = new FileStream(path, FileMode.Create);

        ProgressData data = new ProgressData(game);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static ProgressData LoadGame()
    {
        string path = Application.persistentDataPath + "/game.match3";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            ProgressData data = (ProgressData)formatter.Deserialize(stream);
            stream.Close();

            return data;
        }
        else
        {
            return null;
        }
    }
}
