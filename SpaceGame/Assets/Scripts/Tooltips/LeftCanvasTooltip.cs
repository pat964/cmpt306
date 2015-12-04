using UnityEngine;
using System.Collections;

public class LeftCanvasTooltip : MonoBehaviour {

	// code learned from following example below so some similarities may appear
	// http://answers.unity3d.com/questions/44811/tooltip-when-mousing-over-a-game-object.html
	
	public string toolTipText = ""; 
	private string currentToolTipText = "";
	private GUIStyle guiStyleFore;
	private GUIStyle guiStyleBack;
	private bool displayTip; // if the tip should be displayed after the timer
	private int delay; // the delay time for the tooltip to appear
	private const int TIP_DELAY = 75; // time until the tooltip appears

	public void Start()
	{
		displayTip = false;
		// set up
		guiStyleFore = new GUIStyle();
		guiStyleFore.normal.textColor = Color.yellow;
		guiStyleFore.alignment = TextAnchor.UpperLeft;
		guiStyleFore.wordWrap = true;
		guiStyleBack = new GUIStyle();
		guiStyleBack.normal.textColor = Color.black;
		guiStyleBack.alignment = TextAnchor.UpperLeft;
		guiStyleBack.wordWrap = true;
	}

	public void Update() {
		if (displayTip) {
			delay++;
		}
		if (delay > TIP_DELAY) {
			currentToolTipText = toolTipText;
		}
	}

	// Shows the tool tip after a delay because we may not want to see the data all the time
	public void OnPointerOver() {
		displayTip = true;
	}

	// resets the delay and tool text when mouse event finishes
	public void OnPointerExit ()
	{
		delay = 0;
		displayTip = false;
		currentToolTipText = "";
	}
	
	// makes the tool tip
	public void OnGUI()
	{
		if (currentToolTipText != "")
		{
			float x = Event.current.mousePosition.x;
			float y = Event.current.mousePosition.y;
			GUI.Label (new Rect (x-21,y+21,200,100), currentToolTipText, guiStyleBack);
			GUI.Label (new Rect (x-20,y+20,200,100), currentToolTipText, guiStyleFore);
		}
	}
}
