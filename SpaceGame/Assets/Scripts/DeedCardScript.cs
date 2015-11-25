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


	// Use this for initialization
	void Start () {
		player = transform.GetComponentInParent<playerScript>();
		cardSprite = (GameObject) Instantiate(Resources.Load("Prefabs/CardSprite"));
		cardSprite.transform.SetParent(transform, false);
		cardSprite.transform.localPosition = new Vector2(0,0);
		LoadSprite();
		cardSprite.GetComponent<Button>().onClick.AddListener(() => {cardOnClick();});
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
			cardSprite.transform.GetComponentInChildren<Image> ().sprite = Resources.Load<Sprite> ("Sprites/arifacts");
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
		firstButton.onClick.AddListener(() => createActionMenu(
			new CardAction(sidewaysActions, myAttackVal:1, myBlockVal:1, myInfluenceVal:1, myMoveVal:1)));
		secondButton.onClick.AddListener(() => createActionMenu(topAction));
		thirdButton.onClick.AddListener(() => createActionMenu(bottomAction));
		foreach(Button button in buttons){
			button.onClick.AddListener(() => {Destroy(howToPlayCard);});
		}

	}

	private void createActionMenu(CardAction action){
		List<ActionValPair> pairs = new List<ActionValPair>();
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
					if (action.actions.Any(x => x == Toolbox.BasicAction.RangedAttack)){
						pairs.Add (new ActionValPair(action.attackVal, Toolbox.BasicAction.Attack, action.attackColour));
					}
					break;
				case Toolbox.BattlePhase.Block:
					if (action.actions.Any(x => x == Toolbox.BasicAction.Block)){
						pairs.Add (new ActionValPair(action.blockVal, Toolbox.BasicAction.Block, action.blockColour));
					}
					break;
				case Toolbox.BattlePhase.Attack:
					if (action.actions.Any(x => x == Toolbox.BasicAction.Attack || x == Toolbox.BasicAction.RangedAttack)){
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
			actionsMenu.transform.GetComponentInChildren<Text>().text = "No actions are available at this time";
			actionsMenu.transform.GetComponentInChildren<Button>().onClick.AddListener(() => {Destroy (actionsMenu);});
			actionsMenu.transform.SetParent(player.handCanvas.transform, false);
			Button firstButton = actionsMenu.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "First Button");
			firstButton.GetComponentInChildren<Text>().text = "Exit";
			firstButton.onClick.AddListener(() => {Destroy (actionsMenu);});
		}
	}
}

[System.Serializable] 
public class CardAction {
	public List<Toolbox.BasicAction> actions;
	public int attackVal, blockVal, influenceVal, moveVal, healVal;
	public Toolbox.AttackType attackColour, blockColour;

	public CardAction(List<Toolbox.BasicAction> myActions, int myAttackVal=0, int myBlockVal=0, int myInfluenceVal=0,
	                  int myHealVal=0, int myMoveVal=0, Toolbox.AttackType myAttackColour = Toolbox.AttackType.Summon,
	                  Toolbox.AttackType myBlockColour = Toolbox.AttackType.Summon) {
		actions = myActions;
		attackVal = myAttackVal;
		blockVal = myBlockVal;
		influenceVal = myInfluenceVal;
		moveVal = myMoveVal;
		healVal = myHealVal;
		attackColour = myAttackColour;
		blockColour = myBlockColour;
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
