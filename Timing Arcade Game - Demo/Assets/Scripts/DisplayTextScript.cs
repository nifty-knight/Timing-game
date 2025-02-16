using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisplayTextScript : MonoBehaviour
{
    public GameObject ResultTextPrefab;

    private GameObject canvas;

    // Variables which control speed, size, lifetime of displayed text for attack hits and blocks
    public float lifetime = 0.6f;
    public static float change = 20; // speed at which text changes size
    public float max_scale = 2.5f; // maximum scale size

    public float shake_duration = 1f;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GameObject.FindGameObjectWithTag("Canvas");
    }

    // Coroutine which produces increasing text size effect; destroys text object after lifetime
    public IEnumerator DisplayText(string text, float lifetime, float change, float max_scale)
    {
        GameObject newText = Instantiate(ResultTextPrefab, canvas.transform);
        Vector3 startPosition = newText.transform.position;
        newText.transform.position = startPosition + (UnityEngine.Random.insideUnitSphere * 0.5f); // changes each spawn location slightly
        newText.GetComponent<TMP_Text>().text = text;
        // later update this code so the rotation and position are slightly different each time

        // newText.GetComponent<TMP_Text>().text = text;

        float timer = 0;
        Vector3 scale_change = new Vector3(change, change, 0);

        while (timer < lifetime)
        {
            timer += Time.deltaTime;
            if (newText.transform.localScale.x < max_scale)
                newText.transform.localScale += (scale_change * Time.deltaTime);
            yield return null;
        }

        Destroy(newText);

    }

    /*
    // Shake Text gameobject with a given text object, duration, and strength curve
    public IEnumerator AlsoShakeText(string text, float lifetime, float change, float max_scale, AnimationCurve curve)
    {
        GameObject newText = Instantiate(ResultTextPrefab, canvas.transform);
        Vector3 startPosition = newText.transform.position;
        newText.transform.position = startPosition + (UnityEngine.Random.insideUnitSphere * 0.33f); // changes each spawn location slightly
        newText.GetComponent<TMP_Text>().text = text;

        float elapsedTime = 0f;

        while (elapsedTime < shake_duration)
        {
            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime / shake_duration);
            newText.transform.position = startPosition + (UnityEngine.Random.insideUnitSphere * strength); // * strength
            yield return null;
        }

        transform.position = startPosition;

        float timer = 0;
        Vector3 scale_change = new Vector3(change, change, 0);

        while (timer < lifetime)
        {
            timer += Time.deltaTime;
            if (newText.transform.localScale.x < max_scale)
                newText.transform.localScale += (scale_change * Time.deltaTime);
            yield return null;
        }

        Destroy(newText);
    }
    */
}
