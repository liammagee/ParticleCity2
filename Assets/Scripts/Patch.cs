using UnityEngine;
using FiercePlanet;
using System.Collections;
using System.Collections.Generic;

public class Patch
{
	public float health;
    public Vector2 position;
    public int visits;
    public float lastVisit;
    public Agent lastAgent;
    public static float healthLossPerVisit = -0.5f;
    public static int recuperateRate = 1;


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
        health += Patch.healthLossPerVisit;
        return (health % 10 == 0f);
    }

    public void recuperate() 
    {
        if (health < 100)
            health += Patch.recuperateRate;
    }
}

