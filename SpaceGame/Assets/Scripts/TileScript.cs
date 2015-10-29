using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TileScript : MonoBehaviour {

	public Toolbox.TileType tileType;
	public int tileNumber;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetEnemies(){
		for (int i = 0; i < transform.childCount; i++){
			HexScript hex = transform.GetChild(i).GetComponent<HexScript>();
			if (hex != null){
				switch (hex.hexFeature) {
				case Toolbox.HexFeature.RampageGreen:
					GameObject greenPile = GameObject.Find("Green Enemies");
					if (greenPile.transform.childCount == 1){
						//shuffle pile
					}
					hex.enemiesOnHex.Add(greenPile.transform.GetChild(0).GetComponent<EnemyScript>());
					greenPile.transform.GetChild(0).SetParent(hex.transform);
					break;
				default:
					break;
				}
			}
		}
	}
}
