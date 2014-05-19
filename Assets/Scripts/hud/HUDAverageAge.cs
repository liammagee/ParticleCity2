using System;
using UnityEngine;
using FiercePlanet;
using System.Collections;
using System.Linq;

[RequireComponent (typeof (GameState))]
public class HUDAverageAge : MonoBehaviour 
{
	
	private float startTime;
	private dfLabel label;
	private GameState gameState;
	private int counter;

	void Start()
	{
		gameState = GameObject.Find("Main Camera").GetComponent<GameState>();
		label = GetComponent<dfLabel>();
		if( label == null )
		{
			Debug.LogError( "FPS Counter needs a Label component!" );
		}
	}
	
	void Update()
	{
		float ct = gameState.CurrentTimeInUnits ();
		float average = GameObject.FindGameObjectsWithTag("Agent").Select(c => c.GetComponent<Agent>()).Where (c => c.enabled == true).Select(c => (ct - c.GetBirthdate())).Average();
		label.Text = "Average age: " + (int)average;
	}
}