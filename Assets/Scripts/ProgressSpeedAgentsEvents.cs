using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProgressSpeedAgentsEvents : MonoBehaviour 
{

	public void Start() {
		GameState gamestate = GameObject.Find("Main Camera").GetComponent<GameState>();
		dfProgressBar prog  = gameObject.GetComponents<dfProgressBar>()[0];
		prog.Value = gamestate.speedAgents;
	}

	public void OnValueChanged( dfControl control, float value )
	{
		foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Agent")) {
			Agent agentScript  = (Agent)gameObject.GetComponent("Agent");
			agentScript.particleSpeed = value;
			agentScript.CalculateSpeed();
		}
	}
}
