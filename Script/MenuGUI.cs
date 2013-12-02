using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MenuGUI : MonoBehaviour {

	[SerializeField]
	ItemListWindowController m_itemListWindow;

	[SerializeField]
	ItemActionWindowController m_itemActionWindow;

	[SerializeField]
	GeneralActionWindowController m_generalActionWindow;

	[SerializeField]
	StatusWindowController m_statusWindow;

	[SerializeField]
	private CutSceneWindowController m_cutScene; 

	[SerializeField]
	private TeropWindowController m_terop; 

	[SerializeField]
	private PressButtonWaitController m_pbControl; 
	
	Stack<WindowController> m_windowStack;

	void Awake() {
		m_windowStack = new Stack<WindowController>();
	}

	void Start() {
	}

	public void HideTopWindow() {
		if( m_windowStack.Count > 0 ) {
			WindowController wOnTop = m_windowStack.Pop();
			wOnTop.Close();

//			foreach(WindowController w in m_windowStack) {
				// TODO: show again
//			}

			if( m_windowStack.Count > 0 ) {
				wOnTop = m_windowStack.Peek();
				wOnTop.FocusWindow();
			}
			UserInput.GetUserInput().PopActionEventStack();
		}
	}

	public bool IsAnyMenuWindowOpen() {
		return m_windowStack.Count > 0;
	}

	public void ShowItemListWindow() {
		// push stack to prevent getting multiple show window event
		UserInput.GetUserInput().PushActionEventStack();
		m_itemListWindow.ShowWindow();

		if( m_windowStack.Count > 0 ) {
			WindowController w = m_windowStack.Peek();
			w.UnfocusWindow();
		}
		m_windowStack.Push(m_itemListWindow);
	}

	public void ShowGeneralActionWindow() {
		// push stack to prevent getting multiple show window event
		UserInput.GetUserInput().PushActionEventStack();
		m_generalActionWindow.ShowWindow();

		if( m_windowStack.Count > 0 ) {
			WindowController w = m_windowStack.Peek();
			w.UnfocusWindow();
		}
		m_windowStack.Push(m_generalActionWindow);
	}

	public void ShowItemActionWindow() {
		// push stack to prevent getting multiple show window event
		UserInput.GetUserInput().PushActionEventStack();
		m_itemActionWindow.ShowWindow();

		if( m_windowStack.Count > 0 ) {
			WindowController w = m_windowStack.Peek();
			w.UnfocusWindow();
		}

		m_windowStack.Push(m_itemActionWindow);
	}

	public void ShowStatusWindow() {
		// push stack to prevent getting multiple show window event
		UserInput.GetUserInput().PushActionEventStack();
		m_statusWindow.ShowWindow();
		
		if( m_windowStack.Count > 0 ) {
			WindowController w = m_windowStack.Peek();
			w.UnfocusWindow();
		}
		
		m_windowStack.Push(m_statusWindow);
	}

	public void ShowIndicatorAndWait(InputActionHandler h = null) {
		// push stack to prevent getting multiple show window event
		if( !m_pbControl.IsOpen() ) {
			m_pbControl.SetDelegateConfirmAction(h);
			UserInput.GetUserInput().PushActionEventStack();
			m_pbControl.ShowWindow();
			m_windowStack.Push(m_pbControl);
		}
	}

	public void DoTerop(string msg, InputActionHandler h) {
		m_terop.DoTerop(msg, h);
	}


	public void DoCutsceneWithMessages(List<string> msgs, InputActionHandler h) {
		if(m_cutScene.IsOpen() ) {
			m_cutScene.DoCutsceneWithMessages(msgs, h);
		} else {
			// push stack to prevent getting multiple show window event
			UserInput.GetUserInput().PushActionEventStack();
			m_cutScene.ShowWindow();
			
//			foreach(WindowController w in m_windowStack) {
				// TODO: hide
//			}			
			m_windowStack.Push(m_cutScene);			
			m_cutScene.DoCutsceneWithMessages(msgs, h);
		}
	}
}
