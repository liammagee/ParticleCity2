using System;
using UnityEngine;
using System.Collections;

[RequireComponent (typeof (GameState))]
public class HUDTime : MonoBehaviour 
{
	
	private dfLabel label;
	private GameState gameState;

	void Start()
	{
		label = GetComponent<dfLabel>();
		if( label == null )
		{
			Debug.LogError( "FPS Counter needs a Label component!" );
		}
		
		gameState = GameObject.Find("Main Camera").GetComponent<GameState>();
	}
	
	void Update()
	{
		label.Text = gameState.GetCurrentTime();
	}
}