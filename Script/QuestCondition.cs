using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*
stepover=そのステップを超えて進んだ時（デフォルト）
stepunder=そのステップを超えteinai時
stepin==そのステップに踏み込んだ時
beatall:<enemyid>=そのidの敵をすべて倒したら
beatover:<enemyid>+<N>=そのidの敵をN回倒したら
beatunder:<enemyid>+<N>
item:<itemid>=アイテムを取得したら
notitem:<itemid>=notアイテムを取得したら
levelover:<level>=level以上のレベルに達していたら
levelunder:<level>=level以下だったら
event:<eventid>=イベントが終了していたら
notevent:<eventid>=イベントが終了していたら
*/

public abstract class QuestCondition {

	public abstract bool IsConditionMatched(Step curPosition, Quest q);

	public static QuestCondition BuildCondition(int step, string conditions) {

		QCComposition qcAnd = new QCComposition(QCComposition.Mode.And);

		if( conditions != null && conditions.Length > 0 ) {
			string [] codelist = conditions.Split(',');

			foreach(string code in codelist) {
				string [] kv = code.Split( ':');
				string key = kv[0].Trim();
				string val = string.Empty;

				if( kv.Length > 1 ) {
					val = kv[1].Trim();
				}

				QuestCondition qc = null;
				
				if( key == "stepover" ) {
					qc = new QCStepTest(QCStepTest.Mode.Over, step);
				}
				else if( key == "stepunder" ) {
					qc = new QCStepTest(QCStepTest.Mode.Under, step);
				}
				else if( key == "stepin" ) {
					qc = new QCStepTest(QCStepTest.Mode.Equal, step);
				}
				else if( key == "beatall" ) {
					qc = new QCBeatEnemy(QCBeatEnemy.Mode.All, System.Int32.Parse (val), 0 );
				}
				else if( key == "beatover" ) {
					string [] ekv = code.Split( '+');
					string eid = ekv[0].Trim();
					string en  = ekv[1].Trim();
					qc = new QCBeatEnemy(QCBeatEnemy.Mode.More, System.Int32.Parse (eid), System.Int32.Parse (en) );
				}
				else if( key == "beatunder" ) {
					string [] ekv = code.Split( '+');
					string eid = ekv[0].Trim();
					string en  = ekv[1].Trim();
					qc = new QCBeatEnemy(QCBeatEnemy.Mode.Less, System.Int32.Parse (eid), System.Int32.Parse (en) );
				}
				else if( key == "item" ) {
					qc = new QCHasItem(QCHasItem.Mode.HasItem, System.Int32.Parse (val));
				}
				else if( key == "notitem" ) {
					qc = new QCHasItem(QCHasItem.Mode.DoesntHaveItem, System.Int32.Parse (val));
				}
				else if( key == "levelover" ) {
					qc = new QCLevelTest(QCLevelTest.Mode.Over, System.Int32.Parse (val));
				}
				else if( key == "levelunder" ) {
					qc = new QCLevelTest(QCLevelTest.Mode.Under, System.Int32.Parse (val));
				}
				else if( key == "event" ) {
					qc = new QCEventTest(QCEventTest.Mode.Completed, System.Int32.Parse (val));
				}
				else if( key == "notevent" ) {
					qc = new QCEventTest(QCEventTest.Mode.NotCompleted, System.Int32.Parse (val));
				}

				if(qc != null) {
					qcAnd.Add (qc);
				}
			}
		}

		return qcAnd;
	}
}

/*
 * Condition of player being certain position
 */
public class QCStepTest : QuestCondition {

	public enum Mode {
		Under,
		Over,
		Equal
	}
	private Mode m_mode;
	private int m_idx;

	public QCStepTest(Mode m, int idx) {
		m_mode = m;
		m_idx = idx;
	}

	public override bool IsConditionMatched(Step curPosition, Quest q) {
		switch(m_mode) {
		case Mode.Over:
			return curPosition.index >= m_idx;
		case Mode.Under:
			return curPosition.index < m_idx;
		case Mode.Equal:
			return curPosition.index == m_idx;
		}
		return false;
	}
}

/*
 * Condition of player beating certain enemies
 */
public class QCBeatEnemy : QuestCondition {

	public enum Mode {
		More,
		Less,
		All
	}
	private Mode m_mode;
	private int m_enemyId;
	private int m_nEnemies;

	public QCBeatEnemy(Mode m, int eID, int nEnemies) {
		m_mode = m;
		m_enemyId = eID;
		m_nEnemies = nEnemies;
	}

	public override bool IsConditionMatched(Step curPosition, Quest q) {

		switch(m_mode) {
		case Mode.All:
			// TODO: more memory friendly way
			GameObject[] gos = GameObject.FindGameObjectsWithTag("Actor");
			foreach(GameObject go in gos) {
				EnemyActor a = go.GetComponent<EnemyActor>();
				if(a){
					if( a.id == m_enemyId ) {
						return false;
					}
				}
			}
			return true;
		case Mode.More:
			return m_nEnemies <= EventRecorder.GetManager().GetNumberOfEnemyKillByPlayer(m_enemyId);
		case Mode.Less:
			return m_nEnemies > EventRecorder.GetManager().GetNumberOfEnemyKillByPlayer(m_enemyId);
		}

		return false;
	}
}

/*
 * Condition of player having certain items
 */
public class QCHasItem : QuestCondition {
	public enum Mode {
		HasItem,
		DoesntHaveItem
	}
	private Mode m_mode;
	private int m_itemId;
	
	public QCHasItem(Mode m, int itemId) {
		m_itemId = itemId;
		m_mode = m;
	}
	
	public override bool IsConditionMatched(Step curPosition, Quest q) {
		switch(m_mode) {
		case Mode.HasItem:
			return GameManager.GetManager().currentPlayer.HasItem(m_itemId);
		case Mode.DoesntHaveItem:
			return !GameManager.GetManager().currentPlayer.HasItem(m_itemId);
		}
		return false;
	}
}

/*
 * Condition of player being in certain levels
 */
public class QCLevelTest : QuestCondition {
	public enum Mode {
		Under,
		Over
	}
	private Mode m_mode;
	private int m_level;

	public QCLevelTest(Mode m, int level) {
		m_level = level;
		m_mode = m;
	}

	public override bool IsConditionMatched(Step curPosition, Quest q) {
		switch(m_mode) {
		case Mode.Over:
			return GameManager.GetManager().currentPlayer.level >= m_level;
		case Mode.Under:
			return GameManager.GetManager().currentPlayer.level < m_level;
		}
		return false;
	}
}

/*
 * Condition of event being completed
 */
public class QCEventTest : QuestCondition {
	
	public enum Mode {
		Completed,
		NotCompleted
	}
	private Mode m_mode;
	private int m_eventId;
	
	public QCEventTest(Mode m, int id) {
		m_mode = m;
		m_eventId = id;
	}
	
	public override bool IsConditionMatched(Step curPosition, Quest q) {
		switch(m_mode) {
		case Mode.Completed:
			return q.QuestCondition_IsEventCompleted(m_eventId);
		case Mode.NotCompleted:
			return !q.QuestCondition_IsEventCompleted(m_eventId);
		}
		return false;
	}
}


/*
 * Composition of condition
 */
public class QCComposition : QuestCondition {
	public enum Mode {
		And,
		Or
	}

	private List<QuestCondition> m_conditions;
	private Mode m_mode;

	public QCComposition(Mode m) {
		m_mode = m;
		m_conditions = new List<QuestCondition>();
	}
	
	public void Add(QuestCondition qc) {
		m_conditions.Add (qc);
	}

	public override bool IsConditionMatched(Step curPosition, Quest q) {
		if(m_mode == Mode.And) {
			foreach(QuestCondition qc in m_conditions) {
				if( !qc.IsConditionMatched(curPosition, q) ) {
					return false;
				}
			}
			return true;
		}
		if(m_mode == Mode.Or) {
			foreach(QuestCondition qc in m_conditions) {
				if( qc.IsConditionMatched(curPosition, q) ) {
					return true;
				}
			}
			return false;
		}
		return false;
	}
}
	
