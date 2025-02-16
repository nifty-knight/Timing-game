using System;
using System.Collections;
using System.Collections.Generic;
// using UnityEditorInternal.Profiling.Memory.Experimental.FileFormat;
using UnityEngine;
using UnityEngine.UI;

public class EnemySpawnerScript : MonoBehaviour
{

    public GameObject EnemyPrefab; // Base gameobject which contains all necessary components for each enemy
    public GameObject EnemyHealthBarPrefab;
    public ComboBarScript comboBarScript;
    public GameObject Canvas;

    public static int enemies_killed; // Number of enemies killed in a round

    public float healthbar_x;
    public float healthbar_y;

    private bool level2;
    private bool level3;
    private bool level4;

    // Enemies
    private Enemy Goblin = new Enemy("Goblin", 15, 30, 3f);
    private Enemy FlyingEye = new Enemy("FlyingEye", 5, 26, 2.2f); // - Change flying eye attack time to 1.2f later
    private Enemy Mushroom = new Enemy("Mushroom", 9, 45, 2.6f);
    private Enemy SkeletonWarrior = new Enemy("SkeletonWarrior", 21, 60, 4);
    private Enemy LittleMonster = new Enemy("LittleMonster", 2, 30, 3.5f);
    private Enemy Statue = new Enemy("Statue", 25, 70, 6f);
    private Enemy Minotaur = new Enemy("Minotaur", 18, 40, 2.4f);
    private Enemy HammerGoblin = new Enemy("HammerGoblin", 10, 26, 3.8f);
    private Enemy OrcWarrior = new Enemy("OrcWarrior", 15, 35, 2.8f);
    private Enemy OrcMage = new Enemy("OrcMage", 17, 30, 3.1f);

    // private Enemy Slime = new Enemy("Slime", 10, 15, 1.5f); - animations for slime suck

    private List<Enemy> SpawnList = new List<Enemy>();

    // Start is called before the first frame update
    void Start()
    {
        level2 = false;
        level3 = false;
        level4 = false;

        enemies_killed = 0;

        // Starting Enemies (level 1):
        SpawnList.Add(FlyingEye);
        SpawnList.Add(SkeletonWarrior);
        SpawnList.Add(HammerGoblin);
        SpawnList.Add(OrcWarrior);
        // Adding starting enemies to spawn list
                
        EnemyScript.EnemyDeath += SpawnEnemy;

        // Spawn first enemy
        SpawnEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (enemies_killed >= 7 && !level2)
        {
            level2 = true;
            ActivateLevel2();
        }
        else if (enemies_killed >= 15 && !level3)
        {
            level3 = true;
            ActivateLevel3();
        }
        else if (enemies_killed >= 24 && !level4)
        {
            level4 = true;
        }
    }

    private void SpawnEnemy()
    {
        GameObject newEnemy = Instantiate(EnemyPrefab, transform.position, transform.rotation);
        // TODO: instantiate healthbar, set it as healthbar on enemyScript
        GameObject newHealthBar = Instantiate(EnemyHealthBarPrefab, Canvas.transform);
        // newHealthBar.SetActive(true);
        // newHealthBar.transform.position = new Vector3(healthbar_x, healthbar_y, 0);

        var enemy = SpawnList[UnityEngine.Random.Range(0, SpawnList.Count)];
        newEnemy.GetComponent<EnemyScript>().SetEnemyType(enemy);
        newEnemy.GetComponent<EnemyScript>().HealthBar = newHealthBar.GetComponent<Slider>();
        comboBarScript.SetEnemyInstance(newEnemy.GetComponent<EnemyScript>());
        
    }

    private void ActivateLevel2()
    {
        SpawnList.Add(LittleMonster);
        SpawnList.Add(Minotaur);
    }

    private void ActivateLevel3()
    {
        SpawnList.Add(Statue);
        SpawnList.Add(Goblin);
    }

    private void ActivateLevel4()
    {
        SpawnList.Add(OrcMage);
        SpawnList.Add(Mushroom);
    }

    // Unsubscribes all functions from all events when this object gets destroved
    private void OnDestroy()
    {
        EnemyScript.EnemyDeath -= SpawnEnemy;
    }
}
