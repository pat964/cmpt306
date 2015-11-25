using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoadingManager : Photon.MonoBehaviour {
	
	Rigidbody2D rBody; // rigidbody componant of the platform
	public float moveLength; // length that the moving platform moves
	private int namesOnScreen; // how many names are currently displayed
	public Text[] playerNames; // labels to put player names
	public bool start; // set true by button if master player presses start
	public Button startButton; // button - master client can press to start before room is full

	void Start () {
		namesOnScreen = 0;
		rBody = GetComponent<Rigidbody2D>();

		// turn off button for players that do not own the room
		if (!PhotonNetwork.isMasterClient) {
			startButton.transform.GetChild(0).GetComponent<Text>().enabled = false;
			startButton.image.enabled = false;
			startButton.enabled = false;
		}

		// Put players in room on screen
		for ( int i = 0; i < PhotonNetwork.room.playerCount; i++) {
			playerNames[i].text = PhotonNetwork.playerList[i].name;
			namesOnScreen ++;
		}

	}
	
	void Update () {

		// Put players in room on screen
		if (namesOnScreen < PhotonNetwork.room.playerCount) {
			playerNames [namesOnScreen].text = PhotonNetwork.playerList [0].name;
		}

		// Wait until all players have joined to start, or until player presses start button
		if ( start || (PhotonNetwork.room.playerCount == PhotonNetwork.room.maxPlayers) ) {
			PhotonNetwork.LoadLevel (2);
		} 

		rBody.MovePosition (new Vector2 (rBody.position.x - moveLength, rBody.position.y));
		
	}
	
	// changes the direction when boundary is hit
	void OnTriggerEnter2D(Collider2D hit){
		Flip ();
		moveLength *= -1;
	}

	// triggered by button press, starts the game
	void startNow() {
		start = true;
	}

	// flips rocket direction
	void Flip() {
		Vector3 theScale = transform.localScale;

		theScale.x *= -1;
		theScale.y *= -1;
		transform.localScale = theScale;
	}
}
