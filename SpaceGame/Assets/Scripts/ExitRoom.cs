using UnityEngine;
using System.Collections;

public class ExitRoom : Photon.MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Debug.Log("Key pressed");
			photonView.RPC("Exit", PhotonTargets.AllBuffered);
		}
	}

	[PunRPC]
	public void Exit() {
		PhotonNetwork.LoadLevel(0);
		PhotonNetwork.LeaveRoom();
		PhotonNetwork.Disconnect();
	}
}
