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
        GameState gameState = GameObject.Find ("Main Camera").GetComponent<GameState>();
		TerrainData terrainData = grid.GetTerrain().terrainData; 
		Vector3 terrainSize = terrainData.size;
		
		float adjustPercent = value / 100f;
        float offsetX = gameState.originX;
        float offsetY = gameState.originZ;
        float terrainX = (terrainSize.x / 2f) * adjustPercent;
		float terrainY = (terrainSize.x / 2f) * adjustPercent;
        float terrainZ = (terrainSize.z / 2f) * adjustPercent;
		float wallWidth = 1.0f;
		
        // Set positions and scales on border surfaces
        bottom.transform.position = new Vector3(offsetX, 0, -terrainZ + offsetY);
		bottom.transform.localScale = new Vector3(terrainX * 2, terrainY * 2, wallWidth);
		left.transform.position = new Vector3(-terrainX + offsetX, 0,  offsetY);
		left.transform.localScale = new Vector3(wallWidth, terrainY * 2, terrainZ * 2);
        right.transform.position = new Vector3(terrainX + offsetX, 0, offsetY);
        right.transform.localScale = new Vector3(wallWidth, terrainY * 2, terrainZ * 2);
        top.transform.position = new Vector3(offsetX, 0, terrainZ + offsetY);
        top.transform.localScale = new Vector3(terrainX * 2, terrainY * 2, wallWidth);
        down.transform.position = new Vector3(offsetX, -terrainY, offsetY);
        down.transform.localScale = new Vector3(terrainX * 2, wallWidth, terrainZ * 2);
        up.transform.position = new Vector3(offsetX, terrainY, offsetY);
        up.transform.localScale = new Vector3(terrainX * 2, wallWidth, terrainZ * 2);

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
