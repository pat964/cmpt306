﻿using UnityEngine;
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
	private GameObject hand, deck, discardPile, destroyedCards;
	public GameObject attackLabel, blockLabel, timerLabel, retreatLabel, energyLabel, handSizeLabel, fameLabel, armorLabel, usesLabel, turnPhaseLabel, deckSizeLabel;
	private GameObject portalHex;
	private Transform player;
	public Canvas handCanvas, mainCanvas, overlayCanvas;
	private Camera handCamera, mainCamera;
	public Button endMovesButton, endActionButton, interactButton, restButton, provokeButton;
	public bool canDrawCards = false;
	public bool usedGlade = false;
	public int sourceUsesLeft = 1;
	public bool isResting = false;
	public int redEnergy, greenEnergy, blueEnergy, whiteEnergy, darkEnergy = 0;
	public int[] handSizeIncreaseLevels;
	// audio
	public AudioSource restAudio;
	public MenuAudio menuAudio;

	// Use this for initialization
	void Start () {
		menuAudio = GameObject.Find ("GUI Background").GetComponent<MenuAudio> ();

		overlayCanvas = transform.GetComponentsInChildren<Canvas>().First(x => x.gameObject.name == "Common Area Overlay");
		mainCanvas = transform.GetComponentsInChildren<Canvas>().First(x => x.gameObject.name == "Main Canvas");
		handCanvas = transform.GetComponentsInChildren<Canvas>().First(x => x.gameObject.name == "Hand Canvas");

		// disables our overlay for other people, so they do not see our score, move, and other values
		if(photonView.isMine)
		{
			overlayCanvas.enabled = true;
			mainCanvas.enabled = true;
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
		destroyedCards = GameObject.Find ("Destroyed Cards");
		onHex = portalHex.GetComponent<HexScript>();
		player.position = portalHex.transform.position;
		attackLabel = GetComponentInChildren<Canvas>().transform.GetComponentsInChildren<Text>().First(x => x.gameObject.name == "Attack Label").gameObject;
		blockLabel = GetComponentInChildren<Canvas>().transform.GetComponentsInChildren<Text>().First(x => x.gameObject.name == "Block Label").gameObject;
		timerLabel = GetComponentInChildren<Canvas>().transform.GetComponentsInChildren<Text>().First(x => x.gameObject.name == "Timer").gameObject;
		retreatLabel = GetComponentInChildren<Canvas>().transform.GetComponentsInChildren<Text>().First(x => x.gameObject.name == "Retreat Label").gameObject;
		usesLabel = GetComponentInChildren<Canvas>().transform.GetComponentsInChildren<Text>().First(x => x.gameObject.name == "Uses Remaining").gameObject;
		turnPhaseLabel = mainCanvas.transform.GetComponentsInChildren<Text>().First(x => x.gameObject.name == "Phase Label").gameObject;
		deckSizeLabel = GetComponentInChildren<Canvas>().transform.GetComponentsInChildren<Text>().First(x => x.gameObject.name == "Deck Size Label").gameObject;
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
		restButton = mainCanvas.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "Rest Button");
		restButton.onClick.AddListener(() => DoRest());
		provokeButton = mainCanvas.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "Provoke Button");
		provokeButton.onClick.AddListener(() => ProvokeRampagers());
		ShowProvokeButton(false);
		ShowRestButton(false);
		ShowInteractionButton(false);
		ShowActionButton(false);
		UpdateLabels();
	}

	public Transform getPlayer() {
		return player;
	}
	
	// Update is called once per frame
	void Update () {
		if (photonView.isMine) {
			if (timer >= 0) {
				canDrawCards = false;
				timer -= Time.deltaTime;
				timerLabel.transform.GetComponentInChildren<Text> ().text = "Time Until Next Draw: " + timer.ToString ("n0") + "s";
			} else {
				canDrawCards = true;
				timerLabel.transform.GetComponentInChildren<Text> ().text = "You Will Draw After Your Turn!";
				if (isResting){
					isResting = false;
					menuAudio.PlayAudio();
					restAudio.Stop();
					Manager.SwitchToTurnPhase(Toolbox.TurnPhase.End);
				}
			}
			if (isRetreating) {
				retreatLabel.gameObject.SetActive (true);
			} else {
				retreatLabel.gameObject.SetActive (false);
			}
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
		if (photonView.isMine) {
			PhotonNetwork.player.SetScore (newFame);
			Debug.Log ("Score: " + PhotonNetwork.player.GetScore ());
			Text fameTrack = transform.GetComponentInChildren<Canvas> ().transform.GetChild (1).GetComponent<Text> ();
			UpdateLabels ();
		}
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
			attackLabel.transform.GetChild(0).GetComponent<Text>().text = "Physical: " + attacks[0].ToString();
			attackLabel.transform.GetChild(1).GetComponent<Text>().text = "Ice: " + attacks[1].ToString();
			attackLabel.transform.GetChild(2).GetComponent<Text>().text = "Fire: " + attacks[2].ToString();
			attackLabel.transform.GetChild(3).GetComponent<Text>().text = "Cold Fire: " + attacks[3].ToString();
			blockLabel.transform.GetChild(0).GetComponent<Text>().text = "Physical: " + blocks[0].ToString();
			blockLabel.transform.GetChild(1).GetComponent<Text>().text = "Ice: " + blocks[1].ToString();
			blockLabel.transform.GetChild(2).GetComponent<Text>().text = "Fire: " + blocks[2].ToString();
			blockLabel.transform.GetChild(3).GetComponent<Text>().text = "Cold Fire: " + blocks[3].ToString();
		}
		energyLabel.transform.GetChild(0).GetComponent<Text>().text = "Green: " + greenEnergy.ToString();
		energyLabel.transform.GetChild(1).GetComponent<Text>().text = "Blue: " + blueEnergy.ToString();
		energyLabel.transform.GetChild(2).GetComponent<Text>().text = "Red: " + redEnergy.ToString();
		energyLabel.transform.GetChild(3).GetComponent<Text>().text = "White: " + whiteEnergy.ToString();
		energyLabel.transform.GetChild(4).GetComponent<Text>().text = "Dark: " + darkEnergy.ToString();

		handSizeLabel.GetComponent<Text>().text = "Hand Size: " + maxHandSize.ToString();
		fameLabel.GetComponent<Text>().text = "Fame: " + fame.ToString();
		armorLabel.GetComponent<Text>().text = "Armor: " + armor.ToString();
		usesLabel.GetComponent<Text>().text = "Uses Remaining: " + sourceUsesLeft.ToString();
		deckSizeLabel.GetComponent<Text>().text = "Deck Size: " + deck.transform.childCount;
		turnPhaseLabel.GetComponent<Text>().text = "Phase: " + turnPhase.ToString();
	}

	public void MoveToHex(HexScript hex){
		List<HexScript> oldAdjacentRampagers = GetAdjacentRampagers();
		player.position = hex.transform.position;
		onHex = hex;
		List<HexScript> newAdjacentRampagers = GetAdjacentRampagers();
		if(oldAdjacentRampagers.Count > 0 && newAdjacentRampagers.Count > 0){
			foreach (HexScript rampager in oldAdjacentRampagers.Intersect(newAdjacentRampagers)){
				if (rampager.enemiesOnHex.Count > 0){ 
					if (!rampager.enemiesOnHex.ElementAt(0).isBattling) {
						photonView.RPC("EnemyIsBattling", PhotonTargets.All, rampager.enemiesOnHex.ElementAt(0).gameObject.GetPhotonView().viewID, true);
						rampagingEnemies.Add(rampager.enemiesOnHex.ElementAt(0));
					}
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
			if (!enemy.isBattling) {
				photonView.RPC("IsBattling", PhotonTargets.All, enemy.gameObject.GetPhotonView().viewID, true);
				enemies.Add(enemy);
			}
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
		if (photonView.isMine) {
			card.transform.SetParent(hand.transform);
//			photonView.RPC ("Parenting", PhotonTargets.AllBuffered, card.gameObject.GetPhotonView ().viewID, hand.gameObject.GetPhotonView ().viewID, false);
			card.player = this;
			cardsInHand++;
			ArrangeHand (0);
		}
	}

	public void AddCardToDiscard(DeedCardScript card){
		if (photonView.isMine) {
			card.transform.SetParent(discardPile.transform);
			//			photonView.RPC ("Parenting", PhotonTargets.AllBuffered, card.gameObject.GetPhotonView ().viewID, hand.gameObject.GetPhotonView ().viewID, false);
			card.player = this;
			ArrangeHand (0);
		}
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
		if (photonView.isMine) {
			ShuffleDeck ();
			for (int i = 0; i < maxHandSize; i++) {
				GameObject card = deck.transform.GetChild (0).gameObject;
				photonView.RPC ("Parenting", PhotonTargets.AllBuffered, card.gameObject.GetPhotonView ().viewID, hand.gameObject.GetPhotonView ().viewID, false);
			}
		}
	}

	private void ShuffleDeck() {
		if (photonView.isMine) {
			List<GameObject> cards = new List<GameObject> ();
			for (int i = 0; i < deck.transform.childCount; i++) {
				cards.Add (deck.transform.GetChild (i).gameObject);
			}
			Toolbox.Shuffle (cards);
			deck.transform.DetachChildren ();
			foreach (GameObject card in cards) {
				photonView.RPC ("Parenting", PhotonTargets.AllBuffered, card.GetPhotonView ().viewID, deck.GetPhotonView ().viewID);
			}
		}
	}

	public void ArrangeHand(int index){
		if (photonView.isMine) {
			if (index > 0) {
				if ((handIndex + 1) * 3 < cardsInHand) {
					handIndex++;
				}
			} else if (index < 0) {
				if (handIndex > 0) {
					handIndex--;
				}
			} else {
				handIndex = 0;
			}
			foreach (DeedCardScript card in hand.transform.GetComponentsInChildren<DeedCardScript>()) {
				card.gameObject.SetActive (false);
			}
			for (int i = handIndex * 3, j = 0; i < hand.transform.childCount && j < 3; i++, j++) {
				Transform card = hand.transform.GetChild (i);
				card.gameObject.SetActive (true);
				card.localPosition = new Vector2 (j * card.GetComponentInChildren<Image> ().sprite.bounds.size.x * card.GetComponentInChildren<RectTransform> ().localScale.x * 2, 0);
				card.transform.localScale = new Vector3(1,1,1);
			}
			UpdateLabels();
		}
	}

	public void DiscardCard(DeedCardScript card){
		if (photonView.isMine) {
			photonView.RPC ("Parenting", PhotonTargets.AllBuffered, card.gameObject.GetPhotonView ().viewID, discardPile.GetPhotonView ().viewID, false);
			cardsInHand--;
			ArrangeHand (0);
		}
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
	
	public void AddEnergy(int val, Toolbox.EnergyColour colour){
		switch (colour) {
		case Toolbox.EnergyColour.Dark:
			darkEnergy += val;
			break;
		case Toolbox.EnergyColour.Red:
			redEnergy += val;
			break;
		case Toolbox.EnergyColour.White:
			whiteEnergy += val;
			break;
		case Toolbox.EnergyColour.Green:
			greenEnergy += val;
			break;
		case Toolbox.EnergyColour.Blue:
			blueEnergy += val;
			break;
		default:
			break;
		}
		UpdateLabels();
	}

	public void DoHeal(int val){
		if (photonView.isMine) {
			if (val > 0) {
				if (hand.GetComponentsInChildren<DeedCardScript> (true).Any (x => x.cardType == Toolbox.CardType.Wound)) {
					GameObject wound = hand.GetComponentsInChildren<DeedCardScript> (true).First (x => x.cardType == Toolbox.CardType.Wound).gameObject;
					wound.transform.SetParent (GameObject.Find ("Wound Deck").transform, false);
					cardsInHand--;
					DoHeal (val - 1);
				} else {
					ArrangeHand (0);
				}
			} else {
				ArrangeHand (0);
			}
		}
	}

	public void DrawCards(){
		if (photonView.isMine) {
			while (cardsInHand < maxHandSize) {
				if (deck.transform.childCount > 0) {
					//draw card
					AddCardToHand (deck.transform.GetChild (0).GetComponent<DeedCardScript> ());
				} else if (discardPile.transform.childCount > 0) {
					//shuffle up discard pile to form new deck
					List<GameObject> cards = new List<GameObject> ();
					while (discardPile.transform.childCount > 0) {
						discardPile.transform.GetChild (0).SetParent (deck.transform, false);
					}
					ShuffleDeck ();
				} else {
					//no more cards, probably will never happen... just do nothing
					break;
				}
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
	
	public void ShowRestButton(bool show) {
		restButton.gameObject.SetActive(show);
	}

	public void ShowProvokeButton(bool show){
		provokeButton.gameObject.SetActive(show);
	}

	public void DoRest() {

		restAudio.Play ();
		menuAudio.StopAudio ();

		isResting = true;
		foreach(DeedCardScript card in hand.transform.GetComponentsInChildren<DeedCardScript>(true)) {
			DiscardCard(card);
		}
		ShowMoveButton(false);
		ShowRestButton(false);
		ShowActionButton(false);
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
			interactions.Add(new Interaction(6, Toolbox.InteractionType.Adv_Action));
			break;
		case (Toolbox.HexFeature.DarkMatterResearch):
			interactions.Add(new Interaction(7, Toolbox.InteractionType.DMD));
			break;
		case (Toolbox.HexFeature.Monastary):
			interactions.Add(new Interaction(10, Toolbox.InteractionType.Artifact));
			interactions.Add(new Interaction(2, Toolbox.InteractionType.Heal));
			break;
		case (Toolbox.HexFeature.Town):
			interactions.Add(new Interaction(7, Toolbox.InteractionType.Adv_Action));
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
			ApplyInteractionToButton(firstButton, interactions[0], interactionMenu);
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
			ApplyInteractionToButton(firstButton, interactions[0], interactionMenu);
			Button secondButton = interactionMenu.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "Second Button");
			secondButton.GetComponent<RectTransform>().sizeDelta = new Vector2(190f, 30f);
			secondButton.GetComponentInChildren<Text>().text = interactions[1].type.ToString() + " for " + interactions[1].val.ToString() + " influence";
			ApplyInteractionToButton(secondButton, interactions[1], interactionMenu);
			Button thirdButton = interactionMenu.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "Third Button");
			thirdButton.GetComponentInChildren<Text>().text = "Take no action";
			thirdButton.onClick.AddListener(() => {Destroy (interactionMenu);});
		}
	}

	public void ApplyInteractionToButton(Button button, Interaction interaction, GameObject parentMenu){
		Transform newCard;
		bool applied = false;
			switch (interaction.type) {
			case Toolbox.InteractionType.Adv_Action:
				button.onClick.AddListener(() => {
				if (interaction.val <= influence) {
					GameObject advActionDeck = GameObject.Find ("Advanced Actions Deck");
					if(advActionDeck.transform.childCount > 0){
						newCard = advActionDeck.transform.GetChild(0);
						AddCardToHand(newCard.GetComponent<DeedCardScript>());
						influence -= interaction.val;
						applied = true;
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
						AddCardToHand(newCard.GetComponent<DeedCardScript>());
						influence -= interaction.val;
						applied = true;
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
						AddCardToHand(newCard.GetComponent<DeedCardScript>());
						influence -= interaction.val;
						applied = true;
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
						applied = true;
						UpdateLabels();
					}
				});
				break;
			default:
				break;
			}
		
		button.onClick.AddListener(() => {
			if (applied) {
				Destroy (parentMenu);
			}
		});
	}

	public void UseEnergyDice(energyDiceScript dice){
		if(sourceUsesLeft > 0){
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
			sourceUsesLeft--;
			dice.Roll(true);
			UpdateLabels();
		}
	}

	public bool PayCosts(List<Cost> costs){
		ShowRestButton(false);
		bool destroyCard = false;
		foreach (Cost cost in costs) {
			if(!cost.sacrifice){
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
			} else {
				destroyCard = true;
			}
		}
		UpdateLabels();
		return destroyCard;
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

	public bool rampagersAdjacent(){
		List<HexScript> newAdjacentRampagers = GetAdjacentRampagers();
		if(newAdjacentRampagers.Count > 0){
			return true;
		}
		return false;
	}

	public void ProvokeRampagers(){
		List<EnemyScript> enemies = new List<EnemyScript>();
		List<HexScript> rampagers = GetAdjacentRampagers();
		foreach(HexScript hex in rampagers){
			enemies.Add(hex.enemiesOnHex.ElementAt(0));
		}
		DoBattle(enemies);
	}

	public void DestroyCard(DeedCardScript card){
		photonView.RPC ("Parenting", PhotonTargets.AllBuffered, card.gameObject.GetPhotonView ().viewID, destroyedCards.GetPhotonView ().viewID, false);
		cardsInHand--;
		ArrangeHand(0);
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
	
	[PunRPC] // hides this object from view
	void DisableCanvas(int canvas) {
		PhotonView c = PhotonView.Find (canvas);
		c.enabled = false;
	}

	[PunRPC] // set isBattling to true or false for all
	void EnemyIsBattling(int obj, bool isBattling) {
		PhotonView o = PhotonView.Find (obj);
		o.gameObject.GetComponent<EnemyScript>().isBattling = isBattling;
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
