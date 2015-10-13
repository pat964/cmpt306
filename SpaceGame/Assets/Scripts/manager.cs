using UnityEngine;
using System.Collections;

public class manager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

/// <summary>
/// Toolbox is where we'll put all of our globals and things that should be known project-wide
/// </summary>
public class Toolbox : Singleton<Toolbox> {
	protected Toolbox () {} // guarantee this will be always a singleton only - can't use the constructor!

	public enum CardType{Action, DMD, Artifact, Wound};
	public enum ActionType{Move, Influence, Combat, Heal, Special, Action};
	public enum CardColour{Red, Blue, Green, White, Artifact, Wound};
	public enum TileType{Green, Brown, City};
	public enum TerrainType{Plains, Hills, Forrest, Desert, Mountains, Lake, Swamp, Wasteland};
	public enum HexType{Empty, Adventure, Interaction, Garrison, Rampage};
	public enum HexFeature{Portal, RampageGreen, RampageRed, MineBlue, MineRed, MineGreen, MineWhite,
						   Glade, Town, Monastary, Den, Dungeon, Base, DarkMatterResearch, SpawningGrounds,
						   TerrorLair, CityWhite, CityRed, CityGreen, CityBlue, Maze, Labyrinth, RefugeeCamp,
						   MineDeep};
	public enum EnemyColour{Green, Grey, Purple, Brown, Red, White};
	public enum EnemySpecial{Fortified, DoubleFortified, ArcaneImmune, FireResist, IceResist, PhysicalResist,
							 Elusive, Brutal, Swiftness, Poison, Assassination, NegReputation, PosReputation};
	public enum AttackType{Physical, Fire, Ice, ColdFire, Summon};

	[System.Serializable] 
	public class EnemyAttack {
		public int value;
		public Toolbox.AttackType type;
	};

	public string myGlobalVar = "whatever";
	
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
