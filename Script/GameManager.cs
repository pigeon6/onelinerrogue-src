using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	
	private class SortByActionOrder : IComparer<AbstractActor> {
		public int Compare(AbstractActor x, AbstractActor y) {
			// larger agility should come sooner
			return Mathf.CeilToInt(y.ActionOrder - x.ActionOrder);
		}
	}

	public GameObject playerFab;
	public GameObject mapFab;

	public PlayerActor currentPlayer;
	public AbstractActor currentActor;
	public Quest currentQuest;
	public Map currentMap;
	public int deathLimitStep;
	public int currentTurn;
	public int questIndex = -1;
	public bool paused;
	public bool isInGame;
	public static bool skipTutorial;
	private SortByActionOrder m_sorter;

	[SerializeField]
	private LevelDatabase m_leveldb;

	[SerializeField]
	private GameObject m_tutorial;

	[SerializeField]
	private GameCamera m_gameCamera;

	[SerializeField]
	private string m_afterGameOverSceneName;

	[SerializeField]
	private float m_turnWaitSec = 0.0f;
	private List<AbstractActor> m_actors;

	public LevelDatabase LevelDB { get { return m_leveldb; } }

	static private GameManager s_manager;
	
	public static GameManager GetManager() {
		if( s_manager == null ) {
			GameManager obj = Component.FindObjectOfType(typeof(GameManager)) as GameManager;
			if(obj) {
				s_manager = obj;
			} else {
				GameObject go = new GameObject("GameManager");
				obj = go.AddComponent<GameManager>() as GameManager;
				s_manager = obj;
			}
		}
		return s_manager;
	}

	void Awake() {
		GameManager[] gms = Component.FindObjectsOfType(typeof(GameManager)) as GameManager[];
		if(gms.Length > 1) {
			Debug.LogError("GameManager exists more than one");
			foreach(GameManager gm in gms) {
				Debug.LogError("[GameManager] Name: " + gm.gameObject.name);
			}
		}
		if(s_manager != null && s_manager != this) {
			Debug.Log("[DYING] GameManager:" + gameObject.name + " destroyed");
			Destroy(gameObject);
		} else {
			s_manager = this;
		}
		m_sorter = new SortByActionOrder();

		//doTutorial = true;
	}

	void _InitializeNewGame(int questIndex) {

		currentQuest = GetComponent<Quest>() as Quest;
		currentQuest.Initialize(questIndex, this);

		currentTurn = 0;

		m_actors = new List<AbstractActor>();
		GameObject playerObj = GameObject.Instantiate(playerFab, Vector3.zero, Quaternion.identity) as GameObject;
		_SetupPlayer(playerObj);
		
		GameObject mapobj = GameObject.Instantiate(mapFab, Vector3.zero, Quaternion.identity) as GameObject;
		_SetupMap(mapobj);

		currentQuest.Initialize2(questIndex, this);
	}
	
	/*
	 *  setup map
	 */
	private void _SetupMap(GameObject map) {
		currentMap = map.GetComponent<Map>();
		currentMap.m_gm = this;
		currentMap.GenerateMap(currentQuest.QuestName);
	}

	/*
	 *  Load and set starting setting of player
	 * (like item possesion)
	 */
	private void _SetupPlayer(GameObject player) {
		// TODO: 
		currentPlayer = player.GetComponent<PlayerActor>();
	}
	
	// Use this for initialization
	void Start () {
		// just in case make sure UserInput exists in the world
		UserInput.GetUserInput();
		if( questIndex < 0 ) {
			questIndex = LocalGameSession.GetSession().currentQuest;
		}
		_InitializeNewGame( questIndex );

		_GameSceneBootstrap();
 	}

	private void _GameSceneBootstrap() { 
		UserInput.GetUserInput().PushActionEventStack();

		CommandChain cc = ScriptableObject.CreateInstance<CommandChain>();
		cc.Add (new CommandChain.Command(_StartGame));

		if( !skipTutorial ) {
			//skipTutorial = true;
			GameObject go = GameObject.Instantiate(m_tutorial) as GameObject;
			Tutorial t = go.GetComponent<Tutorial>();
			t.RegisterTutorialCommands(cc);
		}

		GUIManager.GetManager().FadeoutFromBlack(1.0f, 1.0f,cc);
	}

	private void _StartGame(object[] args, CommandChain cc) {
		UserInput.GetUserInput().PopActionEventStack();
		_StartTurnSystem();
		SoundManager.GetManager().PlayBGM(BGMTrack.Quest);
	}
	
	private void _StartTurnSystem() {
		isInGame = true;
		StartCoroutine(_PerformTurns());
	}

	public void ExitGame() {
		_GameOverBootstrap();
	}

	private void _TestWorldEnd() {
		// test death
		for(int i = 0; i < deathLimitStep; ++i) {
			Actor a = currentMap.steps[i].actorOnStep;
			if(a != null) {
				GUIManager.GetManager().Message(a.charName + " は 世界のはてに まきこまれた！" );
				a.Die();
			}
		}
	}

	private void _GameOverBootstrap() { 
		Debug.Log ("[GM] GameOver Process...");
		isInGame = false;
		UserInput.GetUserInput().PushActionEventStack();
		
		CommandChain cc = ScriptableObject.CreateInstance<CommandChain>();
		cc.Add (new CommandChain.Command(_GameOver));

		GUIManager.GetManager().FadeinToBlack(1.0f, 3.0f,cc);
	}

	private void _GameOver(object[] args, CommandChain cc) {
		Application.LoadLevel(m_afterGameOverSceneName);
	}
	
	// Use this for initialization
	private IEnumerator _PerformTurns () {

		// TODO: temp impl
		while(isInGame) {
			// gameover. start bootstrap gameover
//			if(currentPlayer.dead) {
//				Debug.Log ("[GM] Player Dead. starting GameOver...");
//				_GameOverBootstrap();
//				break;
//			}

			while(paused) {
				yield return new WaitForEndOfFrame();
			}


			// proceed event before turn
			if( !currentQuest.ProceedEvent() ) {
				yield return new WaitForEndOfFrame();
				continue;
			}

			if(!isInGame) {
				break;
			}

			EventRecorder.GetManager().RecordTurnBegin();
			++currentTurn;

			// TODO: collect/sort actors
			GameObject[] gos = GameObject.FindGameObjectsWithTag("Actor");
			foreach(GameObject go in gos) {
				AbstractActor a = go.GetComponent<AbstractActor>();
				if(a){
					m_actors.Add(a);
				}
			}

			m_actors.Sort (m_sorter);

			foreach(AbstractActor a in m_actors) {

				if( currentPlayer.Dead ) {
					break;
				}

				if( a == null ) {
					continue;
				}

				if( a.Dead ) {
					continue;
				}

				while( GUIManager.GetManager().IsAnyMenuWindowOpen() ) {
					yield return new WaitForEndOfFrame();
				}
				while(paused) {
					yield return new WaitForEndOfFrame();
				}
				if(!isInGame) {
					break;
				}

				currentActor = a;
				a.BeginTurnAction(this);
				while(!a.IsTurnActionEnd()) {
					yield return new WaitForEndOfFrame();
				}
				currentActor = null;
				if( m_turnWaitSec > 0.0f ) {
					yield return new WaitForSeconds(m_turnWaitSec);
				}
			}

			// wait for camera scroll ends
			if(m_gameCamera != null) {
				do {
					yield return new WaitForEndOfFrame();
				} while(m_gameCamera.IsScrolling);
			}
			_TestWorldEnd();

			m_actors.Clear();
		}

		Debug.Log ("[TurnSystem] TurnSystem Terminated.");
	}
}
