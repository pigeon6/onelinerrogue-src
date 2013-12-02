using UnityEngine;
using System.Collections;
using iGUI;

public class PressButtonWaitController : iGUI.iGUIAction, WindowController {
	
	[SerializeField]
	private iGUI.iGUIImage m_indicator;

	[SerializeField]
	private float m_updateFrameSec = 0.1f; 

	[SerializeField]
	private int m_imgIndex = 0; 

	[SerializeField]
	private InputActionHandler m_delegateConfirmAction; 
	
	[SerializeField]
	private Texture2D[] m_imgs; 

	[SerializeField]
	private bool isInditacotrAnimationRunning; 


	// Use this for initialization
	private IEnumerator _UpdateIcon () {
		isInditacotrAnimationRunning = true;
		while(m_indicator.enabled) {
			m_imgIndex = (m_imgIndex + 1) % m_imgs.Length;
			m_indicator.image = m_imgs[m_imgIndex];
			yield return new WaitForSeconds(m_updateFrameSec);
		}
		isInditacotrAnimationRunning = false;
	}

	public void SetDelegateConfirmAction(InputActionHandler h) {
		m_delegateConfirmAction = h;
	}


	public void ShowWindow() {
		m_indicator.setEnabled(true);
		if(!isInditacotrAnimationRunning) {
			StartCoroutine(_UpdateIcon ());
		}
	}
	
	public void HideWindow() {
		m_delegateConfirmAction = null;
		m_indicator.setEnabled(false);
	}
	
	public void ConfirmButton(int value) {
		GUIManager.GetManager().HideTopWindow();
	}
	
	public override void act (iGUIElement caller) {
		UserInput uim = UserInput.GetUserInput();		
		uim.AddToConformCancelEvent(ConfirmButton, null);
		uim.AddToConformCancelEvent(m_delegateConfirmAction,null);
	}
	
	public void Close() {
		HideWindow();
	}
	public bool IsOpen() {
		return m_indicator.enabled;
	}
	public void FocusWindow() {
		m_indicator.passive = false;
	}
	public void UnfocusWindow() {
		m_indicator.passive = true;
	}
}
