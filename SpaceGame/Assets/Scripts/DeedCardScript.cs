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


	// Use this for initialization
	void Start () {
		cardSprite = (GameObject) Instantiate(Resources.Load("Prefabs/CardSprite"));
		cardSprite.transform.SetParent(transform, false);
		LoadSprite();
	
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
			cardSprite.transform.GetComponentInChildren<SpriteRenderer> ().sprite = Resources.Load<Sprite> (spritePath);
			break;
		case Toolbox.CardColour.Green:
			if(cardType == Toolbox.CardType.Action){
				spritePath = "Sprites/greenbotCard_Template";
			} else {
				spritePath = "Sprites/green+black";
			}
			cardSprite.transform.GetComponentInChildren<SpriteRenderer> ().sprite = Resources.Load<Sprite> (spritePath);
			break;
		case Toolbox.CardColour.Red:
			if(cardType == Toolbox.CardType.Action){
				spritePath = "Sprites/redbotCard_Template";
			} else {
				spritePath = "Sprites/red+black";
			}
			cardSprite.transform.GetComponentInChildren<SpriteRenderer> ().sprite = Resources.Load<Sprite> (spritePath);
			break;
		case Toolbox.CardColour.Blue:
			if(cardType == Toolbox.CardType.Action){
				spritePath = "Sprites/bluebotCard_Template";
			} else {
				spritePath = "Sprites/blue+black";
			}
			cardSprite.transform.GetComponentInChildren<SpriteRenderer> ().sprite = Resources.Load<Sprite> (spritePath);
			break;
		case Toolbox.CardColour.Artifact:
			cardSprite.transform.GetComponentInChildren<SpriteRenderer> ().sprite = Resources.Load<Sprite> ("Sprites/arifacts");
			break;
		case Toolbox.CardColour.Wound:
			cardSprite.transform.GetComponentInChildren<SpriteRenderer> ().sprite = Resources.Load<Sprite> ("Sprites/Wound");
			break;
		default:
			break;
		}
		Text[] cardTexts = cardSprite.transform.GetComponentsInChildren<Text>();
		cardTexts.First(x => x.gameObject.name == "Title").text = cardName;
		if (cardColour != Toolbox.CardColour.Wound){
			cardTexts.First(x => x.gameObject.name == "Top Text").text = topText;
			cardTexts.First(x => x.gameObject.name == "Bottom Text").text = bottomText;
		}
	}
}

public class CardAction {
	//when we get a better idea of how this will work, we'll put the details here
}
