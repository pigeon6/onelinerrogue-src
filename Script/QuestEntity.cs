using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestEntity : ScriptableObject {

	public delegate bool TestQuestComplete();
	
	public enum Kind {
		DialogEvent,
		Terop,
	}

	public int id;
	public bool isEndEvent;
	public bool isCompleted;		// if this QuestEntity is done
	public int step;				// event happens after step has crossed
	public Kind kind;
	public List<string> messages;
	public Quest parent;
	public QuestCondition condition;

	public bool IsCompleted { get { return isCompleted; } }

	public void DoEvent() {
		if(kind == Kind.DialogEvent) {
			parent.QuestAction_CutScene(messages);
		} else if( kind == Kind.Terop ) {
			parent.QuestAction_TeropAndWait( messages[0] );
		}
	}

	public void NotifyEventProceed() {
		if( kind == Kind.DialogEvent ) {
			Debug.Log ("[EventEntity] event:"+id +" | event complete!");
			condition = null;
			isCompleted  = true;
		}
		if( kind == Kind.Terop ) {
			Debug.Log ("[EventEntity] event:"+id +" | event complete!");
			condition = null;
			isCompleted  = true;
		}
	}

	private void _PutGimic(string gimicCode, int stepOffset) {
		Debug.Log ("[EventEntity] put gimic:" + gimicCode);

		if( gimicCode != null && gimicCode.Length > 0 ) {
			string [] codelist = gimicCode.Split(',');

			foreach(string code in codelist) {
				string [] kv = code.Split( ':');
				string key = kv[0].Trim();
				string val = kv[1].Trim();
				
				if( key == "ta" ) {
					// put friend actor
					Debug.Log ("[QUEST][FriendActor] put:" + val);
					parent.Gimic_GenerateTA(val, step + stepOffset);
				}
				else if( key == "item" ) {
					// put item
					Debug.Log ("[QUEST][ITEM] put:" + val);
					int id = System.Int32.Parse(val);
					parent.Gimic_GenerateItem(id, step + stepOffset);
				}
				else if( key == "enemy" ) {
					// put item
					Debug.Log ("[QUEST][Enemy] put:" + val);
					int id = System.Int32.Parse(val);
					parent.Gimic_GenerateEnemy(id, step + stepOffset);
				}
			}
		}
	}

	private void Initialize(QuestData.Param p, Quest q) {
		parent = q;
		id = p.id;
		step = p.step;
		isCompleted = false;
		isEndEvent = p.kind == "endevent";

		condition = QuestCondition.BuildCondition(step, p.condition);

		if( p.kind == "event" || p.kind == "endevent" ) {
			kind = Kind.DialogEvent;
		}
		if( p.kind == "terop" ) {
			kind = Kind.Terop;
		}

		if( p.dialog != null && p.dialog.Length > 0 ) {
			string[] msgs = p.dialog.Split('\n');
			messages = new List<string>();
			foreach(string m in msgs) {
				messages.Add ( string.Format(p.format,m) );
			}
		}

		if(p.gimic != null) {
			for(int i = 0; i<p.gimic.Length;++i) {
				_PutGimic(p.gimic[i], i);
			}
		}
	}

	public static QuestEntity CreateEntity(QuestData.Param p, Quest q) {
		QuestEntity qe = ScriptableObject.CreateInstance<QuestEntity>();
		qe.Initialize(p, q);
		return qe;
	}
}
