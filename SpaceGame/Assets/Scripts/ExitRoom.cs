using UnityEngine;
using System.Collections;

public class ExitRoom : Photon.MonoBehaviour {

	int startPlayers;

	// Use this for initialization
	void Start () {
		startPlayers = PhotonNetwork.playerList.Length;
		Debug.Log (startPlayers);
	}
	
	// Update is called once per frame
	void Update () {
		int currentPlayers = PhotonNetwork.playerList.Length;
		if (startPlayers > currentPlayers) {
			photonView.RPC("Exit", PhotonTargets.AllBuffered);
		}

		if (Input.GetKeyDown (KeyCode.Escape)) {
			Debug.Log("Key pressed");
			photonView.RPC("Exit", PhotonTargets.AllBuffered);
		}
	}

	[PunRPC]
	public void Exit() {
		PhotonNetwork.LoadLevel(0);
	}
}
