using UnityEngine;
using System.Collections;

public class Scorekeeper : MonoBehaviour {

	Hashtable scores;

	// Use this for initialization
	void Start () {
		scores = new Hashtable();
		DontDestroyOnLoad(gameObject);
	}

	public void setScore(int playerID, int score) {
		scores[playerID] = score;
	}

	public int getScore(int playerID) {
		return (int) scores[playerID];
	}

	public void clearScore() {
		scores.Clear ();
	}

	// Update is called once per frame
	void Update () {

	}
}
