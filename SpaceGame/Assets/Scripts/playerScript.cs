using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class playerScript : Photon.MonoBehaviour {
	private static int MAX_REP = 5;
	private static int MIN_REP = -5;
	public static float TURN_TIMER = 30.0f;
	public float timer = TURN_TIMER;
	public Toolbox.TurnPhase turnPhase;
	public bool isBattling = false;
	public Toolbox.BattlePhase battlePhase = Toolbox.BattlePhase.None;
	public bool isRetreating = false;
	public int armor = 2;
	public int maxHandSize = 5;
	public int cardsInHand = 5;
	public int reputation = 0;
	public int fame = 0;
	public int moves = 0;
	public int influence = 0;
	public int handIndex = 0;
	// Physical, Ice, Fire, Cold Fire
	public int[] attacks = Enumerable.Repeat(0, 4).ToArray();
	public int[] blocks = Enumerable.Repeat(0, 4).ToArray();
	public HexScript onHex;
	public List<EnemyScript> rampagingEnemies = new List<EnemyScript>();
	private GameObject hand, deck, discardPile;
	public GameObject attackLabel, blockLabel, timerLabel, retreatLabel, energyLabel, handSizeLabel, fameLabel, armorLabel;
	private GameObject portalHex;
	private Transform player;
	public Canvas handCanvas, mainCanvas, overlayCanvas;
	private Camera handCamera, mainCamera;
	public Button endMovesButton, endActionButton, interactButton;
	public bool canDrawCards = false;
	public bool usedGlade = false;
	public bool usedSource = false;
	public int redEnergy, greenEnergy, blueEnergy, whiteEnergy, darkEnergy = 0;
	public int[] handSizeIncreaseLevels;
	// Use this for initialization
	void Start () {
		overlayCanvas = transform.GetComponentsInChildren<Canvas>().First(x => x.gameObject.name == "Common Area Overlay");
		mainCanvas = transform.GetComponentsInChildren<Canvas>().First(x => x.gameObject.name == "Main Canvas");
		handCanvas = transform.GetComponentsInChildren<Canvas>().First(x => x.gameObject.name == "Hand Canvas");

		// disables our overlay for other people, so they do not see our score, move, and other values
		if(photonView.isMine)
		{
			overlayCanvas.enabled = true;
			mainCanvas.enabled = true;
			handCanvas.enabled = true;
			photonView.RPC("DisableCanvas", PhotonTargets.OthersBuffered, overlayCanvas.gameObject.GetPhotonView().viewID);
			photonView.RPC("DisableCanvas", PhotonTargets.OthersBuffered, mainCanvas.gameObject.GetPhotonView().viewID);
			photonView.RPC("DisableCanvas", PhotonTargets.OthersBuffered, handCanvas.gameObject.GetPhotonView().viewID);

		}

		handSizeIncreaseLevels = new int[3]{10, 25, 50};
		turnPhase = Toolbox.TurnPhase.Move;
		handCamera = GameObject.Find ("Hand Camera").GetComponent<Camera>();
		mainCamera = GameObject.Find ("Main Camera").GetComponent<Camera>();
		handCanvas.worldCamera = handCamera;
		mainCanvas.worldCamera = mainCamera;
		mainCanvas.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "View Hand Button").onClick.AddListener(() => { ArrangeHand(0); });
		mainCanvas.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "View Hand Button").onClick.AddListener(() => { Manager.ChangeCameras("Hand"); });
		handCanvas.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "Return To Game Button").onClick.AddListener(() => { Manager.ChangeCameras("Main"); });
		handCanvas.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "View Next").onClick.AddListener(() => { ArrangeHand(1); });
		handCanvas.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "View Prev").onClick.AddListener(() => { ArrangeHand(-1); });
		player = transform.GetChild (0);
		//portal hex is the seventh child of green tile zero.
		portalHex = GameObject.Find("Green Tile 0").transform.GetChild(6).gameObject;
		onHex = portalHex.GetComponent<HexScript>();
		player.position = portalHex.transform.position;
		attackLabel = GetComponentInChildren<Canvas>().transform.GetComponentsInChildren<Text>().First(x => x.gameObject.name == "Attack Label").gameObject;
		blockLabel = GetComponentInChildren<Canvas>().transform.GetComponentsInChildren<Text>().First(x => x.gameObject.name == "Block Label").gameObject;
		timerLabel = GetComponentInChildren<Canvas>().transform.GetComponentsInChildren<Text>().First(x => x.gameObject.name == "Timer").gameObject;
		retreatLabel = GetComponentInChildren<Canvas>().transform.GetComponentsInChildren<Text>().First(x => x.gameObject.name == "Retreat Label").gameObject;
		attackLabel.SetActive(false);
		blockLabel.SetActive(false);
		hand = handCanvas.transform.GetComponentsInChildren<Transform>().First(x => x.gameObject.name == "Hand").gameObject;
		deck = transform.GetComponentsInChildren<Transform>().First (x => x.gameObject.name == "Deed Deck").gameObject;
		energyLabel = transform.GetComponentsInChildren<Transform>().First (x => x.gameObject.name == "Energy Label").gameObject;
		handSizeLabel = transform.GetComponentsInChildren<Transform>().First (x => x.gameObject.name == "Hand Size Label").gameObject;
		armorLabel = transform.GetComponentsInChildren<Transform>().First (x => x.gameObject.name == "Armor Label").gameObject;
		fameLabel = transform.GetComponentsInChildren<Transform>().First (x => x.gameObject.name == "Fame Track").gameObject;
		discardPile = transform.GetComponentsInChildren<Transform>().First (x => x.gameObject.name == "Discard Pile").gameObject;
		InitDeckAndHand();
		ArrangeHand(0);
		endMovesButton = mainCanvas.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "End Move Button");
		endMovesButton.onClick.AddListener(() => Manager.SwitchToTurnPhase(Toolbox.TurnPhase.Action));
		endActionButton = mainCanvas.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "End Action Button");
		endActionButton.onClick.AddListener(() => Manager.SwitchToTurnPhase(Toolbox.TurnPhase.End));
		interactButton = mainCanvas.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "Interaction Button");
		interactButton.onClick.AddListener(() => PrepInteractionMenu());
		ShowInteractionButton(false);
		ShowActionButton(false);
	}

	public Transform getPlayer() {
		return player;
	}
	
	// Update is called once per frame
	void Update () {
		if (timer >= 0){
			canDrawCards = false;
			timer -= Time.deltaTime;
			timerLabel.transform.GetComponentInChildren<Text>().text = "Time Until Next Draw: " + timer.ToString("n0") + "s";
		} else {
			canDrawCards = true;
			timerLabel.transform.GetComponentInChildren<Text>().text = "You Will Draw After Your Turn!";
		}
		if(isRetreating){
			retreatLabel.gameObject.SetActive(true);
		} else {
			retreatLabel.gameObject.SetActive(false);
		}
	}

	public void IncreaseFame(int amount){
		int newFame = fame + amount;
		foreach(int level in handSizeIncreaseLevels){
			if(fame < level && newFame >= level){
				maxHandSize++;
				armor++;
			}
		}
		fame = newFame;
		Text fameTrack = transform.GetComponentInChildren<Canvas>().transform.GetChild (1).GetComponent<Text>();
		UpdateLabels();
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
		transform.GetComponentInChildren<Canvas>().transform.GetComponentsInChildren<Text>().First(x => x.gameObject.name == "Influence Label").text = "Influence: " + influence.ToString();

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
		energyLabel.transform.GetChild(0).GetComponent<Text>().text = "Green: " + greenEnergy.ToString();
		energyLabel.transform.GetChild(1).GetComponent<Text>().text = "Blue: " + blueEnergy.ToString();
		energyLabel.transform.GetChild(2).GetComponent<Text>().text = "Red: " + redEnergy.ToString();
		energyLabel.transform.GetChild(3).GetComponent<Text>().text = "White: " + whiteEnergy.ToString();
		energyLabel.transform.GetChild(4).GetComponent<Text>().text = "Dark: " + darkEnergy.ToString();

		handSizeLabel.GetComponent<Text>().text = "Hand Size: " + maxHandSize.ToString();
		fameLabel.GetComponent<Text>().text = "Fame: " + fame.ToString();
		armorLabel.GetComponent<Text>().text = "Armor: " + armor.ToString();
	}

	public void MoveToHex(HexScript hex){
		List<HexScript> oldAdjacentRampagers = GetAdjacentRampagers();
		player.position = hex.transform.position;
		onHex = hex;
		List<HexScript> newAdjacentRampagers = GetAdjacentRampagers();
		if(oldAdjacentRampagers.Count > 0 && newAdjacentRampagers.Count > 0){
			foreach (HexScript rampager in oldAdjacentRampagers.Intersect(newAdjacentRampagers)){
				if (rampager.enemiesOnHex.Count > 0){
					rampagingEnemies.Add(rampager.enemiesOnHex.ElementAt(0));
				}
			}
		}
		if(!isRetreating){
			if (hex.hexType == Toolbox.HexType.Garrison){
				//do garrison battle
				DoGarrisonBattle(hex);
			} else {
				if (rampagingEnemies.Count > 0){
					DoBattle(rampagingEnemies);
				}
			}
		} else {
			isRetreating = false;
			Manager.SwitchToTurnPhase(Toolbox.TurnPhase.End);
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
		photonView.RPC("Parenting", PhotonTargets.AllBuffered, card.gameObject.GetPhotonView().viewID, hand.gameObject.GetPhotonView().viewID, false);
		cardsInHand++;
		ArrangeHand (0);
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
		for (int i = 0; i < maxHandSize; i++){
			GameObject card = deck.transform.GetChild(0).gameObject;
			photonView.RPC("Parenting", PhotonTargets.AllBuffered, card.gameObject.GetPhotonView().viewID, hand.gameObject.GetPhotonView().viewID, false);
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
			photonView.RPC("Parenting", PhotonTargets.AllBuffered, card.GetPhotonView().viewID, deck.GetPhotonView().viewID);
		}
	}

	public void ArrangeHand(int index){
		if(index > 0){
			if ((handIndex + 1) * 3 < cardsInHand){
				handIndex++;
			}
		} else if (index < 0) {
			if (handIndex > 0){
				handIndex--;
			}
		} else {
			handIndex = 0;
		}
		foreach(DeedCardScript card in hand.transform.GetComponentsInChildren<DeedCardScript>()){
			card.gameObject.SetActive(false);
		}
		for (int i = handIndex * 3, j = 0; i < hand.transform.childCount && j < 3; i++, j++){
			Transform card = hand.transform.GetChild(i);
			card.gameObject.SetActive(true);
			card.localPosition = new Vector2(j * card.GetComponentInChildren<Image>().sprite.bounds.size.x * card.GetComponentInChildren<RectTransform>().localScale.x * 2, 0);
		}
	}

	public void DiscardCard(DeedCardScript card){
		photonView.RPC("Parenting", PhotonTargets.AllBuffered, card.gameObject.GetPhotonView().viewID, discardPile.GetPhotonView().viewID, false);
		cardsInHand--;
		ArrangeHand(0);
	}

	public void AddAttack(int val, Toolbox.AttackType type){
		switch (type) {
		case Toolbox.AttackType.Physical:
			attacks[0] += val;
			break;
		case Toolbox.AttackType.Ice:
			attacks[1] += val;
			break;
		case Toolbox.AttackType.Fire:
			attacks[2] += val;
			break;
		case Toolbox.AttackType.ColdFire:
			attacks[3] += val;
			break;
		default:
			break;
		}
		UpdateLabels();
	}
	
	public void AddBlock(int val, Toolbox.AttackType type){
		switch (type) {
		case Toolbox.AttackType.Physical:
			blocks[0] += val;
			break;
		case Toolbox.AttackType.Ice:
			blocks[1] += val;
			break;
		case Toolbox.AttackType.Fire:
			blocks[2] += val;
			break;
		case Toolbox.AttackType.ColdFire:
			blocks[3] += val;
			break;
		default:
			break;
		}
		UpdateLabels();
	}

	public void DoHeal(int val){
		if (val > 0){
			if (hand.GetComponentsInChildren<DeedCardScript>(true).Any (x => x.cardType == Toolbox.CardType.Wound)){
				GameObject wound = hand.GetComponentsInChildren<DeedCardScript>(true).First(x => x.cardType == Toolbox.CardType.Wound).gameObject;
				wound.transform.SetParent(GameObject.Find("Wound Deck").transform, false);
				DoHeal(val - 1);
			} else {
				ArrangeHand(0);
			}
		} else {
			ArrangeHand(0);
		}
	}

	public void DrawCards(){
		while (cardsInHand < maxHandSize){
			if(deck.transform.childCount > 0){
				//draw card
				AddCardToHand(deck.transform.GetChild(0).GetComponent<DeedCardScript>());
			} else if (discardPile.transform.childCount > 0) {
				//shuffle up discard pile to form new deck
				List<GameObject> cards = new List<GameObject>();
				while (discardPile.transform.childCount > 0){
					discardPile.transform.GetChild(0).SetParent(deck.transform, false);
				}
				ShuffleDeck();
			} else {
				//no more cards, probably will never happen... just do nothing
				break;
			}
		}
	}

	public void ShowMoveButton(bool show) {
		endMovesButton.gameObject.SetActive(show);
	}
	
	public void ShowActionButton(bool show) {
		endActionButton.gameObject.SetActive(show);
	}
	
	public void ShowInteractionButton(bool show) {
		interactButton.gameObject.SetActive(show);
	}

	public void CheckForInteraction() {
		if(onHex.hexType == Toolbox.HexType.Interaction){
			ShowInteractionButton(true);
		}
	}

	public void PrepInteractionMenu() {
		List<Interaction> interactions = new List<Interaction>();
		switch (onHex.hexFeature){
		case (Toolbox.HexFeature.Base):
			interactions.Add(new Interaction(7, Toolbox.InteractionType.Adv_Action));
			break;
		case (Toolbox.HexFeature.DarkMatterResearch):
			interactions.Add(new Interaction(7, Toolbox.InteractionType.DMD));
			break;
		case (Toolbox.HexFeature.Monastary):
			interactions.Add(new Interaction(10, Toolbox.InteractionType.Artifact));
			interactions.Add(new Interaction(2, Toolbox.InteractionType.Heal));
			break;
		case (Toolbox.HexFeature.Town):
			interactions.Add(new Interaction(10, Toolbox.InteractionType.Adv_Action));
			interactions.Add(new Interaction(3, Toolbox.InteractionType.Heal));
			break;
		default:
			break;
		}
		CreateInteractionMenu(interactions);
	}


	public void CreateInteractionMenu(List<Interaction> interactions){
		GameObject interactionMenu;
		if (interactions.Count == 0){
			return;
		} else if (interactions.Count == 1){
			interactionMenu = (GameObject) Instantiate(Resources.Load("Prefabs/TwoButtonModal"));
			interactionMenu.transform.GetComponentInChildren<Text>().text = "Choose Interaction";
			interactionMenu.transform.SetParent(mainCanvas.transform, false);
			Button firstButton = interactionMenu.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "First Button");
			firstButton.GetComponent<RectTransform>().sizeDelta = new Vector2(190f, 30f);
			firstButton.GetComponentInChildren<Text>().text = interactions[0].type.ToString() + " for " + interactions[0].val.ToString() + " influence";
			ApplyInteractionToButton(firstButton, interactions[0]);
			Button secondButton = interactionMenu.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "Second Button");
			secondButton.GetComponentInChildren<Text>().text = "Take no action";
			secondButton.onClick.AddListener(() => {Destroy (interactionMenu);});
		} else if (interactions.Count == 2){
			interactionMenu = (GameObject) Instantiate(Resources.Load("Prefabs/ThreeButtonModal"));
			interactionMenu.transform.GetComponentInChildren<Text>().text = "Choose Interaction";
			interactionMenu.transform.SetParent(mainCanvas.transform, false);
			Button firstButton = interactionMenu.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "First Button");
			firstButton.GetComponent<RectTransform>().sizeDelta = new Vector2(190f, 30f);
			firstButton.GetComponentInChildren<Text>().text = interactions[0].type.ToString() + " for " + interactions[0].val.ToString() + " influence";
			ApplyInteractionToButton(firstButton, interactions[0]);
			Button secondButton = interactionMenu.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "Second Button");
			secondButton.GetComponent<RectTransform>().sizeDelta = new Vector2(190f, 30f);
			secondButton.GetComponentInChildren<Text>().text = interactions[1].type.ToString() + " for " + interactions[1].val.ToString() + " influence";
			ApplyInteractionToButton(secondButton, interactions[1]);
			Button thirdButton = interactionMenu.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "Third Button");
			thirdButton.GetComponentInChildren<Text>().text = "Take no action";
			thirdButton.onClick.AddListener(() => {Destroy (interactionMenu);});
		}
	}

	public void ApplyInteractionToButton(Button button, Interaction interaction){
		Transform newCard;
			switch (interaction.type) {
			case Toolbox.InteractionType.Adv_Action:
				button.onClick.AddListener(() => {
				if (interaction.val <= influence) {
					GameObject advActionDeck = GameObject.Find ("Advanced Actions Deck");
					if(advActionDeck.transform.childCount > 0){
						newCard = advActionDeck.transform.GetChild(0);
						newCard.SetParent(hand.transform, false);
						newCard.SetAsFirstSibling();
						influence -= interaction.val;
						UpdateLabels();
					}
				}
				});
				break;
			case Toolbox.InteractionType.DMD:
			button.onClick.AddListener(() => {
				if (interaction.val <= influence) {
					GameObject dmdDeck = GameObject.Find ("Dark Matter Device Deck");
					if (dmdDeck.transform.childCount > 0){
						newCard = dmdDeck.transform.GetChild(0);
						newCard.SetParent(hand.transform, false);
						newCard.SetAsFirstSibling();
						influence -= interaction.val;
						UpdateLabels();
					}
				}
				});
				break;
			case Toolbox.InteractionType.Artifact:
			button.onClick.AddListener(() => {
				if (interaction.val <= influence) {
					GameObject artifactDeck = GameObject.Find ("Artifact Deck");
					if (artifactDeck.transform.childCount > 0){
						newCard = artifactDeck.transform.GetChild(0);
						newCard.SetParent(hand.transform, false);
						newCard.SetAsFirstSibling();
						influence -= interaction.val;
						UpdateLabels();
					}
				}
				});
				break;
			case Toolbox.InteractionType.Heal:
				button.onClick.AddListener(() => {
					if (interaction.val <= influence) {
						DoHeal(1);
						influence -= interaction.val;
						UpdateLabels();
					}
				});
				break;
			default:
				break;
			}
	}

	public void UseEnergyDice(energyDiceScript dice){
		if(!usedSource){
			switch(dice.colour) {
			case Toolbox.EnergyColour.Blue:
				blueEnergy++;
				break;
			case Toolbox.EnergyColour.White:
				whiteEnergy++;
				break;
			case Toolbox.EnergyColour.Green:
				greenEnergy++;
				break;
			case Toolbox.EnergyColour.Red:
				redEnergy++;
				break;
			case Toolbox.EnergyColour.Dark:
				darkEnergy++;
				break;
			default:
				break;
			}
			usedSource = true;
			dice.Roll();
			UpdateLabels();
		}
	}

	public void PayCosts(List<Cost> costs){
		foreach (Cost cost in costs) {
			switch (cost.colour){
			case Toolbox.EnergyColour.Blue:
				blueEnergy -= cost.val;
				break;
			case Toolbox.EnergyColour.Green:
				greenEnergy -= cost.val;
				break;
			case Toolbox.EnergyColour.Red:
				redEnergy -= cost.val;
				break;
			case Toolbox.EnergyColour.White:
				whiteEnergy -= cost.val;
				break;
			case Toolbox.EnergyColour.Dark:
				darkEnergy -= cost.val;
				break;
			default:
				break;
			}
		}
	}

	public void CheckEndingEffect(){
		if (onHex.hexFeature == Toolbox.HexFeature.Glade && !usedGlade){
			DoHeal(1);
			usedGlade = true;
		} else if (onHex.hexFeature == Toolbox.HexFeature.CityRed ||
		           onHex.hexFeature == Toolbox.HexFeature.CityGreen ||
		           onHex.hexFeature == Toolbox.HexFeature.CityWhite ||
		           onHex.hexFeature == Toolbox.HexFeature.CityBlue) {
			IncreaseFame(5);
			Manager.GameOver();
		}
	}
	
	public void CheckStartingEffect(){
		if (onHex.hexFeature == Toolbox.HexFeature.Glade){
			darkEnergy++;
			UpdateLabels();
		} else if (onHex.hexFeature == Toolbox.HexFeature.MineBlue){
			blueEnergy++;
			UpdateLabels();
		} else if (onHex.hexFeature == Toolbox.HexFeature.MineGreen){
			greenEnergy++;
			UpdateLabels();
		} else if (onHex.hexFeature == Toolbox.HexFeature.MineWhite){
			whiteEnergy++;
			UpdateLabels();
		} else if (onHex.hexFeature == Toolbox.HexFeature.MineRed){
			redEnergy++;
			UpdateLabels();
		}
	}

	[PunRPC] // adds the child to the parent across the whole network
	void Parenting(int child, int parent){
		PhotonView x = PhotonView.Find (child);
		PhotonView y = PhotonView.Find (parent);
		x.transform.SetParent(y.transform);
	}

	[PunRPC] // adds the child to the parent across the whole network
	void Parenting(int child, int parent, bool worldPositionStays){
		PhotonView x = PhotonView.Find (child);
		PhotonView y = PhotonView.Find (parent);
		
		x.transform.SetParent(y.transform, worldPositionStays);
	}
	
	[PunRPC]
	void DisableCanvas(int canvas) {
		PhotonView c = PhotonView.Find (canvas);
		c.enabled = false;
	}
	
	public class Interaction {
		public int val;
		public Toolbox.InteractionType type;

		public Interaction (int myVal, Toolbox.InteractionType myType){
			val = myVal;
			type = myType;
		}
	}
}
