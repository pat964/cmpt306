using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.UI;


// Made following photon demo so some similarities may exist
public class StartupManager : MonoBehaviour {

	public Vector2 widthAndHeight = new Vector2(600, 400); // menu size

	public void Start() {
		PhotonNetwork.LeaveRoom();
		PhotonNetwork.Disconnect();
	}


	// Sets up lobby GUI 
	public void OnGUI()
	{

		// Make menu
		Rect content = new Rect((Screen.width - this.widthAndHeight.x)/2, (Screen.height - this.widthAndHeight.y)/2, this.widthAndHeight.x, this.widthAndHeight.y);
		GUILayout.Space(20);
		GUI.Box(content, "Welcome to Space Escape");
		GUILayout.BeginArea(content);
		GUILayout.Space(105);
		
		// start online match
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Play Online", GUILayout.Width(125)))
		{
			PhotonNetwork.LoadLevel(1);
		}		
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.Space(50);

		// start offline match
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Play Offline", GUILayout.Width(125)))
		{
			PhotonNetwork.offlineMode = true;
			PhotonNetwork.CreateRoom("Offline");
		}		
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.Space(50);

		// start tutorial
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Tutorial", GUILayout.Width(125)))
		{
			PhotonNetwork.LoadLevel(4);
		}		
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.EndArea();
	}

	// loads game offline when offline is pressed
	public void OnCreatedRoom()
	{
		Debug.Log("OnCreatedRoom");
		PhotonNetwork.LoadLevel (3);
	}

}