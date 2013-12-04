using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using iGUI;

public class QuestClearWindowController : iGUI.iGUIAction, WindowController {
	
	[SerializeField]
	private iGUI.iGUIContainer m_panel;
	
	[SerializeField]
	private iGUI.iGUILabel m_clearMessage;

	[SerializeField]
	private iGUI.iGUILabel m_questMessage;

	[SerializeField]
	private string m_questMessageFormat = "クエスト「{0}」";

	[SerializeField]
	private float m_clearFontInitialSize = 400.0f;

	[SerializeField]
	private float m_bgmCrossFadeSec = 1.0f;

	[SerializeField]
	private float m_teropfedeinoutSec = 1.0f;
	
	[SerializeField]
	private float m_teropShowSec = 1.0f;
	
	[SerializeField]
	private bool m_isShowing;
	
	[SerializeField]
	private InputActionHandler m_endTeropCallback;
	
	public void ShowWindow() {
		m_panel.setEnabled(true);
	}
	
	public void HideWindow() {
		m_panel.setEnabled(false);
	}
	
	public void DoQuestClear(string questName, InputActionHandler endTeropCallback) {
		if(!m_isShowing) {
			m_endTeropCallback = endTeropCallback;
			StartCoroutine(_DoQuestClear(questName));
		}
	}
	
	private static float _easeOut(float v) {
		float sv = 1.0f - Mathf.Sin (Mathf.PI/2.0f * v);
		return 1.0f - (sv*sv);
	}

	private IEnumerator _DoQuestClear(string questName) {
		
		m_isShowing = true;
		
		Color c = Color.white;
		c.a = 0.0f;

		float defaultSize = m_clearMessage.style.fontSize;

		m_questMessage.label.text = string.Format (m_questMessageFormat, questName);
		m_questMessage.setLabelColor(c);
		m_clearMessage.setLabelColor(c);
		m_clearMessage.style.fontSize = (int)m_clearFontInitialSize;
		ShowWindow();

		// TODO: start music
		SoundManager.GetManager().FadeoutBGM(m_bgmCrossFadeSec);

		float tNow = Time.time;
		while( Time.time - tNow <= m_teropShowSec ) {
			float rate = (Time.time - tNow) / m_teropShowSec;
			
			c.a = rate;
			m_questMessage.setLabelColor(c);
			
			yield return new WaitForEndOfFrame();
		}
		yield return new WaitForSeconds(m_teropShowSec);

		GUIManager.GetManager().PlayGUISE(GUISE.QuestClear);

		c.a = 0.0f;
		tNow = Time.time;
		while( Time.time - tNow <= m_teropfedeinoutSec ) {
			float rate = (Time.time - tNow) / m_teropfedeinoutSec;
			
			c.a = rate;
			m_clearMessage.style.fontSize = (int)(Mathf.Lerp (m_clearFontInitialSize, defaultSize, _easeOut(rate)));
			m_clearMessage.setLabelColor(c);
			
			yield return new WaitForEndOfFrame();
		}
		// TODO: SE! dede-nn!
		yield return new WaitForSeconds(m_teropShowSec * 1.5f);
		tNow = Time.time;
		while( Time.time - tNow <= m_teropShowSec ) {
			float rate = (Time.time - tNow) / m_teropShowSec;

		
			c.a = 1.0f - rate;
			m_questMessage.setLabelColor(c);
			m_clearMessage.setLabelColor(c);

			yield return new WaitForEndOfFrame();
		}

		// change BGM
		//SoundManager.GetManager().FadeoutBGM(m_bgmCrossFadeSec);

		HideWindow();
		m_isShowing = false;
		m_endTeropCallback(0);
	}
	
	public override void act (iGUIElement caller) {
	}
	
	public void Close() {
		HideWindow();
	}
	public bool IsOpen() {
		return m_panel.enabled;
	}
	public void FocusWindow() {
		m_panel.passive = false;
	}
	public void UnfocusWindow() {
		m_panel.passive = true;
	}
}
