using UnityEngine;
using System.Collections;

[RequireComponent(typeof(PhotonView))]
public class PlayerTooltip : Photon.MonoBehaviour {

	// code learned from following example below so some similarities may appear
	// http://answers.unity3d.com/questions/44811/tooltip-when-mousing-over-a-game-object.html

	public string toolTipText = ""; 
	private string currentToolTipText = "";
	private GUIStyle guiStyleFore;
	private GUIStyle guiStyleBack;
	private string player;
	private Scorekeeper scorekeeper;

	public void Start()
	{
		scorekeeper = GameObject.Find ("Scorekeeper").GetComponent<Scorekeeper>();
		// sets up tooltip text
		player = "PLAYER:   " + this.photonView.owner.name ;
		string fame =  "\nFAME:   ";
		toolTipText = player + fame;

		// sets up tooltip
		guiStyleFore = new GUIStyle();
		guiStyleFore.normal.textColor = Color.white;
		guiStyleFore.alignment = TextAnchor.UpperCenter ;
		guiStyleFore.wordWrap = true;
		guiStyleBack = new GUIStyle();
		guiStyleBack.normal.textColor = Color.black;
		guiStyleBack.alignment = TextAnchor.UpperCenter ;
		guiStyleBack.wordWrap = true;
	}

	// updates the score 
	public void Update() {
		string fame = "\nFAME:   ";
		if (scorekeeper != null) {
			if (scorekeeper.containsScore (this.photonView.owner.ID)) {
				fame = fame + scorekeeper.getScore (this.photonView.owner.ID);
			}
		}
		toolTipText = player + fame;
	}

	public void OnMouseEnter ()
	{
		currentToolTipText = toolTipText;
	}
	
	public void OnMouseExit ()
	{
		currentToolTipText = "";
	}

	// displays the tooltip if this player is not you
	public void OnGUI()
	{
		if (currentToolTipText != "" && !this.photonView.isMine )
		{
			float x = Event.current.mousePosition.x;
			float y = Event.current.mousePosition.y;
			GUI.Label (new Rect (x-149,y+21,300,60), currentToolTipText, guiStyleBack);
			GUI.Label (new Rect (x-150,y+20,300,60), currentToolTipText, guiStyleFore);
		}
	}

}
