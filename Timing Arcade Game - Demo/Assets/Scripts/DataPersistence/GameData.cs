using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    public int click_count;

    // Note: The below may be helpful for saving data of things such as unlocked items/levels, checkpoints, collectibles, etc.
    // String should represent a unique id
    // public Dictionary<string, bool> some_data;
    // initialize it in GameData() as "some_data = new Dictionary<string, bool>();" to initialize an empty dictionary

    // The values defined in the constructor below will be the
    // initial values the game starts with when there is no
    // data to load (aka new game)
    public GameData()
    {
        this.click_count = 0;
    }
}
