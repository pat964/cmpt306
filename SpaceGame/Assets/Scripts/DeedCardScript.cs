using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

public class DeedCardScript : MonoBehaviour {

	public Toolbox.CardType cardType;
	public List<Toolbox.ActionType> actionTypes;
	public Toolbox.CardColour cardColour;

	public int cardNumber;
	public string cardName;
	public string topText;
	public string bottomText;

	public CardAction topAction;
	public CardAction bottomAction;

	public GameObject cardSprite;
	public playerScript player;

	public List<Cost> topCost;
	public List<Cost> bottomCost;


	// Use this for initialization
	void Start () {
		player = transform.GetComponentInParent<playerScript>();
		cardSprite = (GameObject) Instantiate(Resources.Load("Prefabs/CardSprite"));
		cardSprite.transform.SetParent(transform, false);
		cardSprite.transform.localPosition = new Vector2(0,0);
		LoadSprite();
		cardSprite.GetComponent<Button>().onClick.AddListener(() => {cardOnClick();});
		topCost = new List<Cost>();
		bottomCost = new List<Cost>();
		switch (cardType) {
		case Toolbox.CardType.Action:
			bottomCost.Add (new Cost(1, ColourToEnergy()));
			break;
		case Toolbox.CardType.DMD:
			topCost.Add (new Cost(1, ColourToEnergy()));
			bottomCost.Add (new Cost(1, ColourToEnergy()));
			bottomCost.Add (new Cost(1, Toolbox.EnergyColour.Dark));
			break;
		case Toolbox.CardType.Artifact:
			bottomCost.Add (new Cost(0, Toolbox.EnergyColour.Dark, true));
			break;
		default:
			break;
		}

	}
	
	// Update is called once per frame
	void Update () {

	}
	
	private void LoadSprite(){
		string spritePath;
		switch (cardColour){
		case Toolbox.CardColour.White:
			if(cardType == Toolbox.CardType.Action){
				spritePath = "Sprites/whitebotCard_Template";
			} else {
				spritePath = "Sprites/white+black";
			}
			cardSprite.transform.GetComponentInChildren<Image> ().sprite = Resources.Load<Sprite> (spritePath);
			break;
		case Toolbox.CardColour.Green:
			if(cardType == Toolbox.CardType.Action){
				spritePath = "Sprites/greenbotCard_Template";
			} else {
				spritePath = "Sprites/green+black";
			}
			cardSprite.transform.GetComponentInChildren<Image> ().sprite = Resources.Load<Sprite> (spritePath);
			break;
		case Toolbox.CardColour.Red:
			if(cardType == Toolbox.CardType.Action){
				spritePath = "Sprites/redbotCard_Template";
			} else {
				spritePath = "Sprites/red+black";
			}
			cardSprite.transform.GetComponentInChildren<Image> ().sprite = Resources.Load<Sprite> (spritePath);
			break;
		case Toolbox.CardColour.Blue:
			if(cardType == Toolbox.CardType.Action){
				spritePath = "Sprites/bluebotCard_Template";
			} else {
				spritePath = "Sprites/blue+black";
			}
			cardSprite.transform.GetComponentInChildren<Image> ().sprite = Resources.Load<Sprite> (spritePath);
			break;
		case Toolbox.CardColour.Artifact:
			cardSprite.transform.GetComponentInChildren<Image> ().sprite = Resources.Load<Sprite> ("Sprites/artifacts");
			break;
		case Toolbox.CardColour.Wound:
			cardSprite.transform.GetComponentInChildren<Image> ().sprite = Resources.Load<Sprite> ("Sprites/Wound");
			break;
		default:
			break;
		}
		Text[] cardTexts = cardSprite.transform.GetComponentsInChildren<Text>();
		if (cardColour != Toolbox.CardColour.Wound){
			cardTexts.First(x => x.gameObject.name == "Title").text = cardName;
		} else {
			cardTexts.First(x => x.gameObject.name == "Title").text = "Wound";
		}
		cardTexts.First(x => x.gameObject.name == "Top Text").text = topText;
		cardTexts.First(x => x.gameObject.name == "Bottom Text").text = bottomText;
	}

	public void cardOnClick(){
		if(cardType == Toolbox.CardType.Wound){
			return;
		}
		GameObject howToPlayCard = (GameObject) Instantiate(Resources.Load("Prefabs/ThreeButtonModal"));
		howToPlayCard.transform.SetParent(player.handCanvas.transform, false);
		howToPlayCard.transform.GetComponentInChildren<Text>().text = "How do you want to use this card?";
		howToPlayCard.transform.GetComponentInChildren<Button>().onClick.AddListener(() => {
			Destroy (howToPlayCard);
		});
		Button[] buttons = howToPlayCard.GetComponentsInChildren<Button>();
		Button firstButton = buttons.First (x => x.gameObject.name == "First Button");
		Button secondButton = buttons.First (x => x.gameObject.name == "Second Button");
		Button thirdButton = buttons.First (x => x.gameObject.name == "Third Button");
		firstButton.transform.GetComponentInChildren<Text>().text = "Play Sideways";
		secondButton.transform.GetComponentInChildren<Text>().text = "Play Top";
		thirdButton.transform.GetComponentInChildren<Text>().text = "Play Bottom";
		List<Toolbox.BasicAction> sidewaysActions = new List<Toolbox.BasicAction>();
		sidewaysActions.Add(Toolbox.BasicAction.Attack);
		sidewaysActions.Add(Toolbox.BasicAction.Block);
		sidewaysActions.Add(Toolbox.BasicAction.Influence);
		sidewaysActions.Add(Toolbox.BasicAction.Move);
		firstButton.onClick.AddListener(() => createActionMenu(new CardAction(sidewaysActions, myAttackVal:1, myBlockVal:1, myInfluenceVal:1, myMoveVal:1, myAttackColour:Toolbox.AttackType.Physical, myBlockColour:Toolbox.AttackType.Physical), new List<Cost>()));
		secondButton.onClick.AddListener(() => createActionMenu(topAction, topCost));
		thirdButton.onClick.AddListener(() => createActionMenu(bottomAction, bottomCost));
		foreach(Button button in buttons){
			button.onClick.AddListener(() => {Destroy(howToPlayCard);});
		}

	}

	private void createActionMenu(CardAction action, List<Cost> costs){
		List<ActionValPair> pairs = new List<ActionValPair>();
		if (action.actions.Any(x => x == Toolbox.BasicAction.Source_Uses)){
			pairs.Add (new ActionValPair(action.sourceVal, Toolbox.BasicAction.Source_Uses));
		}
		switch (player.turnPhase) {
		case Toolbox.TurnPhase.Move:
			if (action.actions.Any(x => x == Toolbox.BasicAction.Move)){
				pairs.Add (new ActionValPair(action.moveVal, Toolbox.BasicAction.Move));
			}
			if (action.actions.Any(x => x == Toolbox.BasicAction.Heal)){
				pairs.Add (new ActionValPair(action.healVal, Toolbox.BasicAction.Heal));
			}
			break;
		case Toolbox.TurnPhase.Action:
			if(player.isBattling){
				switch (player.battlePhase){
				case Toolbox.BattlePhase.Ranged:
					if (action.actions.Any(x => x == Toolbox.BasicAction.Ranged_Attack)){
						pairs.Add (new ActionValPair(action.attackVal, Toolbox.BasicAction.Attack, action.attackColour));
					}
					break;
				case Toolbox.BattlePhase.Block:
					if (action.actions.Any(x => x == Toolbox.BasicAction.Block)){
						pairs.Add (new ActionValPair(action.blockVal, Toolbox.BasicAction.Block, action.blockColour));
					}
					break;
				case Toolbox.BattlePhase.Attack:
					if (action.actions.Any(x => x == Toolbox.BasicAction.Attack || x == Toolbox.BasicAction.Ranged_Attack)){
						pairs.Add (new ActionValPair(action.attackVal, Toolbox.BasicAction.Attack, action.attackColour));
					}
					break;
				default:
					break;
				}
			} else {
				if (action.actions.Any(x => x == Toolbox.BasicAction.Influence)){
					pairs.Add (new ActionValPair(action.influenceVal, Toolbox.BasicAction.Influence));
				}
			}
			break;
		default:
			break;
		}
		GameObject actionsMenu;

		if(pairs.Count == 0){
			actionsMenu = (GameObject) Instantiate(Resources.Load("Prefabs/OneButtonModal"));
			actionsMenu.transform.GetComponentInChildren<Button>().onClick.AddListener(() => {Destroy (actionsMenu);});
			actionsMenu.transform.SetParent(player.handCanvas.transform, false);
			actionsMenu.transform.GetComponentInChildren<Text>().text = "No actions available at this time";
			Button firstButton = actionsMenu.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "First Button");
			firstButton.GetComponentInChildren<Text>().text = "Exit";
			firstButton.onClick.AddListener(() => {Destroy (actionsMenu);});
		} else if (pairs.Count == 1) {
			actionsMenu = (GameObject) Instantiate(Resources.Load("Prefabs/OneButtonModal"));
			actionsMenu.transform.GetComponentInChildren<Button>().onClick.AddListener(() => {Destroy (actionsMenu);});
			actionsMenu.transform.SetParent(player.handCanvas.transform, false);
			actionsMenu.transform.GetComponentInChildren<Text>().text = "Choose Action " + CostString(costs);
			Button firstButton = actionsMenu.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "First Button");
			firstButton.GetComponentInChildren<Text>().text = pairs[0].action.ToString() + " " + pairs[0].val.ToString();
			ApplyActionToButton(firstButton, pairs[0], costs, actionsMenu);
			firstButton.onClick.AddListener(() => {Destroy (actionsMenu);});
		} else if (pairs.Count == 2) {
			actionsMenu = (GameObject) Instantiate(Resources.Load("Prefabs/TwoButtonModal"));
			actionsMenu.transform.GetComponentInChildren<Button>().onClick.AddListener(() => {Destroy (actionsMenu);});
			actionsMenu.transform.SetParent(player.handCanvas.transform, false);
			actionsMenu.transform.GetComponentInChildren<Text>().text = "Choose Action" + CostString(costs);
			Button firstButton = actionsMenu.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "First Button");
			firstButton.GetComponentInChildren<Text>().text = pairs[0].action.ToString() + " " + pairs[0].val.ToString();
			ApplyActionToButton(firstButton, pairs[0], costs, actionsMenu);
			firstButton.onClick.AddListener(() => {Destroy (actionsMenu);});
			Button secondButton = actionsMenu.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "Second Button");
			secondButton.GetComponentInChildren<Text>().text = pairs[1].action.ToString() + " " + pairs[1].val.ToString();
			ApplyActionToButton(secondButton, pairs[1], costs, actionsMenu);
			firstButton.onClick.AddListener(() => {Destroy (actionsMenu);});
		} else if (pairs.Count == 3) {
			actionsMenu = (GameObject) Instantiate(Resources.Load("Prefabs/ThreeButtonModal"));
			actionsMenu.transform.GetComponentInChildren<Button>().onClick.AddListener(() => {Destroy (actionsMenu);});
			actionsMenu.transform.SetParent(player.handCanvas.transform, false);
			actionsMenu.transform.GetComponentInChildren<Text>().text = "Choose Action" + CostString(costs);
			Button firstButton = actionsMenu.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "First Button");
			firstButton.GetComponentInChildren<Text>().text = pairs[0].action.ToString() + " " + pairs[0].val.ToString();
			ApplyActionToButton(firstButton, pairs[0], costs, actionsMenu);
			firstButton.onClick.AddListener(() => {Destroy (actionsMenu);});
			Button secondButton = actionsMenu.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "Second Button");
			secondButton.GetComponentInChildren<Text>().text = pairs[1].action.ToString() + " " + pairs[1].val.ToString();
			ApplyActionToButton(secondButton, pairs[1], costs, actionsMenu);
			firstButton.onClick.AddListener(() => {Destroy (actionsMenu);});
			Button thirdButton = actionsMenu.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "Third Button");
			thirdButton.GetComponentInChildren<Text>().text = pairs[2].action.ToString() + " " + pairs[2].val.ToString();
			ApplyActionToButton(thirdButton, pairs[2], costs, actionsMenu);
			thirdButton.onClick.AddListener(() => {Destroy (actionsMenu);});
		} else if (pairs.Count == 4) {
			actionsMenu = (GameObject) Instantiate(Resources.Load("Prefabs/FourButtonModal"));
			actionsMenu.transform.GetComponentInChildren<Button>().onClick.AddListener(() => {Destroy (actionsMenu);});
			actionsMenu.transform.SetParent(player.handCanvas.transform, false);
			actionsMenu.transform.GetComponentInChildren<Text>().text = "Choose Action" + CostString(costs);
			Button firstButton = actionsMenu.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "First Button");
			firstButton.GetComponentInChildren<Text>().text = pairs[0].action.ToString() + " " + pairs[0].val.ToString();
			ApplyActionToButton(firstButton, pairs[0], costs, actionsMenu);
			firstButton.onClick.AddListener(() => {Destroy (actionsMenu);});
			Button secondButton = actionsMenu.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "Second Button");
			secondButton.GetComponentInChildren<Text>().text = pairs[1].action.ToString() + " " + pairs[1].val.ToString();
			ApplyActionToButton(secondButton, pairs[1], costs, actionsMenu);
			firstButton.onClick.AddListener(() => {Destroy (actionsMenu);});
			Button thirdButton = actionsMenu.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "Third Button");
			thirdButton.GetComponentInChildren<Text>().text = pairs[2].action.ToString() + " " + pairs[2].val.ToString();
			ApplyActionToButton(thirdButton, pairs[2], costs, actionsMenu);
			thirdButton.onClick.AddListener(() => {Destroy (actionsMenu);});
			Button fourthButton = actionsMenu.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "Fourth Button");
			fourthButton.GetComponentInChildren<Text>().text = pairs[3].action.ToString() + " " + pairs[3].val.ToString();
			ApplyActionToButton(fourthButton, pairs[3], costs, actionsMenu);
			fourthButton.onClick.AddListener(() => {Destroy (actionsMenu);});
		}
	}

	private void ApplyActionToButton(Button button, ActionValPair pair, List<Cost> costs, GameObject menu){
		//apply appropriate action
		switch (pair.action) {
		case Toolbox.BasicAction.Source_Uses:
			button.onClick.AddListener(() => {
				if (MeetsCosts(costs)){
					player.sourceUsesLeft += pair.val;
					player.UpdateLabels();
					if(player.PayCosts(costs)){
						player.DestroyCard(this);
					} else {
						player.DiscardCard(this);
					}
					Destroy(menu);
				}
			});
			break;
		case Toolbox.BasicAction.Move:
			button.onClick.AddListener(() => {
				if (MeetsCosts(costs)){
					player.moves += pair.val;
					player.UpdateLabels();
					if(player.PayCosts(costs)){
						player.DestroyCard(this);
					} else {
						player.DiscardCard(this);
					}
					Destroy(menu);
				}
			});
			break;
		case Toolbox.BasicAction.Influence:
			button.onClick.AddListener(() => {
				if (MeetsCosts(costs)){
					player.influence += pair.val;
					player.UpdateLabels();
					if(player.PayCosts(costs)){
						player.DestroyCard(this);
					} else {
						player.DiscardCard(this);
					}
					Destroy(menu);
				}
			});
			break;
		case Toolbox.BasicAction.Heal:
			button.onClick.AddListener(() => {
				if (MeetsCosts(costs)){
					player.DoHeal(pair.val);
					if(player.PayCosts(costs)){
						player.DestroyCard(this);
					} else {
						player.DiscardCard(this);
					}
					Destroy(menu);
				}
			});
			break;
		case Toolbox.BasicAction.Block:
			button.onClick.AddListener(() => {
				if (MeetsCosts(costs)){
					player.AddBlock(pair.val, pair.colour);
					if(player.PayCosts(costs)){
						player.DestroyCard(this);
					} else {
						player.DiscardCard(this);
					}
					Destroy(menu);
				}
			});
			break;
		case Toolbox.BasicAction.Ranged_Attack:
		case Toolbox.BasicAction.Attack:
			button.onClick.AddListener(() => {
				if (MeetsCosts(costs)){
					player.AddAttack(pair.val, pair.colour);
					if(player.PayCosts(costs)){
						player.DestroyCard(this);
					} else {
						player.DiscardCard(this);
					}
					Destroy(menu);
				}
			});
			break;
		default:
			break;
		}
	}

	public bool MeetsCosts(List<Cost> costs) {
		foreach (Cost cost in costs){
			if(cost.sacrifice){
				switch (cost.colour){
				case Toolbox.EnergyColour.Blue:
					if(player.blueEnergy < cost.val){
						return false;
					}
					break;
				case Toolbox.EnergyColour.Green:
					if(player.greenEnergy < cost.val){
						return false;
					}
					break;
				case Toolbox.EnergyColour.Red:
					if(player.redEnergy < cost.val){
						return false;
					}
					break;
				case Toolbox.EnergyColour.White:
					if(player.whiteEnergy < cost.val){
						return false;
					}
					break;
				case Toolbox.EnergyColour.Dark:
					if(player.darkEnergy < cost.val){
						return false;
					}
					break;
				default:
					break;
				}
			}

		}
		return true;
	}
	
	public Toolbox.EnergyColour ColourToEnergy(){
		switch (cardColour){
		case Toolbox.CardColour.Blue:
			return Toolbox.EnergyColour.Blue;
		case Toolbox.CardColour.Green:
			return Toolbox.EnergyColour.Green;
		case Toolbox.CardColour.Red:
			return Toolbox.EnergyColour.Red;
		case Toolbox.CardColour.White:
			return Toolbox.EnergyColour.White;
		default:
			return Toolbox.EnergyColour.Dark;
		}
	}

	public string CostString(List<Cost> costs){
		string returnString = "";
		if (costs.Count == 0){
			return returnString;
		} else if(costs.Any (x => x.sacrifice)){
			return ". This will destroy the card!";
		} else{
			returnString = " for ";
			for (int i = 0; i < costs.Count; i++ ){
				if (i == 0 ){
					returnString += costs[i].val.ToString() + " " + costs[i].colour.ToString();
				} else if (i == costs.Count - 1){
					returnString += " and " + costs[i].val.ToString() + " " + costs[i].colour.ToString();
				} else {
					returnString += ", " + costs[i].val.ToString() + " " + costs[i].colour.ToString();
				}
			}
			return returnString;
		}
	}
}

[System.Serializable] 
public class CardAction {
	public List<Toolbox.BasicAction> actions;
	public int attackVal, blockVal, influenceVal, moveVal, healVal, sourceVal;
	public Toolbox.AttackType attackColour, blockColour;

	public CardAction(List<Toolbox.BasicAction> myActions, int myAttackVal=0, int myBlockVal=0, int myInfluenceVal=0,
	                  int myHealVal=0, int myMoveVal=0, Toolbox.AttackType myAttackColour = Toolbox.AttackType.Summon,
	                  Toolbox.AttackType myBlockColour = Toolbox.AttackType.Summon, int mySourceVal=0) {
		actions = myActions;
		attackVal = myAttackVal;
		blockVal = myBlockVal;
		influenceVal = myInfluenceVal;
		moveVal = myMoveVal;
		healVal = myHealVal;
		attackColour = myAttackColour;
		blockColour = myBlockColour;
		sourceVal = mySourceVal;
	}

}

public class ActionValPair {
	public int val;
	public Toolbox.BasicAction action;
	public Toolbox.AttackType colour;

	public ActionValPair(int myVal, Toolbox.BasicAction myAction, Toolbox.AttackType myColour = Toolbox.AttackType.Summon){
		val = myVal;
		action = myAction;
		colour = myColour;
	}
}

public class Cost {
	public int val;
	public Toolbox.EnergyColour colour;
	public bool sacrifice;
	
	public Cost(int myVal, Toolbox.EnergyColour myColour, bool mySacrifice = false){
		val = myVal;
		colour = myColour;
		sacrifice = mySacrifice;
	}
}
