using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class HexScript : MonoBehaviour {
	public bool playerAdjacent;
	public bool playerOn;
	public Toolbox.HexType hexType;
	public Toolbox.HexFeature hexFeature;
	public Toolbox.TerrainType terrainType;

	public bool isSafe;
	public float radius;
	private GameObject player;
	private Renderer myRenderer;
	private float playerProximity;

	// Use this for initialization
	void Start () {
		myRenderer = GetComponent<Renderer>();
		radius = (myRenderer.bounds.size.y / 4) * Mathf.Sqrt(3);
		player = GameObject.Find("Player");
		print (myRenderer.bounds.size.y);
	}

	void FixedUpdate() {
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)){
			HexClicked();
		}
		CheckProximity();
	}

	void CheckProximity() {
		playerProximity = Vector2.Distance(player.GetComponent<Renderer>().bounds.center,
		                                   GetComponent<Renderer>().bounds.center);
		if (playerProximity == 0) {
			playerOn = true;
			playerAdjacent = false;
		} else if (playerProximity <= radius * 2){
			playerOn = false;
			playerAdjacent = true;
		} else {
			playerOn = false;
			playerAdjacent = false;
		}
	}

	void MovePlayerHere(){
		player.transform.position = transform.position;
	}

	void HexClicked() {
		if (playerAdjacent) {
			Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			if(GetComponent<Collider2D>().OverlapPoint(mousePosition))
			{
				player.transform.position = transform.position;
			}

		}
	}
}
