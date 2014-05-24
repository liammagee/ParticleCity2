using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProgressBoundariesEvents : MonoBehaviour 
{

	public void Start() {
		GameState gamestate = GameObject.Find("Main Camera").GetComponent<GameState>();
		dfProgressBar prog  = gameObject.GetComponents<dfProgressBar>()[0];
		prog.Value = gamestate.boundarySize;
	}

	public void OnValueChanged( dfControl control, float value )
	{
		GameState gamestate = GameObject.Find("Main Camera").GetComponent<GameState>();
		gamestate.boundarySize = (int)value;
		BorderControl borderControl = GameObject.Find("Border").GetComponent<BorderControl>();
		borderControl.RedoBorders(value);
	}
}
