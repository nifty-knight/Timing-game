using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyScript : MonoBehaviour
{
    public static event Action<float> EnemyAttack; // Event where enemy attacks, parameter is the enemy's damage
    public static event Action EnemyDeath;

    public Slider HealthBar;

    public AnimationCurve curve;
    public float shake_duration = 0.4f;

    // public bool start; // Used to test ShakeEnemy function

    private Animator animator; // animator controller component on enemy gameobject

    private Enemy current_enemy;

    private float min_attack_time = 1.2f;
    public float attack_time; // Set amount of time between attacks
    public float actual_attack_time; // Slightly randomized time between attacks based on attack_time (constant variable) // TODO: CHANGE THIS VARIABLE BACK TO PRIVATE WHEN DONE TESTING
    private float timer; // keeps track of time
    private bool attacking; // True if enemy is currently attacking

    private bool stunned = false; // Whether or not enemy is stunned or not
    private float stun_time = 2f; // Amount of time enemy is stunned - note - how can this be set more dynamically/change as game goes on?
    private float stun_timer = 0f; // keeps track of how long enemy has been stunned

    private float total_health;
    private float current_health;

    private bool dead = false;

    private bool currently_shaking = false; // True if ShakeEnemy coroutine is currently running

    private float attack_time_decrease = 0.002f;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        ComboBarScript.DealDamage += PlayHurtAnim;
        ComboBarScript.DealDamage += TakeDamage;
        PlayerScript.StunEnemy += StunEnemy;

        if ((current_enemy.AtkTimer - (EnemySpawnerScript.enemies_killed * attack_time_decrease)) < min_attack_time)
            attack_time = min_attack_time;
        else
            attack_time = current_enemy.AtkTimer - (EnemySpawnerScript.enemies_killed * attack_time_decrease);

        ResetAttackTime();
        timer = UnityEngine.Random.Range(0f, actual_attack_time - 0.2f);
        total_health = current_enemy.Health + (EnemySpawnerScript.enemies_killed * 4f); // directly proportional to number of enemies
        current_health = total_health;
        dead = false;
        attacking = false;

        HealthBar.value = 1;

        ReturnToIdleAnim();

        // WHAT YOU ARE CURRENTLY DOING:
            // TEST GAME TO SEE IF ENEMY DIES, A NEW ENEMY SPAWNS
        // Next up:
            // TODO: Create EnemyDeath event, make spawner spawn new random enemy on event trigger - attach event trigger to end of death animation
            // TODO: enemy loses health when PlayerAttack is triggered
            // TODO: on enemy death, play death animation and spawn new enemy - attach event to enemydeath animation
    }

    // Update is called once per frame
    void Update()
    {

        if (!stunned)
        {
            if (timer >= actual_attack_time)
            {
                if (!attacking)
                {
                    timer = 0f;
                    ResetAttackTime();
                    PlayAttackAnim();
                    attacking = true;
                }
                else
                {
                    timer = 0f;
                    ResetAttackTime();
                }
            }
            else
                timer += Time.deltaTime;
        }

        if (stunned)
        {
            if (stun_timer >= stun_time)
            {
                stunned = false;
                stun_timer = 0f;
                ReturnToIdleAnim();
            }
            else
                stun_timer += Time.deltaTime;
        }
    }

    // Reduce enemy current_health, play death animation if current_health <= 0
    public void TakeDamage(float player_damage)
    {
        current_health -= player_damage;
        UpdateHealthBar();
        
        if (!dead && (current_health <= 0))
        {
            PlayDeathAnim();
            Destroy(HealthBar.gameObject);
           //  PlayerScript.player_input_enabled = false; -> change player_input_enabled to static to use this line
            dead = true;
        }
    }

    // Set actual_attack_time to a randomized value within 0.3 or 0.4f of attack_time
    private void ResetAttackTime()
    {
        actual_attack_time = UnityEngine.Random.Range(attack_time - 0.3f, attack_time + 0.4f);
    }

    // Triggers EnemyAttack event on a certain frame of enemy attack
    public void TriggerEnemyAttack()
    {
        EnemyAttack?.Invoke(current_enemy.Damage);
    }

    // Trigger EnemyDeath event and destroy the enemy gameObject - this function is called at the end of the death animation
    public void TriggerEnemyDeath()
    {
        EnemySpawnerScript.enemies_killed++;
        EnemyDeath?.Invoke();
        // PlayerScript.player_input_enabled = true; -> change player_input_enabled to static to use this line
        Destroy(gameObject);
    }

    // Set up parameters to make enemy stunned
    public void StunEnemy()
    {
        stunned = true;
        PlayStunAnim();
    }

    // Shake Enemy on critical hit - called by ComboBarScript in CalculateDamage
    public IEnumerator ShakeEnemy()
    {
        if (!dead && !currently_shaking)
        {
            currently_shaking = true;
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
            currently_shaking = false;
        }
        else
            yield return null;
    }

    // Plays enemy attack animation
    private void PlayAttackAnim()
    {
        if ((current_enemy != null) && !dead)
            animator.Play(current_enemy.Name + "Attack");

    }

    // Plays goblin hurt animation whenever player deals damage
    private void PlayHurtAnim(float dmg)
    {
        if ((current_enemy != null) && !dead) 
            animator.Play(current_enemy.Name + "Hurt");
    }

    // Plays the stun animation
    private void PlayStunAnim()
    {
        if ((current_enemy != null) && !dead)
        { 
            animator.Play(current_enemy.Name + "Stun"); 
        }
    }

    // Plays goblin death animation
    private void PlayDeathAnim()
    {
        if ((current_enemy != null) && !dead) 
            animator.Play(current_enemy.Name + "Death");
    }

    // Set as an event for attack anim so enemy automatically returns to idle after one animation loop
    private void ReturnToIdleAnim()
    {
        if ((current_enemy != null) && !dead) 
            animator.Play(current_enemy.Name + "Idle");
        attacking = false;
    }

    // Sets Enemy to the correct enemy class - called by spawner
    public void SetEnemyType(Enemy enemy)
    {
        current_enemy = enemy;
    }

    // Updates Enemy Health Bar
    public void UpdateHealthBar()
    {
        HealthBar.value = current_health / total_health;
    }

    // Unsubscribes functions to events when the scene gets destroyed
    private void OnDestroy()
    {
        // ComboBarScript.DealDamage -= PlayHurtAnim;
        // ComboBarScript.DealDamage -= TakeDamage;
        PlayerScript.StunEnemy -= StunEnemy;
        ComboBarScript.DealDamage -= PlayHurtAnim;
        ComboBarScript.DealDamage -= TakeDamage;
    }
}
