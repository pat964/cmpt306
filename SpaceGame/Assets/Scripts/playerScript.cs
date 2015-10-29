using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class playerScript : MonoBehaviour {
	private static int MAX_REP = 5;
	private static int MIN_REP = -5;
	public int armor = 2;
	public int handSize = 5;
	public int reputation = 0;
	public int fame = 0;
	public int moves = 0;
	public HexScript onHex;

	private GameObject portalHex;
	// Use this for initialization
	void Start () {
		//portal hex is the seventh child of green tile one.
		portalHex = GameObject.Find("Green Tile 0").transform.GetChild(6).gameObject;
		onHex = portalHex.GetComponent<HexScript>();
		transform.position = portalHex.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.M)){
			moves++;
			UpdateMovesMessage();
		}
	}

	public void IncreaseFame(int amount){
		fame += amount;
		Text fameTrack = GameObject.Find ("Fame Track").GetComponent<Text>();
		fameTrack.text = "Fame: " + fame.ToString();
	}
	
	public void IncreaseReputation(int amount){
		reputation += amount;
		if (reputation < MIN_REP){
			reputation = MIN_REP;
		} else if(reputation > MAX_REP){
			reputation = MAX_REP;
		}
		Text repTrack = GameObject.Find ("Reputation Track").GetComponent<Text>();
		repTrack.text = "Reputation: " + reputation.ToString();
	}

	public void UpdateMovesMessage(){
		GameObject.Find ("Move UI").GetComponent<Text>().text = "Moves: " + moves.ToString();
	}

	public void MoveToHex(HexScript hex){
		transform.position = hex.transform.position;
		if (hex.hexType == Toolbox.HexType.Garrison){
			//do garrison battle
			manager.SwitchToTurnPhase(Toolbox.TurnPhase.Action);
			DoGarrisonBattle(hex.hexFeature);
		} else if (hex.hexType == Toolbox.HexType.Adventure){
			//prompt battle
		} else if (hex.hexType == Toolbox.HexType.Interaction){
			//prompt interaction
		} 
	}

	public void DoGarrisonBattle(Toolbox.HexFeature feature) {
		if (feature == Toolbox.HexFeature.Base){

		}
	}
}
