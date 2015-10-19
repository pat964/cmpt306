﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyScript : MonoBehaviour {
	public string enemyName;
	public Toolbox.EnemyColour colour;
	public int armor;
	public List<Toolbox.EnemyAttack> attacks;
	public List<Toolbox.EnemySpecial> specials;
	public List<Toolbox.Resistance> resistances;
	public int fame;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}