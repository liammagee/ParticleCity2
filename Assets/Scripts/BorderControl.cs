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
		Vector3 terrainSize = Terrain.activeTerrain.terrainData.size;
		float terrainX = (terrainSize.x / 2f);
		float terrainZ = (terrainSize.z / 2f);
		float adjustPercent = value / 100f;
		float heightOffset = 450f;
		float wallWidth = 1.0f;
		float wallHeight = 1000.0f;
		Debug.Log(terrainX);
		Debug.Log(terrainZ);
		Debug.Log(adjustPercent);
		bottom.transform.position = new Vector3(0, heightOffset, -terrainZ * adjustPercent);
		bottom.transform.localScale = new Vector3(terrainX * 2 * adjustPercent, wallHeight, wallWidth);
		left.transform.position = new Vector3(-terrainX * adjustPercent, heightOffset, 0);
		left.transform.localScale = new Vector3(wallWidth, wallHeight, terrainX * 2 * adjustPercent);
		right.transform.position = new Vector3(terrainX * adjustPercent, heightOffset, 0);
		right.transform.localScale = new Vector3(wallWidth, wallHeight, terrainX * 2 * adjustPercent);
		top.transform.position = new Vector3(0, heightOffset, terrainZ * adjustPercent);
		top.transform.localScale = new Vector3(terrainX * 2 * adjustPercent, wallHeight, wallWidth);

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
			agent.transform.position = newPosition;
		}
	}
}
