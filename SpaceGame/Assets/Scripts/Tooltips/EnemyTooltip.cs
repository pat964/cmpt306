using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EnemyScript))]
public class EnemyTooltip : Photon.MonoBehaviour {
	
	// code learned from following example below so some similarities may appear
	// http://answers.unity3d.com/questions/44811/tooltip-when-mousing-over-a-game-object.html
	
	public string toolTipText = ""; 
	private string currentToolTipText = "";
	private GUIStyle guiStyleFore;
	private GUIStyle guiStyleBack;
	private EnemyScript enemy;

	public void Start()
	{
		enemy = this.GetComponent<EnemyScript>();
		// the name of the enemy
		string name = "NAME:   " + enemy.enemyName;
		// the armor of the enemy
		string armor =  "\nARMOR:   " + enemy.armor;
		// the resistances
		string resistances =  "\nRESISTANCES:";
		for (int i = 0; i < enemy.resistances.Count; i++) {
			resistances = resistances + "   " + enemy.resistances[i];
		}
		if (0 == this.GetComponent<EnemyScript> ().resistances.Count) {
			resistances = resistances + "   None";
		}
		// the attacks
		string attacks =  "\nATTACKS: ";
		for (int i = 0; i < enemy.attacks.Count; i++) {
			attacks = attacks + "\nType: " + enemy.attacks[i].type;
			attacks = attacks + "   Strength: " + enemy.attacks[i].value;
		}

		//the specials
		string specials = "\nSPECIALS: ";
		int specialCount = 0;
		foreach( Toolbox.EnemySpecial special in enemy.specials){
			if(special == Toolbox.EnemySpecial.Fortified || 
			   special == Toolbox.EnemySpecial.Brutal ||
			   special == Toolbox.EnemySpecial.Poison ||
			   special == Toolbox.EnemySpecial.Swift){
				specialCount++;
				specials += "\n" + special.ToString();
			}
		}
		if (specialCount == 0){
			specials += "\nNone";
		}

		//the fame
		string fame = "\nFAME: " + enemy.fame;

		toolTipText = name + armor + resistances + attacks + specials + fame;

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
