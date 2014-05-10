using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckboxBuildings : MonoBehaviour 
{
	public void Start() {
		GameState gamestate = GameObject.Find("Main Camera").GetComponent<GameState>();
		dfCheckbox chbox  = gameObject.GetComponents<dfCheckbox>()[0];
		chbox.IsChecked = gamestate.showBuildings;
	}

	public void OnCheckChanged( dfControl control, bool value )
	{
		GameState gamestate = GameObject.Find("Main Camera").GetComponent<GameState>();
		gamestate.showBuildings = value;
	}
}
