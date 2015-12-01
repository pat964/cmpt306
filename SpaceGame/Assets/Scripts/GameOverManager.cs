using UnityEngine;
using System.Collections;

public class GameOverManager : Photon.MonoBehaviour {

	public Vector2 widthAndHeight = new Vector2(600, 400); // menu size

	// Sets up lobby GUI 
	public void OnGUI()
	{

		
		// Make menu
		Rect content = new Rect((Screen.width - this.widthAndHeight.x)/2, (Screen.height - this.widthAndHeight.y)/2, this.widthAndHeight.x, this.widthAndHeight.y);
		GUILayout.Space(20);
		GUI.Box(content, "Game Over");
		GUILayout.BeginArea(content);
		GUILayout.FlexibleSpace();

		// Make player scores
		GUILayout.BeginHorizontal();
		GUILayout.Space(150);
		GUILayout.Label("Standings");
		GUILayout.EndHorizontal();
		PhotonPlayer[] players = PhotonNetwork.playerList;
		for ( int i = 0; i < players.Length; i++ )
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(150);
			GUILayout.Label(players[i].name, GUILayout.Width(180));
			GUILayout.FlexibleSpace();
			GUILayout.Label(players[i].GetScore() + " fame points", GUILayout.Width(100));
			GUILayout.Space(150);
			GUILayout.EndHorizontal();
		}
		GUILayout.FlexibleSpace();

		// main menu button
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Main Menu", GUILayout.Width(125)))
		{
			PhotonNetwork.LeaveRoom();
			PhotonNetwork.Disconnect();
			PhotonNetwork.LoadLevel(0);
		}	
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.Space(30);

		GUILayout.EndArea();
	}
}
