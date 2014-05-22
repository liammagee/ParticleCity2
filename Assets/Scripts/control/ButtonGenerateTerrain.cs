using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonGenerateTerrain : MonoBehaviour 
{
	public void OnClick( dfControl control, dfMouseEventArgs mouseEvent )
	{
		// RandomTerrain randomTerrain = GameObject.Find("Terrain").GetComponent<RandomTerrain>();
		// randomTerrain.enabled = true;
		RandomTerrain.Generate();

		Application.LoadLevel (0);
//		ParticleCity particleCity = GameObject.Find("Main Camera").GetComponent<ParticleCity>();
//		particleCity.SoftRestart();

	}
}
