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
    ShowStats();
  }

  void ShowStats() {
    Debug.Log("Cell size: " + CellSize);
    Debug.Log("Terrain size x: " + terrainSize.x);
    Debug.Log("Terrain size y: " + terrainSize.y);
    Debug.Log("Terrain size z: " + terrainSize.z);
    Debug.Log("Terrain position x: " + origin.x);
    Debug.Log("Terrain position y: " + origin.y);
    Debug.Log("Terrain position z: " + origin.z);
    Debug.Log("Width: " + width);
    Debug.Log("Height: " + height);
    Debug.Log("Alphamap Width: " + terrain.terrainData.alphamapWidth);
    Debug.Log("Alphamap Height: " + terrain.terrainData.alphamapHeight);
    Debug.Log("Heightmap Width: " + terrain.terrainData.heightmapWidth);
    Debug.Log("Heightmap Height: " + terrain.terrainData.heightmapHeight);
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



  void BuildGrid ()
  {  
    InitGrid();
    ShowGrid();    
  }

	void ShowGrid ()
	{  
    TerrainData data = terrain.terrainData;
    Color c1 = Color.green;
    Color c2 = Color.green;
    float xFactor = data.heightmapWidth / data.size.x;
    float yFactor = data.heightmapHeight / data.size.z;
    GameObject dynamicObjects = GameObject.Find("DynamicObjects");
    GameObject visibleGrid = new GameObject("VisibleGrid");
    visibleGrid.transform.parent = dynamicObjects.transform;
    CheckboxShowGrid.visibleGrid = visibleGrid;
    visibleGrid.SetActive(false);

    for(int x = 0; x <= data.heightmapWidth; x += CellSize) 
    {
      float lastHeight = 0.0f;
      int pointCounter = 0;
      GameObject dummy = new GameObject("Grid line at x: " + x);
      dummy.transform.parent = visibleGrid.transform;
      LineRenderer lineRenderer = (LineRenderer)dummy.AddComponent("LineRenderer");
      lineRenderer.material.color = Color.green;
      lineRenderer.SetWidth(0.5f,0.5f);

      List<Vector3> points = new List<Vector3>();

      for(int y = 0; y <= data.heightmapHeight; y++) 
      {
          float height = data.GetHeight(x, y) + 1.0f;
          // Register change in height
          if (y == 0 || height != lastHeight || y == data.heightmapHeight) {
            int newX = (int)(x / xFactor - (data.size.x / 2));
            int newZ = (int)(y / yFactor - (data.size.z / 2));
            Vector3 point = new Vector3(newX, height, newZ);
            points.Add(point);
          }
      }
      lineRenderer.SetVertexCount(points.Count);
      for (int i = 0; i < points.Count; i++) {
        lineRenderer.SetPosition(i, points[i]);
      }
    }

    for(int x = 0; x <= data.heightmapHeight; x += CellSize) 
    {
      float lastHeight = 0.0f;
      int pointCounter = 0;
      GameObject dummy = new GameObject("Grid line at y: " + x);
      dummy.transform.parent = visibleGrid.transform;
      LineRenderer lineRenderer = (LineRenderer)dummy.AddComponent("LineRenderer");
      lineRenderer.material.color = Color.green;
      lineRenderer.SetWidth(0.5f,0.5f);

      List<Vector3> points = new List<Vector3>();

      for(int y = 0; y <= data.heightmapWidth; y++) 
      {
          float height = data.GetHeight(y, x) + 1.0f;
          // Register change in height
          if (y == 0 || height != lastHeight || y == data.heightmapHeight) {
            int newX = (int)(y / yFactor - (data.size.x / 2));
            int newZ = (int)(x / xFactor - (data.size.z / 2));
            Vector3 point = new Vector3(newX, height, newZ);
            points.Add(point);
          }
      }
      lineRenderer.SetVertexCount(points.Count);
      for (int i = 0; i < points.Count; i++) {
        lineRenderer.SetPosition(i, points[i]);
      }
    }
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
		Vector3 terrainPosition = new Vector3(worldPosition.x + terrain.terrainData.size.x / 2, worldPosition.y, worldPosition.z + terrain.terrainData.size.z / 2);
		int w = (int)(terrainPosition.x / terrain.terrainData.size.x * terrain.terrainData.alphamapWidth);
		int h = (int)(terrainPosition.z / terrain.terrainData.size.z * terrain.terrainData.alphamapHeight);
		Vector2 actualPos = new Vector2 (w, h);

    if (! gridCells.ContainsKey (actualPos)) {
      gridCells.Add (actualPos, 0);
      currentCells.Add (actualPos, 0);
    }
    gridCells[actualPos]++;
	}
}
