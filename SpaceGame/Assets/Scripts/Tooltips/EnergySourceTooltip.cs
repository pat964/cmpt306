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
	private int delay; // the delay time for the tooltip to appear
	private const int TIP_DELAY = 75; // time until the tooltip appears
	private const int HELP_DELAY = 125; // time until additional help is added

	public void Start()
	{
		toolTipText = "ENERGY TYPE: " + this.GetComponent<energyDiceScript> ().colour;
		// set up
		guiStyleFore = new GUIStyle();
		guiStyleFore.normal.textColor = Color.white;
		guiStyleFore.alignment = TextAnchor.UpperCenter ;
		guiStyleFore.wordWrap = true;
		guiStyleBack = new GUIStyle();
		guiStyleBack.normal.textColor = Color.black;
		guiStyleBack.alignment = TextAnchor.UpperCenter ;
		guiStyleBack.wordWrap = true;
	}
	
	// Shows the tool tip after a delay because we may not want to see hex data all the time
	public void OnMouseOver() {
		delay++;
		if (delay > HELP_DELAY) { // if user is looking at this long enough, add help text
			currentToolTipText = toolTipText + "\nYou can use an energy source to power " +
				"up a card of the same colour. Only one can be used per turn, and once used a new one" +
				"will take its place.";
		}
		else if (delay > TIP_DELAY) { // else just show regular text
			currentToolTipText = toolTipText;
		}
	}
	
	// resets the delay and tool text when mouse event finishes
	public void OnMouseExit ()
	{
		delay = 0;
		currentToolTipText = "";
	}
	
	// makes the tool tip
	public void OnGUI()
	{
		if (currentToolTipText != "")
		{
			float x = Event.current.mousePosition.x;
			float y = Event.current.mousePosition.y;
			GUI.Label (new Rect (x-149,y+21,300,60), currentToolTipText, guiStyleBack);
			GUI.Label (new Rect (x-150,y+20,300,60), currentToolTipText, guiStyleFore);
		}
	}
}
