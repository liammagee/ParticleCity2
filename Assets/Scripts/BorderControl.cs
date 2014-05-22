using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BorderControl : MonoBehaviour 
{
	public void RedoBorders(float value) 
	{
		GameObject bottom = GameObject.Find("CubeBottomBorder");
		GameObject left = GameObject.Find("CubeLeftBorder");
        GameObject right = GameObject.Find("CubeRightBorder");
        GameObject top = GameObject.Find("CubeTopBorder");
        GameObject down = GameObject.Find("CubeDownBorder");
        GameObject up = GameObject.Find("CubeUpBorder");
		Grid grid = GameObject.Find ("GridOrigin").GetComponent<Grid>();
		TerrainData terrainData = grid.GetTerrain().terrainData; 
		Vector3 terrainSize = terrainData.size;
		float terrainX = (terrainSize.x / 2f);
		float terrainY = (terrainSize.x / 2f);
        float terrainZ = (terrainSize.z / 2f);
		float adjustPercent = value / 100f;
		float heightOffset = 450f;
		float wallWidth = 1.0f;
		float wallHeight = 1000.0f;
		bottom.transform.position = new Vector3(0, 0, -terrainZ * adjustPercent);
		bottom.transform.localScale = new Vector3(terrainX * 2 * adjustPercent, terrainY * 2 * adjustPercent, wallWidth);
		left.transform.position = new Vector3(-terrainX * adjustPercent, 0, 0);
		left.transform.localScale = new Vector3(wallWidth, terrainY * 2 * adjustPercent, terrainZ * 2 * adjustPercent);
        right.transform.position = new Vector3(terrainX * adjustPercent, 0, 0);
        right.transform.localScale = new Vector3(wallWidth, terrainY * 2 * adjustPercent, terrainZ * 2 * adjustPercent);
        top.transform.position = new Vector3(0, 0, terrainZ * adjustPercent);
        top.transform.localScale = new Vector3(terrainX * 2 * adjustPercent, terrainY * 2 * adjustPercent, wallWidth);
        down.transform.position = new Vector3(0, -terrainY * adjustPercent, 0);
        down.transform.localScale = new Vector3(terrainX * 2 * adjustPercent, wallWidth, terrainZ * 2 * adjustPercent);
        up.transform.position = new Vector3(0, terrainY * adjustPercent, 0);
        up.transform.localScale = new Vector3(terrainX * 2 * adjustPercent, wallWidth, terrainZ * 2 * adjustPercent);

//		Grid grid = GameObject.Find ("GridOrigin").GetComponent<Grid>();
//        GameState gameState = GameObject.Find ("Main Camera").GetComponent<GameState>();
//		TerrainData terrainData = grid.GetTerrain().terrainData; 
//		Vector3 terrainSize = terrainData.size;
//		float adjustPercent = value / 100f;
//		float newDimension = terrainSize.x * adjustPercent;
//		gameObject.transform.localScale = new Vector3(newDimension, newDimension, newDimension);
//        gameObject.transform.position  = new Vector3(gameState.originX, 0, gameState.originZ);

		// Ensure agents are inside the bordrs
		foreach (GameObject agent in GameObject.FindGameObjectsWithTag("Agent")) {
			Vector3 newPosition = agent.transform.position;
			if (newPosition.x < -value * 10)
				newPosition.x = -value * 10 + 101;
			if (newPosition.x > value * 10)
				newPosition.x = value * 10 - 101;
			if (newPosition.z < -value * 10)
				newPosition.z = -value * 10 + 101;
			if (newPosition.z > value * 10)
				newPosition.z = value * 10 - 101;
			if (newPosition.y < -value * 10)
				newPosition.y = -value * 10 + 101;
			if (newPosition.y > value * 10)
				newPosition.y = value * 10 - 101;
			agent.transform.position = newPosition;
		}
	}
}
