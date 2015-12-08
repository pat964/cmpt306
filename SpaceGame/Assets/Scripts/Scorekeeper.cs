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
		if (!scores.Contains(playerID)) {
			Debug.Log("Player ID missing.");
		}
		return (int) scores[playerID];
	}

	public bool containsScore(int playerID) {
		return scores.Contains(playerID);
	}

	public void clearScore() {
		scores.Clear ();
	}

	// Update is called once per frame
	void Update () {

	}
}
