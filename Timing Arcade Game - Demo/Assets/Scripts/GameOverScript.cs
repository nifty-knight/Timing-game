using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{
    public TMP_Text KillCount; // Text displaying how many enemies were killed

    // Start is called before the first frame update
    void Start()
    {
        KillCount.text = "Enemies Killed:<br>" + EnemySpawnerScript.enemies_killed.ToString();
    }

    /*
    // Update is called once per frame
    void Update()
    {
        
    }
    */

    // Switches scene to battle scene- attached to Retry button
    public void LoadBattleScene()
    {
        SceneManager.LoadScene("Battle");
    }

    // Switches scene to main menu- attached to Main Menu button
    public void LoadMainMenuScene()
    {

        SceneManager.LoadScene("MainMenu");
    }
}
