using UnityEngine;
using System.Collections;

public class GeneralTooltips : MonoBehaviour {

	// code learned from following example below so some similarities may appear
	// http://answers.unity3d.com/questions/44811/tooltip-when-mousing-over-a-game-object.html
	
	public string toolTipText = ""; 
	private string currentToolTipText = "";
	private GUIStyle guiStyleFore;
	private GUIStyle guiStyleBack;
	
	public void Start()
	{
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
	
	public void OnMouseEnter ()
	{
		currentToolTipText = toolTipText;
	}
	
	public void OnMouseExit ()
	{
		currentToolTipText = "";
	}
	
	// sets up the text
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
