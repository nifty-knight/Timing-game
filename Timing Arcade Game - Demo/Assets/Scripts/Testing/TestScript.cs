using System.Collections;
using System.Collections.Generic;
// using System.Security.Cryptography;
using TMPro;
// using Unity.VisualScripting;
using UnityEngine;

public class TestScript : MonoBehaviour, IDataPersistence
{
    public ShakeScript shakeScript;
    public GameObject ComboTextPrefab;
    public TMP_Text ClickCounter;

    public bool start;

    private int cur_combo = 0;
    public float text_lifetime = 0.4f;
    public float text_speed = 1f;

    private int click_count = 0;

    // Start is called before the first frame update
    void Start()
    {
        ClickCounter.text = "Clicks: " + click_count;
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            start = false;
            StartCoroutine(shakeScript.Shaking());
        }

        // on click - show current combo, have it fade out
        if (Input.GetMouseButtonDown(0))
        {
            click_count++;
            ClickCounter.text = "Clicks: " + click_count;
            cur_combo++;
            StartCoroutine(ShowCombo());
        }
    }

    private IEnumerator ShowCombo()
    {
        // create a new instance of a text object
            // TODO: spawn it in a random location
        // set its text to cur_combo
        // have it move over a certain lifetime
        // fade out
        // destroy the instance

        GameObject newComboText = Instantiate(ComboTextPrefab, GameObject.FindGameObjectWithTag("Canvas").transform);
        newComboText.transform.position = newComboText.transform.position + (Random.insideUnitSphere * 0.33f);
        newComboText.GetComponent<TMP_Text>().text = cur_combo.ToString();

        float elapsedTime = 0f;

        while (elapsedTime < text_lifetime)
        {
            elapsedTime += Time.deltaTime;
            newComboText.transform.Translate(Vector3.right * Time.deltaTime * text_speed);
            newComboText.transform.Translate(Vector3.up * Time.deltaTime * text_speed);
            yield return null;
        }

        Destroy(newComboText);
    }

    /*
    private int PickDir()
    {
        if (Random.Range(0, 2) == 0)
            return 1;
        else return -1;
    }
    */

    /*
    public IEnumerator ShakeEnemy()
    {
            Vector3 startPosition = transform.position;
            float elapsedTime = 0f;

            while (elapsedTime < shake_duration)
            {
                elapsedTime += Time.deltaTime;
                float strength = curve.Evaluate(elapsedTime / shake_duration);
                transform.position = startPosition + (UnityEngine.Random.insideUnitSphere * strength); // * strength
                yield return null;
            }

            transform.position = startPosition;
    }
    */

    public void LoadData(GameData data)
    {
        this.click_count = data.click_count;
    }

    public void SaveData(ref GameData data)
    {
        data.click_count = this.click_count;
    }
}
