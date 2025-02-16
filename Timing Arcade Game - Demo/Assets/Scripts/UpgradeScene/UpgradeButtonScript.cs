using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButtonScript : MonoBehaviour
{
    public string button_id; // Unique identifier for upgrade buttons
    // Format: 000
    // - first char: 0 or 1. 0 = Attack tree, 1 = Defense tree
    // - second char: zero-based position in tree, 0 being the first unlocked button
    // - third char: describes which ability/upgrade the button unlocks - TODO: I'll write a description down somewhere with corresponding numbers


    public string button_name = "";

    [TextArea]
    public string description = "";

    // Start is called before the first frame update
    void Start()
    {
        // gameObject.GetComponent<Button>().onClick.AddListener(CheckButtonClicked);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CheckButtonClicked()
    {
        Debug.Log(button_name + " clicked!");
    }
}
