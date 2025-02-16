using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    public Slider HealthBar;

    // private float health_bar_width = 162.4f; // Width of full health bar

    // Start is called before the first frame update
    void Start()
    {
        HealthBar = GetComponent<Slider>();

        // ResetHealthBar();
    }

    public void ResetHealthBar()
    {
        HealthBar.value = 1;
    }

    public void UpdateHealthBar(float percent_health)
    {
        HealthBar.value = percent_health;
    }
}
