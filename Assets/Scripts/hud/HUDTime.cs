using System;
using UnityEngine;
using System.Collections;

public class HUDTime : MonoBehaviour 
{
	
	private float startTime;
	private dfLabel label;
	private GameState gameState;

	void Start()
	{
		gameState = GameObject.Find("Main Camera").GetComponent<GameState>();
		label = GetComponent<dfLabel>();
		if( label == null )
		{
			Debug.LogError( "FPS Counter needs a Label component!" );
		}
		
		startTime = Time.deltaTime;
	}
	
	void Update()
	{
		label.Text = gameState.GetCurrentTime();
	}
}