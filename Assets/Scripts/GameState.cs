
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameState : MonoBehaviour 
{
	// Declare properties
	private static GameState instance;
	
	
	
	
	// ---------------------------------------------------------------------------------------------------
	// gamestate()
	// --------------------------------------------------------------------------------------------------- 
	// Creates an instance of gamestate as a gameobject if an instance does not exist
	// ---------------------------------------------------------------------------------------------------
	public static GameState Instance
	{
		get
		{
			if(instance == null)
			{
				instance = new GameObject("GameState").AddComponent<GameState>();
			}
 
			return instance;
		}
	}	
	
	// Sets the instance to null when the application quits
	public void OnApplicationQuit()
	{
		instance = null;
	}
	// ---------------------------------------------------------------------------------------------------
	

	public int numAgents;
	public int speedAgents;
	public int boundarySize;
	public int chanceOfBuilding;

	public int timeOrigin;
	public string timeUnits;
	public int timeSecondsPerUnit;

	public bool showNetwork;
	public bool showBuildings;
	public bool showUpdatedTerrain;
	public bool showGrid;

	public float originX = 0;
	public float originZ = 0;


	public void Start() {
		if (numAgents == 0)
			numAgents = 10;
		if (speedAgents == 0)
			speedAgents = 10;
		if (boundarySize == 0)
			boundarySize = 10;
		showNetwork = false;
		showBuildings = false;
		showUpdatedTerrain = false;
		showGrid = false;

		InitiateClock();
	}

	private float internalClock;
	private float lastTime;
	private float currentTime;

	public void InitiateClock() 
	{
		internalClock = this.timeOrigin;
		currentTime = Time.time;
		lastTime = Time.time;
	}

	public void AdjustTimeScale(int timeSecondsPerUnit) 
	{
		this.timeSecondsPerUnit = timeSecondsPerUnit;
		UpdateClock();
	}

	public void UpdateClock() 
	{
		currentTime = Time.time;
		float elapsedTime = currentTime - lastTime;
		internalClock += (elapsedTime / (float)this.timeSecondsPerUnit);
		lastTime = currentTime;
	}

	public string GetCurrentTime() 
	{
		UpdateClock();
		string format = System.String.Format( "{0}: {1,10:d}", this.timeUnits, (int)internalClock);
		return format;
	}
}

