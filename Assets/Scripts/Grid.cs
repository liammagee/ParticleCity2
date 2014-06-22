using UnityEngine;
using FiercePlanet;
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
    private BorderControl borderControl;
    private ParticleCity particleCity;

	// Different dimensions - cached on Start()
	private float widthOfTerrain;
	private float heightOfTerrain;
	private int widthInCells;
	private int heightInCells;
    private Vector2 cellDimensions;
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

    private List<Vector2> currentCells;
    private Dictionary<Vector2, Patch> allPatches;
    private Dictionary<Vector2, Patch> patches;
    private Dictionary<Vector2, Building> buildings;

    private GameState gameState;

    float[,] heightmapData;
    public float indicatorSize = 1.0f;
    public float indicatorOffsetY = 5.0f;

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

    Transform dynamicObjectsTransform;
    GameObject baseBuilding;

    void Awake()
    {
        terrainData = terrain.terrainData;
        
        terrainSize = terrainData.size;
        origin = terrain.transform.position;
        
        currentCells = new List<Vector2> ();
        allPatches = new Dictionary<Vector2, Patch> ();
        patches = new Dictionary<Vector2, Patch> ();
        buildings = new Dictionary<Vector2, Building> ();
        
        gameState = GameObject.Find ("Main Camera").GetComponent<GameState> ();
        particleCity = GameObject.Find ("Main Camera").GetComponent<ParticleCity> ();
        borderControl = GameObject.Find ("Border").GetComponent<BorderControl> ();
        baseBuilding = GameObject.Find("BaseBuilding");
        dynamicObjectsTransform = GameObject.Find("DynamicObjects").transform;

        widthOfTerrain = (int)terrainSize.x;
        heightOfTerrain = (int)terrainSize.z;
        widthInCells = (int)widthOfTerrain / cellSize;
        heightInCells = (int)heightOfTerrain / cellSize;
        cellDimensions = new Vector2(widthInCells, heightInCells);
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
    }

	void Start ()
	{

		// Re-colorise the splatmap, in case colors were distorted from previous runs
		ReverseTerrain.Splatmap();

		// Do something like this to load raw data
		//LoadHeightmapData ("./Assets/Melbourne.raw");

//        BuildGrid ();  
        PaintGrid ();  

        ShowStats ();
        RescalePatches();

		// Kick off coroutines
        StartCoroutine("PeriodicUpdate");
	}
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
        return terrain;
    }
    
    


    public Vector2 GetCellDimensions()
    {
        return cellDimensions;
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


    /// <summary>
    /// For performance, update the terrain and buildings periodically
    /// </summary>
    IEnumerator PeriodicUpdate() 
    {
        for (;;) 
        {
            PaintTerrain ();
            PaintBuildings ();
            RecuperatePatches();
            ReviseBuildings();
            ClearCurrentCells();
            yield return new WaitForSeconds(0.25f);
        }
    }
    
    void RecuperatePatches()
    {
        foreach(KeyValuePair<Vector2, Patch> entry in patches) 
        {
            Patch patch = entry.Value;
            if (!currentCells.Contains(patch.position))
                patch.recuperate();
        }
    }
    
    public void RescalePatches()
    {
        patches.Clear();
        foreach(KeyValuePair<Vector2, Patch> entry in allPatches) 
        {
            Vector2 position = entry.Key;
            Patch patch = entry.Value;
            Vector3 position3d = new Vector3(position.x + (widthOfTerrain / 2f), 0, position.y + (heightOfTerrain / 2f));
            if (borderControl.WithinBorders(position3d))
                patches.Add(position, patch);
        }
    }

    void ReviseBuildings()
    {
        List<Building> destroyedBuildings = new List<Building>();
        foreach(KeyValuePair<Vector2, Building> entry in buildings) 
        {
            Building building = entry.Value;

            // Deteriorate the building
            if (!currentCells.Contains(building.position))
                building.disintegrate();

            if (building.health <= 0)
            {
                destroyedBuildings.Add(building);
            }
            else
            {
                // Calculate the chance of growing the building horizontally and / or vertically
                float chance = ChanceOfGrowingBuilding(building);
                float r = UnityEngine.Random.Range(0f, 1f);
                if (r < chance)
                {
                    GrowBuilding(building);
                }
            }
        }
        foreach (Building building in destroyedBuildings)
        {
            buildings.Remove(building.position);
        }
    }

    void GrowBuilding(Building building)
    {
        float buildingsDimension = cellSize / 4f;
        Vector3 localScale = building.gameObject.transform.localScale;
        bool canGrowHorizontal = localScale.x < (cellSize / 2f) && localScale.z < (cellSize / 2f);
        float vertical = UnityEngine.Random.Range(0f, 1f);
        vertical = (! canGrowHorizontal ? 1.0f : vertical);
        if (vertical > 0.5f)
        {
            localScale.y += buildingsDimension;
        }
        else 
        {
            if (vertical > 0.25f)
                localScale.x += buildingsDimension;
            else
                localScale.z += buildingsDimension;
        }
        building.gameObject.transform.localScale = localScale;
    }

    float ChanceOfGrowingBuilding(Building building)
    {
        float relativeDistance = GetRelativeDistanceFromCentralCell(building.position);
        float baRatio = GetBuildingAgentRatio();
        return Mathf.Clamp01(relativeDistance * baRatio);
    }

    float GetBuildingAgentRatio()
    {
        int agentCount = particleCity.GetAgentCount();
        int buildingCount = buildings.Count;
        float baRatio = 0f;
        if (agentCount > 0) 
        {
            baRatio = buildingCount / agentCount;
            baRatio = (baRatio > 1.0f ? 1.0f : baRatio);
        }
        return 1 - baRatio;
    }

    float GetRelativeDistanceFromCentralCell(Vector2 position)
    {
        Vector2 dimensions = borderControl.GetHabitableDimension();
        Vector2 centralCell = GetCentralCell();
        Vector2 bottomLeft = centralCell - dimensions;
        float totalDistance = Mathf.Abs(Vector2.Distance(centralCell, bottomLeft));
        float relativeDistance = Mathf.Abs(Vector2.Distance(centralCell, position));
        return 1 - (relativeDistance / totalDistance);
    }

    Vector2 GetCentralCell() 
    {
        return new Vector2(
            ((gameState.originX + widthOfTerrain / 2f) / widthOfTerrain) * widthInCells,
            ((gameState.originZ + heightOfTerrain / 2f) / heightOfTerrain ) * heightInCells
        );
    }



	void Update ()
	{
	}	

	void ClearCurrentCells() 
	{
        currentCells.Clear();
	}


	void BuildGrid ()
	{  
		Color c1 = Color.green;
		Color c2 = Color.green;
		float xFactor = heightmapWidth / widthOfTerrain;
		float yFactor = heightmapHeight / heightOfTerrain;
		GameObject visibleGrid = new GameObject ("VisibleGrid");
        visibleGrid.transform.parent = dynamicObjectsTransform;
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

                // Add a new Patch while here
                if (y % cellSize == 0) 
                {
                    Vector2 position = new Vector2(x, y);
                    allPatches.Add (position, new Patch(position));
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

    void PaintGrid()
    {
        GameObject visibleGrid = new GameObject ("VisibleGrid");
        visibleGrid.transform.parent = dynamicObjectsTransform;
        CheckboxShowGrid.visibleGrid = visibleGrid;
        visibleGrid.SetActive (false);

        int dimension = 129;
        int vertexLength = Mathf.FloorToInt (widthOfTerrain / ((float)dimension - 1));

        Vector3 position = new Vector3(-Mathf.FloorToInt(widthOfTerrain / 2f), -1.0f, -Mathf.FloorToInt(widthOfTerrain / 2f));
        GameObject meshObject = CreateMeshObject("TerrainMesh", vertexLength, position);
        meshObject.transform.parent = visibleGrid.transform;
        Mesh m = (meshObject.GetComponent<MeshFilter>()).mesh;

        Vector3[] verts = new Vector3[dimension * dimension * 2];
        Vector2[] uvs = new Vector2[dimension * dimension * 2];
        int[] tris = new int[ (dimension - 1) * 2 * (dimension - 1) * 3 * 2];
    
        float uvStep = 1.0f / 4.0f;
        
        int index = 0;
        int triIndex = 0;
        for ( int z = 0; z < (dimension) ; z += 1)
        {
            for ( int x = 0; x < (dimension) ; x += 1 )
            {
                verts[ index ] = new Vector3(x * vertexLength - 0.5f, 0, z * vertexLength - 0.5f);
                verts[ index + 1 ] = new Vector3(x * vertexLength + 0.5f, 0, z * vertexLength + 0.5f);
                uvs[ index ] = new Vector2( ((float)x * vertexLength) * uvStep - 0.5f, ((float)z * vertexLength) * uvStep - 0.5f );
                uvs[ index + 1 ] = new Vector2( ((float)x * vertexLength) * uvStep + 0.5f, ((float)z * vertexLength) * uvStep + 0.5f );

                if (x < dimension - 1 && z < dimension - 1 ) 
                {
                    tris[ triIndex + 0 ] = index + 0;
                    tris[ triIndex + 1 ] = index + 1;
                    tris[ triIndex + 2 ] = index + 2;
                    
                    tris[ triIndex + 3 ] = index + 1;
                    tris[ triIndex + 4 ] = index + 3;
                    tris[ triIndex + 5 ] = index + 2;
                    
                    tris[ triIndex + 6 ] = index + 0;
                    tris[ triIndex + 7 ] = index + (dimension * 2);
                    tris[ triIndex + 8 ] = index + 1;
                    
                    tris[ triIndex + 9 ] = index + 1;
                    tris[ triIndex + 10 ] = index + (dimension * 2);
                    tris[ triIndex + 11 ] = index + (dimension * 2 + 1);
                    triIndex += 12;
                }   

                index += 2;
            }
        }

        verts = AdjustVertices(verts, dimension);

        // - Build Mesh -
        m.vertices = verts;
        m.uv = uvs;
        m.triangles = tris;

        m.RecalculateBounds();
        m.RecalculateNormals();
    }


    
    public Vector3[] AdjustVertices(Vector3[] verts, int dimension)
    {
        float xFactor = heightmapWidth / (float)widthOfTerrain;
        float yFactor = heightmapHeight / (float)heightOfTerrain;
        if (heightmapData == null)
            heightmapData = terrainData.GetHeights( 0, 0, heightmapWidth, heightmapHeight );
        for (int i = 0; i < verts.Length; i++)
        {
            Vector3 vert = verts[i];
            int position = Mathf.FloorToInt(i / 2f);
            int x = Mathf.FloorToInt((position % (float)dimension)) * cellSize - Mathf.FloorToInt(widthOfTerrain / 2f);
            int z = Mathf.FloorToInt((position / (float)dimension)) * cellSize - Mathf.FloorToInt(heightOfTerrain / 2f);
            float y = terrain.SampleHeight(new Vector3(x, 0, z)) + 3f;

//            float y = heightmapData[ (int)z, (int)y] * terrainSize.y; // heightmapData is Y,X ; not X,Y (reversed)
            verts[i] = new Vector3(vert.x, y, vert.z);
        }
        return verts;
    }
    
    public Vector2 GetHeightmapPosition(Vector3 point)
    {
        Vector2 heightmapPos = new Vector3();
        // find the heightmap position of that hit
        heightmapPos.x = Mathf.FloorToInt(( point.x / (float)widthOfTerrain ) * widthInCells);
        heightmapPos.y = Mathf.FloorToInt(( point.z / (float)heightOfTerrain ) * heightInCells ); 
        
        // clamp to heightmap dimensions to avoid errors
        heightmapPos.x = Mathf.Clamp( heightmapPos.x, 0, widthInCells - 1 );
        heightmapPos.y = Mathf.Clamp( heightmapPos.y, 0, heightInCells - 1 );
        
        return heightmapPos;
    }
    

    public Vector3[,] CalculateGrid(Vector2 heightmapPos, int width, int height, int increment)
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
                calcVector.z = heightmapPos.y * cellSize + ( z * indicatorSize );
                
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

    public void UpdateMesh(Vector3[,] mapGrid, Mesh mesh)
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
    
    void PaintNetwork()
    {
        
    }



    GameObject CreateMeshObject(string name, int vertexLength, Vector3 position)
    {
        Type[] meshTypes = new Type[2];
        meshTypes[0] = typeof(MeshFilter);
        meshTypes[1] = typeof(MeshRenderer);

        GameObject meshObject = new GameObject(name, meshTypes);
        meshObject.transform.position = position;
//        meshObject.transform.localScale = new Vector3(vertexLength, 1, vertexLength);
        meshObject.transform.localScale = new Vector3(1, 1, 1);

        MeshFilter filter = (MeshFilter)meshObject.GetComponent<MeshFilter>();
        MeshRenderer r = meshObject.GetComponent<MeshRenderer>();
        r.material.color = Color.red;

        Mesh m = new Mesh ();
        filter.mesh = m;
        m.name = name + "_Mesh";
        m.Clear();
        return meshObject;
    }

	void PaintTerrain ()
	{
		bool reviseTerrain = gameState.showUpdatedTerrain;

		if (reviseTerrain) {

            // Obtain the alphamap to update
            Vector2[] extents = borderControl.GetBorderVertices();
            int originX = Mathf.FloorToInt((extents[0].x / widthInCells ) * alphamapWidth);
            int originY = Mathf.FloorToInt((extents[0].y / widthInCells) * alphamapHeight);
            int endX = Mathf.FloorToInt(((extents[1].x - extents[0].x) / widthInCells) * alphamapWidth);
            int endY = Mathf.FloorToInt(((extents[1].y - extents[0].y) / widthInCells) * alphamapHeight);
            int cellOriginX = (int)extents[0].x;
            int cellOriginY = (int)extents[0].y;
            float[,,] changeableMap = terrainData.GetAlphamaps (originX, originY, endX, endY);


            foreach (KeyValuePair<Vector2, Patch> entry in patches) {
                Vector2 actualPos = entry.Key;
                Patch patch = entry.Value;
                if (patch.SignificantChange())
                {
                    int count = (int)patch.health;
                    int cellOffsetX = (int)((actualPos.x - cellOriginX) / widthInCells * alphamapWidth);
                    int cellOffsetY = (int)((actualPos.y - cellOriginY) / heightInCells * alphamapHeight);
                    int maxCount = (count > 100 ? 100 : count);
                    for (int y = cellOffsetY; y < cellOffsetY + (int)alphamapHeightPerCell; y++) {
                        for (int x = cellOffsetX; x < cellOffsetX + (int)alphamapWidthPerCell; x++) {
                            changeableMap [y, x, 0] = (maxCount / 100.0f);
                            changeableMap [y, x, 1] = 1 - (maxCount / 100.0f);
                            changeableMap [y, x, 2] = 0f;
                            changeableMap [y, x, 3] = 0f;
                        }
                    }
                }
            }
            terrainData.SetAlphamaps (originX, originY, changeableMap);
        }
    }

	void PaintBuildings ()
	{
		bool buildBuildings = gameState.showBuildings;

		if (buildBuildings) {
			if (currentCells != null)
			{
				foreach (Vector2 actualPos in currentCells) {
					if (! buildings.ContainsKey (actualPos)) {
                        float r = UnityEngine.Random.Range(0, 100);
                        if (r < gameState.chanceOfBuilding) {
                            AddBuilding (actualPos);
                        }
					}
				}

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

    public void AddBuilding(Vector2 actualPos)
    {
        if (buildings.ContainsKey(actualPos))
        {
            Debug.Log ("Already a building here!");
            return;
        }
        float buildingsDimension = cellSize / 4f;

        int w = (int)actualPos.x;
        int h = (int)actualPos.y;
        
        GameObject gameObject = null;
        int x = (int)((w / (float)widthInCells) * widthOfTerrain);
        int y = (int)((h / (float)heightInCells) * heightOfTerrain);
        int normalisedX = x - (int)(widthOfTerrain / 2) - (cellSize / 2);
        int normalisedY = y - (int)(heightOfTerrain / 2) - (cellSize / 2);
        float height = terrain.SampleHeight(new Vector3(normalisedX, 0, normalisedY));
     
        if (height > 1.0f)
        {
            int buildingIndex = UnityEngine.Random.Range(0, 8);
            //                          GameObject buildingPrefab = buildingPrefabs[buildingIndex];
            //                          gameObject = (GameObject)Instantiate(buildingPrefab);
            gameObject = (GameObject)Instantiate(baseBuilding);
            Building building = gameObject.GetComponent<Building>();
            building.position = actualPos;
            gameObject.name = ("Building at: (" + normalisedX + ", " + normalisedY + ", " + height + ")");
            
            Vector3 position = new Vector3 (normalisedX, height, normalisedY);
            Vector3 scale = new Vector3 (buildingsDimension, buildingsDimension, buildingsDimension);
            gameObject.transform.position = position;
            gameObject.transform.localScale = scale;
            gameObject.transform.parent = dynamicObjectsTransform;
            buildings.Add (actualPos, building);
        }
    }
    
    public Patch TurnOnCell (Vector3 worldPosition)
    {
        if (!borderControl.WithinBorders(worldPosition))
            return null;
        // Normalise to the terrain space
		Vector3 terrainPosition = new Vector3 (
          worldPosition.x + widthOfTerrain / 2 , 
          worldPosition.y, 
          worldPosition.z + heightOfTerrain / 2
		);
		int w = (int)(terrainPosition.x / widthOfTerrain * widthInCells);
		int h = (int)(terrainPosition.z / heightOfTerrain * heightInCells);

        Patch patch = null;
        if (w < widthInCells && h < heightInCells) {
			Vector2 actualPos = new Vector2 (w, h);

			if (! patches.ContainsKey (actualPos)) {
                patch = new Patch(actualPos);
                patches.Add(actualPos, patch);
			}
            else 
            {
                patch = patches[actualPos];
            }
            bool significantChange = patch.visit();
            if (!currentCells.Contains(actualPos) && significantChange)
                currentCells.Add(actualPos);

            if (buildings.ContainsKey (actualPos)) {
                Building building = buildings[actualPos];
                building.visit();
            }

		}
        return patch;
	}
}

