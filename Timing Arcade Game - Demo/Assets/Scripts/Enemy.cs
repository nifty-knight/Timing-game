using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Enemy
{
    public string Name; // need to put 'new' to avoid warning - the base for all unity classes also includes a member called name

    public float Damage; // amount of damage enemy does

    public float Health; // amount of health enemy starts with

    public float AtkTimer; // time between attacks
    // TODO: later, update this so enemies can attack in a range of times

    public Enemy(string name, float damage, float health, float atkTimer)
    {
        this.Name = name;
        this.Damage = damage;
        this.Health = health;
        this.AtkTimer = atkTimer;
    }

}
