using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BorderControl : MonoBehaviour 
{
	public void RedoBorders(float value) 
	{
		int intValue = (int)value;
		GameObject bottom = GameObject.Find("CubeBottomBorder");
		GameObject left = GameObject.Find("CubeLeftBorder");
		GameObject right = GameObject.Find("CubeRightBorder");
		GameObject top = GameObject.Find("CubeTopBorder");
		bottom.transform.position = new Vector3(0, 450, intValue * 20);
		bottom.transform.localScale = new Vector3(intValue * 20, 1000, 200);
		left.transform.position = new Vector3(-intValue * 20, 450, 0);
		left.transform.localScale = new Vector3(200, 1000, intValue * 20);
		right.transform.position = new Vector3(intValue * 20, 450, 0);
		right.transform.localScale = new Vector3(200, 1000, intValue * 20);
		top.transform.position = new Vector3(0, 450, -intValue * 20);
		top.transform.localScale = new Vector3(intValue * 20, 1000, 200);

		// Ensure agents are inside the bordrs
		foreach (GameObject agent in GameObject.FindGameObjectsWithTag("Agent")) {
			Vector3 newPosition = agent.transform.position;
			if (newPosition.x < -value * 10)
				newPosition.x = -value * 10 + 101;
			if (newPosition.x > value * 10)
				newPosition.x = value * 10 - 101;
			if (newPosition.z < -value * 10)
				newPosition.z = -value * 10 + 101;
			if (newPosition.z > value * 10)
				newPosition.z = value * 10 - 101;
			agent.transform.position = newPosition;
		}
	}
}
