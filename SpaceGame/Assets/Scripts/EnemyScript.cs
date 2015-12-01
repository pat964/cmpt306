using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class EnemyScript : Photon.MonoBehaviour {
	public string enemyName;
	public Toolbox.EnemyColour colour;
	public int armor;
	public List<Toolbox.EnemyAttack> attacks;
	public List<Toolbox.EnemySpecial> specials;
	public List<Toolbox.Resistance> resistances;
	public int fame;
	public bool faceDown = true;
	public bool siteFortification = false;
	public List<GameObject> myLabels;
	public Toggle myToggle;
	public HexScript homeHex;
	public bool defeated = false;

	GameObject enemySprite;

	// Use this for initialization
	void Start () {
		myLabels = new List<GameObject>();
//		enemySprite = PhotonNetwork.Instantiate("Prefabs/EnemySprite", Vector2.zero, new Quaternion(), 0);
		enemySprite = (GameObject) Instantiate(Resources.Load("Prefabs/EnemySprite"));
		LoadEnemySprite();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetFacing(bool faceUp){
		//do stuff
	}

	//Attach a prefab as a child to this object
	private void LoadEnemySprite() {

		switch (colour) {
		case Toolbox.EnemyColour.Brown:
			photonView.RPC("Parenting", PhotonTargets.AllBuffered, enemySprite.GetPhotonView().viewID, photonView.viewID, false);
			photonView.RPC ("EnemySpriteHelper", PhotonTargets.AllBuffered, "Sprites/Enemies/dungeon");
			break;
		case Toolbox.EnemyColour.Green:
			photonView.RPC("Parenting", PhotonTargets.AllBuffered, enemySprite.GetPhotonView().viewID, photonView.viewID, false);
			photonView.RPC ("EnemySpriteHelper", PhotonTargets.AllBuffered, "Sprites/Enemies/sword");
			break;
		case Toolbox.EnemyColour.Grey:
			photonView.RPC("Parenting", PhotonTargets.AllBuffered, enemySprite.GetPhotonView().viewID, photonView.viewID, false);
			photonView.RPC ("EnemySpriteHelper", PhotonTargets.AllBuffered, "Sprites/Enemies/base");
			break;
		case Toolbox.EnemyColour.Purple:
			photonView.RPC("Parenting", PhotonTargets.AllBuffered, enemySprite.GetPhotonView().viewID, photonView.viewID, false);
			photonView.RPC ("EnemySpriteHelper", PhotonTargets.AllBuffered, "Sprites/Enemies/darkmatterresearch");
			break;
		case Toolbox.EnemyColour.Red:
			photonView.RPC("Parenting", PhotonTargets.AllBuffered, enemySprite.GetPhotonView().viewID, photonView.viewID, false);
			photonView.RPC ("EnemySpriteHelper", PhotonTargets.AllBuffered, "Sprites/Enemies/terrorlair");
			break;
		case Toolbox.EnemyColour.White:
			photonView.RPC("Parenting", PhotonTargets.AllBuffered, enemySprite.GetPhotonView().viewID, photonView.viewID, false);
			photonView.RPC ("EnemySpriteHelper", PhotonTargets.AllBuffered, "Sprites/Enemies/whitecity");
			break;

		default:

			break;
		}
	}

	[PunRPC] // changes the enemy sprite
	void EnemySpriteHelper(string hexfeature){
		enemySprite.GetComponent<SpriteRenderer> ().sprite = Resources.Load<Sprite> (hexfeature);
	}

	[PunRPC] // adds the child to the parent across the whole network
	void Parenting(int child, int parent, bool worldPositionStays){
		PhotonView x = PhotonView.Find (child);
		PhotonView y = PhotonView.Find (parent);
		
		x.transform.SetParent(y.transform, worldPositionStays);
	}
}
