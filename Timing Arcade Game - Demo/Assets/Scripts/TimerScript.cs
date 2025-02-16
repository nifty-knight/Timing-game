using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimerScript : MonoBehaviour
{
    public TMP_Text TimerDisplay;
    public GameObject TimeUpDisplay;
    private GameObject TimeUpText;
    public DisplayTextScript displayText;
    public AnimationCurve curve;

    public float shake_duration;

    public float time_added_per_kill; // Time added to timer every time an enemy is killed
    public float start_time_per_round; // How much time player starts with each round
    private float timer; // keeps track of how much time has passed

    private bool game_ended; // True if time is up

    // Variables which control speed, size, lifetime of displayed text for Time up display
    public float lifetime = 3f;
    public float change = 30; // speed at which text changes size
    public float max_scale = 4; // maximum scale size

    // Start is called before the first frame update
    void Start()
    {
        TimeUpDisplay.SetActive(false);
        TimeUpText = TimeUpDisplay.transform.GetChild(0).gameObject;
        timer = start_time_per_round;
        game_ended = false;
        // EnemyScript.EnemyDeath += AddTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            TimerDisplay.text = timer.ToString("f1");
        }
        else if (!game_ended)
        {
            // coroutine - display time up text object, shake it
            StartCoroutine(DisplayTimeUp());
        }
    }

    // Add time to timer whenever enemy is killed
    private void AddTime()
    {
        timer += time_added_per_kill;
        StartCoroutine(displayText.DisplayText("+" + time_added_per_kill.ToString() + " secs", lifetime, change, max_scale));
        // TODO: ResultText.display - "+3s"
    }

    /*
    private IEnumerator DisplayTimeUp()
    {
        StartCoroutine(displayText.DisplayText("Time's Up!", lifetime, change, max_scale));
        SceneManager.LoadScene("GameOver");

        yield return null;
    }
    */

    // Display Time Up gameobject and load game over scene
    // TODO: Pause game at this point so that player doesn't die while it is happening
    public IEnumerator DisplayTimeUp()
    {
        TimeUpDisplay.SetActive(true);
        Vector3 startPosition = TimeUpDisplay.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < shake_duration)
        {
            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime / shake_duration);
            TimeUpText.transform.position = startPosition + (UnityEngine.Random.insideUnitSphere * strength); // * strength
            yield return null;
        }

        TimeUpDisplay.transform.position = startPosition;
        SceneManager.LoadScene("GameOver");
    }

    private void OnDestroy()
    {
        // EnemyScript.EnemyDeath -= AddTime;
    }
}
