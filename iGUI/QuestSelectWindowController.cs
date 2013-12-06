using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using iGUI;

public class QuestSelectWindowController : iGUI.iGUIAction, WindowController {

	[SerializeField]
	private iGUI.iGUIPanel m_panel;

	[SerializeField]
	private iGUI.iGUIScrollView m_scrollView;

	[SerializeField]
	private iGUI.iGUIImage m_selectionIndicator;

	[SerializeField]
	private List<QuestMenuItemController> m_menuItems;

	[SerializeField]
	private QuestDescription m_qd;

	[SerializeField]
	private int m_curItemIndex;

	public void ShowWindow() {
		m_panel.setEnabled(true);
	}

	public void HideWindow() {
		m_panel.setEnabled(false);
	}

	public void ItemMenuButton(int value) {
		GUIManager.GetManager().HideTopWindow();
	}
	
	public void ConfirmButton(int value) {
		GUIManager.GetManager().HideTopWindow();
		LocalGameSession.GetSession().currentQuest = m_menuItems[m_curItemIndex].Description.id;
		Application.LoadLevel("Game");
	}

	public void CancelButton(int value) {
		GUIManager.GetManager().HideTopWindow();
		Application.LoadLevel (0);
	}

	public void DpadButton(int x, int y) {

		if( m_menuItems.Count > 0 ) {

			int newIndex = m_curItemIndex;

			// key down
			if( y == -1 ) {
				newIndex += 1;
			}
			// key up
			if( y == 1 ) {
				newIndex -= 1;
			}

			_SelectItem(newIndex);
		}
	}

	public float itemHeight = 60.0f; 

	/*
	 * Action OnEnable of Window
	 */
	public override void act (iGUIElement caller) {

		Debug.Log ("Hello Quest Selectors");

		UserInput uim = UserInput.GetUserInput();

		uim.AddToDpadEvent(DpadButton);
		uim.AddToConformCancelEvent(ConfirmButton, CancelButton);

		m_selectionIndicator.setHeight(itemHeight);

		// update item view
		_UpdateQuestView();

		_SelectItem(LocalGameSession.GetSession().currentQuest);
	}
	
	private void _SelectItem(int index) {
		if( index >= m_menuItems.Count || index < 0 ) {
			return;
		} 
		if( index == m_curItemIndex ) {
			return;
		}
		else {
			GUIManager.GetManager().PlayGUISE(GUISE.Select);
			
			m_curItemIndex = index;
			
			m_selectionIndicator.setPosition(_GetItemPositionForSelector(m_curItemIndex));

			float visibleAreaHeight = m_scrollView.containerRect.height;
			float visibleMin = m_scrollView.scrollPosition.y;
			float visibleMax = visibleMin + visibleAreaHeight;
			int visibleItems = Mathf.FloorToInt(visibleAreaHeight / itemHeight)-1;
			int visibleIndexMin = Mathf.FloorToInt(visibleMin / itemHeight) + 1;
			int visibleIndexMax = Mathf.FloorToInt(visibleMax / itemHeight) - 1;

			if( visibleIndexMin > m_curItemIndex ) {
				m_scrollView.scrollToVertical(itemHeight * m_curItemIndex, 0.1f);
			} else if( visibleIndexMax < m_curItemIndex ) {
				m_scrollView.scrollToVertical(itemHeight * (m_curItemIndex - visibleItems), 0.1f);
			}
		}
	}

	private static bool _TestCondition(string conditions) {

		bool bConditionMatch = true;

		if( conditions != null && conditions.Length > 0 ) {
			string [] codelist = conditions.Split(',');
			
			foreach(string code in codelist) {
				string [] kv = code.Split( ':');
				string key = kv[0].Trim();
				string val = string.Empty;
				
				if( kv.Length > 1 ) {
					val = kv[1].Trim();
				}
				
				if( key == "quest" ) {
					bConditionMatch &= LocalGameSession.GetSession().IsQuestCompleted(System.Int32.Parse (val));
				}								
			}
		}

		return bConditionMatch;
	}
	

	private void _UpdateQuestView() {

		Debug.Log ("Updating Quest View");

		m_scrollView.areaHeight = Mathf.Max (m_scrollView.containerRect.height,
		                                     m_qd.sheets[0].list.Count * itemHeight);
		m_scrollView.refreshStyle();
		int i = 0;
		// if there are items than current itemcotainer, add some more
		foreach(QuestDescription.Param qd in m_qd.sheets[0].list) {
			if( _TestCondition(qd.condition ) ) {
				iGUI.iGUIElement e = m_scrollView.addSmartObject("QuestMenuItem");
				QuestMenuItemController qmic = e.GetComponent<QuestMenuItemController>() as QuestMenuItemController;
				if(qmic == null) Debug.LogError ("qmic is null!");
				// TODO: if quest already cleared
				qmic.Initialize(qd, false);
				
				qmic.Panel.setWidth(0.9f);
				qmic.Panel.setHeight(itemHeight);
				qmic.Panel.setPosition(new Vector2(0.0f, itemHeight * i));
				++i;
				
				m_menuItems.Add(qmic);
			}
		}

		_SelectItem(m_curItemIndex);
	}
	
	private Vector2 _GetItemPositionForSelector(int index) {
		return new Vector2( 0.0f, index * itemHeight);
	}

	public void Close() {
		m_panel.setEnabled(false);
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
