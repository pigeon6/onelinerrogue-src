using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LocalGameSession : MonoBehaviour {

	static private LocalGameSession s_manager;

	// index of QuestDatabase.sheet
	public int currentQuest;
	public List<int> completedQuests;

	public static LocalGameSession GetSession() {
		if( s_manager == null ) {
			LocalGameSession obj = Component.FindObjectOfType(typeof(LocalGameSession)) as LocalGameSession;
			if(obj) {
				s_manager = obj;
			} else {
				GameObject go = new GameObject("__LocalGameSession");
				obj = go.AddComponent<LocalGameSession>() as LocalGameSession;
				DontDestroyOnLoad(go);
				s_manager = obj;
			}
		}
		return s_manager;
	}

	public void MarkQuestComplete(int questId) {
		if( !completedQuests.Contains(questId) ) {
			completedQuests.Add (questId);
		}
	}

	public bool IsQuestCompleted(int questId) {
		return completedQuests.Contains(questId);
	}

	void Awake() {
		currentQuest = 0;
		completedQuests = new List<int>();
		DontDestroyOnLoad(gameObject);
	}
}
