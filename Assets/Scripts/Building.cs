using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Building : MonoBehaviour
{
    Vector3 dimensions;
    float firstConstructed;
    float lastUpdated;
    int uses;
    float health;
    public Vector2 position;

    public void Start() 
	{
        dimensions = new Vector3(1f, 1f, 1f);
        firstConstructed = Time.time;
        uses = 0;
        health = 100;
	}


    public void visit() 
    {
        lastUpdated = Time.time;
        uses++;
    }

    public void disintegrate()
    {
        if (health > 0)
            health -= (uses / 100);
        uses = 0;
    }
}

