using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Test2Script : MonoBehaviour
{

    // Objective:
    // - figure out a way to make on screen text look cool
    // - thinking coroutine which rapidly increases text size, then decreases it, then makes it disappear

    public GameObject ResultTextPrefab;
    private GameObject Canvas;

    public bool start = false;

    public float lifetime;
    public float max_scale; // Note: this variable corresponds only to one axis, but I have both x and y increasing at same rate
    public float change;

    // or just choose a speed? instead of setting specific initial and final sizes

    // Start is called before the first frame update
    void Start()
    {
        Canvas = GameObject.FindGameObjectWithTag("Canvas");
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            start = false;
            StartCoroutine(ChangeTextSize());
        }
    }

    IEnumerator ChangeTextSize()
    {
        // coroutine which instantiates a prefab, rapidly increases text size, then decreases it, then makes it disappear
        GameObject newText = Instantiate(ResultTextPrefab, Canvas.transform);
        // later update this code so the rotation and position are slightly different each time

        newText.GetComponent<TMP_Text>().text = "Boom!";

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
}
