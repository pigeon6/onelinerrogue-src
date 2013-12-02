using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MessageGUI : MonoBehaviour {

	private const int kMSG_LINES = 6;

	[SerializeField]
	private iGUI.iGUILabel m_label;

	private List<string> m_messages;
	
	public void AppendMessage(string msg) {
		m_messages.Add(msg);
		_UpdateMessage();
	}

	private void _UpdateMessage() {
		string s = "";
		for(int i = Mathf.Max (0, m_messages.Count - kMSG_LINES); i < m_messages.Count; ++i) {
			s += m_messages[i] + "\n";
		}
		/*
		 * display s
		 */
		m_label.label.text = s;
	}

	// Use this for initialization
	void Start () {
		m_messages = new List<string>();
		m_label.label.text = "";
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
