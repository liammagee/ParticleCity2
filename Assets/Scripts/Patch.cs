using UnityEngine;
using FiercePlanet;
using System.Collections;
using System.Collections.Generic;

public class Patch
{
    public float oldHealth;
    public float health;
    public Vector2 position;
    public int visits;
    public float lastVisit;
    public Agent lastAgent;
    public bool significantChange = false;
    public static float healthLossPerVisit = -0.5f;
    public static int recuperateRate = 2;


    public Patch(Vector2 position) 
    {
        this.position = position;
        this.health = 100f;
        lastVisit = Time.time;
    }
    
    public bool visit() 
    {
        visits++;
        lastVisit = Time.time;
        oldHealth = health;
        health += Patch.healthLossPerVisit;
        return (health % 10 == 0f);
    }

    public void recuperate() 
    {
        if (health < 100 - Patch.recuperateRate)
        {
            oldHealth = health;
            health += Patch.recuperateRate;
        }
    }

    public bool SignificantChange() 
    {
        return (Mathf.FloorToInt(oldHealth / 10f) !=  Mathf.FloorToInt(health / 10f));
    }
}

