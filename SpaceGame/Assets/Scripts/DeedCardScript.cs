using UnityEngine;
using System.Collections.Generic;
using System.Collections;

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


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

public class CardAction {
	//when we get a better idea of how this will work, we'll put the details here
}
