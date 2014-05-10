using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
 
	public Terrain terrain; //terrain grid is attached to
	public int CellSize = 5;
	private Vector3 terrainSize;
	private Vector3 origin;
	private int width;
	private int height;
	public static int counter = 0;
	private List<GameObject> objects;
  private Dictionary<Vector2, int> gridCells;
  private Dictionary<Vector2, int> currentCells;
	private Dictionary<Vector2, int> newCells;
  private Dictionary<Vector2, GameObject> buildings;
  private Vector3 heightmapPos;
  private Vector3 rayHitPoint;

	void Start ()
	{
    CellSize = 5;

    terrain = Terrain.activeTerrain;
		terrainSize = terrain.terrainData.size;
		origin = terrain.transform.position;
  
		width = (int)terrainSize.x / CellSize;
		height = (int)terrainSize.z / CellSize;

		objects = new List<GameObject> ();
    gridCells = new Dictionary<Vector2, int> ();
    currentCells = new Dictionary<Vector2, int> ();
		newCells = new Dictionary<Vector2, int> ();
    buildings = new Dictionary<Vector2, GameObject> ();

		BuildGrid ();  
  }

	void Update (){
    int tw = (int)(terrain.terrainData.alphamapWidth / width);
    int th = (int)(terrain.terrainData.alphamapHeight / height);

    GameState gameState = GameObject.Find("Main Camera").GetComponent<GameState>();
    bool reviseTerrain = gameState.showUpdatedTerrain;
    bool buildBuildings = gameState.showBuildings;

    if (currentCells != null && reviseTerrain) {
      foreach(KeyValuePair<Vector2, int> entry in currentCells)
      {
        Vector2 actualPos = entry.Key;
        int count = gridCells[actualPos];
        int w = (int)actualPos.x;
        int h = (int)actualPos.y;
        int maxCount = (count > 10 ? 10 : count);
        float[,,] existingMap = terrain.terrainData.GetAlphamaps (w, h, tw, th);
        if (existingMap [0, 0, 0] != 1 - (maxCount / 10.0f)) {
          float[,,] map = new float[tw, th, 2];
          for (int x = 0; x < tw; x++) {
            for (int y = 0; y < th; y++) {
              map [x, y, 0] = 1 - (maxCount / 10.0f);
              map [x, y, 1] = (maxCount / 10.0f);
            }
          }
          terrain.terrainData.SetAlphamaps (w, h, map);
        }
      }
    }
    currentCells = new Dictionary<Vector2, int> ();

    if (buildBuildings && gridCells != null) {
      foreach(KeyValuePair<Vector2, int> entry in gridCells)
      {
        Vector2 actualPos = entry.Key;
        int count = entry.Value;
        int w = (int)actualPos.x;
        int h = (int)actualPos.y;
        if (count > 10) {
          GameObject dynamicObjects = GameObject.Find("DynamicObjects");
          GameObject gameObject = null;
          if (buildings.ContainsKey(actualPos)) {
            gameObject = buildings[actualPos];
            GameObject.Destroy(gameObject);
            buildings.Remove(actualPos);
          }
          int maxCount = (count > 50 ? 50 : count) - 10;

          int x = (int)((w / (float)terrain.terrainData.alphamapWidth) * terrain.terrainData.size.x) - (int)(terrain.terrainData.size.x / 2);
          int y = (int)((h / (float)terrain.terrainData.alphamapHeight) * terrain.terrainData.size.z) - (int)(terrain.terrainData.size.z / 2);

          gameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
          gameObject.name = ("Building at: (" + x + ", " + y +  ")");

          // TODO: Use actual terrain height for y value
          Vector3 position = new Vector3(x, 0, y);
          Vector3 scale = new Vector3(4.0f, maxCount * 4.0f, 4.0f);
          gameObject.transform.position = position;
          gameObject.transform.localScale = scale;
          gameObject.transform.parent = dynamicObjects.transform;
          buildings.Add(actualPos, gameObject);
        }
      }
    }
	}

void RaycastToTerrain()
        {
            RaycastHit hit;
            Ray rayPos = Camera.main.ScreenPointToRay( Input.mousePosition );
         
            if ( Physics.Raycast( rayPos, out hit, Mathf.Infinity ) ) // also consider a layermask to just the terrain layer
            {
               Debug.DrawLine( Camera.main.transform.position, hit.point, Color.red );
               rayHitPoint = hit.point;
            }
        }

        void GetHeightmapPosition()
        {
            // find the heightmap position of that hit
            heightmapPos.x = ( rayHitPoint.x / terrainSize.x ) * ((float) heightmapWidth );
            heightmapPos.z = ( rayHitPoint.z / terrainSize.z ) * ((float) heightmapHeight );
         
            // convert to integer
            heightmapPos.x = Mathf.Round( heightmapPos.x );
            heightmapPos.z = Mathf.Round( heightmapPos.z );
         
            // clamp to heightmap dimensions to avoid errors
            heightmapPos.x = Mathf.Clamp( heightmapPos.x, 0, heightmapWidth - 1 );
            heightmapPos.z = Mathf.Clamp( heightmapPos.z, 0, heightmapHeight - 1 );
        }

  private TerrainData terrainData;
  private int heightmapWidth;
  private int heightmapHeight;
  private float[,] heightmapData;
 
  void GetTerrainData()
  {
      if ( !terrain )
      {
         terrain = Terrain.activeTerrain;
      }
   
      terrainData = terrain.terrainData;
   
      terrainSize = terrain.terrainData.size;
   
      heightmapWidth = terrain.terrainData.heightmapWidth;
      heightmapHeight = terrain.terrainData.heightmapHeight;
   
      heightmapData = terrainData.GetHeights( 0, 0, heightmapWidth, heightmapHeight );
  }

// --------------------------------------------------------------------------- Calculate Grid
         
  private Vector3[,] mapGrid = new Vector3[ 9, 9 ];
   
  public float indicatorSize = 1.0f;
  public float indicatorOffsetY = 5.0f;
   
   
  void CalculateGrid()
  {
      for ( int z = -4; z < 5; z ++ )
      {
         for ( int x = -4; x < 5; x ++ )
         {
           Vector3 calcVector;
   
           calcVector.x = heightmapPos.x + ( x * indicatorSize );
           calcVector.x /= ((float) heightmapWidth );
           calcVector.x *= terrainSize.x;
   
           float calcPosX = heightmapPos.x + ( x * indicatorSize );
           calcPosX = Mathf.Clamp( calcPosX, 0, heightmapWidth - 1 );
   
           float calcPosZ = heightmapPos.z + ( z * indicatorSize );
           calcPosZ = Mathf.Clamp( calcPosZ, 0, heightmapHeight - 1 );
   
           calcVector.y = heightmapData[ (int)calcPosZ, (int)calcPosX ] * terrainSize.y; // heightmapData is Y,X ; not X,Y (reversed)
           calcVector.y += indicatorOffsetY; // raise slightly above terrain
   
           calcVector.z = heightmapPos.z + ( z * indicatorSize );
           calcVector.z /= ((float) heightmapHeight );
           calcVector.z *= terrainSize.z;
   
           mapGrid[ x + 4, z + 4 ] = calcVector;
         }
      }
  }

// --------------------------------------------------------------------------- INDICATOR MESH
         
  private Mesh mesh;
   
  private Vector3[] verts;
  private Vector2[] uvs;
  private int[] tris;
         

  public void ConstructMesh()
  {  
    mesh = new Mesh();
    MeshFilter f = Terrain.activeTerrain.GetComponent<MeshFilter>();
    f.mesh = mesh;
    mesh.name = gameObject.name + "Mesh";
    mesh.Clear(); 
    verts = new Vector3[9 * 9];
    uvs = new Vector2[9 * 9];
    tris = new int[ 8 * 2 * 8 * 3];

    float uvStep = 1.0f / 8.0f;
         
    int index = 0;
    int triIndex = 0;

    for ( int z = 0; z < 9; z ++ )
    {
       for ( int x = 0; x < 9; x ++ )
       {
         verts[ index ] = new Vector3( x, 0, z );
         uvs[ index ] = new Vector2( ((float)x) * uvStep, ((float)z) * uvStep );
 
         if ( x < 8 && z < 8 )
         {
          tris[ triIndex + 0 ] = index + 0;
          tris[ triIndex + 1 ] = index + 9;
          tris[ triIndex + 2 ] = index + 1;
 
          tris[ triIndex + 3 ] = index + 1;
          tris[ triIndex + 4 ] = index + 9;
          tris[ triIndex + 5 ] = index + 10;
 
          triIndex += 6;
         }
 
         index ++;
       }
    }
 
 
    // - Build Mesh -
    mesh.vertices = verts;
    mesh.uv = uvs;
    mesh.triangles = tris;
 
    mesh.RecalculateBounds();  
    mesh.RecalculateNormals();
    
  }
 
  void UpdateMesh()
  {
      int index = 0;
   
      for ( int z = 0; z < 9; z ++ )
      {
         for ( int x = 0; x < 9; x ++ )
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
    InitGrid();
    
	}
 
	public Vector3 GetWorldPosition (Vector2 gridPosition)
	{
		return new Vector3 (origin.z + (gridPosition.x * CellSize), origin.y, origin.x + (gridPosition.y * CellSize));
	}
 
	public Vector2 GetGridPosition (Vector3 worldPosition)
	{
		return new Vector2 (worldPosition.z / CellSize, worldPosition.x / CellSize);
	}

  public void InitGrid() {
    float[,,] map = new float[terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight, 2];
    for (int x = 0; x < terrain.terrainData.alphamapWidth; x++) {
      for (int y = 0; y < terrain.terrainData.alphamapHeight; y++) {
        map[x, y, 0] = 1.0f;
        map[x, y, 1] = 0.0f;
      }
    }
    terrain.terrainData.SetAlphamaps (0, 0, map);
  }

	public void TurnOnCell (Vector3 worldPosition)
	{
		int counter = 0;
		Vector3 terrainPosition = new Vector3 (worldPosition.x + terrain.terrainData.size.x / 2, worldPosition.y, worldPosition.z + terrain.terrainData.size.z / 2);
		// Vector2 pos = new Vector2((int)(terrainPosition.x / CellSize), (int)(terrainPosition.z / CellSize));
  //   if (pos.x < 0 || pos.x > width || pos.y < 0 || pos.y > height)
  //     return;
		// int tw = (int)(terrain.terrainData.alphamapWidth / width);
		// int th = (int)(terrain.terrainData.alphamapHeight / height);
    // int w = (int)(tw * pos.x);
    // int h = (int)(th * pos.y);
		int w = (int)(terrainPosition.x / terrain.terrainData.size.x * terrain.terrainData.alphamapWidth);
		int h = (int)(terrainPosition.z / terrain.terrainData.size.z * terrain.terrainData.alphamapHeight);
		Vector2 actualPos = new Vector2 (w, h);
    // Debug.Log(w + ":" + h);

    if (! gridCells.ContainsKey (actualPos)) {
      gridCells.Add (actualPos, 0);
      currentCells.Add (actualPos, 0);
    }
    gridCells[actualPos]++;
	}
}
