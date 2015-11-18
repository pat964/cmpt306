using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class playerScript : Photon.MonoBehaviour {
	private static int MAX_REP = 5;
	private static int MIN_REP = -5;
	public int armor = 2;
	public int handSize = 5;
	public int reputation = 0;
	public int fame = 0;
	public int moves = 0;
	// Physical, Ice, Fire, Cold Fire
	public int[] attacks = Enumerable.Repeat(0, 4).ToArray();
	public int[] blocks = Enumerable.Repeat(0, 4).ToArray();
	public HexScript onHex;
	public List<EnemyScript> rampagingEnemies = new List<EnemyScript>();
	private GameObject hand, deck;
	public GameObject attackLabel,blockLabel;
	private GameObject portalHex;
	private Transform player;
	private Canvas handCanvas, mainCanvas;
	private Camera handCamera, mainCamera;
	// Use this for initialization
	void Start () {
		handCanvas = transform.GetComponentsInChildren<Canvas>().First(x => x.gameObject.name == "Hand Canvas");
		mainCanvas = transform.GetComponentsInChildren<Canvas>().First(x => x.gameObject.name == "Main Canvas");
		handCamera = GameObject.Find ("Hand Camera").GetComponent<Camera>();
		mainCamera = GameObject.Find ("Main Camera").GetComponent<Camera>();
		handCanvas.worldCamera = handCamera;
		mainCanvas.worldCamera = mainCamera;
		mainCanvas.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "View Hand Button").onClick.AddListener(() => { Manager.ChangeCameras("Hand"); });
		handCanvas.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "Return To Game Button").onClick.AddListener(() => { Manager.ChangeCameras("Main"); });
		player = transform.GetChild (0);
		//portal hex is the seventh child of green tile zero.
		portalHex = GameObject.Find("Green Tile 0").transform.GetChild(6).gameObject;
		onHex = portalHex.GetComponent<HexScript>();
		player.position = portalHex.transform.position;
		attackLabel = GetComponentInChildren<Canvas>().transform.GetChild (3).gameObject;
		blockLabel = GetComponentInChildren<Canvas>().transform.GetChild (4).gameObject;
		attackLabel.SetActive(false);
		blockLabel.SetActive(false);
		hand = handCanvas.transform.GetComponentsInChildren<Transform>().First(x => x.gameObject.name == "Hand").gameObject;
		deck = transform.GetComponentsInChildren<Transform>().First (x => x.gameObject.name == "Deed Deck").gameObject;
		InitDeckAndHand();
		ArrangeHand();

		if(photonView.isMine)
		{
			transform.GetComponentInChildren<Canvas>().transform.GetChild (0).GetComponent<Text>().enabled = true;
			transform.GetComponentInChildren<Canvas>().transform.GetChild (1).GetComponent<Text>().enabled = true;
			transform.GetComponentInChildren<Canvas>().transform.GetChild (2).GetComponent<Text>().enabled = true;

		}
	}

	public Transform getPlayer() {
		return player;
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void IncreaseFame(int amount){
		fame += amount;
		Text fameTrack = transform.GetComponentInChildren<Canvas>().transform.GetChild (1).GetComponent<Text>();
		fameTrack.text = "Fame: " + fame.ToString();
	}
	
	public void IncreaseReputation(int amount){
		reputation += amount;
		if (reputation < MIN_REP){
			reputation = MIN_REP;
		} else if(reputation > MAX_REP){
			reputation = MAX_REP;
		}
		Text repTrack = transform.GetComponentInChildren<Canvas>().transform.GetChild (0).GetComponent<Text>();
		repTrack.text = "Reputation: " + reputation.ToString();
	}

	public void UpdateLabels() {
		transform.GetComponentInChildren<Canvas>().transform.GetChild (2).GetComponent<Text>().text = "Moves: " + moves.ToString();
		if (attackLabel.activeSelf && blockLabel.activeSelf){
			attackLabel.transform.GetChild(0).GetComponent<Text>().text = "P: " + attacks[0].ToString();
			attackLabel.transform.GetChild(1).GetComponent<Text>().text = "I: " + attacks[1].ToString();
			attackLabel.transform.GetChild(2).GetComponent<Text>().text = "F: " + attacks[2].ToString();
			attackLabel.transform.GetChild(3).GetComponent<Text>().text = "CF: " + attacks[3].ToString();
			blockLabel.transform.GetChild(0).GetComponent<Text>().text = "P: " + blocks[0].ToString();
			blockLabel.transform.GetChild(1).GetComponent<Text>().text = "I: " + blocks[1].ToString();
			blockLabel.transform.GetChild(2).GetComponent<Text>().text = "F: " + blocks[2].ToString();
			blockLabel.transform.GetChild(3).GetComponent<Text>().text = "CF: " + blocks[3].ToString();
		}
	}

	public void MoveToHex(HexScript hex){
		List<HexScript> oldAdjacentRampagers = GetAdjacentRampagers();
		player.position = hex.transform.position;
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
			Manager.SwitchToTurnPhase(Toolbox.TurnPhase.Action);
			DoGarrisonBattle(hex);
		} else if (hex.hexType == Toolbox.HexType.Adventure){
			Manager.SwitchToTurnPhase(Toolbox.TurnPhase.Action);
			if (rampagingEnemies.Count > 0){
				DoBattle(rampagingEnemies);
			} else {
				//prompt adventure
			}
		} else if (hex.hexType == Toolbox.HexType.Interaction){
			Manager.SwitchToTurnPhase(Toolbox.TurnPhase.Action);
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
		Manager.InitiateBattle(enemies);
	}

	public void SetBattleUI (bool active)
	{
		attackLabel.SetActive(active);
		blockLabel.SetActive(active);
	}

	public void AddCardToHand(DeedCardScript card){
		photonView.RPC("Parenting", PhotonTargets.AllBuffered, card.gameObject.GetPhotonView().viewID, hand.gameObject.GetPhotonView().viewID);
	}

	private List<HexScript> GetAdjacentRampagers(){
		Collider2D[] AdjacentHexes = Physics2D.OverlapCircleAll(player.position, player.GetComponent<Renderer>().bounds.size.y + 1);
		List<HexScript> myReturn = new List<HexScript>();
		foreach(Collider2D hexCollider in AdjacentHexes){
			HexScript hex = hexCollider.gameObject.GetComponent<HexScript>();
			if (hex != null && hex.hexType == Toolbox.HexType.Rampage){
				myReturn.Add(hex);
			}
		}
		return myReturn;
	}

	private void InitDeckAndHand() {
		ShuffleDeck();
		for (int i = 0; i < handSize; i++){
			GameObject card = deck.transform.GetChild(0).gameObject;
			card.transform.SetParent(hand.transform, false);
		}
	}

	private void ShuffleDeck() {
		List<GameObject> cards = new List<GameObject>();
		for(int i = 0; i < deck.transform.childCount; i++){
			cards.Add(deck.transform.GetChild(i).gameObject);
		}
		Toolbox.Shuffle(cards);
		deck.transform.DetachChildren();
		foreach(GameObject card in cards){
			card.transform.SetParent(deck.transform);
		}
	}

	public void ArrangeHand(){
		for (int i = 0; i < hand.transform.childCount; i++){
			Transform card = hand.transform.GetChild(i);
			card.localPosition = new Vector2(card.localPosition.x + i * card.GetComponentInChildren<SpriteRenderer>().bounds.size.x * 1.5f, 0);
		}
	}

	[PunRPC] // adds the child to the parent across the whole network
	void Parenting(int child, int parent){
		PhotonView x = PhotonView.Find (child);
		PhotonView y = PhotonView.Find (parent);
		
		x.transform.SetParent(y.transform);
	}
}
