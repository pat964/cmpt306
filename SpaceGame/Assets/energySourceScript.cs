using UnityEngine;
using System.Collections;

public class energySourceScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		RollAll();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void RollAll() {
		for(int i = 0; i < gameObject.transform.childCount; i++)
		{
			GameObject child = gameObject.transform.GetChild(i).gameObject;
			child.SendMessage("Roll");
		}
	}
}
