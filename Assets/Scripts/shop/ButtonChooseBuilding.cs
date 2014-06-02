using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonChooseBuilding : MonoBehaviour 
{
    public string buildingType;

    Grid grid;
    MouseWithBuilding mouseWithBuilding;
    dfPanel shopPanel;

    void Start()
    {
        grid = GameObject.Find ("GridOrigin").GetComponent<Grid>();
        mouseWithBuilding = GameObject.Find ("BuildingSelectorPanel").GetComponent<MouseWithBuilding>();
        shopPanel = GameObject.Find ("BuildingSelectorPanel").GetComponent<dfPanel>();
    }

	public void OnClick( dfControl control, dfMouseEventArgs mouseEvent )
	{
        dfGUIManager.PopModal();
        shopPanel.IsVisible = false;
        //        shopPanel.IsEnabled = false;
        dfButton button = ((dfButton)control);
        string sprite = ((dfButton)control).BackgroundSprite;
        List<dfAtlas.ItemInfo> items = ((dfButton)control).Atlas.Items;
        dfAtlas.ItemInfo foundItem = null;
        foreach (dfAtlas.ItemInfo item in items)
        {
            if (item.name == sprite)
            {
                foundItem = item;
                break;
            }
        }
        if (foundItem != null && foundItem.region != null)
        {
            Rect region = foundItem.region;
            mouseWithBuilding.showCustomCursor = true;
        }
    }
}
