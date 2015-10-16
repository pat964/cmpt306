using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class energyDiceScript : MonoBehaviour {

	public Toolbox.EnergyColour colour;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Roll(){
		System.Array values = System.Enum.GetValues(typeof(Toolbox.EnergyColour));
		SetColour((Toolbox.EnergyColour)values.GetValue(Toolbox.random.Next(values.Length)));
	}

	public void SetColour(Toolbox.EnergyColour newColour) {
		colour = newColour;
	}
}
