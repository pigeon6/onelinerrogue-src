using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using iGUI;

public class TeropWindowController : iGUI.iGUIAction, WindowController {
	
	[SerializeField]
	private iGUI.iGUIContainer m_panel;
	
	[SerializeField]
	private iGUI.iGUILabel m_teropMessage;

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
	
	public void DoTerop(string msg, InputActionHandler endTeropCallback) {
		if(!m_isShowing) {
			m_endTeropCallback = endTeropCallback;
			StartCoroutine(_DoTerop(msg));
		}
	}
	
	private IEnumerator _DoTerop(string msg) {
		
		m_isShowing = true;
		
		Color c = Color.white;

		m_teropMessage.label.text = msg;
		m_teropMessage.setLabelColor(c);
		ShowWindow();

		yield return new WaitForSeconds(m_teropShowSec);

		float tNow = Time.time;
		while( Time.time - tNow <= m_teropfedeinoutSec ) {
			float rate = (Time.time - tNow) / m_teropfedeinoutSec;

			c.a = 1.0f - rate;

			m_teropMessage.setLabelColor(c);

			yield return new WaitForEndOfFrame();
		}

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
