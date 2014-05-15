using System.Collections;
using System.Collections.Generic;
using LibNoise.Unity;
using LibNoise.Unity.Generator;
using LibNoise.Unity.Operator;
using System.IO;
using System;
using UnityEngine;

public enum NoiseTypeGenerator {Perlin, Billow, RiggedMultifractal, Voronoi, Mix};

public class RandomTerrain : MonoBehaviour {
    private Noise2D m_noiseMap = null;
    private Texture2D[] m_textures = new Texture2D[3];
    public int resolution = 64; 
    public NoiseTypeGenerator noise = NoiseTypeGenerator.Perlin;
    public float zoom = 1f; 
    public float offset = 0f;
    
    public void Start() {
    }
    

    public void Generate() {	
        resolution = Terrain.activeTerrain.terrainData.alphamapHeight;

        // Create the module network
        ModuleBase moduleBase;
        switch(noise) {
            case NoiseTypeGenerator.Billow:	
        	moduleBase = new Billow();
        	break;
        	
            case NoiseTypeGenerator.RiggedMultifractal:	
        	moduleBase = new RiggedMultifractal();
        	break;   
        	
            case NoiseTypeGenerator.Voronoi:	
        	moduleBase = new Voronoi();
        	break;             	         	
        	
          	case NoiseTypeGenerator.Mix:            	
        	Perlin perlin = new Perlin();
        	RiggedMultifractal rigged = new RiggedMultifractal();
        	moduleBase = new Add(perlin, rigged);
        	break;
        	
        	default:
        	moduleBase = new Perlin();
        	break;
        	
        }
         
        // Initialize the noise map
        this.m_noiseMap = new Noise2D(resolution, resolution, moduleBase);
        double left = offset + UnityEngine.Random.Range(-1f, 0f) * 1/zoom;
        double right = offset + UnityEngine.Random.Range(0f, 1f) * 1/zoom;
        double top = offset + UnityEngine.Random.Range(-1f, 0f) * 1/zoom;
        double bottom = offset + UnityEngine.Random.Range(0f, 1f) * 1/zoom;
        this.m_noiseMap.GeneratePlanar(left, right, top, bottom);
        


        TerrainData terrainData = Terrain.activeTerrain.terrainData;
        terrainData.SetHeights(0, 0, this.m_noiseMap.GetData(0.1f));

    }
}