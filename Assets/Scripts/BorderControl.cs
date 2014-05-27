using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BorderControl : MonoBehaviour 
{
    Transform left;
    Transform right;
    Transform top;
    Transform bottom;
    Transform up;
    Transform down;
    Transform []borderEdges;
    Grid grid;
    GameState gameState;
    TerrainData terrainData;
    Vector3 terrainSize;
    Vector2 cellDimensions;
    Vector2[] extents;

    void Start()
    {
        bottom = GameObject.Find("CubeBottomBorder").transform;
        left = GameObject.Find("CubeLeftBorder").transform;
        right = GameObject.Find("CubeRightBorder").transform;
        top = GameObject.Find("CubeTopBorder").transform;
        down = GameObject.Find("CubeDownBorder").transform;
        up = GameObject.Find("CubeUpBorder").transform;
        borderEdges = new Transform[]{bottom, left, right, top, down, up};
		grid = GameObject.Find ("GridOrigin").GetComponent<Grid>();
        gameState = GameObject.Find ("Main Camera").GetComponent<GameState>();
		terrainData = grid.GetTerrain().terrainData; 
		terrainSize = terrainData.size;
        cellDimensions = grid.GetCellDimensions();
    }

    public bool WithinBorders(Vector3 point)
    {
    	bool within = true;
        if (point.x <= left.position.x || point.x >= right.position.x)
        	within = false;
        if (point.y <= down.position.y || point.y >= up.position.y)
        	within = false;
        if (point.z <= bottom.position.z || point.z >= top.position.z)
        	within = false;
        return within;
    }
    
    public Vector2[] GetBorderVertices()
    {
        return extents;
    }


	public void RedoBorders(float value) 
	{
		float adjustPercent = value / 100f;
        int cellSize = grid.cellSize;
        float offsetX = gameState.originX;
        float offsetY = gameState.originZ;
        int cellOffsetX = Mathf.FloorToInt(gameState.originX / cellSize);
        int cellOffsetY = Mathf.FloorToInt(gameState.originZ / cellSize);
        cellDimensions = grid.GetCellDimensions();
        cellDimensions *= 0.5f;
        int x = Mathf.FloorToInt(adjustPercent * cellDimensions.x);
        int y = Mathf.FloorToInt(adjustPercent * cellDimensions.y);
        float terrainX = x * cellSize;
        float terrainY = x * cellSize;
        float terrainZ = y * cellSize;
		float wallWidth = 1.0f;
        extents = new Vector2[]
        {
            new Vector2(cellDimensions.x - x + cellOffsetX, cellDimensions.y - y + cellOffsetY),
            new Vector2(cellDimensions.x + x + cellOffsetX, cellDimensions.y + y + cellOffsetY)
        };

        // Set positions and scales on border surfaces
        for (int i = 0; i < borderEdges.Length; i++) 
        {
            Transform t = borderEdges[i];
            t.localScale = new Vector3(terrainX * 2, terrainY * 2, wallWidth);
        }
        bottom.position = new Vector3(offsetX, 0, -terrainZ + offsetY);
		left.position = new Vector3(-terrainX + offsetX, 0,  offsetY);
        right.position = new Vector3(terrainX + offsetX, 0, offsetY);
        top.position = new Vector3(offsetX, 0, terrainZ + offsetY);
        down.position = new Vector3(offsetX, -terrainY, offsetY);
        up.position = new Vector3(offsetX, terrainY, offsetY);

		// Ensure agents are inside the bordrs
        // TODO: This logic is wrong
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

