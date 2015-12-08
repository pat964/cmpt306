using UnityEngine;
using System.Collections;

public class ExitRoom : Photon.MonoBehaviour {

	int startPlayers;
	bool escapePressed;
	public Vector2 widthAndHeight = new Vector2(600, 400); // menu size

	// Use this for initialization
	void Start () {
		startPlayers = PhotonNetwork.playerList.Length;
		escapePressed = false;
	}
	
	// Update is called once per frame
	void Update () {
		int currentPlayers = PhotonNetwork.playerList.Length;
		if (startPlayers > currentPlayers) {
			photonView.RPC("Exit", PhotonTargets.AllBuffered);
		}

		if (Input.GetKeyDown (KeyCode.Escape)) {
			Debug.Log("Escape pressed");
			escapePressed = true;
		}
	}

	// Confirms that the player wants to leave 
	public void OnGUI()
	{
		if (escapePressed) {
			// Make menu
			Rect content = new Rect ((Screen.width - this.widthAndHeight.x) / 2, (Screen.height - this.widthAndHeight.y) / 2, this.widthAndHeight.x, this.widthAndHeight.y);
			GUILayout.Space (20);
			GUI.Box (content, "Exit");
			GUILayout.BeginArea (content);
			GUILayout.Space (105);
		
			// start online match
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			GUILayout.Label ("Are you sure you want to quit?", GUILayout.Width (190));
			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();
			GUILayout.Space (50);

			// start online match
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			if (GUILayout.Button ("Return to Game", GUILayout.Width (125))) {
				escapePressed = false;
			}		
			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();
			GUILayout.Space (50);
		
			// start offline match
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			if (GUILayout.Button ("Main Menu", GUILayout.Width (125))) {
				photonView.RPC ("Exit", PhotonTargets.AllBuffered);
			}		
			GUILayout.FlexibleSpace ();
			GUILayout.EndHorizontal ();
		
			GUILayout.EndArea ();
		}
	}

	[PunRPC]
	public void Exit() {
		PhotonNetwork.LoadLevel(0);
	}
}
