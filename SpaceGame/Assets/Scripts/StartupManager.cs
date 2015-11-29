using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.UI;


// Made following photon demo so some similarities may exist
public class StartupManager : MonoBehaviour {

	public Vector2 widthAndHeight = new Vector2(600, 400); // menu size
	private Vector2 scrollPos = Vector2.zero; 
	
	// Sets up lobby GUI 
	public void OnGUI()
	{
		
		// Make menu
		Rect content = new Rect((Screen.width - this.widthAndHeight.x)/2, (Screen.height - this.widthAndHeight.y)/2, this.widthAndHeight.x, this.widthAndHeight.y);
		GUILayout.Space(20);
		GUI.Box(content, "Welcome to [name of game]");
		GUILayout.BeginArea(content);
		GUILayout.Space(125);
		
		// start game
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Start game", GUILayout.Width(125)))
		{
			PhotonNetwork.LoadLevel(1);
		}		
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.Space(100);

		// start tutorial
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Start tutorial", GUILayout.Width(125)))
		{
			PhotonNetwork.LoadLevel(4);
		}		
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.EndArea();
	}

}