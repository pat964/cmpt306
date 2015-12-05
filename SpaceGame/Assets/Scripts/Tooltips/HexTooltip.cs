using UnityEngine;
using System.Collections;

[RequireComponent(typeof(HexScript))]
public class HexTooltip : MonoBehaviour {
	
	// code learned from following example below so some similarities may appear
	// http://answers.unity3d.com/questions/44811/tooltip-when-mousing-over-a-game-object.html
	
	public string toolTipText = "";
	private string currentToolTipText = ""; 
	private string toolTipAdditionalText = ""; // extra feature help info
	private GUIStyle guiStyleFore;
	private GUIStyle guiStyleBack;
	private int delay; // the delay time for the tooltip to appear
	private const int TIP_DELAY = 75; // time until the tooltip appears
	private const int HELP_DELAY = 150; // time until additional help is added

	public void Start()
	{
		// set up
		delay = 0;
		HexScript thisHex = this.GetComponent<HexScript>();
		guiStyleFore = new GUIStyle();
		guiStyleFore.normal.textColor = Color.white;
		guiStyleFore.alignment = TextAnchor.UpperCenter ;
		guiStyleFore.wordWrap = true;
		guiStyleBack = new GUIStyle();
		guiStyleBack.normal.textColor = Color.black;
		guiStyleBack.alignment = TextAnchor.UpperCenter ;
		guiStyleBack.wordWrap = true;

		// adds the type info
		string terrain = "TYPE:   " + thisHex.terrainType;

		// adds the cost info
		string cost = "\nCOST:   ";
		if (thisHex.terrainType.ToString().Equals("Lake") || thisHex.terrainType.ToString().Equals("Mountains")) {
			cost =  cost +  "Impassable";
		}
		else {
			cost =  cost +  HexScript.TerrainTypeToVal(thisHex.terrainType);
		}

		// adds the hex feature info
		string feature = "";
		if (thisHex.hexFeature.ToString ().Equals("Portal")) {
			feature = "\nFEATURE:   Crash site";
			toolTipAdditionalText = "\nThe shattered remains of your poor rocket lie here.";
		}
		else if (thisHex.hexFeature.ToString ().Equals("Glade")) {
			feature = "\nFEATURE:   Glade";
			toolTipAdditionalText = "\nOnce Per timer, ending on here Heals 1.\nStarting a turn here gains you one Dark Energy.";
		}
		else if (thisHex.hexFeature.ToString ().Equals("Town")) {
			feature = "\nFEATURE:   Town";
			toolTipAdditionalText = "\nThe residents of the town will give you an Advanced Action for 7 Influence.\nThey will also Heal 1 for 3 Influence.";
		}
		else if (thisHex.hexFeature.ToString ().Equals("Monastary")) {
			feature = "\nFEATURE:   Monastary";
			toolTipAdditionalText = "\nThe monks of the Monastary will give you a precious artifact for 10 Influence.\nThey will also Heal 1 for 2 Influence.";
		}
		else if (thisHex.hexFeature.ToString ().Equals("Base")) {
			feature = "\nFEATURE:   Base";
			toolTipAdditionalText = "\nThe soldiers at the base will give you an Advanced Action for 6 Influence.";
		}
		else if (thisHex.hexFeature.ToString ().Equals("DarkMatterResearch")) {
			feature = "\nFEATURE:   Dark Matter Research Lab";
			toolTipAdditionalText = "\nThe scientists at the Dark Matter Research Lab will give you a powerful Dark Matter Device for 7 influence.";
		}
		else if (thisHex.hexFeature.ToString ().Equals("Maze")) {
			feature = "\nFEATURE:   Maze";
			toolTipAdditionalText = "\n";
		}
		else if (thisHex.hexFeature.ToString ().Equals("Labyrinth")) {
			feature = "\nFEATURE:   Labyrinth";
			toolTipAdditionalText = "\n";
		}
		else if (thisHex.hexFeature.ToString ().Equals ("RampageGreen") ||
		    thisHex.hexFeature.ToString ().Equals ("RampageRed") ) {
			feature = "\nFEATURE:   Rampaging Alien";
			toolTipAdditionalText = "\nYou cannot move here, but moving adjacent to here twice in a row will provoke the vicious aliens to battle!";
		}
		else if (thisHex.hexFeature.ToString ().Equals ("CityWhite") ||
		         thisHex.hexFeature.ToString ().Equals ("CityRed") ||
		         thisHex.hexFeature.ToString ().Equals ("CityGreen") ||
		         thisHex.hexFeature.ToString ().Equals ("CityBlue")) {
			feature = "\nFEATURE:   City";
			toolTipAdditionalText = "\nFreedom! The city facilitates intergalactic travel.\nGain 5 fame and end the game.";
		}
		else if (thisHex.hexFeature.ToString ().Equals ("MineBlue") ||
		         thisHex.hexFeature.ToString ().Equals ("MineRed") ||
		         thisHex.hexFeature.ToString ().Equals ("MineGreen") ||
		         thisHex.hexFeature.ToString ().Equals ("MineWhite") ||
		         thisHex.hexFeature.ToString ().Equals ("MineDeep")) {
			feature = "\nFEATURE:   Energy Mine";
			toolTipAdditionalText = "\nBeginning your turn here will give you an energy of the mine's colour";
		} 
		else {
			toolTipAdditionalText = "";
		}

		toolTipText = terrain + cost + feature;
	}

	// Shows the tool tip after a delay because we may not want to see hex data all the time
	public void OnMouseOver() {
		delay++;
		if (delay > HELP_DELAY) { // if user is looking at this long enough, add help text
			currentToolTipText = toolTipText + toolTipAdditionalText;
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
