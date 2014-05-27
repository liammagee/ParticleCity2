using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ToggleButtonEvents : MonoBehaviour 
{

	public void OnClick( dfControl control, dfMouseEventArgs mouseEvent )
	{
        if (Time.timeScale == 0.0F) 
        {
            dfButton button = (dfButton)control;
            button.Text = "Pause";
            Time.timeScale = 1.0F;
        }
		else
        {
            dfButton button = (dfButton)control;
            button.Text = "Play";
            Time.timeScale = 0.0F;
        }
	}
}
