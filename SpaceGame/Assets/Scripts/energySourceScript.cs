using UnityEngine;
using System.Collections;

public class energySourceScript : MonoBehaviour {

	public playerScript player;
	bool playAudio = false;

	// Use this for initialization
	void Start () {
		player = transform.GetComponentInParent<playerScript>();
		RollAll();
		playAudio = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void RollAll() {
		if (playAudio) {
			this.GetComponent<AudioSource> ().Play ();
		}
		foreach(energyDiceScript dice in transform.GetComponentsInChildren<energyDiceScript>()){
			dice.Roll(false);
		}
	}
}
