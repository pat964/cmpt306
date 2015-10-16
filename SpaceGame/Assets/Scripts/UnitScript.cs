using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitScript : MonoBehaviour {

	public bool isReady = true;
	public bool isWounded = false;
	public bool isPoisoned = false;
	public List<Toolbox.HexFeature> recruitmentLocations;
	public int influenceCost;
	public int armor;
	public List<Toolbox.Resistance> resistances;
	public List<Toolbox.ActionType> actionTypes;
	public List<string> actionTexts;
	public List<Action> actions;

	[System.Serializable]
	//An action represents one section of the unit card, only one action may be used
	//to spend the unit. When an action is chosen, all subactions are used.
	public class Action {
		public List<BasicSubAction> subactions;
	};

	public interface SubAction{};

	[System.Serializable]
	//Basically anything basic with an associated Int value
	public class BasicSubAction : SubAction{
		//separate entries in this list should be treated as ORs (not ANDs)
		public List<Toolbox.BasicAction> action;
		public List<int> actionValues;
	}

	[System.Serializable]
	//I hate this, but it may be necessary. Units have so many special cases that it is difficult
	//to blanket them all in a nice abstraction.
	//ex. stop x enemies from attacking, or apply phys. resist to all units (ugh)
	public class SpecialSubAction : SubAction{
		//i have no idea
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
