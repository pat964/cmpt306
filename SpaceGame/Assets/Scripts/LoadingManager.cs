using UnityEngine;
using System.Collections;

public class LoadingManager : Photon.MonoBehaviour {
	
	Rigidbody2D rBody; // rigidbody componant of the platform
	public float moveLength; // length that the moving platform moves
	
	// Use this for initialization
	void Start () {
		rBody = GetComponent<Rigidbody2D>();
		
	}
	
	// Wait until all players have joined to start
	void Update () {
		if (PhotonNetwork.room.playerCount == PhotonNetwork.room.maxPlayers) {
			PhotonNetwork.LoadLevel (2);
		} 
		// TODO - Make this work for when offline is checked
		// TODO - time out after certain amount of time
		// TODO - Add 'start' button so it will with players in room even if max is not reached
		rBody.MovePosition (new Vector2 (rBody.position.x - moveLength, rBody.position.y));
		
	}
	
	// changes the direction when boundary is hit
	void OnTriggerEnter2D(Collider2D hit){
		Flip ();
		moveLength *= -1;
	}
	
	void Flip() {
		Vector3 theScale = transform.localScale;
		Debug.Log (theScale);
		
		theScale.x *= -1;
		theScale.y *= -1;
		transform.localScale = theScale;
	}
}
