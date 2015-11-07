using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TileScript : Photon.MonoBehaviour {

	public Toolbox.TileType tileType;
	public int tileNumber;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	[PunRPC] // adds the child to the parent across the whole network
	void Parenting(int child, int parent, bool worldPositionStays){
		PhotonView x = PhotonView.Find (child);
		PhotonView y = PhotonView.Find (parent);
		
		x.transform.SetParent(y.transform, worldPositionStays);
	}
	
	public void SetEnemies(){
		GameObject greenPile = GameObject.Find("Green Enemies");
		GameObject redPile = GameObject.Find("Red Enemies");
		GameObject whitePile = GameObject.Find("White Enemies");
		GameObject brownPile = GameObject.Find("Brown Enemies");
		GameObject purplePile = GameObject.Find("Purple Enemies");
		GameObject greyPile = GameObject.Find("Grey Enemies");
		EnemyScript newEnemy;
		for (int i = 0; i < transform.childCount; i++){
			HexScript hex = transform.GetChild(i).GetComponent<HexScript>();
			if (hex != null){
				switch (hex.hexFeature) {
				case Toolbox.HexFeature.RampageGreen:
					if (greenPile.transform.childCount == 1){
						Manager.ShuffleEnemyStack(Toolbox.EnemyColour.Green);
					}
					newEnemy = greenPile.transform.GetChild(0).gameObject.GetComponent<EnemyScript>();
					newEnemy.SetFacing(true);
					newEnemy.homeHex = hex;
					hex.enemiesOnHex.Add(newEnemy);
					photonView.RPC("Parenting", PhotonTargets.AllBuffered, greenPile.transform.GetChild(0).gameObject.GetPhotonView().viewID, hex.gameObject.GetPhotonView().viewID, false);
					break;
				case Toolbox.HexFeature.RampageRed:
					if (redPile.transform.childCount == 1){
						Manager.ShuffleEnemyStack(Toolbox.EnemyColour.Red);
					}
					newEnemy = redPile.transform.GetChild(0).gameObject.GetComponent<EnemyScript>();
					newEnemy.SetFacing(true);
					newEnemy.homeHex = hex;
					hex.enemiesOnHex.Add(newEnemy);
					photonView.RPC("Parenting", PhotonTargets.AllBuffered, redPile.transform.GetChild(0).gameObject.GetPhotonView().viewID, hex.gameObject.GetPhotonView().viewID, false);
					break;
				case Toolbox.HexFeature.Base:
					if (greyPile.transform.childCount == 1){
						Manager.ShuffleEnemyStack(Toolbox.EnemyColour.Grey);
					}
					newEnemy = greyPile.transform.GetChild(0).gameObject.GetComponent<EnemyScript>();
					hex.enemiesOnHex.Add(newEnemy);
					photonView.RPC("Parenting", PhotonTargets.AllBuffered, greyPile.transform.GetChild(0).gameObject.GetPhotonView().viewID, hex.gameObject.GetPhotonView().viewID, false);
					if (Toolbox.Instance.isDay){
						newEnemy.SetFacing(true);
					}
					newEnemy.homeHex = hex;
					newEnemy.siteFortification = true;
					break;
				case Toolbox.HexFeature.DarkMatterResearch:
					if (purplePile.transform.childCount == 1){
						Manager.ShuffleEnemyStack(Toolbox.EnemyColour.Purple);
					}
					newEnemy = purplePile.transform.GetChild(0).gameObject.GetComponent<EnemyScript>();
					hex.enemiesOnHex.Add(newEnemy);
					photonView.RPC("Parenting", PhotonTargets.AllBuffered, purplePile.transform.GetChild(0).gameObject.GetPhotonView().viewID, hex.gameObject.GetPhotonView().viewID, false);
					if (Toolbox.Instance.isDay){
						newEnemy.SetFacing(true);
					}
					newEnemy.homeHex = hex;
					newEnemy.siteFortification = true;
					break;
				default:
					break;
				}
			}
		}
	}
}
