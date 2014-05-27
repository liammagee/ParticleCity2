using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProgressBoundariesEvents : MonoBehaviour 
{
    GameState gamestate;
    BorderControl borderControl;
    Grid grid;
    dfProgressBar prog;

	public void Start() {
		gamestate = GameObject.Find("Main Camera").GetComponent<GameState>();
        borderControl = GameObject.Find("Border").GetComponent<BorderControl>();
        grid = GameObject.Find("GridOrigin").GetComponent<Grid>();
		prog  = gameObject.GetComponents<dfProgressBar>()[0];
		prog.Value = gamestate.boundarySize;
	}

	public void OnValueChanged( dfControl control, float value )
	{
		gamestate.boundarySize = (int)value;
		borderControl = GameObject.Find("Border").GetComponent<BorderControl>();
		borderControl.RedoBorders(value);
        grid.RescalePatches();
	}
}
