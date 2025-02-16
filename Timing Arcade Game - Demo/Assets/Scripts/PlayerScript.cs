using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerScript : MonoBehaviour
{
    public static event Action PlayerAttack; // triggered when player presses attack button
    public static event Action PlayerBlock; // triggered when player presses block button
    public static event Action StunEnemy; // triggered when player does a perfect block

    // public TMP_Text ResultText;
    public HealthBarScript healthBarScript;
    public ComboBarScript comboBarScript;
    public DisplayTextScript displayText;

    private float total_health = 100; // Player's total health
    private float current_health; // Player's current health

    private Animator animator; // animator controller component on player
    private bool right_hand; // Variable to switch between left and right jabs

    private bool blocking; // true if player is currently blocking 
    private float block_timer; // Timer which measures how long player has been blocking
    private float perfect_parry = 0.1f; // Time window for perfect parry

    private float damage_blocked = 0.95f; // Damage blocked by imperfect shielding - it is a percentage of enemy damage blocked
    // TODO 11: Add possibility for critical hits for enemies

    private bool player_input_enabled; // true if player can press buttons (and have them do something)
    private bool player_stunned; // True if player is currently playing hurt animation

    // Variables which control speed, size, lifetime of displayed text for blocks
    public float lifetime = 0.6f;
    private float size_change; // speed at which text changes size
    public float max_scale = 2.5f; // maximum scale size

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();

        PlayerAttack += PlayAttackAnim;
        PlayerBlock += PlayBlockAnim;
        EnemyScript.EnemyAttack += TakeDamage;
        EnemyScript.EnemyDeath += UpdateDamageBlocked;

        current_health = total_health;
        size_change = DisplayTextScript.change;
        player_input_enabled = true;
        player_stunned = false;
        healthBarScript.ResetHealthBar();
    }

    // Update is called once per frame
    void Update()
    {
        // TODO 0: PARRY & BLOCK SYSTEM
        // what I'm thinking: every time enemy attacks, triggers enemy attack event
        // PlayerScript.TakeDamage() runs every time enemy attack happens
        // the script decides what to do based on if player is attacking, blocking, parrying, doing nothing, etc.

        // TODO 1: introduce a delay so that player can't take action for a little bit even after block is finished - lag which always occurs unless perfect parry happens

        if (blocking)
        {
            block_timer += Time.deltaTime;
        }

    }

    // Causes various effects on player depending on what player is doing when EnemyAttack is triggered
    // TODO: create text to say the debug.log words - not necessarily all of them or at all in final version
    // TODO: create more visual effects
    // - flash white when hurt so that it's more obvious
    // - create visual block effect - transparent shield?
    // - create visual parry effect - screen shake, enemy shaken?
    private void TakeDamage(float enemy_damage)
    {
        if (!player_stunned)
        {
            if (block_timer == 0)
            {
                StartCoroutine(displayText.DisplayText("Player Hurt!", lifetime, size_change, max_scale));
                animator.Play("PlayerHurt");
                current_health -= enemy_damage;
                healthBarScript.UpdateHealthBar(current_health / total_health);
                comboBarScript.BlockEndCombo();
            }
            else if (block_timer <= perfect_parry)
            {
                StartCoroutine(displayText.DisplayText("Perfect Block!", lifetime, size_change, max_scale));
                StunEnemy?.Invoke();
            }
            else
            {
                StartCoroutine(displayText.DisplayText("Blocked!", lifetime, size_change, max_scale));
                current_health -= enemy_damage * (1 - damage_blocked);
                healthBarScript.UpdateHealthBar(current_health / total_health);
            }

            if (current_health <= 0)
            {
                SceneManager.LoadScene("GameOver");
            }
        }
    }

    // Triggers the PlayerAttack event; attached to AttackButton & runs when AttackButton is pressed
    public void AttackButtonPressed()
    {
        if (player_input_enabled) 
        { 
            PlayerAttack?.Invoke(); 
        }
    }

    // Triggers the PlayerBlock event; attached to BlockButton, runs when BlockButton is pressed
    public void BlockButtonPressed()
    {
        if (player_input_enabled)
        {
            blocking = true;
            PlayerBlock?.Invoke();
        }
    }

    // Enables player input
    public void EnablePlayerInput()
    {
        player_input_enabled = true;
    }

    // Disables player input so when they press buttons, nothing happens - meant to prevent block from being interrupted
    public void DisablePlayerInput()
    {
        player_input_enabled = false;
    }

    private void UpdateDamageBlocked()
    {
        damage_blocked = 0.98f - (EnemySpawnerScript.enemies_killed * 0.03f);
    }

    // Resets block_timer to zero once player has finished blocking - attached as event to PlayerBlock animation
    public void ResetBlockTimer()
    {
        blocking = false;
        block_timer = 0;
    }

    // Plays the player attack animation; switches between right and left every punch
    private void PlayAttackAnim()
    {
        if (right_hand)
        {
            animator.Play("PlayerRightJab");
        }
        else
        {
            animator.Play("PlayerLeftJab");
        }

        right_hand = !right_hand;

    }

    // Players player hurt animation - happens when player doesn't block
    private void PlayHurtAnim()
    {
        animator.Play("PlayerHurt");
        player_stunned = true;
    }

    // Plays player block animation
    // TODO 5: add special effects for perfect guard/counter/parry - maybe in ComboBarScript.TakeDamage
    private void PlayBlockAnim()
    {
        animator.Play("PlayerBlock");
    }

    // Set as an event for player attack and block anims so player automatically returns to idle after one animation loop
    private void ReturnToIdleAnim()
    {
        animator.Play("PlayerIdle");
        player_stunned = false;
    }

    // Unsubscribes functions to events when the scene gets destroyed
    private void OnDestroy()
    {
        PlayerAttack -= PlayAttackAnim;
        PlayerBlock -= PlayBlockAnim;
        EnemyScript.EnemyAttack -= TakeDamage;
        EnemyScript.EnemyDeath -= UpdateDamageBlocked;
    }
}
