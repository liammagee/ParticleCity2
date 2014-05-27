using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq; // used for Sum of array

public class ReverseTerrain : MonoBehaviour 
{

	[MenuItem("Terrain/Reverse Horizontal")]
	public static void ReverseTerrainAction()
	{
		TerrainData terrainData = Terrain.activeTerrain.terrainData;
		int width = terrainData.heightmapWidth;
		int height = terrainData.heightmapHeight;
		float[,] heightmaps = terrainData.GetHeights(0, 0, width, height);
		float[,] newHeights = new float[width, height];
		for (int x = 0; x < width ; x++) {
			for (int y = 0; y < height ; y++) {
				newHeights[width - 1 - x, y] = heightmaps[x, y];
			}
		}
		terrainData.SetHeights(0, 0, newHeights);
	}

	[MenuItem("Terrain/Clear Terrain")]
	public static void ClearTerrain()
	{
		TerrainData terrainData = Terrain.activeTerrain.terrainData;
		int width = terrainData.heightmapWidth;
		int height = terrainData.heightmapHeight;
		float[,] heightmaps = terrainData.GetHeights(0, 0, width, height);
		float[,] newHeights = new float[width, height];
		for (int x = 0; x < width ; x++) {
			for (int y = 0; y < height ; y++) {
				// Set 1m above sea level
				newHeights[x, y] = 0.0f;
			}
		}
		terrainData.SetHeights(0, 0, newHeights);
    float[, ,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];
    for (int y = 0; y < terrainData.alphamapHeight; y++)
    {
    for (int x = 0; x < terrainData.alphamapWidth; x++)
     {
     		float[] splatWeights = new float[terrainData.alphamapLayers];
				splatmapData[x, y, 0] = 1.0f;
				splatmapData[x, y, 1] = 0f;
				splatmapData[x, y, 2] = 0f;
				splatmapData[x, y, 3] = 0f;
     }
   }
    terrainData.SetAlphamaps(0, 0, splatmapData);
	}

	[MenuItem("Terrain/Splatmap")]
	/// <summary>
	///  Taken from https://alastaira.wordpress.com/2013/11/14/procedural-terrain-splatmapping/
	/// </summary>
	public static void Splatmap() {
        TerrainData terrainData = Terrain.activeTerrain.terrainData;
 
        // Splatmap data is stored internally as a 3d array of floats, so declare a new empty array ready for your custom splatmap data:
        float[, ,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];
         
        for (int y = 0; y < terrainData.alphamapHeight; y++)
            {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
             {
                // Normalise x/y coordinates to range 0-1 
                float y_01 = (float)y/(float)terrainData.alphamapHeight;
                float x_01 = (float)x/(float)terrainData.alphamapWidth;
                 
                // Sample the height at this location (note GetHeight expects int coordinates corresponding to locations in the heightmap array)
                float height = terrainData.GetHeight(Mathf.RoundToInt(y_01 * terrainData.heightmapHeight),Mathf.RoundToInt(x_01 * terrainData.heightmapWidth) );
                 
                // Calculate the normal of the terrain (note this is in normalised coordinates relative to the overall terrain dimensions)
                Vector3 normal = terrainData.GetInterpolatedNormal(y_01,x_01);
      
                // Calculate the steepness of the terrain
                float steepness = terrainData.GetSteepness(y_01,x_01);
                 
                // Setup an array to record the mix of texture weights at this point
                float[] splatWeights = new float[terrainData.alphamapLayers];
                 
				// The following is referenced from https://alastaira.wordpress.com/2013/11/14/procedural-terrain-splatmapping/
				/*
                // CHANGE THE RULES BELOW TO SET THE WEIGHTS OF EACH TEXTURE ON WHATEVER RULES YOU WANT
                // Texture[0] has constant influence
                splatWeights[0] = 0.5f;
                 
                // Texture[1] is stronger at lower altitudes
                splatWeights[1] = Mathf.Clamp01((terrainData.heightmapHeight - height));
                 
                // Texture[2] stronger on flatter terrain
                // Note "steepness" is unbounded, so we "normalise" it by dividing by the extent of heightmap height and scale factor
                // Subtract result from 1.0 to give greater weighting to flat surfaces
                splatWeights[2] = 1.0f - Mathf.Clamp01(steepness*steepness/(terrainData.heightmapHeight/5.0f));
                 
                // Texture[3] increases with height but only on surfaces facing positive Z axis 
                splatWeights[3] = height * Mathf.Clamp01(normal.z);
				*/
				if (height < 1.0f) {
					splatWeights[0] = 0f;
					splatWeights[1] = 0f;
					splatWeights[2] = 1.0f;
					splatWeights[3] = 0f;
				}
				else if (height < 30.0f) {
					splatWeights[0] = 1.0f;
					splatWeights[1] = 0f;
					splatWeights[2] = 0f;
					splatWeights[3] = 0f;
				}
				else if (height < 100.0f) {
					splatWeights[0] = 0f;
					splatWeights[1] = 1.0f;
					splatWeights[2] = 0f;
					splatWeights[3] = 0f;
				}
				else {
					splatWeights[0] = 0f;
					splatWeights[1] = 0f;
					splatWeights[2] = 0f;
					splatWeights[3] = 1.0f;
				}

				
				// Sum of all textures weights must add to 1, so calculate normalization factor from sum of weights
                float z = splatWeights.Sum();
                 
                // Loop through each terrain texture
                for(int i = 0; i<terrainData.alphamapLayers; i++){
                     
                    // Normalize so that sum of all texture weights = 1
                    splatWeights[i] /= z;
                     
                    // Assign this point to the splatmap array
                    splatmapData[x, y, i] = splatWeights[i];
                }
            }
        }
      
        // Finally assign the new splatmap to the terrainData:
        terrainData.SetAlphamaps(0, 0, splatmapData);
	}
}

