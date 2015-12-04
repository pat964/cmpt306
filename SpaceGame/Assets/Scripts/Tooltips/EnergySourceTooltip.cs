using UnityEngine;
using System.Collections;

[RequireComponent(typeof(energyDiceScript))]
public class EnergySourceTooltip : MonoBehaviour {

	// code learned from following example below so some similarities may appear
	// http://answers.unity3d.com/questions/44811/tooltip-when-mousing-over-a-game-object.html
	
	public string toolTipText = ""; 
	private string currentToolTipText = "";
	private GUIStyle guiStyleFore;
	private GUIStyle guiStyleBack;
	private bool displayTip;
	private int delay; // the delay time for the tooltip to appear
	private const int TIP_DELAY = 75; // time until the tooltip appears

	public void Start()
	{
		displayTip = false;
		toolTipText = "\nYou can use an energy source to power up a card of " +
			"the same colour. Only one can be used per turn, and once used a new " +
			"one will take its place. They are shuffled at the end of each turn.";
				// set up
		guiStyleFore = new GUIStyle();
		guiStyleFore.normal.textColor = Color.white;
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

	// Shows the tool tip after a delay because we may not want to see hex data all the time
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
