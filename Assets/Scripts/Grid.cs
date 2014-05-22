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
	private TerrainData originalTerrainData;
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
	private GameObject terrainMesh;
	private Mesh cursorMesh;

	// Cursor variables
	private float[,] heightmapData;
	public float indicatorSize = 1.0f;
	public float indicatorOffsetY = 5.0f;

	private Vector3[] verts;
	private Vector2[] uvs;
	private int[] tris;

	// Building types - need to be assigned distinct prefabs
	public GameObject building1;
	public GameObject building2;
	public GameObject building3;
	public GameObject building4;
	public GameObject building5;
	public GameObject building6;
	public GameObject building7;
	public GameObject building8;
	public GameObject building9;
	private List<GameObject> buildingPrefabs;

	private float[] GetHeights(){
		float[] heights = new float[262144];
		
		for(int i=0; i<heights.Length; i++)
			heights[i] = i / 262144.0f;
		return heights;
	}
	
	void LoadHeightmapData(string heighmapFile)
	{

		float []heights = GetHeights();
		using (System.IO.FileStream f = System.IO.File.OpenRead(heighmapFile)) {
			
			using (System.IO.BinaryReader br = new System.IO.BinaryReader(f)) {
				
				int res = Mathf.CeilToInt (Mathf.Sqrt (f.Length / 2));
				
				terrainData.heightmapResolution = res + 1;
				
				terrainData.size = new Vector3 (res * 2, res / 10, res * 2);
				
				terrainData.baseMapResolution = res;
				
				terrainData.SetDetailResolution (res, 8);
				
				float[,] heightData = new float[res,res];

				int x, y;
				for (x=0; x<res; x++) {
					
					for (y=0; y<res; y++) {
						
						heightData [x, y] = heights[br.ReadUInt16()];
						
					}
					
				}
				
				for(x=res; x<res+1;x++){
					
					for (y=0; y<res; y++) 
						
						heightData [x, y] = heightData [x-1, y];
					
				}
				
				for(x=0; x<res;x++){
					
					for (y=res; y<res+1; y++) 
						
						heightData [x, y] = heightData [x, y-1];
					
				}
				terrainData.SetHeights (0, 0, heightData);
				
			}
		}
	}

	public Terrain GetTerrain()
	{
//		if (terrain == null) {
////			terrain = (Terrain)Instantiate(Terrain.activeTerrain);
//			terrain = Terrain.activeTerrain;
//			originalTerrainData = Terrain.activeTerrain.terrainData;
////			Terrain.activeTerrain.gameObject.SetActive (false);
//		}
		return terrain;
	}

//	public void OnApplicationQuit() {
//		terrain.terrainData = originalTerrainData; 
//	}

	void Start ()
	{

		// Re-colorise the splatmap, in case colors were distorted from previous runs
		ReverseTerrain.Splatmap();

		// Do something like this to load raw data
		//LoadHeightmapData ("./Assets/Melbourne.raw");

		// Re-colorise the splatmap, in case colors were distorted from previous runs
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
		heightmapData = terrainData.GetHeights( 0, 0, heightmapWidth, heightmapHeight );

		// Create array of prefabs
		buildingPrefabs = new List<GameObject> ();
		buildingPrefabs.Add(building1);
		buildingPrefabs.Add(building2);
		buildingPrefabs.Add(building3);
		buildingPrefabs.Add(building4);
		buildingPrefabs.Add(building5);
		buildingPrefabs.Add(building6);
		buildingPrefabs.Add(building7);
		buildingPrefabs.Add(building8);
		buildingPrefabs.Add(building9);

		BuildGrid ();  
		ShowStats ();
		ConstructMesh();

		// Kick off coroutine
		StartCoroutine("PeriodicUpdate");
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

	public float GetHeightmapX(int terrainCoord) 
	{
		return terrainCoord * (heightmapWidth / (float)widthOfTerrain);
	}

	public float GetHeightmapY(int terrainCoord) 
	{
		return terrainCoord * (heightmapHeight / (float)heightOfTerrain);
	}

	public float GetAlphamapX(int terrainCoord) 
	{
		return terrainCoord * (alphamapWidth / (float)widthOfTerrain);
	}

	public float GetAlphamapY(int terrainCoord) 
	{
		return terrainCoord * (alphamapHeight / (float)heightOfTerrain);
	}

	public bool InsideTerrain(Vector3 position) {
		float x = position.x;
		float y = position.z;
		return (x > -(widthOfTerrain / 2f) && x < (widthOfTerrain / 2f) && y > -(heightOfTerrain / 2f) && y < (heightOfTerrain / 2f));
	}

	void ConstructMesh() 
	{
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

	/// <summary>
	/// For performance, update the terrain and buildings periodically
	/// </summary>
	IEnumerator PeriodicUpdate() 
	{
		for (;;) 
		{
			PaintTerrain ();
			PaintBuildings ();
			ClearCurrentCells();
			yield return new WaitForSeconds(0.1f);
		}
	}


	void Update ()
	{
		ProcessCursorMesh ();
	}	

	void ClearCurrentCells() 
	{
		currentCells = new Dictionary<Vector2, int> ();
	}



	void ProcessCursorMesh () {
		Vector3 rayHitPoint = RaycastToTerrain ();
		Vector3 heightmapPos = GetHeightmapPosition (rayHitPoint);
		Vector3[,] mapGrid = CalculateGrid (heightmapPos, cellSize, cellSize, 1);
		UpdateMesh (mapGrid, cursorMesh);
	}


	Vector3 RaycastToTerrain()
	{
		RaycastHit hit;
		Ray rayPos = Camera.main.ScreenPointToRay( Input.mousePosition );
		Vector3 rayHitPoint = new Vector3();
		
		if ( Physics.Raycast( rayPos, out hit, Mathf.Infinity ) ) // also consider a layermask to just the terrain layer
		{
			//Debug.DrawLine( Camera.main.transform.position, hit.point, Color.red );
			rayHitPoint = hit.point;
			rayHitPoint.x += (	terrainSize.x / 2);
			rayHitPoint.z += (terrainSize.z / 2);
		}
		return rayHitPoint;
	}

	Vector3 GetHeightmapPosition(Vector3 point)
	{
		Vector3 heightmapPos = new Vector3();
		// find the heightmap position of that hit
		heightmapPos.x = Mathf.FloorToInt(( point.x / (float)widthOfTerrain ) * widthInCells);
		heightmapPos.z = Mathf.FloorToInt(( point.z / (float)heightOfTerrain ) * heightInCells ); 
		
		// convert to integer
		heightmapPos.x = Mathf.Round( heightmapPos.x );
		heightmapPos.z = Mathf.Round( heightmapPos.z );
		
		// clamp to heightmap dimensions to avoid errors
		heightmapPos.x = Mathf.Clamp( heightmapPos.x, 0, widthInCells - 1 );
		heightmapPos.z = Mathf.Clamp( heightmapPos.z, 0, heightInCells - 1 );

		return heightmapPos;
	}


	
	Vector3[,] CalculateGrid(Vector3 heightmapPos, int width, int height, int increment)
	{
		float xFactor = heightmapWidth / (float)widthOfTerrain;
		float yFactor = heightmapHeight / (float)heightOfTerrain;
		Vector3[,] mapGrid = new Vector3[ width, height ];
        if (heightmapData == null)
            heightmapData = terrainData.GetHeights( 0, 0, heightmapWidth, heightmapHeight );
		for ( int z = 0; z < height; z ++ )
		{
			for ( int x = 0; x < width; x ++ )
			{
				Vector3 calcVector;
				
				calcVector.x = heightmapPos.x * cellSize + ( x * indicatorSize );
				calcVector.z = heightmapPos.z * cellSize + ( z * indicatorSize );

				float calcPosX = calcVector.x;
				calcPosX *= xFactor;
				calcPosX *= increment;
				calcPosX = Mathf.Clamp( calcPosX, 0, heightmapWidth - 1 );
				
				float calcPosZ = calcVector.z;
				calcPosZ *= yFactor;
				calcPosZ *= increment;
				calcPosZ = Mathf.Clamp( calcPosZ, 0, heightmapHeight - 1 );
				
				calcVector.y = heightmapData[ (int)calcPosZ, (int)calcPosX ] * terrainSize.y; // heightmapData is Y,X ; not X,Y (reversed)
				calcVector.y += indicatorOffsetY; // raise slightly above terrain

				mapGrid[ x, z ] = calcVector;
			}
		}
		return mapGrid;
	}

	void UpdateMesh(Vector3[,] mapGrid, Mesh mesh)
	{
		Vector3[] verts = mesh.vertices;
		int index = 0;
		for ( int z = 0; z < mapGrid.GetLength(1); z ++ )
		{
			for ( int x = 0; x < mapGrid.GetLength(0); x ++ )
			{
				verts[ index ] = mapGrid[ x, z ];
				index ++;
			}
		}
		
		// assign to mesh
		mesh.vertices = verts;

		mesh.RecalculateBounds();
		mesh.RecalculateNormals();
	}

	void BuildGrid ()
	{  
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

		// Mesh version - WORK IN PROGRESS
		/*
		Type[] terrainMeshTypes = new Type[2];
		int sampleFactor = Mathf.FloorToInt (widthOfTerrain / 128f);
		int terrainCellSize = Mathf.FloorToInt(128);
		terrainMeshTypes[0] = typeof(MeshFilter);
		terrainMeshTypes[1] = typeof(MeshRenderer);
		terrainMesh = new GameObject("TerrainMesh", terrainMeshTypes);
		terrainMesh.transform.position = new Vector3(-Mathf.FloorToInt(widthOfTerrain / 2f), -indicatorOffsetY, -Mathf.FloorToInt(widthOfTerrain / 2f));
		terrainMesh.transform.localScale = new Vector3(widthOfTerrain / 128f, 1, widthOfTerrain / 128f);
//		terrainMesh.transform.parent = gameObject.transform;
		MeshFilter filter = (MeshFilter)terrainMesh.GetComponent<MeshFilter>();
		MeshRenderer r = terrainMesh.GetComponent<MeshRenderer>();
		r.material.color = Color.red;
		Mesh m = new Mesh ();
		filter.mesh = m;
		m.name = terrainMesh.name + "Mesh";
		m.Clear();


		Vector3[] verts = new Vector3[(int)terrainCellSize * (int)terrainCellSize];
		Vector2[] uvs = new Vector2[(int)terrainCellSize * (int)terrainCellSize];
		int[] tris = new int[ ((int)terrainCellSize - 1) * 2 * ((int)terrainCellSize - 1) * 3];
		
		float uvStep = 1.0f / 8.0f;
		
		int index = 0;
		int triIndex = 0;
		
		for ( int z = 0; z < terrainCellSize; z ++ )
		{
			for ( int x = 0; x < terrainCellSize; x ++ )
			{
				verts[ index ] = new Vector3( x * sampleFactor, 0, z * sampleFactor );
				uvs[ index ] = new Vector2( ((float)x * sampleFactor) * uvStep, ((float)z * sampleFactor) * uvStep );
				
				if ( x < terrainCellSize - 1 && z < terrainCellSize - 1 )
				{
					tris[ triIndex + 0 ] = index + 0;
					tris[ triIndex + 1 ] = index + terrainCellSize;
					tris[ triIndex + 2 ] = index + 1;
					
					tris[ triIndex + 3 ] = index + 1;
					tris[ triIndex + 4 ] = index + terrainCellSize;
					tris[ triIndex + 5 ] = index + terrainCellSize + 1;
					
					triIndex += 6;
				}
				
				index ++;
			}
		}

		// - Build Mesh -
		m.vertices = verts;
		m.uv = uvs;
		m.triangles = tris;
		
		m.RecalculateBounds();
		m.RecalculateNormals();

		Vector3 heightmapPos = GetHeightmapPosition (Vector3.zero);
		Vector3[,] mapGrid = CalculateGrid (heightmapPos, terrainCellSize, terrainCellSize, sampleFactor);
		UpdateMesh (mapGrid, m);
		*/
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
	}

	void PaintBuildings ()
	{
		bool buildBuildings = gameState.showBuildings;

		if (buildBuildings) {
			GameObject dynamicObjects = GameObject.Find("DynamicObjects");
			GameObject baseBuilding = GameObject.Find("BaseBuilding");
			float buildingsDimension = cellSize / 4f;
			if (currentCells != null) 
			{
				foreach (KeyValuePair<Vector2, int> entry in currentCells) {
					Vector2 actualPos = entry.Key;
					int count = entry.Value;
					int w = (int)actualPos.x;
					int h = (int)actualPos.y;
					if (! buildings.ContainsKey (actualPos)) {

						GameObject gameObject = null;
						int x = (int)((w / (float)widthInCells) * widthOfTerrain);
						int y = (int)((h / (float)heightInCells) * heightOfTerrain);
						int normalisedX = x - (int)(widthOfTerrain / 2);
						int normalisedY = y - (int)(heightOfTerrain / 2);
						float height = terrainData.GetHeight((int)GetHeightmapX(x), (int)GetHeightmapY(y)) + (buildingsDimension / 2);

						float r = UnityEngine.Random.Range(0, 100);
						if (r < gameState.chanceOfBuilding && height > 1.0f) {
							int buildingIndex = UnityEngine.Random.Range(0, 8);
//							GameObject buildingPrefab = buildingPrefabs[buildingIndex];
//							gameObject = (GameObject)Instantiate(buildingPrefab);
							gameObject = (GameObject)Instantiate(baseBuilding);
							gameObject.name = ("Building at: (" + normalisedX + ", " + normalisedY + ", " + height + ")");
							
							// TODO: Use actual terrain height for y value
							Vector3 position = new Vector3 (normalisedX, height, normalisedY);
							Vector3 scale = new Vector3 (buildingsDimension, buildingsDimension, buildingsDimension);
							gameObject.transform.position = position;
							gameObject.transform.localScale = scale;
							gameObject.transform.parent = dynamicObjects.transform;
							buildings.Add (actualPos, gameObject);
						}
					}
				}

//				foreach (KeyValuePair<Vector2, GameObject> entry in buildings) 
//				{
//					Vector2 actualPos = entry.Key;
//					GameObject building = entry.Value;
//					Vector3 scale = building.transform.localScale;
//					building.transform.localScale = new Vector3(scale.x, scale.y * 1.01f, scale.z);
//				}
			}
		}
		else 
		{
			// Remove the buildings if this flag is turned off?
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
