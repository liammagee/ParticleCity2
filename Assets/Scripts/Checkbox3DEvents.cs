using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Checkbox3DEvents : MonoBehaviour 
{

	public void OnCheckChanged( dfControl control, bool value )
	{
		foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("Agent")) {
			Agent agentScript = (Agent)gameObject.GetComponent("Agent");
			agentScript.using3d = value;
			if (!agentScript.using3d) {
				agentScript.ResetZAxis();
			}

			Debug.Log(agentScript.using3d);
		}
	}

}
