using UnityEngine;
using System.Collections;

public class manager : MonoBehaviour {

	private static float HEX_RAD = 1.643f;
	private static float HEX_SIDE_LENGTH = 1.9267f;
	public int MAP_HEIGHT = 8;
	private static int NUM_GREEN_TILES = 8; 
	private static int NUM_BROWN_TILES = 3; 
	private static int NUM_CITY_TILES = 1; 
	private GameObject gameBoard;

	public Transform tileFrame;
	// Use this for initialization
	void Start () {
		gameBoard = GameObject.Find("Game Board");
		BuildTileDeck();
		BuildMapFrame();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void BuildTileDeck() {
		GameObject tileDeck = GameObject.Find("Tile Deck");
		GameObject greenTiles = GameObject.Find("Green Tiles");
		GameObject brownTiles = GameObject.Find("Brown Tiles");
		GameObject cityTiles = GameObject.Find("City Tiles");

		for (int i = 0; i<NUM_GREEN_TILES; i++){
			int index = Toolbox.random.Next(0, greenTiles.transform.childCount);
			greenTiles.transform.GetChild(index).SetParent(tileDeck.transform);
		}
		
		for (int i = 0; i<NUM_BROWN_TILES; i++){
			int index = Toolbox.random.Next(0, brownTiles.transform.childCount);
			brownTiles.transform.GetChild(index).SetParent(tileDeck.transform);
		}
		
		for (int i = 0; i<NUM_CITY_TILES; i++){
			int index = Toolbox.random.Next(0, cityTiles.transform.childCount);
			cityTiles.transform.GetChild(index).SetParent(tileDeck.transform);
		}
	}
	
	private void BuildMapFrame() {
		Transform rootFrame = (Transform) GameObject.Instantiate(tileFrame, gameBoard.transform.position, Quaternion.Euler(0,0,90));
		rootFrame.SetParent(gameBoard.transform);
		BuildHorizontalRow(MAP_HEIGHT, rootFrame.position);
		BuildVerticalRow(MAP_HEIGHT, rootFrame.position);
		GameObject.Find("Green Tile 0").transform.position = rootFrame.position;
		GameObject.Find("Green Tile 0").transform.rotation = rootFrame.rotation;
		Destroy(rootFrame.gameObject);

	}

	private void BuildHorizontalRow(int loopVar, Vector2 position){
		if (loopVar > 0){
			Vector2 newPosition = new Vector2(position.x + 4 * HEX_RAD + 0.09f, position.y + (3f * HEX_SIDE_LENGTH));
			Transform newFrame = (Transform) GameObject.Instantiate(tileFrame, newPosition, Quaternion.Euler(0,0,90));
			newFrame.SetParent(gameBoard.transform);
			BuildHorizontalRow(loopVar - 1, newFrame.position);
			BuildVerticalRow(loopVar - 1, newFrame.position);
			if (loopVar == MAP_HEIGHT){
				GameObject.Find("Tile Deck").transform.GetChild(0).position = newFrame.position;
				GameObject.Find("Tile Deck").transform.GetChild(0).rotation = newFrame.rotation;
				GameObject.Find("Tile Deck").transform.GetChild(0).SetParent(gameBoard.transform);
				Destroy(newFrame.gameObject);
			}
		}
	}

	private void BuildVerticalRow(int loopVar, Vector2 position){
		if (loopVar > 0){
			Vector2 newPosition = new Vector2(position.x - HEX_RAD, position.y + (4.5f * HEX_SIDE_LENGTH));
			Transform newFrame = (Transform) GameObject.Instantiate(tileFrame, newPosition, Quaternion.Euler(0,0,90));
			newFrame.SetParent(gameBoard.transform);
			BuildVerticalRow(loopVar - 1, newFrame.position);
			if (loopVar == MAP_HEIGHT){
				GameObject.Find("Tile Deck").transform.GetChild(0).position = newFrame.position;
				GameObject.Find("Tile Deck").transform.GetChild(0).rotation = newFrame.rotation;
				GameObject.Find("Tile Deck").transform.GetChild(0).SetParent(gameBoard.transform);
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
	};

	public string myGlobalVar = "whatever";
	public bool isDay = true;
	
	public Language language = new Language();
	
	void Awake () {
		// Your initialization code here
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

