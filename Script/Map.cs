using UnityEngine;
using System.Collections;

public class Map : MonoBehaviour {

//	static int kMAX_LENGTH 	= 128;
//	static int kMIN_LENGTH 	= 32;
	static int kTILE_SIZE 	= 32;
	static float kTILE_SIZE_WORLD = 1.6f;
	public float actorZOffset = 1.0f;
	public float itemZOffset  = 0.9f;
	public Step[] steps;

	public GameManager m_gm;
	public tk2dTiledSprite stepTile;

	[SerializeField]
	private Vector3 m_floorGimicOffset = new Vector3(-0.1737356f,0.1478726f,0.0f);

	[SerializeField]
	private GameObject m_enemyFab;
	[SerializeField]
	private GameObject m_itemFab;

	private EnemyDatabase m_enemyDB;
	private ItemDatabase m_itemDB;
	private FloorGimicDatabase m_fgDB;

	private GameObject m_enemyParent;
	private GameObject m_itemParent;
	private GameObject m_eventParent;
	private GameObject m_gimicParent;

	void Awake () {
		m_enemyDB = GetComponent<EnemyDatabase>() as EnemyDatabase;
		m_itemDB = GetComponent<ItemDatabase>() as ItemDatabase;
		m_fgDB = GetComponent<FloorGimicDatabase>() as FloorGimicDatabase;
	}
	
	/*
	 * Generates given level;
	 * called from gamemanager
	 */ 
	public void GenerateMap(string questName) {

		/*
		 * Should load some data for each currentSteps for generation
		 */

//		int length = Random.Range (kMIN_LENGTH, kMAX_LENGTH);
		int length = 1000;
		steps = new Step[length];

		for(int i = 0; i< length; ++i) {
			steps[i] = ScriptableObject.CreateInstance<Step>();
			steps[i].index = i;
			steps[i].map = this;
		}

		stepTile.dimensions = new Vector2(kTILE_SIZE * length, kTILE_SIZE);

		steps[0].SetActor(m_gm.currentPlayer);

		m_enemyParent = new GameObject("enemies");
		m_itemParent  = new GameObject("items");
		m_eventParent = new GameObject("events");
		m_gimicParent = new GameObject("floorgimic");
		m_enemyParent.transform.parent = transform;
		m_itemParent.transform.parent = transform;
		m_eventParent.transform.parent = transform;
		m_gimicParent.transform.parent = transform;

		_GenerateEnemy(questName);
		_GenerateItem(questName);
		_GenerateFloorGimic(questName);
	}

	/*
	 * generate enemy
	 */ 
	private void _GenerateEnemy(string questName) {
		int length = steps.Length;
		int enemyPopGap = Random.Range (5, 10);
		int nEnemies = ((int) length / enemyPopGap ) + 1;

		int initialNoEnemyZone = m_enemyDB.GetMinimumEnemyStep(questName);

		for(int i = 0; i < nEnemies; ++i) {
			int popStep = enemyPopGap * i + Random.Range (-3, 3);
			popStep = Mathf.Clamp (popStep, 0, length-1);
			while (steps[popStep].actorOnStep != null && popStep < length) {
				++popStep;
			}
			if( popStep >= length ) {
				Debug.Log ("[EnemyGen] max step reached. finishing enemy pop.. ");
				break; 
			}
			if( popStep < initialNoEnemyZone ) {
				continue;
			}
			
			//Debug.Log ("[Enemy] pop at "+ popStep);
			
			GameObject eo = GameObject.Instantiate(m_enemyFab, Vector3.zero, Quaternion.identity) as GameObject;
			EnemyActor ea = eo.GetComponent<EnemyActor>();
			ea.Initialize(questName, popStep, m_enemyDB, m_itemDB);
			steps[popStep].SetActor(ea);
			eo.transform.parent = m_enemyParent.transform;
		}
	}

	/*
	 * generate item
	 */ 
	private void _GenerateItem(string questName) {
		int length = steps.Length;
		int itemPopGap = Random.Range (3, 15);
		int nItems = ((int) length / itemPopGap ) + 1;
		int initialNoItemZone = m_itemDB.GetMinimumItemStep(questName);

		for(int i = 0; i < nItems; ++i) {
			int popStep = itemPopGap * i + Random.Range (-2, 2);
			popStep = Mathf.Clamp (popStep, 0, length-1);
			while (steps[popStep].itemOnStep != null && popStep < length) {
				++popStep;
			}
			if( popStep >= length ) {
				Debug.Log ("[ItemGen] max step reached. finishing item pop.. ");
				break; 
			}
			if( popStep < initialNoItemZone ) {
				continue;
			}

			//Debug.Log ("[Enemy] pop at "+ popStep);
			
			GameObject obj = GameObject.Instantiate(m_itemFab, Vector3.zero, Quaternion.identity) as GameObject;
			Item item = obj.GetComponent<Item>();
			item.Initialize(questName, popStep, m_itemDB);
			steps[popStep].SetItem(item);
			obj.transform.parent = m_itemParent.transform;
		}
	}

	/*
	 * generate item
	 */ 
	private void _GenerateFloorGimic(string questName) {
		int length = steps.Length;
		int fgPopGap = Random.Range (3, 20);
		int nGimics = ((int) length / fgPopGap ) + 1;
		int initialNoGimicZone = m_fgDB.GetMinimumFloorGimicStep(questName);
		
		for(int i = 0; i < nGimics; ++i) {
			int popStep = fgPopGap * i + Random.Range (-2, 2);
			popStep = Mathf.Clamp (popStep, 0, length-1);
			while (steps[popStep].floorGimicOnStep != null && popStep < length) {
				++popStep;
			}
			if( popStep >= length ) {
				Debug.Log ("[FgGen] max step reached. finishing item pop.. ");
				break; 
			}
			if( popStep < initialNoGimicZone ) {
				continue;
			}

			FloorGimic fg = m_fgDB.CreateGimic(questName, popStep);
			steps[popStep].SetFloorGimic(fg);
			fg.gameObject.transform.parent = m_gimicParent.transform;
		}
	}


	public void GenerateItemDynamic(int itemid, int step) {

		if( steps[step].itemOnStep != null ) {
			Destroy(steps[step].itemOnStep.gameObject);
		}

		GameObject obj = GameObject.Instantiate(m_itemFab, Vector3.zero, Quaternion.identity) as GameObject;
		Item item = obj.GetComponent<Item>();
		item.Initialize(itemid, step, m_itemDB);
		steps[step].SetItem(item);
		obj.transform.parent = m_itemParent.transform;
	}

	public void GenerateEnemyDynamic(int enemyid, int step) {
		GameObject eo = GameObject.Instantiate(m_enemyFab, Vector3.zero, Quaternion.identity) as GameObject;
		EnemyActor ea = eo.GetComponent<EnemyActor>();
		ea.Initialize(enemyid, m_enemyDB, m_itemDB);
		steps[step].SetActor(ea);
		eo.transform.parent = m_enemyParent.transform;
	}

	public void GenerateFloorGimicDynamic(int fgid, int step) {
		FloorGimic fg = m_fgDB.CreateGimic(fgid);
		steps[step].SetFloorGimic(fg);
		fg.gameObject.transform.parent = m_gimicParent.transform;
	}

	public Item CreateItem(ItemEntity e) {
		GameObject obj = GameObject.Instantiate(m_itemFab, Vector3.zero, Quaternion.identity) as GameObject;
		Item item = obj.GetComponent<Item>();
		item.Initialize(e);
		obj.transform.parent = m_itemParent.transform;
		return item;
	}

	public int ClampIndexToMapSize(int stepIndex) {
		return Mathf.Clamp (stepIndex, 0, steps.Length);
	}

	public Vector3 GetActorStepPosition(int index) {
		Vector3 pos = transform.position;
		return new Vector3(pos.x - kTILE_SIZE_WORLD * index, pos.y, pos.z - actorZOffset);
	}

	public Vector3 GetItemStepPosition(int index) {
		Vector3 pos = transform.position;
		return new Vector3(pos.x - kTILE_SIZE_WORLD * index, pos.y, pos.z - itemZOffset);
	}

	public Vector3 GetFloorGimicStepPosition(int index) {
		Vector3 pos = transform.position;
		return new Vector3(pos.x - kTILE_SIZE_WORLD * index + m_floorGimicOffset.x, pos.y + m_floorGimicOffset.y, pos.z + m_floorGimicOffset.z);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
