using UnityEngine;
using System.Collections;
using iGUI;

public class ItemActionWindowController : iGUI.iGUIAction, WindowController {

	[SerializeField]
	private iGUI.iGUIPanel m_panel;

	[SerializeField]
	private iGUI.iGUIImage m_selectionIndicator;

	[SerializeField]
	private iGUI.iGUILabel m_useLabel;
	[SerializeField]
	private iGUI.iGUILabel m_throwLabel;
	[SerializeField]
	private iGUI.iGUILabel m_equipLabel;
	[SerializeField]
	private iGUI.iGUILabel m_putLabel;

	[SerializeField]
	private int m_curIndex;

	[SerializeField]
	private int m_currentItemIndex;

	[SerializeField]
	private PlayerActor m_currentPlayer;

	private static int kCOMMAND_SIZE = 4;

	public void ShowWindow() {
		m_panel.setEnabled(true);
	}

	public void HideWindow() {
		m_panel.setEnabled(false);
	}

	public void UpdateItemTarget(PlayerActor currentPlayer, int currentIndex) {
		m_currentPlayer = currentPlayer;
		m_currentItemIndex = currentIndex;

		m_putLabel.passive 	 = !m_currentPlayer.CanPutItemOnGround(currentIndex);
		m_throwLabel.passive = !m_currentPlayer.CanThrowItem(currentIndex);
		m_equipLabel.passive = !m_currentPlayer.CanEquipItem(currentIndex);
		//m_useLabel.passive 	 = !m_currentPlayer.CanUseItem(currentIndex);

		string uselabel = null;
		if( m_currentPlayer.CanEquipItem(currentIndex) ) {
			uselabel = m_currentPlayer.IsEquippingItem(currentIndex) ? "はずす" : "そうび";
			m_useLabel.passive = false;
		}
		else if(m_currentPlayer.CanEatItem(currentIndex)) {
			uselabel = "たべる";
			m_useLabel.passive = false;
		}
		else {
			uselabel = "つかう";
			m_useLabel.passive = !m_currentPlayer.CanUseItem(currentIndex);
		}

		m_useLabel.label.text = uselabel;

		// TODO:
		m_equipLabel.label.text = "";
		m_equipLabel.passive = true;
	}

	public void ConfirmButton(int value) {
		Debug.Log ("Action:" + m_curIndex);
		switch(m_curIndex) {
		case 0: // use
			if( m_currentPlayer.CanEquipItem(m_currentItemIndex) ) {
				GUIManager.GetManager().PlayGUISE(GUISE.ButtonConfirm);
				GUIManager.GetManager().HideTopWindow();	// this window
				GUIManager.GetManager().HideTopWindow();	// itemlist window
				m_currentPlayer.DoEquipItem(m_currentItemIndex);
			}
			else if( m_currentPlayer.CanUseItem(m_currentItemIndex) ) {
				GUIManager.GetManager().PlayGUISE(GUISE.ButtonConfirm);
				GUIManager.GetManager().HideTopWindow();	// this window
				GUIManager.GetManager().HideTopWindow();	// itemlist window
				m_currentPlayer.DoUseItem(m_currentItemIndex);
			}
			else {
				GUIManager.GetManager().PlayGUISE(GUISE.ButtonNotAvailable);
				GUIManager.GetManager().Message("このアイテムは つかえません");
			}
			break;
		case 1: // throw
			if( m_currentPlayer.CanThrowItem(m_currentItemIndex) ) {
				GUIManager.GetManager().PlayGUISE(GUISE.ButtonConfirm);
				GUIManager.GetManager().HideTopWindow();	// this window
				GUIManager.GetManager().HideTopWindow();	// itemlist window
				m_currentPlayer.DoThrowItem(m_currentItemIndex);
			}
			break;
		case 2: // put
			if( m_currentPlayer.CanPutItemOnGround(m_currentItemIndex) ) {
				GUIManager.GetManager().PlayGUISE(GUISE.ButtonConfirm);
				GUIManager.GetManager().HideTopWindow();	// this window
				GUIManager.GetManager().HideTopWindow();	// itemlist window
				m_currentPlayer.DoPutItemOnGround(m_currentItemIndex);
			} else {
				GUIManager.GetManager().PlayGUISE(GUISE.ButtonNotAvailable);
				GUIManager.GetManager().Message("じめんに なにか ものがあります");
			}
			break;
		case 3: // equip
//			if( m_currentPlayer.CanEquipItem(m_currentItemIndex) ) {
//				GUIManager.GetManager().PlayGUISE(GUISE.ButtonConfirm);
//				GUIManager.GetManager().HideTopWindow();	// this window
//				GUIManager.GetManager().HideTopWindow();	// itemlist window
//				m_currentPlayer.DoEquipItem(m_currentItemIndex);
//			} else {
//				GUIManager.GetManager().PlayGUISE(GUISE.ButtonNotAvailable);
//				GUIManager.GetManager().Message("このアイテムは そうび できません");
//			}
			break;
		}
	}
	
	public void CancelButton(int value) {
		GUIManager.GetManager().HideTopWindow();
	}

	public void DpadButton(int x, int y) {
		
		m_curIndex = (m_curIndex - y) % kCOMMAND_SIZE;
		if( m_curIndex < 0 ) {
			m_curIndex = kCOMMAND_SIZE - 1;
		}
		GUIManager.GetManager().PlayGUISE(GUISE.Select);

		//Debug.Log ("Selecting Item!!");
		Rect curSepectionPosSize = m_selectionIndicator.positionAndSize;
		m_selectionIndicator.positionTo(new Vector2(curSepectionPosSize.x, curSepectionPosSize.y), 
		                                _GetItemPositionForSelector(m_curIndex), 0.1f);
	}
	
	public override void act (iGUIElement caller) {
		m_curIndex = 0;
		m_selectionIndicator.setPosition(_GetItemPositionForSelector(m_curIndex));

		UserInput uim = UserInput.GetUserInput();
		
		uim.AddToDpadEvent(DpadButton);
		uim.AddToConformCancelEvent(ConfirmButton, CancelButton);		
	}
	
	private Vector2 _GetItemPositionForSelector(int index) {
		if( index < 0 ) {
			index = kCOMMAND_SIZE - 1;
		}
		return new Vector2( 0.5f, ((int)(index%4)) * 0.33f);
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
