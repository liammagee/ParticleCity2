using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckboxShowTerrain : MonoBehaviour 
{
	public static Terrain terrain;
	private GameState gameState;


	public void Start() {
		gameState = GameObject.Find("Main Camera").GetComponent<GameState>();
		dfCheckbox chbox  = gameObject.GetComponents<dfCheckbox>()[0];
		chbox.IsChecked = gameState.showTerrain;
	}

	public void OnCheckChanged( dfControl control, bool value )
	{
		if (gameState != null)
			gameState = GameObject.Find("Main Camera").GetComponent<GameState>();
		gameState.showTerrain = value;
		if (terrain == null) {
			terrain = GameObject.Find("GridOrigin").GetComponent<Grid>().GetTerrain();
		}
		terrain.enabled = value;
	}
}
//http://www.everyday3d.com/unity3d/drawing/lines3d.unitypackage