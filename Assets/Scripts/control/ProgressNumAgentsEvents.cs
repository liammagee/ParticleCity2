using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProgressNumAgentsEvents : MonoBehaviour 
{

	public void Start() {
		GameState gamestate = GameObject.Find("Main Camera").GetComponent<GameState>();
		dfProgressBar prog  = gameObject.GetComponents<dfProgressBar>()[0];
		prog.Value = gamestate.numAgents;
	}

	public void OnValueChanged( dfControl control, float value )
	{
		GameState gamestate = GameObject.Find("Main Camera").GetComponent<GameState>();
		gamestate.numAgents = (int)value;
		ParticleCity particleCity = GameObject.Find("Main Camera").GetComponent<ParticleCity>();
		particleCity.ResizeAgents((int)value);
	}
}
