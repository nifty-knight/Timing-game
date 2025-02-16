using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string file_name;

    private GameData gameData;
    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;
    public static DataPersistenceManager instance {  get; private set; }
    // note: { get; private set; } means that other classes will be able to
    //  see data from this class but will not be able to modify it

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Data Persistence Manager in the scene.");
        }
        instance = this;
    }

    private void Start()
    {
        this.dataHandler = new FileDataHandler(Application.persistentDataPath, file_name);
        this.dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void NewGame()
    {
        this.gameData = new GameData();
    }

    public void LoadGame()
    {
        // Load any saved data from file using data handler - null if file doesn't exist
        this.gameData = dataHandler.Load();

        // if no data to load, initialize a new game
        if (this.gameData == null)
        {
            Debug.Log("No data was found. Initializing data to defaults.");
            NewGame();
        }

        // TODO: push loaded data to all other scripts that need it
        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.LoadData(gameData);
        }

        // Debug.Log("Loaded Click count = " + gameData.click_count.ToString());
    }

    public void SaveGame()
    {
        // TODO: pass data to other scripts so they can update it
        // TODO: save data to a file using data handler

        foreach (IDataPersistence dataPersistenceObj in dataPersistenceObjects)
        {
            dataPersistenceObj.SaveData(ref gameData);
        }

        // Debug.Log("Saved Click count = " + gameData.click_count);

        // save data to a file using data handler
        dataHandler.Save(gameData);
    }

    // Saves game data when application pauses - since apparently OnApplicationQuit doesn't always work on android
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        { 
            SaveGame(); 
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects()
    {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        // note: the above gets all scripts/objects which are monobehaviour AND use IDataPersistence
        // note: need "using System.Linq" in order to use the above line

    return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
