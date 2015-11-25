using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine.UI;


// Made following photon demo so some similarities may exist
public class NetworkManager : MonoBehaviour {
	
	private const string VERSION = "v0.0.1";
	public string roomName = "Room name"; // Default room name
	public string playerName = "Player name"; // Default room name
	public string roomSize = "1"; // Default room size
	private bool offline; // true if playing offline, false otherwise

	public Text statusLabel;
	public Vector2 widthAndHeight = new Vector2(600, 400); // menu size
	private Vector2 scrollPos = Vector2.zero; 
	private RoomInfo[] roomsList; // list of rooms
	
	public void Awake()
	{
		// sync levels automatically
		PhotonNetwork.automaticallySyncScene = true;

		offline = false;

		// checks if this client was just created (and not yet online). if so, we connect
		if (PhotonNetwork.connectionStateDetailed == PeerState.PeerCreated)
		{
			// Connect to the photon master-server.
			PhotonNetwork.ConnectUsingSettings("0.9");
		}
		
	}
	
	// Sets up lobby GUI 
	public void OnGUI()
	{
		if (!PhotonNetwork.connected)
		{
			if (PhotonNetwork.connecting)
			{
				GUILayout.Label("Connecting to: " + PhotonNetwork.ServerAddress);
			}
			else
			{
				GUILayout.Label("Not connected. Check console output. Detailed connection state: " + PhotonNetwork.connectionStateDetailed + " Server: " + PhotonNetwork.ServerAddress);
			}
			return;
		}
		
		
		// Make lobby menu
		Rect content = new Rect((Screen.width - this.widthAndHeight.x)/2, (Screen.height - this.widthAndHeight.y)/2, this.widthAndHeight.x, this.widthAndHeight.y);
		GUILayout.Space(20);
		GUI.Box(content, "Join or Create Room");
		GUILayout.BeginArea(content);
		GUILayout.Space(40);
		
		// Player name
		GUILayout.BeginHorizontal();
		GUILayout.Space(20);
		GUILayout.Label("Player name:", GUILayout.Width(100)); 
		playerName = GUILayout.TextField( playerName, 25, GUILayout.Width(292)); 
		GUILayout.Space(45);

		// play offline
		offline = GUILayout.Toggle (offline, "    Play offline");
		GUILayout.Space(15);
		GUILayout.EndHorizontal();
		GUILayout.Space(25);
		
		// Create room
		GUILayout.BeginHorizontal();
		GUILayout.Space(20);
		GUILayout.Label("Room name:", GUILayout.Width(100));
		this.roomName = GUILayout.TextField(this.roomName, 10, GUILayout.Width(120)); // room name
		GUILayout.Space(40);
		GUILayout.Label("Room size:", GUILayout.Width(100)); // room size
		this.roomSize = GUILayout.TextField(this.roomSize, 1, GUILayout.Width(23));
		this.roomSize = Regex.Replace(this.roomSize, @"[^0-4]", ""); // room size 1-4
		GUILayout.Space(37); // create room button
		if (GUILayout.Button("Create Room", GUILayout.Width(125)))
		{
			if (offline) {
				PhotonNetwork.Disconnect ();
				PhotonNetwork.offlineMode = true;
				PhotonNetwork.CreateRoom("Offline");
			}
			else {
				if (playerName.Equals("")) {
					statusLabel.text = "Please enter a player name";
				} 
				else if (this.roomName.Equals("")) {
					statusLabel.text = "Please enter a room name";
				}
				else {
					PhotonNetwork.playerName = playerName; 
					PlayerPrefs.SetString("playerName", playerName); 
					PhotonNetwork.CreateRoom(this.roomName, new RoomOptions() {maxPlayers = byte.Parse(roomSize)}, null);
				}
			}
		}
		GUILayout.Space(15);
		
		GUILayout.EndHorizontal();
		GUILayout.Space(50);
		
		// Join random room
		GUILayout.BeginHorizontal();
		GUILayout.Space(20);
		GUILayout.Label(PhotonNetwork.countOfPlayers + " users are online in " + PhotonNetwork.countOfRooms + " rooms.");
		GUILayout.FlexibleSpace();
		if (PhotonNetwork.GetRoomList ().Length > 0) {	
			if (GUILayout.Button ("Join Random", GUILayout.Width (125))) {
				if (playerName.Equals("")) {
					statusLabel.text = "Please enter a player name";
				} 
				else {
					PhotonNetwork.playerName = playerName; 
					PlayerPrefs.SetString("playerName", playerName); 
					PhotonNetwork.JoinRandomRoom ();
				}
			}
		}
		GUILayout.Space(15);
		GUILayout.EndHorizontal();
		
		// Create rooms list
		GUILayout.Space(30);
		if (PhotonNetwork.GetRoomList().Length == 0)
		{	
			GUILayout.BeginHorizontal();
			GUILayout.Space(20);
			GUILayout.Label("There are currently no games available.");
			GUILayout.EndHorizontal();
		}
		else
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(20);
			GUILayout.Label(PhotonNetwork.GetRoomList().Length + " rooms available:");
			GUILayout.EndHorizontal();
			// TODO - not working
			this.scrollPos = GUILayout.BeginScrollView(this.scrollPos);
			foreach (RoomInfo roomInfo in PhotonNetwork.GetRoomList())
			{
				GUILayout.BeginHorizontal();
				GUILayout.Space(20);
				GUILayout.Label(roomInfo.name + " " + roomInfo.playerCount + "/" + roomInfo.maxPlayers);
				if (GUILayout.Button("Join", GUILayout.Width(125)))
				{
					if (playerName.Equals("")) {
						statusLabel.text = "Please enter a player name";
					} 
					else {
						PhotonNetwork.playerName = playerName; 
						PlayerPrefs.SetString("playerName", playerName); 
						PhotonNetwork.JoinRoom(roomInfo.name);
					}
				}
				GUILayout.Space(15);
				GUILayout.EndHorizontal();
			}
			GUILayout.EndScrollView();

		}

		GUILayout.EndArea();
	}
	
	// When joined by clicking already created room button
	public void OnJoinedRoom()
	{
		Debug.Log("OnJoinedRoom");
	}
	
	// loads first level when room created
	public void OnCreatedRoom()
	{
		Debug.Log("OnCreatedRoom");
		if (offline) {
			PhotonNetwork.LoadLevel (2);
		}
		else {
			PhotonNetwork.LoadLevel(1);
		}
	}

	// Error stuff
	public void OnPhotonCreateRoomFailed()
	{
		statusLabel.text = "Error: Can't create room. Try another room name.";
		GUILayout.Label("Error: Can't create room. Try another room name.");
		Debug.Log("OnPhotonCreateRoomFailed got called.");
	}
	
	public void OnPhotonJoinRoomFailed(object[] cause)
	{
		statusLabel.text = "Error: Can't join room (room closed or full). " + cause[1] + ".";
		Debug.Log("OnPhotonJoinRoomFailed got called.");
	}
	
	public void OnPhotonRandomJoinFailed()
	{
		statusLabel.text = "Error: Can't join random room (none found).";
		Debug.Log("OnPhotonRandomJoinFailed got called.");
	}
	
	public void OnDisconnectedFromPhoton()
	{
		Debug.Log("Disconnected from Photon.");
	}
	
	public void OnFailedToConnectToPhoton(object parameters)
	{
		Debug.Log("OnFailedToConnectToPhoton. StatusCode: " + parameters + " ServerAddress: " + PhotonNetwork.networkingPeer.ServerAddress);
	}
	
}
