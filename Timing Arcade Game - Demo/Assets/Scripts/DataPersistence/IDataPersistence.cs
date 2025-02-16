using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDataPersistence
{
    void LoadData(GameData gameData);

    void SaveData(ref GameData gameData);
    // note: taking gameData as a reference allows the method to modify the gameData
}