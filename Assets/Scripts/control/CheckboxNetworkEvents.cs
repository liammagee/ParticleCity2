using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckboxNetworkEvents : MonoBehaviour 
{
	private GameState gameState;

	public void Start() {
		gameState = GameObject.Find("Main Camera").GetComponent<GameState>();
		dfCheckbox chbox  = gameObject.GetComponents<dfCheckbox>()[0];
		chbox.IsChecked = gameState.showNetwork;
	}

	public void OnCheckChanged( dfControl control, bool value )
	{
		gameState.showNetwork = value;

		// foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Agent")) {
		// 	Agent agentScript  = (Agent)gameObject.GetComponent("Agent");
		// 	agentScript.showNetwork = value;
		// }
	}

}
