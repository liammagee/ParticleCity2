
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
	}

	public int getNumAgents() {
		return numAgents;
	}

	public int getSpeedAgents() {
		return speedAgents;
	}
}

