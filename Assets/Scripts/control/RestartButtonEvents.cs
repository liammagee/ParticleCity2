using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RestartButtonEvents : MonoBehaviour 
{

	public void OnClick( dfControl control, dfMouseEventArgs mouseEvent )
	{
		Application.LoadLevel (0);
//		ParticleCity particleCity = GameObject.Find("Main Camera").GetComponent<ParticleCity>();
//		particleCity.SoftRestart();
	}
}
