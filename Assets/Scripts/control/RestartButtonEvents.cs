using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RestartButtonEvents : MonoBehaviour 
{
    GameState gameState = null;

    void Awake()
    {
        gameState = GameObject.Find ("Main Camera").GetComponent<GameState>();
    }

	public void OnClick( dfControl control, dfMouseEventArgs mouseEvent )
	{
		Application.LoadLevel (0);
//		ParticleCity particleCity = GameObject.Find("Main Camera").GetComponent<ParticleCity>();
//		particleCity.SoftRestart();
	}
}
