using UnityEngine;
using FiercePlanet;
using System.Collections;
using System.Collections.Generic;

public class CheckboxShowTerrain : MonoBehaviour 
{
	public static Terrain terrain;
    private GameState gameState;
    private Material skybox;


	public void Start() {
		gameState = GameObject.Find("Main Camera").GetComponent<GameState>();
		dfCheckbox chbox  = gameObject.GetComponents<dfCheckbox>()[0];
		chbox.IsChecked = gameState.showTerrain;
        skybox = RenderSettings.skybox;
	}

	public void OnCheckChanged( dfControl control, bool value )
	{
        // Set the game state 
		if (gameState != null)
			gameState = GameObject.Find("Main Camera").GetComponent<GameState>();
		gameState.showTerrain = value;
        gameState.using3d = !value;

        // Switch the skybox alpha channel
        if (value)
        {
            Material m = new Material(Shader.Find("Diffuse"));
            if (m.HasProperty("_Color")) {
                m.color = Color.black;
                RenderSettings.skybox = m;
            }
        }
        else 
        {
            RenderSettings.skybox = skybox;
        }

        // Toggle the terrain
		if (terrain == null) {
			terrain = GameObject.Find("GridOrigin").GetComponent<Grid>().GetTerrain();
		}
        terrain.enabled = !value;

        // Update the position of agents
        if (! value) {
            foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Agent")) {
                Agent agentScript = (Agent)gameObject.GetComponent("Agent");
                if (!gameState.using3d) {
                    agentScript.ResetZAxis();
                }
            }
        }
	}
}
//http://www.everyday3d.com/unity3d/drawing/lines3d.unitypackage