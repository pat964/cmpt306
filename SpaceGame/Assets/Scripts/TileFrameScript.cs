using UnityEngine;
using System.Collections;

public class TileFrameScript : Photon.MonoBehaviour {
	public bool playerAdjacent;
	public float radius = 8f;
	private playerScript player;
	private Renderer myRenderer;
	private float playerProximity;
	private GameObject gameBoard;
	// Use this for initialization
	void Start () {
		gameBoard = GameObject.Find("Game Board");
		myRenderer = GetComponent<Renderer>();
	}
	
	void Update () {
		if (null == player) {
			playerScript[] players = (playerScript[]) FindObjectsOfType (typeof(playerScript));
			for (int i = 0; i < PhotonNetwork.playerList.Length; i++) {
				if (players [i].gameObject.GetPhotonView ().isMine) {
					player = players [i];
				}
			}
		} else {
			if (Input.GetMouseButtonDown (0)) {
				TileClicked ();
			}
			CheckProximity ();
		}
	}
	
	void CheckProximity() {
		playerProximity = Vector2.Distance(player.getPlayer().GetComponent<Renderer>().bounds.center,
		                                   myRenderer.bounds.center);
		if (playerProximity <= radius){
			playerAdjacent = true;
		} else {
			playerAdjacent = false;
		}
	}

	void TileClicked() {
		if (playerAdjacent && player.moves >= 2) {
			Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			
			if(GetComponent<Collider2D>().OverlapPoint(mousePosition))
			{
				ExploreNewTile();
			}
			
		}
	}

	void ExploreNewTile() {
		Transform tileDeck = GameObject.Find ("Tile Deck").transform;
		if (tileDeck.childCount > 0) {
			player.moves -= 2;
			player.UpdateLabels();
			photonView.RPC("Reposition", PhotonTargets.All, tileDeck.GetChild(0).gameObject.GetPhotonView().viewID, transform.position, transform.rotation);
			tileDeck.GetChild(0).gameObject.GetComponent<TileScript>().SetEnemies();
			photonView.RPC("Parenting", PhotonTargets.All, tileDeck.GetChild(0).gameObject.GetPhotonView().viewID, gameBoard.GetPhotonView().viewID);
			photonView.RPC("Destroy", PhotonTargets.MasterClient, gameObject.GetPhotonView().viewID);
		}
	}

	[PunRPC] // adds the child to the parent across the whole network
	void Reposition(int target, Vector3 pos, Quaternion rot){
		PhotonView.Find (target).transform.position = pos;
		PhotonView.Find (target).transform.rotation = rot;
	}

	[PunRPC] // adds the child to the parent across the whole network
	void Destroy(int target){
		PhotonNetwork.Destroy(PhotonView.Find (target));
	}

	[PunRPC] // adds the child to the parent across the whole network
	void Parenting(int child, int parent){
		PhotonView x = PhotonView.Find (child);
		PhotonView y = PhotonView.Find (parent);
		
		x.transform.SetParent(y.transform);
	}

	[PunRPC] // adds the child to the parent across the whole network
	void Parenting(int child, int parent, bool worldPositionStays){
		PhotonView x = PhotonView.Find (child);
		PhotonView y = PhotonView.Find (parent);
		x.transform.SetParent (y.transform, worldPositionStays);
	}
}
