using UnityEngine;
using FiercePlanet;
using System.Collections;
using System.Collections.Generic;

public class Checkbox3DEvents : MonoBehaviour 
{
	private GameState gameState;

	public void Start() {
		gameState = GameObject.Find("Main Camera").GetComponent<GameState>();
		dfCheckbox chbox  = gameObject.GetComponents<dfCheckbox>()[0];
		chbox.IsChecked = gameState.using3d;
	}

	public void OnCheckChanged( dfControl control, bool value )
	{
		gameState.using3d = value;
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
