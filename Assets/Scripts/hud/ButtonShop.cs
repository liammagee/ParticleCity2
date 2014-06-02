using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonShop : MonoBehaviour 
{
    dfPanel shopPanel;
    
    void Start()
    {
        shopPanel = GameObject.Find ("BuildingSelectorPanel").GetComponent<dfPanel>();
    }

    public void OnClick( dfControl control, dfMouseEventArgs mouseEvent )
	{
        shopPanel.IsEnabled = true;
        shopPanel.IsVisible = true;
        dfGUIManager.PushModal(shopPanel);
	}
}
