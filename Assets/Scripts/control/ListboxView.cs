using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ListboxView : MonoBehaviour 
{

	public void OnSelectedIndexChanged( dfControl control, int value )
	{
		GameObject camera = GameObject.Find("Main Camera");
		GameState gamestate = camera.GetComponent<GameState>();
		MouseOrbit orbit = camera.GetComponent<MouseOrbit>();
		orbit.enabled = false;
		switch(value) {
			case 0:
				camera.transform.position = new Vector3(gamestate.originX, 590f, gamestate.originZ);
				camera.transform.rotation = Quaternion.Euler(90f, 180f, 0f);
				break;
			case 1:
				camera.transform.position = new Vector3(gamestate.originX + 0f, 350f, gamestate.originZ + -300f);
				camera.transform.rotation = Quaternion.Euler(45f, 180f, 0f);
				break;
			case 2:
				camera.transform.position = new Vector3(gamestate.originX + -350f, 350f, gamestate.originZ + -350f);
				camera.transform.rotation = Quaternion.Euler(30f, 225f, 0f);
				break;
			case 3:
				orbit.enabled = true;
				break;
		}
	}

}
