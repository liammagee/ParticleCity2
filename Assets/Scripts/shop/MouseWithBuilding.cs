using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MouseWithBuilding : MonoBehaviour 
{
    public string buildingType;
    public bool showCustomCursor;

    Grid grid;
    Mesh cursorMesh;
    TerrainData terrainData;
    Vector3 terrainSize;
    int cellSize;

    // Cursor variables
    float[,] heightmapData;
    public float indicatorSize = 1.0f;
    public float indicatorOffsetY = 5.0f;

    Vector3[] verts;
    Vector2[] uvs;
    int[] tris;

    private float widthOfTerrain;
    private float heightOfTerrain;
    private int widthInCells;
    private int heightInCells;
    private int heightmapWidth;
    private int heightmapHeight;

    void Start()
    {
//        Screen.showCursor = false;
        showCustomCursor = false;
        grid = GameObject.Find ("GridOrigin").GetComponent<Grid>();

        cellSize = grid.cellSize;
        terrainData = grid.GetTerrain().terrainData;
        terrainSize = terrainData.size;

        ConstructMesh();
    }
    
    
    void Update()
    {
        if (showCustomCursor) 
        {
            ProcessCursorMesh();
        }
    }

    
    void ConstructMesh() 
    {
        if ( !cursorMesh )
        {
            cursorMesh = new Mesh ();
            MeshFilter filter = grid.GetTerrain().GetComponent<MeshFilter>();
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

    

    void ProcessCursorMesh () {
        Vector3 rayHitPoint = RaycastToTerrain ();
        Vector2 heightmapPos = grid.GetHeightmapPosition (rayHitPoint);
        Vector3[,] mapGrid = grid.CalculateGrid (heightmapPos, cellSize, cellSize, 1);
        UpdateMesh (mapGrid, cursorMesh);

        if (Input.GetMouseButtonDown(0)) 
        {
            grid.AddBuilding(heightmapPos + new Vector2(1, 1));
            showCustomCursor = false;
            cursorMesh.Clear();  
        }
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
            rayHitPoint.x += (terrainSize.x / 2);
            rayHitPoint.z += (terrainSize.z / 2);
        }
        return rayHitPoint;
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


}
