using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckboxTerrainUpdate : MonoBehaviour 
{
	private GameState gameState;

	public void Start() {
		gameState = GameObject.Find("Main Camera").GetComponent<GameState>();
		dfCheckbox chbox  = gameObject.GetComponents<dfCheckbox>()[0];
		chbox.IsChecked = gameState.showUpdatedTerrain;
	}

	public void OnCheckChanged( dfControl control, bool value )
	{
		gameState = GameObject.Find("Main Camera").GetComponent<GameState>();
		gameState.showUpdatedTerrain = value;
	}
}
