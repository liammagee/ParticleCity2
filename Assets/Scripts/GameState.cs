
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
	

	// Population settings
	public int numAgents;
	public int speedAgents;
	public float percentageOfNewAgentsPerTimeUnit;
	public float variationOfNewAgents;
	public float chanceOfReproduction;

	// Environment settings
	public int boundarySize;
	public int chanceOfBuilding;

	// Origin of agent placements
	public float originX = 0;
	public float originZ = 0;

	// Checable Settings
	public bool using3d = false;
	public bool showNetwork = false;
	public bool showBuildings;
	public bool showGrid;
	public bool showTerrain;
	public bool showUpdatedTerrain;

	// Time variables
	public int timeOrigin;
	public string timeUnits;
	public int timeSecondsPerUnit;
	private float lastTimeInUnits;
	private float currentTimeInUnits;
	private bool changedTimeUnit;
	private float lastRecordedTime;
	private bool initiated = false;

	public void Awake() {
		initiated = false;
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

//        DontDestroyOnLoad(this);

		InitiateClock();
	}

	public void Update() 
	{
		UpdateClock();
	}

	public void InitiateClock() 
	{
		currentTimeInUnits = timeOrigin;
		lastTimeInUnits = currentTimeInUnits;
		changedTimeUnit = false;
		lastRecordedTime = Time.time;
		initiated = true;
	}
	
	public bool ChangeInTime() 
	{
		return changedTimeUnit;
	}
	
	public float CurrentTimeInUnits() 
	{
		if (!initiated)
			InitiateClock ();
		return currentTimeInUnits;
	}
	
	public float LastTimeInUnits() 
	{
		return lastTimeInUnits;
	}

	public void AdjustTimeScale(int value) 
	{
		timeSecondsPerUnit = value;
	}

	public void UpdateClock() 
	{
		float currentTime = Time.time;
		float elapsedTime = currentTime - lastRecordedTime;
		currentTimeInUnits += (elapsedTime / (float)timeSecondsPerUnit);
		changedTimeUnit = ((int)currentTimeInUnits > (int)lastTimeInUnits);
		lastTimeInUnits = currentTimeInUnits;
		lastRecordedTime = currentTime;
	}

	public string GetCurrentTime() 
	{
		return System.String.Format( "{0}: {1,10}", timeUnits, (int)currentTimeInUnits);
	}
}

