using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Quest : MonoBehaviour {


	[System.SerializableAttribute]
	class TA {
		public string 			name  = string.Empty;
		public GameObject		fab   = null;
	}
	public Vector3 taOffset;

	[SerializeField]
	private QuestData m_questData;
	
	[SerializeField]
	private bool m_questComplete;

	[SerializeField]
	private int m_questIdx;
	
	[SerializeField]
	private List<QuestEntity> m_events;

	[SerializeField]
	private GameManager m_gm;

	[SerializeField]
	private int m_inturrptingCount;

	[SerializeField]
	private QuestEntity m_curEntity;

	[SerializeField]
	private TA[] m_tas;
	
	public string QuestName { get { return m_questData.sheets[m_questIdx].name; } }
	
	public void Initialize(int questIndex, GameManager gm) {
		m_questIdx = questIndex;
		m_gm = gm;
	}

	public void Initialize2(int questIndex, GameManager gm) {
		m_inturrptingCount = 0;
		m_questComplete = false;
		m_questIdx = questIndex;
		m_events = new List<QuestEntity>();
		m_gm = gm;
		_LoadQuest(questIndex);
		m_curEntity = null;
	}
	
	private void _LoadQuest(int questIndex) {
		m_events.Clear ();
		foreach(QuestData.Param p in m_questData.sheets[questIndex].list) {
			QuestEntity qe = QuestEntity.CreateEntity(p, this);
			m_events.Add(qe);
		}
	}

	private void _WaitForCutsceneEnd(int value) {
		m_curEntity.NotifyEventProceed();
		--m_inturrptingCount;
	}

	public bool QuestCondition_IsEventCompleted(int eventId) {
		foreach(QuestEntity q in m_events) {
			if(q.id == eventId) {
				return q.IsCompleted;
			}
		}
		return false;
	}
	
	// return if action was proceeded
	// if false need try again
	public bool QuestAction_CutScene(List<string> msgs) {
		if( m_inturrptingCount == 0 ) {
			m_gm.currentPlayer.FaceDirection(DirectionType.BACK);
			GUIManager.GetManager().DoCutsceneWithMessages(msgs, _WaitForCutsceneEnd);
			++m_inturrptingCount;
			return true;
		} else {
			return false;
		}
	}

	public bool QuestAction_TeropAndWait(string message) {
		if( m_inturrptingCount == 0 ) {
			GUIManager.GetManager().DoTerop(message, _WaitForCutsceneEnd);
			++m_inturrptingCount;
			return true;
		} else {
			return false;
		}
	}
	
	public bool QuestAction_QuestClearAndWait(string questName) {
		if( m_inturrptingCount == 0 ) {
			GUIManager.GetManager().DoQuestClear(questName, _WaitForCutsceneEnd);
			++m_inturrptingCount;
			return true;
		} else {
			return false;
		}
	}

	public void Gimic_GenerateTA(string val, int step) {
		// TODO: put this into map
		foreach(TA t in m_tas) {
			if( t.name == val ) {
				Vector3 pos = m_gm.currentMap.GetActorStepPosition(step) + taOffset;
				GameObject.Instantiate(t.fab, pos, t.fab.transform.rotation);
			}
		}
	}

	public void Gimic_GenerateItem(int itemid, int step) {
		m_gm.currentMap.GenerateItemDynamic(itemid, step);
	}

	public void Gimic_GenerateEnemy(int enemyid, int step) {
		m_gm.currentMap.GenerateEnemyDynamic(enemyid, step);
	}

	private QuestEntity _FindNextAvailableEvent() {
		PlayerActor pa = GameManager.GetManager().currentPlayer; 

		foreach(QuestEntity qe in m_events) {
			if( !qe.IsCompleted && qe.condition != null && qe.condition.IsConditionMatched(pa.step, this) ) {
				return qe;
			}
		}
		return null;
	}

	public bool ProceedEvent() {

		if( m_questComplete ) {
			LocalGameSession.GetSession().MarkQuestComplete(m_questIdx);
			m_gm.ExitGame();
			return false;
		}

		else {
			// no appropriate event found. try find some...
			if(m_curEntity == null) {
				m_curEntity = _FindNextAvailableEvent();
			}
			// if there is something found or ongoing, do it.
			if( m_curEntity != null ) {
				// if event is complete, test if quest is complete
				// if not find next one
				if( m_curEntity.IsCompleted ) {
					if( m_curEntity.isEndEvent ) {
						m_questComplete = true;
					}
					m_curEntity = null;
					return false;
				} else {
					m_curEntity.DoEvent();
					return false;
				}
			} 
		}

		return true;
	}
	
	// true if event inturrupt turn system and stop game from going forward
	public bool IsInturrptingGame() {
		return m_inturrptingCount > 0;
	}
}
