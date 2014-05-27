using System;
using UnityEngine;
using FiercePlanet;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent (typeof (GameState))]
public class HUDAverageAge : MonoBehaviour 
{
	
	private float startTime;
	private dfLabel label;
	private GameState gameState;
    private ParticleCity particleCity;
	private int counter;

	void Start()
	{
        gameState = GameObject.Find("Main Camera").GetComponent<GameState>();
        particleCity = GameObject.Find("Main Camera").GetComponent<ParticleCity>();
        label = GetComponent<dfLabel>();
		if( label == null )
		{
			Debug.LogError( "FPS Counter needs a Label component!" );
		}
	}
	
	void Update()
	{
		float ct = gameState.CurrentTimeInUnits ();
        List<Agent> agents = particleCity.GetAgents();
        float average = 0;
        if (agents.Count > 0)
		    average = agents.Select(c => c.GetComponent<Agent>()).Where (c => c.enabled == true).Select(c => (ct - c.GetBirthdate())).Average();
		label.Text = "Average age: " + (int)average;
	}
}