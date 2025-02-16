using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    /*
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    */

    public void StartGame()
    {
        SceneManager.LoadScene("Battle");
    }

    public void LoadTutorial()
    {
        TutorialControllerScript.index = 0;
        SceneManager.LoadScene("TutorialScene");
    }

    public void LoadAdvanced()
    {
        TutorialControllerScript.index = 4;
        SceneManager.LoadScene("TutorialScene");
    }
}
