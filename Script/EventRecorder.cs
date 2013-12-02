using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventRecorder : MonoBehaviour {

	[SerializeField]
	private int m_playerMovedSteps;

	[SerializeField]
	private int m_playerBackedSteps;

	[SerializeField]
	private int m_maxStepsPlayerReached;

	[SerializeField]
	private int m_turnOfGame;

	[SerializeField]
	private int m_enemyAllKillByPlayerCount;

	[SerializeField]
	private Dictionary<int, int> m_enemyKillByPlayerCount;

	public int PlayerMovedSteps 		{ get { return m_playerMovedSteps; } }
	public int PlayerBackedSteps 		{ get { return m_playerBackedSteps; } }
	public int MaxStepsPlayerReached 	{ get { return m_maxStepsPlayerReached; } }
	public int TurnOfGame 				{ get { return m_turnOfGame; } }
	public int NumberOfAllEnemyKillByPlayer		{ get { return m_enemyAllKillByPlayerCount; } }

	static private EventRecorder s_manager;
	
	public static EventRecorder GetManager() {
		if( s_manager == null ) {
			EventRecorder obj = Component.FindObjectOfType(typeof(EventRecorder)) as EventRecorder;
			if(obj) {
				s_manager = obj;
			} else {
				Debug.LogError("EventRecorder does not exist in scene");
			}
		}
		return s_manager;
	}
	
	void Awake() {
		EventRecorder[] gms = Component.FindObjectsOfType(typeof(EventRecorder)) as EventRecorder[];
		if(gms.Length > 1) {
			Debug.LogError("EventRecorder exists more than one");
			foreach(EventRecorder gm in gms) {
				Debug.LogError("[EventRecorder] Name: " + gm.gameObject.name);
			}
		}
		if(s_manager != null && s_manager != this) {
			Debug.Log("[DYING] EventRecorder:" + gameObject.name + " destroyed");
			Destroy(gameObject);
		}
		m_enemyKillByPlayerCount = new Dictionary<int, int>();
	}

	public void RecordTurnBegin() {
		++m_turnOfGame;
	}
	public void RecordActorKill(Actor killed, Actor killer) {

		EnemyActor eKilled = killed as EnemyActor;
		PlayerActor pKiller = killer as PlayerActor;

		if( eKilled != null && pKiller != null ) {
			m_enemyAllKillByPlayerCount += 1;
			if( !m_enemyKillByPlayerCount.ContainsKey(eKilled.id) ) {
				m_enemyKillByPlayerCount.Add (eKilled.id, 0);
			}
			m_enemyKillByPlayerCount[eKilled.id] += 1;
		}
	}

	public int GetNumberOfEnemyKillByPlayer(int enemyId) {
		return m_enemyKillByPlayerCount[enemyId];
	}

	public void RecordPlayerStep(Step next, Step now) {
		m_playerMovedSteps += 1;
		m_maxStepsPlayerReached = Mathf.Max (m_maxStepsPlayerReached, next.index);
		if( now.index > next.index ) {
			m_playerBackedSteps += 1;
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
