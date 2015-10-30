using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
	public List<EnemyScript> rampagingEnemies;

	private GameObject portalHex;
	// Use this for initialization
	void Start () {
		//portal hex is the seventh child of green tile one.
		portalHex = GameObject.Find("Green Tile 0").transform.GetChild(6).gameObject;
		onHex = portalHex.GetComponent<HexScript>();
		transform.position = portalHex.transform.position;
		rampagingEnemies = new List<EnemyScript>();
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
		List<HexScript> oldAdjacentRampagers = GetAdjacentRampagers();
		transform.position = hex.transform.position;
		List<HexScript> newAdjacentRampagers = GetAdjacentRampagers();
		if(oldAdjacentRampagers.Count > 0 && newAdjacentRampagers.Count > 0){
			foreach (HexScript rampager in oldAdjacentRampagers.Intersect(newAdjacentRampagers)){
				if (rampager.enemiesOnHex.Count > 0){
					rampagingEnemies.Add(rampager.enemiesOnHex.ElementAt(0));
				}
			}
		}
		if (hex.hexType == Toolbox.HexType.Garrison){
			//do garrison battle
			manager.SwitchToTurnPhase(Toolbox.TurnPhase.Action);
			DoGarrisonBattle(hex);
		} else if (hex.hexType == Toolbox.HexType.Adventure){
			manager.SwitchToTurnPhase(Toolbox.TurnPhase.Action);
			if (rampagingEnemies.Count > 0){
				DoBattle(rampagingEnemies);
			} else {
				//prompt adventure
			}
		} else if (hex.hexType == Toolbox.HexType.Interaction){
			manager.SwitchToTurnPhase(Toolbox.TurnPhase.Action);
			if (rampagingEnemies.Count > 0){
				DoBattle(rampagingEnemies);
			} else {
				//prompt interaction
			}
		} else if (hex.hexType == Toolbox.HexType.Empty){
			if (rampagingEnemies.Count > 0){
				DoBattle(rampagingEnemies);
			}
		}
		rampagingEnemies.Clear();
	}

	public void DoGarrisonBattle(HexScript hex) {
		List<EnemyScript> enemies = new List<EnemyScript>();
		enemies.AddRange(hex.enemiesOnHex);
		foreach(EnemyScript enemy in rampagingEnemies){
			enemies.Add(enemy);
		}
		DoBattle(enemies);
	}

	public void DoBattle(List<EnemyScript> enemies){
		print ("Do battle!");
	}

	private List<HexScript> GetAdjacentRampagers(){
		Collider2D[] AdjacentHexes = Physics2D.OverlapCircleAll(transform.position, GetComponent<Renderer>().bounds.size.y);
		List<HexScript> myReturn = new List<HexScript>();
		foreach(Collider2D hexCollider in AdjacentHexes){
			HexScript hex = hexCollider.gameObject.GetComponent<HexScript>();
			if (hex != null && hex.hexType == Toolbox.HexType.Rampage){
				myReturn.Add(hex);
			}
		}
		return myReturn;
	}
}
