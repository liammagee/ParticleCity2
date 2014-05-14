using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
 
	// Terrain variables
	public int cellSize;
	public Terrain terrain;
	private TerrainData terrainData;
	private Vector3 terrainSize;
	private Vector3 origin;

	// Different dimensions - cached on Start()
	private float widthOfTerrain;
	private float heightOfTerrain;
	private int widthInCells;
	private int heightInCells;
	private int alphamapWidth;
	private int alphamapHeight;
	private int heightmapWidth;
	private int heightmapHeight;
	private float alphamapWidthPerCell;
	private float alphamapHeightPerCell;

	float heightmapWidthRatio = 0.0f;
	float heightmapHeightRatio = 0.0f;
	float alphamapWidthRatio = 0.0f;
	float alphamapHeightRatio = 0.0f;

	private Dictionary<Vector2, int> gridCells;
	private Dictionary<Vector2, int> currentCells;
	private Dictionary<Vector2, GameObject> buildings;
	private GameState gameState;
	private Mesh cursorMesh;

	// Cursor variables
	private Vector3[,] mapGrid;
	private float[,] heightmapData;
	public float indicatorSize = 1.0f;
	public float indicatorOffsetY = 5.0f;
	private Vector3 rayHitPoint;
	private Vector3 heightmapPos;

	private Vector3[] verts;
	private Vector2[] uvs;
	private int[] tris;


	void Start ()
	{
		cellSize = 16;

		terrain = Terrain.activeTerrain;
		terrainData = terrain.terrainData;
		terrainSize = terrainData.size;
		origin = terrain.transform.position;
  
		gridCells = new Dictionary<Vector2, int> ();
		currentCells = new Dictionary<Vector2, int> ();
		buildings = new Dictionary<Vector2, GameObject> ();

		gameState = GameObject.Find ("Main Camera").GetComponent<GameState> ();

		widthOfTerrain = (int)terrainSize.x;
		heightOfTerrain = (int)terrainSize.z;
		widthInCells = (int)widthOfTerrain / cellSize;
		heightInCells = (int)heightOfTerrain / cellSize;
		alphamapWidth = terrainData.alphamapWidth;
		alphamapHeight = terrainData.alphamapHeight;
		heightmapWidth = terrainData.heightmapWidth;
		heightmapHeight = terrainData.heightmapHeight;
		alphamapWidthPerCell = (alphamapWidth / (float)widthInCells);
		alphamapHeightPerCell = (alphamapHeight / (float)heightInCells);

		heightmapWidthRatio = heightmapWidth / (float)widthInCells;
		heightmapHeightRatio = heightmapHeight / (float)heightInCells;
		alphamapWidthRatio = alphamapWidth / (float)widthInCells;
		alphamapHeightRatio = alphamapHeight / (float)heightInCells;

		// Cursor variable initialisation
		mapGrid = new Vector3[ cellSize, cellSize ];
		heightmapData = terrainData.GetHeights( 0, 0, heightmapWidth, heightmapHeight );

		BuildGrid ();  
		ShowStats ();
		ConstructMesh();
	}

	void ShowStats ()
	{
		Debug.Log ("Cell size: " + cellSize);
		Debug.Log ("Terrain size x: " + terrainSize.x);
		Debug.Log ("Terrain size y: " + terrainSize.y);
		Debug.Log ("Terrain size z: " + terrainSize.z);
		Debug.Log ("Terrain position x: " + origin.x);
		Debug.Log ("Terrain position y: " + origin.y);
		Debug.Log ("Terrain position z: " + origin.z);
		Debug.Log ("Width in cells: " + widthInCells);
		Debug.Log ("Height in cells: " + heightInCells);
		Debug.Log ("Alphamap Width: " + alphamapWidth);
		Debug.Log ("Alphamap Height: " + alphamapHeight);
		Debug.Log ("Heightmap Width: " + heightmapWidth);
		Debug.Log ("Heightmap Height: " + heightmapHeight);
		Debug.Log ("Alphamap Width in Cells: " + alphamapWidthPerCell);
		Debug.Log ("Alphamap Height in Cells: " + alphamapHeightPerCell);
	}


	void ConstructMesh() {

		if ( !cursorMesh )
		{
			cursorMesh = new Mesh ();
			MeshFilter filter = terrain.GetComponent<MeshFilter>();
			filter.mesh = cursorMesh;
			cursorMesh.name = gameObject.name + "Mesh";
		}
		cursorMesh.Clear();  
	
		verts = new Vector3[cellSize * cellSize];
		uvs = new Vector2[cellSize * cellSize];
		tris = new int[ (cellSize - 1) * 2 * (cellSize - 1) * 3];
		
		float uvStep = 1.0f / 8.0f;
		
		int index = 0;
		int triIndex = 0;
		
		for ( int z = 0; z < cellSize; z ++ )
		{
			for ( int x = 0; x < cellSize; x ++ )
			{
				verts[ index ] = new Vector3( x, 0, z );
				uvs[ index ] = new Vector2( ((float)x) * uvStep, ((float)z) * uvStep );
				
				if ( x < cellSize - 1 && z < cellSize - 1 )
				{
					tris[ triIndex + 0 ] = index + 0;
					tris[ triIndex + 1 ] = index + cellSize;
					tris[ triIndex + 2 ] = index + 1;
					
					tris[ triIndex + 3 ] = index + 1;
					tris[ triIndex + 4 ] = index + cellSize;
					tris[ triIndex + 5 ] = index + cellSize + 1;
					
					triIndex += 6;
				}
				
				index ++;
			}
		}
		
		
		// - Build Mesh -
		cursorMesh.vertices = verts;
		cursorMesh.uv = uvs;
		cursorMesh.triangles = tris;
		
		cursorMesh.RecalculateBounds();  
		cursorMesh.RecalculateNormals();
	}


	void Update ()
	{
		PaintTerrain ();
		PaintBuildings ();
		ProcessCursorMesh ();
	}	

	void ProcessCursorMesh () {
		RaycastToTerrain ();

		GetHeightmapPosition ();

		CalculateGrid ();

		UpdateMesh ();

	}


	void RaycastToTerrain()
	{
		RaycastHit hit;
		Ray rayPos = Camera.main.ScreenPointToRay( Input.mousePosition );
		
		if ( Physics.Raycast( rayPos, out hit, Mathf.Infinity ) ) // also consider a layermask to just the terrain layer
		{
			//Debug.DrawLine( Camera.main.transform.position, hit.point, Color.red );
			rayHitPoint = hit.point;
			rayHitPoint.x += (	terrainSize.x / 2);
			rayHitPoint.z += (terrainSize.z / 2);
		}
	}

	void GetHeightmapPosition()
	{
		// find the heightmap position of that hit
		heightmapPos.x = Mathf.FloorToInt(( rayHitPoint.x / (float)widthOfTerrain ) * widthInCells);
		heightmapPos.z = Mathf.FloorToInt(( rayHitPoint.z / (float)heightOfTerrain ) * heightInCells ); 
		
		// convert to integer
		heightmapPos.x = Mathf.Round( heightmapPos.x );
		heightmapPos.z = Mathf.Round( heightmapPos.z );
		
		// clamp to heightmap dimensions to avoid errors
		heightmapPos.x = Mathf.Clamp( heightmapPos.x, 0, widthInCells - 1 );
		heightmapPos.z = Mathf.Clamp( heightmapPos.z, 0, heightInCells - 1 );
	}


	
	void CalculateGrid()
	{
		float xFactor = heightmapWidth / (float)widthOfTerrain;
		float yFactor = heightmapHeight / (float)heightOfTerrain;

		for ( int z = 0; z < cellSize; z ++ )
		{
			for ( int x = 0; x < cellSize; x ++ )
			{
				Vector3 calcVector;
				
				calcVector.x = heightmapPos.x * cellSize + ( x * indicatorSize );
				calcVector.z = heightmapPos.z * cellSize + ( z * indicatorSize );

				float calcPosX = calcVector.x;
				calcPosX *= xFactor;
				calcPosX = Mathf.Clamp( calcPosX, 0, heightmapWidth - 1 );
				
				float calcPosZ = calcVector.z;
				calcPosZ *= yFactor;
				calcPosZ = Mathf.Clamp( calcPosZ, 0, heightmapHeight - 1 );
				
				calcVector.y = heightmapData[ (int)calcPosZ, (int)calcPosX ] * terrainSize.y; // heightmapData is Y,X ; not X,Y (reversed)
				calcVector.y += indicatorOffsetY; // raise slightly above terrain

				mapGrid[ x, z ] = calcVector;
			}
		}
	}

	void UpdateMesh()
	{
		int index = 0;
		
		for ( int z = 0; z < cellSize; z ++ )
		{
			for ( int x = 0; x < cellSize; x ++ )
			{
				verts[ index ] = mapGrid[ x, z ];
				
				index ++;
			}
		}
		
		// assign to mesh
		cursorMesh.vertices = verts;

		cursorMesh.RecalculateBounds();
		cursorMesh.RecalculateNormals();
	}

	void BuildGrid ()
	{  
//		InitGrid ();
		ShowGrid ();    
	}


	void ShowGrid ()
	{  
		Color c1 = Color.green;
		Color c2 = Color.green;
		float xFactor = heightmapWidth / widthOfTerrain;
		float yFactor = heightmapHeight / heightOfTerrain;
		GameObject dynamicObjects = GameObject.Find ("DynamicObjects");
		GameObject visibleGrid = new GameObject ("VisibleGrid");
		visibleGrid.transform.parent = dynamicObjects.transform;
		CheckboxShowGrid.visibleGrid = visibleGrid;
		visibleGrid.SetActive (false);

		for (int x = 0; x <= widthOfTerrain; x += cellSize) {
			float lastHeight = 0.0f;
			int pointCounter = 0;
			GameObject dummy = new GameObject ("Grid line at x: " + x);
			dummy.transform.parent = visibleGrid.transform;
			LineRenderer lineRenderer = (LineRenderer)dummy.AddComponent ("LineRenderer");
			lineRenderer.material.color = Color.green;
			lineRenderer.SetWidth (0.5f, 0.5f);

			List<Vector3> points = new List<Vector3> ();


			for (int y = 0; y <= heightOfTerrain; y ++) {
				float height = terrainData.GetHeight ((int)(x * xFactor), (int)(y * yFactor)) + 1.0f;
				// Register change in height
				if (y == 0 || height != lastHeight || y == heightInCells) {
					int newX = (int)(x - (widthOfTerrain / 2));
					int newZ = (int)(y - (heightOfTerrain / 2));
					Vector3 point = new Vector3 (newX, height, newZ);
					points.Add (point);
				}
			}
			lineRenderer.SetVertexCount (points.Count);
			for (int i = 0; i < points.Count; i++) {
				lineRenderer.SetPosition (i, points [i]);
			}
		}

		for (int x = 0; x <= heightOfTerrain; x += cellSize) {
			float lastHeight = 0.0f;
			int pointCounter = 0;
			GameObject dummy = new GameObject ("Grid line at y: " + x);
			dummy.transform.parent = visibleGrid.transform;
			LineRenderer lineRenderer = (LineRenderer)dummy.AddComponent ("LineRenderer");
			lineRenderer.material.color = Color.green;
			lineRenderer.SetWidth (0.5f, 0.5f);

			List<Vector3> points = new List<Vector3> ();

			for (int y = 0; y <= widthOfTerrain; y++) {
				float height = terrainData.GetHeight ((int)(y * yFactor), (int)(x * xFactor)) + 1.0f;
				// Register change in height
				if (y == 0 || height != lastHeight || y == heightmapHeight) {
					int newX = (int)(y - (heightOfTerrain / 2));
					int newZ = (int)(x - (widthOfTerrain / 2));
					Vector3 point = new Vector3 (newX, height, newZ);
					points.Add (point);
				}
			}
			lineRenderer.SetVertexCount (points.Count);
			for (int i = 0; i < points.Count; i++) {
				lineRenderer.SetPosition (i, points [i]);
			}
		}
	}

	void PaintTerrain ()
	{
		bool reviseTerrain = gameState.showUpdatedTerrain;

		if (reviseTerrain && currentCells != null) {
			foreach (KeyValuePair<Vector2, int> entry in currentCells) {
				Vector2 actualPos = entry.Key;
				int count = gridCells [actualPos];
				int w = (int)(actualPos.x / (float)widthInCells * (float)alphamapWidth);
				int h = (int)(actualPos.y / (float)heightInCells * (float)alphamapHeight);
				int maxCount = (count > 10 ? 10 : count);
				float[,,] existingMap = terrainData.GetAlphamaps (w, h, (int)alphamapWidthPerCell, (int)alphamapHeightPerCell);
				if (existingMap [0, 0, 0] != 1 - (maxCount / 10.0f)) {
					float[,,] map = new float[(int)alphamapWidthPerCell, (int)alphamapHeightPerCell, terrainData.alphamapLayers];
					for (int x = 0; x < (int)alphamapWidthPerCell; x++) {
						for (int y = 0; y < (int)alphamapHeightPerCell; y++) {
							map [x, y, 0] = 1 - (maxCount / 10.0f);
							map [x, y, 1] = (maxCount / 10.0f);
						}
					}
					terrainData.SetAlphamaps (w, h, map);
				}
			}
		}
		currentCells = new Dictionary<Vector2, int> ();
	}

	void PaintBuildings ()
	{
		bool buildBuildings = gameState.showBuildings;
		if (buildBuildings && gridCells != null) {
			foreach (KeyValuePair<Vector2, int> entry in gridCells) {
				Vector2 actualPos = entry.Key;
				int count = entry.Value;
				int w = (int)actualPos.x;
				int h = (int)actualPos.y;
				if (count > 10 && count <= 40) {
					GameObject dynamicObjects = GameObject.Find("DynamicObjects");
					GameObject gameObject = null;
					if (buildings.ContainsKey (actualPos)) {
						gameObject = buildings [actualPos];
						Vector3 scale = gameObject.transform.localScale;
						gameObject.transform.localScale = new Vector3(scale.x, count * cellSize, scale.z);
					}
					else {
						int x = (int)((w / (float)widthInCells) * widthOfTerrain) - (int)(widthOfTerrain / 2);
						int y = (int)((h / (float)heightInCells) * heightOfTerrain) - (int)(heightOfTerrain / 2);
						gameObject = GameObject.CreatePrimitive (PrimitiveType.Cube);
						gameObject.name = ("Building at: (" + x + ", " + y + ")");
						
						// TODO: Use actual terrain height for y value
						Vector3 position = new Vector3 (x, 0, y);
						Vector3 scale = new Vector3 (cellSize, count * cellSize, cellSize);
						gameObject.transform.position = position;
						gameObject.transform.localScale = scale;
						gameObject.transform.parent = dynamicObjects.transform;
						buildings.Add (actualPos, gameObject);
					}
				}
			}
		}
	}
 
	public Vector3 GetWorldPosition (Vector2 gridPosition)
	{
		return new Vector3 (origin.z + (gridPosition.x * cellSize), origin.y, origin.x + (gridPosition.y * cellSize));
	}
 
	public Vector2 GetGridPosition (Vector3 worldPosition)
	{
		return new Vector2 (worldPosition.z / cellSize, worldPosition.x / cellSize);
	}

	public void InitGrid ()
	{
		float[,,] map = new float[alphamapWidth, alphamapHeight, 2];
		for (int x = 0; x < alphamapWidth; x++) {
			for (int y = 0; y < alphamapHeight; y++) {
				map [x, y, 0] = 1.0f;
				map [x, y, 1] = 0.0f;
			}
		}
		terrain.terrainData.SetAlphamaps (0, 0, map);
	}

	public void TurnOnCell (Vector3 worldPosition)
	{
		// Normalise to the terrain space
		Vector3 terrainPosition = new Vector3 (
      worldPosition.x + widthOfTerrain / 2, 
      worldPosition.y, 
      worldPosition.z + heightOfTerrain / 2
		);
		int w = (int)(terrainPosition.x / widthOfTerrain * widthInCells);
		int h = (int)(terrainPosition.z / heightOfTerrain * heightInCells);

		if (w < widthInCells && h < heightInCells) {
			Vector2 actualPos = new Vector2 (w, h);
			if (! gridCells.ContainsKey (actualPos)) {
				gridCells.Add (actualPos, 0);
				currentCells.Add (actualPos, 0);
			}
			gridCells [actualPos]++;
		}
	}
}
