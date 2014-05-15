using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckboxTerrainUpdate : MonoBehaviour 
{
	public void Start() {
		GameState gamestate = GameObject.Find("Main Camera").GetComponent<GameState>();
		dfCheckbox chbox  = gameObject.GetComponents<dfCheckbox>()[0];
		chbox.IsChecked = gamestate.showUpdatedTerrain;
	}

	public void OnCheckChanged( dfControl control, bool value )
	{
		GameState gameState = GameObject.Find("Main Camera").GetComponent<GameState>();
		gameState.showUpdatedTerrain = value;
	}
}
