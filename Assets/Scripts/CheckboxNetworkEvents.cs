using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckboxNetworkEvents : MonoBehaviour 
{
	public void Start() {
		GameState gamestate = GameObject.Find("Main Camera").GetComponent<GameState>();
		dfCheckbox chbox  = gameObject.GetComponents<dfCheckbox>()[0];
		chbox.IsChecked = gamestate.showNetwork;
	}

	public void OnCheckChanged( dfControl control, bool value )
	{
		GameState gameState = GameObject.Find("Main Camera").GetComponent<GameState>();
		gameState.showNetwork = value;

		// foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Agent")) {
		// 	Agent agentScript  = (Agent)gameObject.GetComponent("Agent");
		// 	agentScript.showNetwork = value;
		// }
	}

}
