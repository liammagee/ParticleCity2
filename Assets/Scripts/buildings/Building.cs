using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace FiercePlanet {

    public class Building : MonoBehaviour
    {
        // Public variables
        public float health;
        public Vector2 position;

        // Private variables
        Vector3 dimensions;
        float firstConstructed;
        float lastUpdated;
        int uses;

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
    
}

