using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class FileDataHandler
{
    private string dataDirPath = ""; // Later, set string to Application.persistentDataPath

    private string dataFileName = ""; // I think I can make this up

    public FileDataHandler(string dataDirPath, string dataFileName)
    {
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public GameData Load()
    {
        // get the full path to the save file (including file name) using Path.Combine
        string full_path = Path.Combine(dataDirPath, dataFileName);

        GameData loaded_data = null;

        if (File.Exists(full_path))
        {
            try
            {
                // load serialized data from file
                string data_to_load = "";
                using (FileStream stream = new FileStream(full_path, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        data_to_load = reader.ReadToEnd();
                    }
                }

                // deserialize data from JSON back into C# object
                loaded_data = JsonUtility.FromJson<GameData>(data_to_load);
            }
            catch (Exception e)
            {
                Debug.LogError("Error occurred when trying to load data to file: " + full_path + "\n" + e);
            }
        }

        return loaded_data;
    }

    public void Save(GameData data)
    {
        // get the full path to the save file (including file name) using Path.Combine
        string full_path = Path.Combine(dataDirPath, dataFileName);

        try
        {
            // create directory path that the file will be written to in case it doesn't already exist
            Directory.CreateDirectory(Path.GetDirectoryName(full_path));

            // serialize C# game data object into JSON string
            string dataToStore = JsonUtility.ToJson(data, true);

            // write serialized data to the file
            using (FileStream stream = new FileStream(full_path, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error occurred when trying to save data to file: " + full_path + "\n" + e);
        }
    }
}

