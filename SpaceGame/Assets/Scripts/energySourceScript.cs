using UnityEngine;
using System.Collections;

public class energySourceScript : MonoBehaviour {

	public playerScript player;
	// Use this for initialization
	void Start () {
		player = transform.GetComponentInParent<playerScript>();
		RollAll();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void RollAll() {
		foreach(energyDiceScript dice in transform.GetComponentsInChildren<energyDiceScript>()){
			dice.Roll();
		}
	}
}
