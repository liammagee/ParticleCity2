using System;
using UnityEngine;
using System.Collections;

public class HUDAgentCount : MonoBehaviour 
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

		GameObject[] agents = GameObject.FindGameObjectsWithTag("Agent");
		label.Text = "Agent count: " + agents.Length;
		
	}
}