using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class Manager : Photon.MonoBehaviour {

	private static float HEX_RAD = 1.643f;
	private static float HEX_SIDE_LENGTH = 1.9267f;
	public int MAP_HEIGHT = 8;
	public bool devTools = true;
	private static int NUM_GREEN_TILES = 8; 
	private static int NUM_BROWN_TILES = 3; 
	private static int NUM_CITY_TILES = 1; 
	private GameObject gameBoard;
	private static Camera mainCamera, battleCamera, handCamera;
	private static Canvas battleArea, handCanvas;
	private static playerScript player;
	private static List<EnemyScript> battleEnemies = new List<EnemyScript>();
	private static PhotonView scenePhotonView;

	public Transform tileFrame;
	// Use this for initialization
	void Start () {

		player = PhotonNetwork.Instantiate ("Prefabs/PlayerContainer", Vector2.zero, new Quaternion (), 0).GetComponent<playerScript>();

		scenePhotonView = this.GetComponent<PhotonView>();
		gameBoard = GameObject.Find ("Game Board");
		mainCamera = GameObject.Find ("Main Camera").GetComponent<Camera> ();
		battleCamera = GameObject.Find ("Battle Camera").GetComponent<Camera> ();
		handCamera = GameObject.Find ("Hand Camera").GetComponent<Camera> ();
		battleArea = GameObject.Find ("Battle Area").GetComponent<Canvas> ();
		handCanvas = player.transform.GetComponentsInChildren<Canvas>().First (x => x.gameObject.name == "Hand Canvas");
		battleArea.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "View Hand Button").onClick.AddListener(() => { player.ArrangeHand(0); });
		battleArea.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "View Hand Button").onClick.AddListener(() => { Manager.ChangeCameras("Hand"); });
		handCanvas.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "Return To Game Button").onClick.AddListener(() => { Manager.ChangeCameras("Main"); });
		if (PhotonNetwork.isMasterClient) {
			ShuffleAllEnemies ();
			BuildTileDeck ();
			BuildMapFrame ();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (devTools){
			if (Input.GetKeyDown(KeyCode.M)){
				player.moves += 20;
				player.UpdateLabels();
			}
			if (Input.GetKeyDown(KeyCode.Alpha1)){
				player.attacks[0] += 1;
				player.UpdateLabels();
			}
			if (Input.GetKeyDown(KeyCode.Alpha2)){
				player.attacks[1] += 1;
				player.UpdateLabels();
			}
			if (Input.GetKeyDown(KeyCode.Alpha3)){
				player.attacks[2] += 1;
				player.UpdateLabels();
			}
			if (Input.GetKeyDown(KeyCode.Alpha4)){
				player.attacks[3] += 1;
				player.UpdateLabels();
			}
			if (Input.GetKeyDown(KeyCode.Alpha5)){
				player.blocks[0] += 1;
				player.UpdateLabels();
			}
			if (Input.GetKeyDown(KeyCode.Alpha6)){
				player.blocks[1] += 1;
				player.UpdateLabels();
			}
			if (Input.GetKeyDown(KeyCode.Alpha7)){
				player.blocks[2] += 1;
				player.UpdateLabels();
			}
			if (Input.GetKeyDown(KeyCode.Alpha8)){
				player.blocks[3] += 1;
				player.UpdateLabels();
			}
		}
	}

	[PunRPC] // adds the child to the parent across the whole network
	void Parenting(int child, int parent){
		PhotonView x = PhotonView.Find (child);
		PhotonView y = PhotonView.Find (parent);
		
		x.transform.SetParent(y.transform);
	}

	[PunRPC] // adds the child to the parent across the whole network
	void Parenting(int child, int parent, bool worldSpaceStays){
		PhotonView x = PhotonView.Find (child);
		PhotonView y = PhotonView.Find (parent);
		
		x.transform.SetParent(y.transform, worldSpaceStays);
	}

	private void BuildTileDeck() {
		GameObject tileDeck = GameObject.Find("Tile Deck");
		GameObject greenTiles = GameObject.Find("Green Tiles");
		GameObject brownTiles = GameObject.Find("Brown Tiles");
		GameObject cityTiles = GameObject.Find("City Tiles");

		for (int i = 0; i<NUM_GREEN_TILES; i++){
			int index = Toolbox.random.Next(0, greenTiles.transform.childCount);
			photonView.RPC("Parenting", PhotonTargets.AllBuffered, greenTiles.transform.GetChild(index).gameObject.GetPhotonView().viewID, tileDeck.GetPhotonView().viewID);
		}
		
		for (int i = 0; i<NUM_BROWN_TILES; i++){
			int index = Toolbox.random.Next(0, brownTiles.transform.childCount);
			photonView.RPC("Parenting", PhotonTargets.AllBuffered, brownTiles.transform.GetChild(index).gameObject.GetPhotonView().viewID, tileDeck.GetPhotonView().viewID);
		}
		
		for (int i = 0; i<NUM_CITY_TILES; i++){
			int index = Toolbox.random.Next(0, cityTiles.transform.childCount);
			photonView.RPC("Parenting", PhotonTargets.AllBuffered, cityTiles.transform.GetChild(index).gameObject.GetPhotonView().viewID, tileDeck.GetPhotonView().viewID);
		}
	}
	
	private void BuildMapFrame() {
		Transform rootFrame = PhotonNetwork.Instantiate("Prefabs/TileFrame", gameBoard.transform.position, Quaternion.Euler(0,0,90), 0).transform;
		photonView.RPC("Parenting", PhotonTargets.AllBuffered, rootFrame.gameObject.GetPhotonView().viewID, gameBoard.GetPhotonView().viewID);

		BuildHorizontalRow(MAP_HEIGHT, rootFrame.position);
		BuildVerticalRow(MAP_HEIGHT, rootFrame.position);
		GameObject.Find("Green Tile 0").transform.position = rootFrame.position;
		GameObject.Find("Green Tile 0").transform.rotation = rootFrame.rotation;
		Destroy(rootFrame.gameObject);

	}

	private void BuildHorizontalRow(int loopVar, Vector2 position){
		if (loopVar > 0){
			Vector2 newPosition = new Vector2(position.x + 4 * HEX_RAD + 0.09f, position.y + (3f * HEX_SIDE_LENGTH));
			Transform newFrame = PhotonNetwork.Instantiate("Prefabs/TileFrame", newPosition, Quaternion.Euler(0,0,90), 0).transform;
			photonView.RPC("Parenting", PhotonTargets.AllBuffered, newFrame.gameObject.GetPhotonView().viewID, gameBoard.GetPhotonView().viewID);

			BuildHorizontalRow(loopVar - 1, newFrame.position);
			BuildVerticalRow(loopVar - 1, newFrame.position);
			if (loopVar == MAP_HEIGHT){
				Transform tileDeck = GameObject.Find("Tile Deck").transform;
				tileDeck.GetChild(0).position = newFrame.position;
				tileDeck.GetChild(0).rotation = newFrame.rotation;
				tileDeck.GetChild(0).gameObject.GetComponent<TileScript>().SetEnemies();
				photonView.RPC("Parenting", PhotonTargets.AllBuffered, tileDeck.GetChild(0).gameObject.GetPhotonView().viewID, gameBoard.GetPhotonView().viewID);
				Destroy(newFrame.gameObject);
			}
		}
	}

	private void BuildVerticalRow(int loopVar, Vector2 position){
		if (loopVar > 0){
			Vector2 newPosition = new Vector2(position.x - HEX_RAD, position.y + (4.5f * HEX_SIDE_LENGTH));
			Transform newFrame = PhotonNetwork.Instantiate("Prefabs/TileFrame", newPosition, Quaternion.Euler(0,0,90), 0).transform;
			photonView.RPC("Parenting", PhotonTargets.AllBuffered, newFrame.gameObject.GetPhotonView().viewID, gameBoard.GetPhotonView().viewID);

			BuildVerticalRow(loopVar - 1, newFrame.position);
			if (loopVar == MAP_HEIGHT){
				Transform tileDeck = GameObject.Find("Tile Deck").transform;
				tileDeck.GetChild(0).position = newFrame.position;
				tileDeck.GetChild(0).rotation = newFrame.rotation;
				tileDeck.GetChild(0).gameObject.GetComponent<TileScript>().SetEnemies();
				photonView.RPC("Parenting", PhotonTargets.AllBuffered, tileDeck.GetChild(0).gameObject.GetPhotonView().viewID, gameBoard.GetPhotonView().viewID);
				Destroy(newFrame.gameObject);
			}
		}
	}

	public static void SwitchToTurnPhase(Toolbox.TurnPhase phase){
		if (phase == Toolbox.TurnPhase.Move){
			//stuff
		} else if (phase == Toolbox.TurnPhase.Action){
			//stuff
		} else if (phase == Toolbox.TurnPhase.End){
			//stuff
		}
	}

	public static void ShuffleAllEnemies(){
		foreach(Toolbox.EnemyColour colour in System.Enum.GetValues(typeof(Toolbox.EnemyColour))){
			ShuffleEnemyStack(colour);
		}
	}

	public static void ShuffleEnemyStack(Toolbox.EnemyColour colour){
		string colourString = colour.ToString();
		List<GameObject> enemies = new List<GameObject>();
		GameObject enemyStack = GameObject.Find(colourString + " Enemies");
		GameObject discard = enemyStack.transform.FindChild("Discard Pile").gameObject;
		scenePhotonView.RPC("Parenting", PhotonTargets.AllBuffered, discard.GetPhotonView().viewID, null);

		for(int i = 0; i < discard.transform.childCount; i++){
			enemies.Add(discard.transform.GetChild(i).gameObject);
		}
		discard.transform.DetachChildren();
		for (int i = 0; i < enemyStack.transform.childCount; i++){
			enemies.Add(enemyStack.transform.GetChild(i).gameObject);
		}
		enemyStack.transform.DetachChildren();
		Toolbox.Shuffle(enemies);
		foreach(GameObject enemy in enemies){
			scenePhotonView.RPC("Parenting", PhotonTargets.AllBuffered, enemy.GetPhotonView().viewID, enemyStack.GetPhotonView().viewID);

		}
		scenePhotonView.RPC("Parenting", PhotonTargets.AllBuffered, discard.GetPhotonView().viewID, enemyStack.GetPhotonView().viewID);
	}

	public static void ChangeCameras (string camera)
	{
		Button returnToGame = handCanvas.transform.GetComponentsInChildren<Button>().First(x => x.gameObject.name == "Return To Game Button");
		if (camera.Equals("Main")) {
			mainCamera.enabled = true;
			battleCamera.enabled = false;
			handCamera.enabled = false;
			returnToGame.onClick.RemoveAllListeners();
			returnToGame.onClick.AddListener(() => { Manager.ChangeCameras("Main"); });
		} else if (camera.Equals("Battle")) {
			mainCamera.enabled = false;
			battleCamera.enabled = true;
			handCamera.enabled = false;
			returnToGame.onClick.RemoveAllListeners();
			returnToGame.onClick.AddListener(() => { Manager.ChangeCameras("Battle"); });
		} else if (camera.Equals("Hand")) {
			mainCamera.enabled = false;
			battleCamera.enabled = false;
			handCamera.enabled = true;
		}
	}

	public static void InitiateBattle (List<EnemyScript> enemies)
	{
		// TODO: Go through battle, and add info popups
		battleEnemies.InsertRange(0, enemies);
		Manager.ChangeCameras("Battle");
		player.SetBattleUI(true);
		Toolbox.Instance.isBattling = true;
		player.attacks = Enumerable.Repeat(0, 4).ToArray();
		player.blocks = Enumerable.Repeat(0, 4).ToArray();
		SetupEnemies();
		SetUpRangedPhase();
	}

	private static void SetupEnemies ()
	{
		float partitionWidth = battleCamera.pixelWidth / battleEnemies.Count;
		for (int i = 0; i < battleEnemies.Count; i++){
			battleEnemies[i].SetFacing(true);
			battleEnemies[i].transform.SetParent(battleCamera.gameObject.transform, false);
			battleEnemies[i].transform.localScale = new Vector3(20, 20, 0);
			battleEnemies[i].transform.position =
				battleCamera.ScreenToWorldPoint(new Vector3(partitionWidth * i + partitionWidth / 2,
				                                            battleCamera.pixelHeight / 2,
				                                            1));
			//Initiate enemy name label
			Text enemyNameLabel = ((GameObject)Instantiate(Resources.Load("Prefabs/Label"))).GetComponent<Text>();
			enemyNameLabel.transform.SetParent(battleArea.transform, false);
			enemyNameLabel.transform.position = (battleEnemies[i].transform.position);
			enemyNameLabel.transform.position = new Vector3(enemyNameLabel.transform.position.x, enemyNameLabel.transform.position.y + 5, enemyNameLabel.transform.position.z);
			battleEnemies[i].myLabels.Add(enemyNameLabel.gameObject);
			enemyNameLabel.text = battleEnemies[i].enemyName;

			//Initiate enemy armor label
			Text enemyArmorLabel = ((GameObject)Instantiate(Resources.Load("Prefabs/Label"))).GetComponent<Text>();
			enemyArmorLabel.transform.SetParent(battleArea.transform, false);
			enemyArmorLabel.transform.position = (battleEnemies[i].transform.position);
			enemyArmorLabel.transform.position = new Vector3(enemyArmorLabel.transform.position.x, enemyArmorLabel.transform.position.y + 2.8f, enemyArmorLabel.transform.position.z);
			battleEnemies[i].myLabels.Add(enemyArmorLabel.gameObject);
			enemyArmorLabel.text = string.Format("Armour: {0} ", battleEnemies[i].armor.ToString());
			if (battleEnemies[i].resistances.Count > 0) {
				if (battleEnemies[i].resistances.Contains(Toolbox.Resistance.Physical)){
					enemyArmorLabel.text += "P ";
				}
				if (battleEnemies[i].resistances.Contains(Toolbox.Resistance.Fire)){
					enemyArmorLabel.text += "F ";
				}
				if (battleEnemies[i].resistances.Contains(Toolbox.Resistance.Ice)){
					enemyArmorLabel.text += "I";
				}
			}

			//Intantiate Attack labels
			for (int j = 0; j < battleEnemies[i].attacks.Count; j++){
				Text enemyAttackLabel = ((GameObject)Instantiate(Resources.Load("Prefabs/Label"))).GetComponent<Text>();
				enemyAttackLabel.transform.SetParent(battleArea.transform, false);
				enemyAttackLabel.transform.position = (battleEnemies[i].transform.position);
				enemyAttackLabel.transform.position = new Vector3(enemyAttackLabel.transform.position.x - 8, enemyAttackLabel.transform.position.y - 2 * j, enemyAttackLabel.transform.position.z);
				enemyAttackLabel.text = string.Format("Attack: {0} ", battleEnemies[i].attacks[j].value);
				battleEnemies[i].attacks[j].label = enemyAttackLabel;
				switch (battleEnemies[i].attacks[j].type) {
				case Toolbox.AttackType.Physical:
					enemyAttackLabel.text += "P";
					break;
				case Toolbox.AttackType.Fire:
					enemyAttackLabel.text += "F";
					break;
				case Toolbox.AttackType.Ice:
					enemyAttackLabel.text += "I";
					break;
				case Toolbox.AttackType.ColdFire:
					enemyAttackLabel.text += "CF";
					break;
				case Toolbox.AttackType.Summon:
					enemyAttackLabel.text += "S";
					break;
				}
				Toggle attackToggle = ((GameObject)Instantiate(Resources.Load("Prefabs/Toggle"))).GetComponent<Toggle>();
				attackToggle.transform.SetParent(battleArea.transform, false);
				attackToggle.transform.position = (battleEnemies[i].attacks[j].label.transform.position);
				attackToggle.transform.position = new Vector3(attackToggle.transform.position.x - 2.3f, attackToggle.transform.position.y, attackToggle.transform.position.z);
				battleEnemies[i].attacks[j].myToggle = attackToggle;
				attackToggle.transform.GetComponentInChildren<Text>().text = "";
				attackToggle.gameObject.SetActive(false);
			}

			//Initiate Fame Label
			Text enemyFameLabel = ((GameObject)Instantiate(Resources.Load("Prefabs/Label"))).GetComponent<Text>();
			enemyFameLabel.transform.SetParent(battleArea.transform, false);
			enemyFameLabel.transform.position = (battleEnemies[i].transform.position);
			enemyFameLabel.transform.position = new Vector3(enemyFameLabel.transform.position.x, enemyFameLabel.transform.position.y - 4f, enemyFameLabel.transform.position.z);
			battleEnemies[i].myLabels.Add(enemyFameLabel.gameObject);
			enemyFameLabel.text = string.Format("Fame: {0}", battleEnemies[i].fame);

			//Initiate Checkbox
			Toggle enemyToggle = ((GameObject)Instantiate(Resources.Load("Prefabs/Toggle"))).GetComponent<Toggle>();
			enemyToggle.transform.SetParent(battleArea.transform, false);
			enemyToggle.transform.position = (battleEnemies[i].transform.position);
			enemyToggle.transform.position = new Vector3(enemyToggle.transform.position.x, enemyToggle.transform.position.y - 6f, enemyToggle.transform.position.z);
			battleEnemies[i].myToggle = enemyToggle;
			enemyToggle.transform.GetComponentInChildren<Text>().text = "Attack?";
		}

	}

	private static void SetUpRangedPhase () {
		GameObject.Find("Battle Phase Label").GetComponent<Text>().text = "Phase: Ranged Attack";

		//Init Attack Button
		Button attackButton = ((GameObject)Instantiate(Resources.Load("Prefabs/Button"))).GetComponent<Button>();
		attackButton.name = "Attack Button";
		attackButton.transform.SetParent(battleArea.transform, false);
		attackButton.transform.position = new Vector3(attackButton.transform.position.x + 5, attackButton.transform.position.y - 12f, attackButton.transform.position.z);
		attackButton.transform.GetComponentInChildren<Text>().text = "Attack";
		attackButton.onClick.AddListener(() => { DoAttack(true); });

		
		//Init Skip Button
		Button skipButton = ((GameObject)Instantiate(Resources.Load("Prefabs/Button"))).GetComponent<Button>();
		skipButton.name = "Skip Button";
		skipButton.transform.SetParent(battleArea.transform, false);
		skipButton.transform.position = new Vector3(skipButton.transform.position.x - 2, skipButton.transform.position.y - 12f, skipButton.transform.position.z);
		skipButton.transform.GetComponentInChildren<Text>().text = "Skip Phase";
		skipButton.onClick.AddListener(() => { SetUpBlockPhase(); });
	}
	
	private static void SetUpBlockPhase() {
		GameObject.Find("Battle Phase Label").GetComponent<Text>().text = "Phase: Block";

		//innactive enemy toggle, and active attack toggles
		foreach(EnemyScript enemy in battleEnemies){
			if (!enemy.defeated){
				enemy.myToggle.gameObject.SetActive(false);
				foreach(Toolbox.EnemyAttack attack in enemy.attacks){
					attack.myToggle.gameObject.SetActive(true);
				}
			}
		}

		//Convert attack button to Block button
		Button blockButton = GameObject.Find ("Attack Button").GetComponent<Button>();
		blockButton.name = "Block Button";
		blockButton.transform.GetComponentInChildren<Text>().text = "Block";
		blockButton.onClick.RemoveAllListeners();
		blockButton.onClick.AddListener(() => { DoBlock(); });

		//Convert Skip Button
		Button skipButton = GameObject.Find ("Skip Button").GetComponent<Button>();
		skipButton.transform.GetComponentInChildren<Text>().text = "Take Damage";
		skipButton.onClick.RemoveAllListeners();
		skipButton.onClick.AddListener(() => { TakeDamage(); });
	}

	private static void SetUpAttackPhase() {
		GameObject.Find("Battle Phase Label").GetComponent<Text>().text = "Phase: Attack";
		
		//active all undefeated enemy toggles
		foreach(EnemyScript enemy in battleEnemies){
			if (!enemy.defeated){
				enemy.myToggle.gameObject.SetActive(true);
			}
		}
		
		//Convert Block button to attack button
		Button attackButton = GameObject.Find ("Block Button").GetComponent<Button>();
		attackButton.name = "Attack Button";
		attackButton.transform.GetComponentInChildren<Text>().text = "Attack";
		attackButton.onClick.RemoveAllListeners();
		attackButton.onClick.AddListener(() => { DoAttack(false); });
		
		//Convert Skip Button
		Button skipButton = GameObject.Find ("Skip Button").GetComponent<Button>();
		skipButton.transform.GetComponentInChildren<Text>().text = "Skip Phase";
		skipButton.onClick.RemoveAllListeners();
		skipButton.onClick.AddListener(() => { SetUpEndPhase(); });
	}

	private static void SetUpEndPhase ()
	{
		//Change label and add summary
		GameObject.Find("Battle Phase Label").GetComponent<Text>().text = "Phase: Battle Finished";
		Text summaryLabel = ((GameObject)Instantiate(Resources.Load("Prefabs/Label"))).GetComponent<Text>();
		summaryLabel.name = "Summary Label";
		summaryLabel.transform.SetParent(GameObject.Find("Battle Phase Label").transform, false);
		summaryLabel.transform.localPosition = new Vector3(0, -25, 0);
		if (battleEnemies.All(x => x.defeated)){
			summaryLabel.text = "Victory! All Enemies Defeated!";
		} else {
			Toolbox.Instance.isRetreating = true;
			int defeatedCount = 0;
			foreach (EnemyScript enemy in battleEnemies){
				if (enemy.defeated){
					defeatedCount++;
				} else {
					enemy.myToggle.gameObject.SetActive(false);
				}
			}
			summaryLabel.text = string.Format("Retreat! {0} out of {1} enemy(s) defeated!", defeatedCount, battleEnemies.Count);
		}

		//destroy one button and convert other
		Destroy(GameObject.Find ("Attack Button"));
		Button skipButton = GameObject.Find ("Skip Button").GetComponent<Button>();
		skipButton.transform.GetComponentInChildren<Text>().text = "Finish";
		skipButton.transform.position = new Vector2(battleArea.transform.position.x, skipButton.transform.position.y);
		skipButton.onClick.RemoveAllListeners();
		skipButton.onClick.AddListener(() => { CleanUpBattle(); });

	}

	private static void DoAttack(bool isRanged){
		List<EnemyScript> selectedEnemies = new List<EnemyScript>();
		float armorSum = 0f;
		foreach (EnemyScript enemy in battleEnemies){
			if (!enemy.defeated && enemy.myToggle.isOn){
				selectedEnemies.Add(enemy);
			}
		}
		if (selectedEnemies.Count == 0){
			return;
		}
		foreach(EnemyScript enemy in selectedEnemies){
			if (isRanged && 
			    (enemy.specials.Contains(Toolbox.EnemySpecial.Fortified) || enemy.siteFortification) ) {
				return;
			}
			armorSum += enemy.armor;
		}

		//Apply Damage
		if (selectedEnemies.Exists(x => x.resistances.Contains(Toolbox.Resistance.Physical))){
			armorSum -= player.attacks[0] /2f;
		} else {
			armorSum -= player.attacks[0];
		}

		if (selectedEnemies.Exists(x => x.resistances.Contains(Toolbox.Resistance.Ice))){
			armorSum -= player.attacks[1] /2f;
		} else {
			armorSum -= player.attacks[1];
		}

		if (selectedEnemies.Exists(x => x.resistances.Contains(Toolbox.Resistance.Fire))){
			armorSum -= player.attacks[2] /2f;
		} else {
			armorSum -= player.attacks[2];
		}

		if (selectedEnemies.Exists(x => x.resistances.Contains(Toolbox.Resistance.Ice)) &&
		    selectedEnemies.Exists(x => x.resistances.Contains(Toolbox.Resistance.Fire))){
			armorSum -= player.attacks[3] /2f;
		} else {
			armorSum -= player.attacks[3];
		}

		//Did the player provide enough attack?
		if (armorSum <= 0){
			foreach(EnemyScript enemy in selectedEnemies){
				enemy.defeated = true;
				enemy.SetFacing(false);
				player.IncreaseFame(enemy.fame);
				Text enemyDefeatedLabel = ((GameObject)Instantiate(Resources.Load("Prefabs/Label"))).GetComponent<Text>();
				enemyDefeatedLabel.text = "Defeated";
				enemyDefeatedLabel.transform.SetParent(battleArea.transform, false);
				enemyDefeatedLabel.transform.position = (enemy.myToggle.transform.position);
				enemyDefeatedLabel.transform.position = new Vector3(enemyDefeatedLabel.transform.position.x, enemyDefeatedLabel.transform.position.y, enemyDefeatedLabel.transform.position.z);
				enemy.myLabels.Add(enemyDefeatedLabel.gameObject);
				enemy.myToggle.gameObject.SetActive(false);
			}
			player.attacks = Enumerable.Repeat(0, 4).ToArray();
			player.UpdateLabels();
		}

		//Check if all enemies are defeated
		if (battleEnemies.All(x => x.defeated == true)){
			SetUpEndPhase();
		}

	}

	private static void DoBlock(){
		Toolbox.EnemyAttack selectedAttack = null;
		foreach (EnemyScript enemy in battleEnemies){
			if (!enemy.defeated){
				foreach(Toolbox.EnemyAttack attack in enemy.attacks){
					if (attack.myToggle.isOn){
						if (selectedAttack != null){
							return;
						} else{
							selectedAttack = attack;
						}
					}
				}
			}
		}
		bool blocked = false;
		switch (selectedAttack.type){
		case Toolbox.AttackType.Physical:
			if(selectedAttack.value - player.blocks[0] - player.blocks[1] - player.blocks[2] - player.blocks[3] <= 0){
				blocked = true;
			}
			break;
		case Toolbox.AttackType.Ice:
			if(selectedAttack.value - player.blocks[0]/2f - player.blocks[1]/2f - player.blocks[2] - player.blocks[3] <= 0){
				blocked = true;
			}
			break;
		case Toolbox.AttackType.Fire:
			if(selectedAttack.value - player.blocks[0]/2f - player.blocks[1] - player.blocks[2]/2f - player.blocks[3] <= 0){
				blocked = true;
			}
			break;
		case Toolbox.AttackType.ColdFire:
			if(selectedAttack.value - player.blocks[0]/2f - player.blocks[1]/2f - player.blocks[2]/2f - player.blocks[3] <= 0){
				blocked = true;
			}
			break;
		case Toolbox.AttackType.Summon:
			blocked = true;
			break;
		}
		if (blocked){
			player.blocks = Enumerable.Repeat(0, 4).ToArray();
			selectedAttack.blocked = true;
			selectedAttack.label.text = "Blocked";
			selectedAttack.myToggle.isOn = false;
			selectedAttack.myToggle.gameObject.SetActive(false);
			if (battleEnemies.All(x => x.defeated || x.attacks.All(y => y.blocked == true))){
				SetUpAttackPhase();
			}
		}
	}

	private static void TakeDamage() {
		List<Toolbox.EnemyAttack> landedAttacks = new List<Toolbox.EnemyAttack>();
		foreach(EnemyScript enemy in battleEnemies){
			if (!enemy.defeated){
				foreach(Toolbox.EnemyAttack attack in enemy.attacks){
					if (!attack.blocked){
						landedAttacks.Add(attack);
					}
				}
			}
		}

		foreach(Toolbox.EnemyAttack attack in landedAttacks){
			int damage = attack.value;
			int woundsTaken = 0;
			if (damage > 0){
				do {
					player.AddCardToHand(GameObject.Find("Wound Deck").transform.GetComponentInChildren<DeedCardScript>());
					damage -= player.armor;
					woundsTaken++;
				} while (damage > 0);
			}
			if (woundsTaken == 1){
				attack.label.text ="Took 1 Hit!";
			} else{
				attack.label.text = string.Format("Took {0} Hits!", woundsTaken);
			}
			attack.myToggle.gameObject.SetActive(false);
		}
		SetUpAttackPhase();
	}

	private static void CleanUpBattle(){
		//Destroy Skip Button and summary label
		Destroy (GameObject.Find("Skip Button"));
		Destroy (GameObject.Find ("Summary Label"));
		//Resolve each enemy seperately
		foreach(EnemyScript enemy in battleEnemies){
			CleanUpEnemy(enemy);
		}
		battleEnemies = new List<EnemyScript>();

		//if player is retreating and on a safe space, end phase, else must retreat first
		if (Toolbox.Instance.isRetreating){
			if(player.onHex.isSafe){
				Toolbox.Instance.isRetreating = false;
				Manager.SwitchToTurnPhase(Toolbox.TurnPhase.End);
			}
		}

		//return camera to board
		ChangeCameras ("Main");
	}

	private static void CleanUpEnemy(EnemyScript enemy){
		//reset enemy scale
		enemy.transform.localScale = new Vector2(10,10);
		//clean up all attacks
		foreach(Toolbox.EnemyAttack attack in enemy.attacks){
			Destroy(attack.myToggle);
			Destroy(attack.label);
			attack.blocked = false;
		}

		//clean up labels and toggle
		foreach (GameObject label in enemy.myLabels){
			Destroy(label);
		}
		enemy.myLabels = new List<GameObject>();
		Destroy(enemy.myToggle);

		//if enemy was defeated, reset it and return it to discard pile
		//else return it to board
		if(enemy.defeated){
			enemy.defeated = false;
			enemy.SetFacing(false);
			enemy.homeHex.enemiesOnHex.Remove(enemy);
			if (enemy.homeHex.enemiesOnHex.Count == 0){
				enemy.homeHex.isSafe = true;
				if (enemy.homeHex.hexType == Toolbox.HexType.Garrison){
					enemy.homeHex.hexType = Toolbox.HexType.Interaction;
				}
			}
			enemy.homeHex = null;
			enemy.siteFortification = false;
			DiscardEnemy(enemy);
		} else {
			enemy.transform.position = enemy.homeHex.transform.position;
		}
	}

	private static void DiscardEnemy(EnemyScript enemy){
		GameObject enemyStack;
		GameObject discardPile;
		switch (enemy.colour){
		case Toolbox.EnemyColour.Green:
			enemyStack = GameObject.Find("Green Enemies");
			discardPile = enemyStack.transform.GetChild(enemyStack.transform.childCount - 1).gameObject;
			scenePhotonView.RPC("Parenting", PhotonTargets.AllBuffered, enemy.gameObject.GetPhotonView().viewID, discardPile.GetPhotonView().viewID, false);
			break;
		case Toolbox.EnemyColour.Grey:
			enemyStack = GameObject.Find("Grey Enemies");
			discardPile = enemyStack.transform.GetChild(enemyStack.transform.childCount - 1).gameObject;
			scenePhotonView.RPC("Parenting", PhotonTargets.AllBuffered, enemy.gameObject.GetPhotonView().viewID, discardPile.GetPhotonView().viewID, false);
			break;
		case Toolbox.EnemyColour.Brown:
			enemyStack = GameObject.Find("Brown Enemies");
			discardPile = enemyStack.transform.GetChild(enemyStack.transform.childCount - 1).gameObject;
			scenePhotonView.RPC("Parenting", PhotonTargets.AllBuffered, enemy.gameObject.GetPhotonView().viewID, discardPile.GetPhotonView().viewID, false);
			break;
		case Toolbox.EnemyColour.Purple:
			enemyStack = GameObject.Find("Purple Enemies");
			discardPile = enemyStack.transform.GetChild(enemyStack.transform.childCount - 1).gameObject;
			scenePhotonView.RPC("Parenting", PhotonTargets.AllBuffered, enemy.gameObject.GetPhotonView().viewID, discardPile.GetPhotonView().viewID, false);
			break;
		case Toolbox.EnemyColour.White:
			enemyStack = GameObject.Find("White Enemies");
			discardPile = enemyStack.transform.GetChild(enemyStack.transform.childCount - 1).gameObject;
			scenePhotonView.RPC("Parenting", PhotonTargets.AllBuffered, enemy.gameObject.GetPhotonView().viewID, discardPile.GetPhotonView().viewID, false);
			break;
		case Toolbox.EnemyColour.Red:
			enemyStack = GameObject.Find("Red Enemies");
			discardPile = enemyStack.transform.GetChild(enemyStack.transform.childCount - 1).gameObject;
			scenePhotonView.RPC("Parenting", PhotonTargets.AllBuffered, enemy.gameObject.GetPhotonView().viewID, discardPile.GetPhotonView().viewID, false);
			break;
		}
	}
}

/// <summary>
/// Toolbox is where we'll put all of our globals and things that should be known project-wide
/// </summary>
public class Toolbox : Singleton<Toolbox> {
	public static float UNIT_LENGTH = 1f;

	public static System.Random random = new System.Random();
	protected Toolbox () {} // guarantee this will be always a singleton only - can't use the constructor!

	public enum TurnPhase{Move, Action, End};
	public enum CardType{Action, DMD, Artifact, Wound};
	public enum ActionType{Move, Influence, Combat, Heal, Special, Action};
	public enum BasicAction{Influence, Move, FireAttack, IceAttack, ColdFireAttack, FireBlock,
						    IceBlock, ColdFireBlock, Heal, ReduceAttack};
	public enum CardColour{Red, Blue, Green, White, Artifact, Wound};
	public enum TileType{Green, Brown, City};
	public enum TerrainType{Plains, Hills, Forest, Desert, Mountains, Lake, Swamp, Wasteland}; // CHANGED
	public enum HexType{Empty, Adventure, Interaction, Garrison, Rampage}; 
	public enum HexFeature{Empty, Portal, RampageGreen, RampageRed, MineBlue, MineRed, MineGreen, MineWhite,
						   Glade, Town, Monastary, Den, Dungeon, Base, DarkMatterResearch, Ruins, SpawningGrounds,
						   TerrorLair, CityWhite, CityRed, CityGreen, CityBlue, Maze, Labyrinth, RefugeeCamp,
						   MineDeep};
	public enum EnemyColour{Green, Grey, Purple, Brown, Red, White};
	public enum EnemySpecial{Fortified, DoubleFortified, ArcaneImmune, Elusive, Brutal, Swiftness, Poison,
		Assassination, Paralyze, Cumbersome, NegReputation, PosReputation};
	public enum Resistance{Fire, Ice, Physical};
	public enum AttackType{Physical, Fire, Ice, ColdFire, Summon};
	public enum RuinType{Battle, Energy};
	public enum EnergyColour{Red, Green, White, Blue, Gold, Dark};
	public enum Reward{sevenFame, tenFame, Unit, DMD, AdvancedAction, Artifact};

	[System.Serializable] 
	public class EnemyAttack {
		public int value;
		public Toolbox.AttackType type;
		public bool blocked = false;
		public Text label;
		public Toggle myToggle;
	};

	public bool isBattling = false;
	public bool isRetreating = false;
	public bool isDay = true;
	
	public Language language = new Language();
	
	void Awake () {
		// Your initialization code here
	}
	
	public static void Shuffle<T>(IList<T> list) {
		int n = list.Count;
		while (n > 1) {
			int k = (random.Next(0, n) % n);
			n--;
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}
}

[System.Serializable]
public class Language {
	public string current;
	public string lastLang;
}

/// <summary>
/// Don't worry about this class, its here so we can use globals effectively
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T _instance;
	
	private static object _lock = new object();
	
	public static T Instance
	{
		get
		{
			if (applicationIsQuitting) {
				Debug.LogWarning("[Singleton] Instance '"+ typeof(T) +
				                 "' already destroyed on application quit." +
				                 " Won't create again - returning null.");
				return null;
			}
			
			lock(_lock)
			{
				if (_instance == null)
				{
					_instance = (T) FindObjectOfType(typeof(T));
					
					if ( FindObjectsOfType(typeof(T)).Length > 1 )
					{
						Debug.LogError("[Singleton] Something went really wrong " +
						               " - there should never be more than 1 singleton!" +
						               " Reopenning the scene might fix it.");
						return _instance;
					}
					
					if (_instance == null)
					{
						GameObject singleton = new GameObject();
						_instance = singleton.AddComponent<T>();
						singleton.name = "(singleton) "+ typeof(T).ToString();
						
						DontDestroyOnLoad(singleton);
						
						Debug.Log("[Singleton] An instance of " + typeof(T) + 
						          " is needed in the scene, so '" + singleton +
						          "' was created with DontDestroyOnLoad.");
					} else {
						Debug.Log("[Singleton] Using instance already created: " +
						          _instance.gameObject.name);
					}
				}
				
				return _instance;
			}
		}
	}
	
	private static bool applicationIsQuitting = false;
	/// <summary>
	/// When Unity quits, it destroys objects in a random order.
	/// In principle, a Singleton is only destroyed when application quits.
	/// If any script calls Instance after it have been destroyed, 
	///   it will create a buggy ghost object that will stay on the Editor scene
	///   even after stopping playing the Application. Really bad!
	/// So, this was made to be sure we're not creating that buggy ghost object.
	/// </summary>
	public void OnDestroy () {
		applicationIsQuitting = true;
	}
}

static public class MethodExtensionForMonoBehaviourTransform {
	/// <summary>
	/// Gets or add a component. Usage example:
	/// BoxCollider boxCollider = transform.GetOrAddComponent<BoxCollider>();
	/// </summary>
	static public T GetOrAddComponent<T> (this Component child) where T: Component {
		T result = child.GetComponent<T>();
		if (result == null) {
			result = child.gameObject.AddComponent<T>();
		}
		return result;
	}
}

