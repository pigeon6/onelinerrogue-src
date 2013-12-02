using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using iGUI;

public class CutSceneWindowController : iGUI.iGUIAction, WindowController {

	[SerializeField]
	private iGUI.iGUIContainer m_panel;
	
	[SerializeField]
	private iGUI.iGUIImage m_blackAbove;
	[SerializeField]
	private iGUI.iGUIImage m_blackBelow;

	[SerializeField]
	private iGUI.iGUILabel m_cutMessage;
	[SerializeField]
	private iGUI.iGUIPanel m_gameMessagePanel;
	[SerializeField]
	private iGUI.iGUILabel m_gameStatusLabel;
	
	[SerializeField]
	private float m_cutfedeinoutSec = 1.0f;

	[SerializeField]
	private float m_fadeinToMsgIntervalSec = 1.0f;

	[SerializeField]
	private float m_msgSpeedIntervalSec = .1f;

	[SerializeField]
	private StringBuilder m_strb;

	[SerializeField]
	private Queue<string> m_msgQueue;

	[SerializeField]
	private bool m_isShowing;

	[SerializeField]
	private bool m_acceptButton;

	[SerializeField]
	private InputActionHandler m_endCutSceneConfirmAction;

	[SerializeField]
	private int m_lineCharLength = 15;

	public void ShowWindow() {
		m_panel.setEnabled(true);
	}

	public void HideWindow() {
		m_panel.setEnabled(false);
	}

	public void DoCutsceneWithMessages(List<string> msgs, InputActionHandler endCutSceneConfirmAction) {
		if(!m_isShowing) {
			m_endCutSceneConfirmAction = endCutSceneConfirmAction;
			StartCoroutine(_ShowWindow(msgs));
		} else {
			foreach(string m in msgs) {
				m_msgQueue.Enqueue(m);
			}
		}
	}

	private IEnumerator _ShowWindow(List<string> msgs) {

		m_isShowing = true;

		if( m_msgQueue == null ) {
			m_msgQueue = new Queue<string>();
		} else {
			m_msgQueue.Clear();
		}
		foreach(string m in msgs) {
			m_msgQueue.Enqueue(m);
		}

		m_gameMessagePanel.setEnabled(false);
		m_gameStatusLabel.setEnabled(false);

		Color c = Color.black;
		c.a = 0.0f;
		m_blackAbove.setBackgroundColor(c);
		m_blackBelow.setBackgroundColor(c);

		Rect aboveRect = m_blackAbove.positionAndSize;
		Rect belowRect = m_blackBelow.positionAndSize;

		m_blackAbove.setPositionAndSize(new Rect(aboveRect.x, aboveRect.y, aboveRect.width, 0.0f));
        m_blackAbove.setPositionAndSize(new Rect(belowRect.x, belowRect.y, belowRect.width, 0.0f));

		m_cutMessage.label.text = string.Empty;
		m_cutMessage.setEnabled(false);

		m_panel.setEnabled(true);

		float tNow = Time.time;
		while( Time.time - tNow <= m_cutfedeinoutSec ) {
			float rate = (Time.time - tNow) / m_cutfedeinoutSec;

			float v = _easeOut(rate);

			float aboveH = Mathf.Lerp (0.0f, aboveRect.height, v);
			float belowH = Mathf.Lerp (0.0f, belowRect.height, v);

			c.a = rate;
			Rect rA = new Rect(aboveRect.x, aboveRect.y, aboveRect.width, aboveH);
			Rect rB = new Rect(belowRect.x, belowRect.y, belowRect.width, belowH);

			m_blackAbove.setBackgroundColor(c);
			m_blackBelow.setBackgroundColor(c);
			m_blackAbove.setPositionAndSize(rA);
			m_blackBelow.setPositionAndSize(rB);

			yield return new WaitForEndOfFrame();
		}
		m_blackAbove.setBackgroundColor(Color.black);
		m_blackBelow.setBackgroundColor(Color.black);
		m_blackAbove.setPositionAndSize(aboveRect);
		m_blackBelow.setPositionAndSize(belowRect);

		yield return new WaitForSeconds(m_fadeinToMsgIntervalSec);

		StartCoroutine(_DisplayNextMessage());
	}

	private static float _easeOut(float v) {
		float sv = 1.0f - Mathf.Sin (Mathf.PI/2.0f * v);
		return 1.0f - (sv*sv);
	}
	
	private IEnumerator _HideWindow() {

		m_cutMessage.label.text = string.Empty;
		m_cutMessage.setEnabled(false);

		Rect aboveRect = m_blackAbove.positionAndSize;
		Rect belowRect = m_blackBelow.positionAndSize;
		Color c = m_blackAbove.backgroundColor;

		float tNow = Time.time;
		while( Time.time - tNow <= m_cutfedeinoutSec ) {
			float rate = (Time.time - tNow) / m_cutfedeinoutSec;

			float v = _easeOut(rate);
			float aboveH = Mathf.Lerp (aboveRect.height, 0.0f, v);
			float belowH = Mathf.Lerp (belowRect.height, 0.0f, v);
			
			c.a = 1.0f - rate;
			Rect rA = new Rect(aboveRect.x, aboveRect.y, aboveRect.width, aboveH);
			Rect rB = new Rect(belowRect.x, belowRect.y, belowRect.width, belowH);
			
			m_blackAbove.setBackgroundColor(c);
			m_blackBelow.setBackgroundColor(c);
			m_blackAbove.setPositionAndSize(rA);
			m_blackBelow.setPositionAndSize(rB);
			
			yield return new WaitForEndOfFrame();
		}

		m_panel.setEnabled(false);
		m_blackAbove.setBackgroundColor(Color.black);
		m_blackBelow.setBackgroundColor(Color.black);
		m_blackAbove.setPositionAndSize(aboveRect);
		m_blackBelow.setPositionAndSize(belowRect);

		m_gameMessagePanel.setEnabled(true);
		m_gameStatusLabel.setEnabled(true);
		m_isShowing = false;

		GUIManager.GetManager().HideTopWindow();

		m_endCutSceneConfirmAction(0);
	}

	private IEnumerator _DisplayNextMessage() {
		m_acceptButton = false;
		StringBuilder sb = new StringBuilder();

		m_cutMessage.label.text = string.Empty;
		m_cutMessage.setEnabled(true);
		yield return new WaitForSeconds(m_msgSpeedIntervalSec);

		string msg = m_msgQueue.Dequeue();
		for( int i = 0; i< msg.Length; ++i ) {
			sb.Append(msg[i]);
			if(i != 0 && i%m_lineCharLength == 0) {
				sb.Append("\n");
			}
			m_cutMessage.label.text = sb.ToString();
			yield return new WaitForSeconds(m_msgSpeedIntervalSec);
		}

//		GUIManager.GetManager().ShowIndicatorAndWait();
		m_acceptButton = true;
	}
	
	public void ConfirmButton(int value) {
		if( m_acceptButton ) {
			m_acceptButton = false;
			if( m_msgQueue.Count > 0 ) {
				StartCoroutine(_DisplayNextMessage());
			} else {
				StartCoroutine(_HideWindow());
			}
		}
	}
	
	public void CancelButton(int value) {
		ConfirmButton(value);
	}
	
	public override void act (iGUIElement caller) {
		UserInput uim = UserInput.GetUserInput();
		uim.AddToConformCancelEvent(ConfirmButton, CancelButton);		
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
