using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class ComboBarScript : MonoBehaviour
{
    public static event Action<float> DealDamage; // Event triggered when player successfully deals damage to enemy

    private GameObject ComboBar;
    public GameObject Slider;
    public GameObject GoalBar;
    // public ParticleSystem HitEffect;
    public TMP_Text ComboCounter;
    //  public TMP_Text ResultText;
    public GameObject ComboTextPrefab;
    public DisplayTextScript displayText;
    // public GameObject ResultTextPrefab;

    //  private GameObject canvas;



    // Variables to control combotext lifetime and speed
    private float text_lifetime = 0.5f;
    private float text_speed = 1f;

    private EnemyScript enemyScript;

    private bool is_active; // Whether or not the combobar is active or not; starts not active
    private float hit_range = 0.15f; // maximum distance (as a percentage of combobar) from goalbar to get a hit + continue combo
    private float critical_hit_range; // max percent distance of combobar from goalbar to get a critical hit (currently set in Start() to 1/3 of hit_range)
    private float good_hit_range; // max percent distance of combobar from goalbar to get a good hit (currently set in Start() to 60% of hit_range)

    private int dir = 1; // Current direction of slider (1 or -1); Starts to the right
    private float start_speed = 3.0f; // speed slider starts at on every reset (increases as combo increases)
    private float cur_speed; // current speed of slider when activated
    private float speed_increase = 0.5f; // speed increase multiplier - affects how much faster slider gets after each successive combo

    // Starting coordinates for slider
    private float slider_start_x = -2.08f; 
    private float slider_end_x = 2.08f; // end x-coord of slider, where slider stops

    // Starting coordinate range for goalbar
    private float goalbar_min_x = -1.8f; // Farthest position to the left where goalbar can spawn
    private float goalbar_max_x = 2f; // Farthest position to the right where goal bar can spawn
    // private float current_range = 0.3f; // When used, causes goal bar to only spawn within a percentage of its range (more on right side) - controls difficulty

    private int cur_combo; // Current combo count
    private int max_combo; // Highest achieved combo
    private bool combo_ended = false; // True if player tapped close enough to goal bar to extend combo

    public float combobar_length = 4.08f; // length of combo bar; NOT attached to object, must be MANUALLY UPDATED

    private Vector3 startPosition; // start position of ResultText; initialized in Start

    private float base_damage = 8; // Base damage of player - FIND A BETTER WAY TO IMPLEMENT THIS - goes in playerscript???
    float critical_hit_multiplier = 2; // damage multiplier for critical hits
    float weak_hit_multiplier = 0.5f; // damage multiplier for weak hits
    float combo_multiplier = 0.5f; // multiplier which increases damage based on combo number

    // Variables which control speed, size, lifetime of displayed text for attacks
    public float lifetime = 0.65f;
    private float size_change; // speed at which text changes size - set to same as change in DisplayTextScript in Start()
    public float max_scale = 2.5f; // maximum scale size
    public float critical_max_scale = 3;

    // Start is called before the first frame update
    void Start()
    {
        critical_hit_range = hit_range / 3;
        good_hit_range = hit_range * 0.6f;
        combo_multiplier = (cur_combo * 0.4f) + 0.6f; // Can fine-tune these numbers/design
        size_change = DisplayTextScript.change;
        ComboBar = this.GameObject();
        // canvas = GameObject.FindGameObjectWithTag("Canvas");

        // enemies_killed = 0;
        is_active = false;
        Slider.transform.position = new Vector3(slider_start_x, Slider.transform.position.y, Slider.transform.position.z); // Reset Slider position (may put in reset function? or next-combo and/or activate)
        UpdateComboCounter(false);
        ComboBar.GetComponent<SpriteRenderer>().enabled = true;
        Slider.GetComponent<SpriteRenderer>().enabled = is_active;
        GoalBar.GetComponent<SpriteRenderer>().enabled = is_active;

        PlayerScript.PlayerAttack += NextCombo;
        PlayerScript.PlayerAttack += Activate;
        PlayerScript.PlayerBlock += BlockEndCombo;
        // EnemyScript.EnemyDeath += AddToKillCount;

    }

    // Update is called once per frame
    void Update()
    {
        if (is_active)
        {
            UpdateSlider(cur_speed);
            /*
            if (Input.GetMouseButtonDown(0))
            {
                NextCombo(Mathf.Abs(Slider.transform.position.x - GoalBar.transform.position.x));
            }
            */
        }
    }

    // Sets up the combo bar
    private void Activate()
    {
        if (combo_ended)
            // Debug.Log("Combo ended!");
            combo_ended = false;
        else if (!is_active)
        {
            is_active = true;

            // ComboBar.GetComponent<SpriteRenderer>().enabled = is_active;
            Slider.GetComponent<SpriteRenderer>().enabled = is_active;
            GoalBar.GetComponent<SpriteRenderer>().enabled = is_active;
            Slider.transform.position = new Vector3(slider_start_x, Slider.transform.position.y, Slider.transform.position.z);
            cur_speed = start_speed + (speed_increase * cur_combo); // TODO: NEED TO FIND A BETTER/more nuanced WAY TO INCREASE THIS - use ln or log?
            SetGoalBarPos();
            // DealDamage?.Invoke();
        }

    }

    // End combo or activate next one, depending on player action
    // Takes in a number between [0, 1]: closer to zero the number is, higher the damage, better the score, etc.
    private void NextCombo()
    {
        var hit_dist = Mathf.Abs(Slider.transform.position.x - GoalBar.transform.position.x);

        // Debug.Log("Set up NextCombo!");

        // if (hit_dist < hit_range)
        // CalculateDamage(hit_dist), ShowText(hit_dist), UpdateComboCounter(true), Activate()
        // otherwise, end combo: show fail text (Missed!), UpdateComboCounter(false), hide combobar, set is_active = false
        if (is_active)
        {
            is_active = false;

            if (hit_dist < ConvertToDistance(hit_range))
            {
                // Debug.Log("Successful hit!");
                combo_ended = false;
                // HitEffect.Play(); TODO: figure out why particle effect is not working
                UpdateComboCounter(true);
                DealDamage?.Invoke(CalculateDamage(hit_dist));
                StartCoroutine(ShowCombo());
            }
            else
            {
                // Debug.Log("Fail!");
                // is_active = false;
                combo_ended = true;
                cur_speed = 0;
                UpdateComboCounter(false);
                // ComboBar.GetComponent<SpriteRenderer>().enabled = is_active;
                Slider.GetComponent<SpriteRenderer>().enabled = is_active;
                GoalBar.GetComponent<SpriteRenderer>().enabled = is_active;
                // ResultText.enabled = false;
            }

            ShowText(hit_dist);
        }

    }

    // Sets all parameters and gameobjects to correct state for when combo has ended by blocking
    public void BlockEndCombo()
    {
        is_active = false;
        combo_ended = false;
        cur_speed = 0;
        UpdateComboCounter(false);
        // ComboBar.GetComponent<SpriteRenderer>().enabled = is_active;
        Slider.GetComponent<SpriteRenderer>().enabled = is_active;
        GoalBar.GetComponent<SpriteRenderer>().enabled = is_active;
        // ResultText.enabled = false;
    }

    // Converts p into distance from goalbar as that percentage of the length of ComboBar
    private float ConvertToDistance(float p)
    {
        return p * combobar_length;
    }

    // Sets position of goalbar randomly within range [goalbar_min_x, goalbar_max_x]
    private void SetGoalBarPos()
    {
        GoalBar.transform.position = new Vector3(UnityEngine.Random.Range(goalbar_min_x, goalbar_max_x), GoalBar.transform.position.y, GoalBar.transform.position.z);
    }

    // Update position of slider based on its speed, or set speed to zero, trigger event when it reaches the end
    // Takes in current speed of the slider
    // TODO 0: Change design so that combo ends on timer, not on slider hit end bar
    private void UpdateSlider(float cur_speed)
    {
        var pos = Slider.transform.position.x;

        // if outside bounds, change direction and reset slider position to be within boundary on the end
        if ((pos < slider_start_x) ||  (pos > slider_end_x))
        {
            dir = -dir;  

            if (pos < slider_start_x)
                Slider.transform.position = new Vector3(slider_start_x, Slider.transform.position.y, Slider.transform.position.z);
            else
                Slider.transform.position = new Vector3(slider_end_x, Slider.transform.position.y, Slider.transform.position.z);
        }

        Slider.transform.Translate(Vector3.right * Time.deltaTime * cur_speed * dir);
    }

    // Show different text onscreen for a limited time (running a coroutine?) depending on player timing accuracy
    // TODO 3: have the text appear within a random range and with a limited range of random rotation
    private void ShowText(float hit_dist)
    {
        var hit_range_dist = ConvertToDistance(hit_range);
        var crit_hit_dist = ConvertToDistance(critical_hit_range);
        var good_hit_dist = ConvertToDistance(good_hit_range);

        // ResultText.enabled = true;

        // Set ResultText.transform to slightly random positions and rotations
        // ResultText.transform.position = startPosition + (UnityEngine.Random.insideUnitSphere * 0.33f); // changes each spawn location slightly
        // ResultText.transform.Rotate(0, 0, UnityEngine.Random.Range(-6f, 6f), Space.Self);

        if (hit_dist < hit_range_dist)
        {
            if (hit_dist < crit_hit_dist) // if hit within good_hit_range
            {
                StartCoroutine(enemyScript.ShakeEnemy());
                StartCoroutine(displayText.DisplayText("CRITICAL Hit!", lifetime, size_change, critical_max_scale));
            } 
            else if (hit_dist < good_hit_dist)
            {
                StartCoroutine(displayText.DisplayText("Good Hit!", lifetime, size_change, max_scale));
            }
            else
            {
                StartCoroutine(displayText.DisplayText("Weak Hit!", lifetime, size_change, max_scale));
            }
        }
        else
        {
            StartCoroutine(displayText.DisplayText("Missed!", lifetime, size_change, max_scale));
        }
    }

    /*
    // Coroutine which produces increasing text size effect; destroys text object after lifetime
    IEnumerator DisplayText(string text)
    {
        GameObject newText = Instantiate(ResultTextPrefab, canvas.transform);
        Vector3 startPosition = newText.transform.position;
        newText.transform.position = startPosition + (UnityEngine.Random.insideUnitSphere * 0.33f); // changes each spawn location slightly
        newText.GetComponent<TMP_Text>().text = text;
        // later update this code so the rotation and position are slightly different each time

        newText.GetComponent<TMP_Text>().text = text;

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

    // Create moving text on screen which displays current combo number - only happens for nonzero cur_combo
    // Spawns new text object in canvas in a slightly random location, moves it over text_lifetime then destroys it at the end
    // Can have multiple of these running at once
    private IEnumerator ShowCombo()
    {
        
        if (cur_combo != 0)
        {
            GameObject newComboText = Instantiate(ComboTextPrefab, GameObject.FindGameObjectWithTag("Canvas").transform);
            newComboText.transform.position = newComboText.transform.position + (UnityEngine.Random.insideUnitSphere * 0.33f);
            newComboText.GetComponent<TMP_Text>().text = "x" + cur_combo.ToString();

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
        else 
            yield return null;
    }

    // Calculate damage dealt by player based on how accurate their timing is
    // Takes in a float: hit_dist is distance away from the goal bar
    // Proposed Design: Base damage (static for the round) + accuracy damage (percent away from goalbar = percent additional damage of base dmg)
    // OR: simpler- if critical hit, x2 base damage, if good hit, x1 damage, if weak hit, x0.5 damage
    private float CalculateDamage(float hit_dist)
    {
        var hit_range_dist = ConvertToDistance(hit_range);
        var crit_hit_dist = ConvertToDistance(critical_hit_range);
        var good_hit_dist = ConvertToDistance(good_hit_range);

        if (hit_dist < hit_range_dist)
        {
            if (hit_dist < crit_hit_dist) // if hit within good_hit_range
            {
                // if (enemyScript != null) { StartCoroutine(enemyScript.ShakeEnemy()); }
                return critical_hit_multiplier * base_damage * combo_multiplier * cur_combo;
            }
            else if (hit_dist < good_hit_dist)
            {
                return base_damage * combo_multiplier * cur_combo;
            }
            else
            {
                return weak_hit_multiplier * base_damage * combo_multiplier * cur_combo;
            }
        }
        else
            return 0;
    }

    // Update current and max combo counters based on result of combo
    // Takes in a boolean: true if combo continues, false if combo is ended
    private void UpdateComboCounter(bool result)
    {
        if (result)
        { 
            cur_combo++;
            if (cur_combo > max_combo)
            { 
                max_combo = cur_combo;
                // MaxComboCounter.text = "Max: " + max_combo.ToString();
            }
        }
        else
            cur_combo = 0;

        ComboCounter.text = "Kill Count: " + EnemySpawnerScript.enemies_killed.ToString() + " Max Combo: " + max_combo.ToString();
    }

    // Sets enemyScript to the script attached to the current enemy gameObject;
    // Called by EnemySpawnerScript when spawning a new enemy
    public void SetEnemyInstance(EnemyScript script)
    {
        enemyScript = script;
    }
    // Unsubscribes functions to events when the scene gets destroyed
    private void OnDestroy()
    {
        PlayerScript.PlayerAttack -= NextCombo;
        PlayerScript.PlayerAttack -= Activate;
        PlayerScript.PlayerBlock -= BlockEndCombo;
        // EnemyScript.EnemyDeath -= AddToKillCount;
    }

}
