using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class EnemyScript : MonoBehaviour {
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

	// Use this for initialization
	void Start () {
		myLabels = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetFacing(bool faceUp){
		//do stuff
	}
}
