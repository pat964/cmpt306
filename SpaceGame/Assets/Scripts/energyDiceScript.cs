using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class energyDiceScript : Photon.MonoBehaviour {

	public energySourceScript source;
	public Toolbox.EnergyColour colour;
	public Image image;
	// Use this for initialization
	void Start () {
		source = transform.GetComponentInParent<energySourceScript>();
		GetComponent<Button>().onClick.AddListener(() => source.player.UseEnergyDice(this));
		image = GetComponent<Image>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Roll(){
		System.Array values = System.Enum.GetValues (typeof(Toolbox.EnergyColour));
		SetColour((Toolbox.EnergyColour)values.GetValue(Toolbox.random.Next(values.Length)));
	}

	public void SetColour(Toolbox.EnergyColour newColour) {
		colour = newColour;
		switch (colour){
		case Toolbox.EnergyColour.Blue:
			image.color = Color.blue;
			break;
		case Toolbox.EnergyColour.Red:
			image.color = Color.red;
			break;
		case Toolbox.EnergyColour.Green:
			image.color = Color.green;
			break;
		case Toolbox.EnergyColour.White:
			image.color = Color.white;
			break;
		case Toolbox.EnergyColour.Dark:
			image.color = Color.black;
			break;
		default:
			break;
		}
	}

}
