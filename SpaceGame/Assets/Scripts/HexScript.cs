﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class HexScript : Photon.MonoBehaviour {
	public bool playerAdjacent;
	public bool playerOn;
	public Toolbox.HexType hexType;
	public Toolbox.HexFeature hexFeature;
	public Toolbox.TerrainType terrainType;
	public List<EnemyScript> enemiesOnHex;

	public bool isSafe;
	public float radius;
	private playerScript player;
	private Renderer myRenderer;
	private float playerProximity;

	GameObject featureSprite;

	// Use this for initialization
	void Start () {
		enemiesOnHex = new List<EnemyScript>();
		player = (playerScript) FindObjectOfType(typeof(playerScript));
		myRenderer = GetComponent<Renderer>();
		radius = (myRenderer.bounds.size.y / 4) * Mathf.Sqrt(3);
		LoadTerrainSprite();
		featureSprite = (GameObject) Instantiate(Resources.Load("Prefabs/HexFeature"));
		LoadHexFeatureSprite();

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
		playerProximity = Vector2.Distance(player.getPlayer().GetComponent<Renderer>().bounds.center,
		                                   myRenderer.bounds.center);
		if (playerProximity <= 0.5) {
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
		player.MoveToHex(this);
	}

	void HexClicked() {
		if (playerAdjacent && player.moves >= HexScript.TerrainTypeToVal(terrainType) &&
		    (hexType != Toolbox.HexType.Rampage || enemiesOnHex.Count == 0)) {
			Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

			if(GetComponent<Collider2D>().OverlapPoint(mousePosition))
			{
				player.moves -= HexScript.TerrainTypeToVal(terrainType);
				player.UpdateLabels();
				MovePlayerHere();
			}

		}
	}

	static int TerrainTypeToVal(Toolbox.TerrainType terrain){
		int plainsVal = 2;
		int hillsVal = 3;
		int desertVal = 5;
		int forrestVal = 5;
		int lakeVal = 999;
		int mountainsVal = 999;
		int wastelandVal = 4;
		int swampVal = 5;
		if (Toolbox.Instance.isDay) {
			forrestVal = 3;
		} else {
			desertVal = 3;
		}

		switch (terrain){
		case Toolbox.TerrainType.Plains:
			return plainsVal;
		case Toolbox.TerrainType.Hills:
			return hillsVal;
		case Toolbox.TerrainType.Desert:
			return desertVal;
		case Toolbox.TerrainType.Wasteland:
			return wastelandVal;
		case Toolbox.TerrainType.Forest:
			return forrestVal;
		case Toolbox.TerrainType.Lake:
			return lakeVal;
		case Toolbox.TerrainType.Mountains:
			return mountainsVal;
		case Toolbox.TerrainType.Swamp:
			return swampVal;
		default:
			throw new UnityException("Terrain type does not exist in Hex class");
		}
	}

	private void LoadTerrainSprite() {
		//SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
		switch (terrainType){
		case Toolbox.TerrainType.Plains:
			photonView.RPC("ChangeColor", PhotonTargets.AllBuffered, "Sprites/HexGreen");
			break;
		case Toolbox.TerrainType.Hills:
			photonView.RPC("ChangeColor", PhotonTargets.AllBuffered, "Sprites/HexBrown");
			break;
		case Toolbox.TerrainType.Desert:
			photonView.RPC("ChangeColor", PhotonTargets.AllBuffered, "Sprites/HexYellow" );
			break;
		case Toolbox.TerrainType.Wasteland:
			photonView.RPC("ChangeColor", PhotonTargets.AllBuffered, "Sprites/HexRed");
			break;
		case Toolbox.TerrainType.Forest:
			photonView.RPC("ChangeColor", PhotonTargets.AllBuffered, "Sprites/HexDarkGreen");
			break;
		case Toolbox.TerrainType.Lake:
			photonView.RPC("ChangeColor", PhotonTargets.AllBuffered, "Sprites/HexBlue");
			break;
		case Toolbox.TerrainType.Mountains:
			photonView.RPC("ChangeColor", PhotonTargets.AllBuffered, "Sprites/HexGrey");
			break;
		case Toolbox.TerrainType.Swamp:
			photonView.RPC("ChangeColor", PhotonTargets.AllBuffered, "Sprites/HexBlack");
			break;
		default:
			throw new UnityException("Terrain type does not exist in Hex class");
		}
	}

	private void LoadHexFeatureSprite() {
		switch (hexFeature){
		case Toolbox.HexFeature.Empty:

			break;
		case Toolbox.HexFeature.Portal:
			featureSprite.transform.SetParent(transform, false); 
			photonView.RPC("HexFeatureHelper", PhotonTargets.AllBuffered, "Sprites/HexFeature/portal" );
			break;
		case Toolbox.HexFeature.MineGreen:
			featureSprite.transform.SetParent(transform, false); 
			photonView.RPC("HexFeatureHelper", PhotonTargets.AllBuffered, "Sprites/HexFeature/greenbattery" );
			break;
		case Toolbox.HexFeature.MineRed:
			featureSprite.transform.SetParent(transform, false); 
			photonView.RPC("HexFeatureHelper", PhotonTargets.AllBuffered, "Sprites/Hexfeature/redbattery" );
			break;
		case Toolbox.HexFeature.MineWhite:
			featureSprite.transform.SetParent(transform, false); 
			photonView.RPC("HexFeatureHelper", PhotonTargets.AllBuffered, "Sprites/HexFeature/emptybattery" );
			break;
		case Toolbox.HexFeature.MineBlue:
			featureSprite.transform.SetParent(transform, false); 
			photonView.RPC("HexFeatureHelper", PhotonTargets.AllBuffered, "Sprites/HexFeature/bluebattery" );
			break;
		case Toolbox.HexFeature.Glade:
			featureSprite.transform.SetParent(transform, false); 
			photonView.RPC("HexFeatureHelper", PhotonTargets.AllBuffered, "Sprites/HexFeature/glade" );
			break;
		case Toolbox.HexFeature.Town:
			featureSprite.transform.SetParent(transform, false); 
			photonView.RPC("HexFeatureHelper", PhotonTargets.AllBuffered, "Sprites/HexFeature/town" );
			break;
		case Toolbox.HexFeature.Monastary:
			featureSprite.transform.SetParent(transform, false); 
			photonView.RPC("HexFeatureHelper", PhotonTargets.AllBuffered, "Sprites/HexFeature/monastary" );
			break;
		case Toolbox.HexFeature.Den:
			featureSprite.transform.SetParent(transform, false); 
			photonView.RPC("HexFeatureHelper", PhotonTargets.AllBuffered, "Sprites/HexFeature/den" );
			break;
		case Toolbox.HexFeature.Dungeon:
			featureSprite.transform.SetParent(transform, false); 
			photonView.RPC("HexFeatureHelper", PhotonTargets.AllBuffered, "Sprites/HexFeature/dungeon" );
			break;
		case Toolbox.HexFeature.Base:
			featureSprite.transform.SetParent(transform, false); 
			photonView.RPC("HexFeatureHelper", PhotonTargets.AllBuffered, "Sprites/HexFeature/base" );
			break;
		case Toolbox.HexFeature.DarkMatterResearch:
			featureSprite.transform.SetParent(transform, false); 
			photonView.RPC("HexFeatureHelper", PhotonTargets.AllBuffered, "Sprites/HexFeature/darkmatterresearch" );
			break;
		case Toolbox.HexFeature.Ruins:
			featureSprite.transform.SetParent(transform, false); 
			photonView.RPC("HexFeatureHelper", PhotonTargets.AllBuffered, "Sprites/ruins" );
			break;
		case Toolbox.HexFeature.SpawningGrounds:
			//No sprite yet
			//featureSprite.transform.SetParent(transform, false); 
			//photonView.RPC("HexFeatureHelper", PhotonTargets.AllBuffered, "Sprites/HexFeature/spawninggrounds" );
			break;
		case Toolbox.HexFeature.TerrorLair:
			featureSprite.transform.SetParent(transform, false); 
			photonView.RPC("HexFeatureHelper", PhotonTargets.AllBuffered, "Sprites/HexFeature/terrorlair" );
			break;
		case Toolbox.HexFeature.CityBlue:
			featureSprite.transform.SetParent(transform, false); 
			photonView.RPC("HexFeatureHelper", PhotonTargets.AllBuffered, "Sprites/HexFeature/bluebattery" );
			break;
		case Toolbox.HexFeature.CityGreen:
			featureSprite.transform.SetParent(transform, false); 
			photonView.RPC("HexFeatureHelper", PhotonTargets.AllBuffered, "Sprites/HexFeature/greencity" );
			break;
		case Toolbox.HexFeature.CityRed:
			featureSprite.transform.SetParent(transform, false); 
			photonView.RPC("HexFeatureHelper", PhotonTargets.AllBuffered, "Sprites/HexFeature/redcity" );
			break;
		case Toolbox.HexFeature.CityWhite:
			featureSprite.transform.SetParent(transform, false); 
			photonView.RPC("HexFeatureHelper", PhotonTargets.AllBuffered, "Sprites/HexFeature/whitecity" );
			break;

		default:
			break;
		}
	}

	[PunRPC] // changes color
	void ChangeColor(string color){
		GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(color);
	}

	[PunRPC] // changes the hex feature image
	void HexFeatureHelper(string hexfeature){
		featureSprite.GetComponent<SpriteRenderer> ().sprite = Resources.Load<Sprite> (hexfeature);
	}

}
	