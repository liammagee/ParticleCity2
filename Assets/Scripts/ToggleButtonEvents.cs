using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ToggleButtonEvents : MonoBehaviour 
{

	public void OnClick( dfControl control, dfMouseEventArgs mouseEvent )
	{
		if (Time.timeScale == 0.0F)
			Time.timeScale = 1.0F;
		else
			Time.timeScale = 0.0F;
	}
}
