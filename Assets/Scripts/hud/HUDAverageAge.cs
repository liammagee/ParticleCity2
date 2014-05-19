using System;
using UnityEngine;
using FiercePlanet;
using System.Collections;
using System.Linq;

public class HUDAverageAge : MonoBehaviour 
{
	
	private float startTime;
	private dfLabel label;
	private GameState gameState;
	private int counter;

	void Start()
	{
		label = GetComponent<dfLabel>();
		if( label == null )
		{
			Debug.LogError( "FPS Counter needs a Label component!" );
		}
	}
	
	void Update()
	{
		float average = GameObject.FindGameObjectsWithTag("Agent").Select(c => c.GetComponent<Agent>().GetAge()).Average();

		label.Text = "Average age: " + average;
		
	}
}