using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckboxShowGrid : MonoBehaviour 
{
	public static GameObject visibleGrid;
	private GameState gameState;


	public void Start() {
		gameState = GameObject.Find("Main Camera").GetComponent<GameState>();
		dfCheckbox chbox  = gameObject.GetComponents<dfCheckbox>()[0];
		chbox.IsChecked = gameState.showGrid;
	}

	public void OnCheckChanged( dfControl control, bool value )
	{
		gameState.showGrid = value;
		if (visibleGrid == null) {
			visibleGrid = GameObject.Find("VisibleGrid");
		}
		visibleGrid.SetActive(value);
	}
}
//http://www.everyday3d.com/unity3d/drawing/lines3d.unitypackage