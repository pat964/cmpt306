using UnityEngine;
using System.Collections;

public class TileFrameScript : MonoBehaviour {
	public bool playerAdjacent;
	public float radius = 8f;
	private playerScript player;
	private Renderer myRenderer;
	private float playerProximity;
	private GameObject gameBoard;
	// Use this for initialization
	void Start () {
		gameBoard = GameObject.Find("Game Board");
		player = (playerScript) FindObjectOfType(typeof(playerScript));
		myRenderer = GetComponent<Renderer>();
	}
	
	void Update () {
		if (Input.GetMouseButtonDown(0)){
			TileClicked();
		}
		CheckProximity();
	}
	
	void CheckProximity() {
		playerProximity = Vector2.Distance(player.GetComponent<Renderer>().bounds.center,
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
		if (GameObject.Find("Tile Deck").transform.childCount > 0) {
			player.moves -= 2;
			player.UpdateMovesMessage();
			GameObject.Find("Tile Deck").transform.GetChild(0).position = transform.position;
			GameObject.Find("Tile Deck").transform.GetChild(0).rotation = transform.rotation;
			GameObject.Find("Tile Deck").transform.GetChild(0).SetParent(gameBoard.transform);
			Destroy(gameObject);
		}
	}
}
