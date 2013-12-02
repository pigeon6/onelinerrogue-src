using UnityEngine;
using System.Collections;

public class LocalGameSession : MonoBehaviour {

	static private LocalGameSession s_manager;

	// index of QuestDatabase.sheet
	public int currentQuest;
	public int completedQuest;

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

	void Awake() {
		currentQuest = 0;
		completedQuest = -1;
		DontDestroyOnLoad(gameObject);
	}
}
