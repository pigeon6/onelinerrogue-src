using UnityEngine;
using System.Collections;
using iGUI;

public class GeneralActionWindowController : iGUI.iGUIAction, WindowController {

	[SerializeField]
	private iGUI.iGUIPanel m_panel;

	[SerializeField]
	private iGUI.iGUIImage m_selectionIndicator;
	
	[SerializeField]
	private iGUI.iGUILabel m_pickup;

	[SerializeField]
	private iGUI.iGUILabel m_stamp;

	[SerializeField]
	private iGUI.iGUILabel m_jump;

	[SerializeField]
	private iGUI.iGUILabel m_rest;

	[SerializeField]
	private int m_curIndex;
	
	[SerializeField]
	private PlayerActor m_currentPlayer;

	private static int kCOMMAND_SIZE = 4;
	
	public void ShowWindow() {
		m_panel.setEnabled(true);
	}
	
	public void HideWindow() {
		m_panel.setEnabled(false);
	}
	
	public void ConfirmButton(int value) {
		Debug.Log ("Action:" + m_curIndex);
		switch(m_curIndex) {
		case 0: // pickup item
			if( m_currentPlayer.CanPickupItemOnGround() ) {
				GUIManager.GetManager().HideTopWindow();	// this window
				m_currentPlayer.DoPickupItemOnGround();
			} else {
				GUIManager.GetManager().Message("じめんには なにも ありません");
			}
			break;
		case 1: // stamp
			if( m_currentPlayer.CanStampAndStill() ) {
				GUIManager.GetManager().HideTopWindow();	// this window
				m_currentPlayer.DoStampAndStill();
			}
			break;
		case 2: // jump
			if( m_currentPlayer.CanJump() ) {
				GUIManager.GetManager().HideTopWindow();	// this window
				m_currentPlayer.DoJump();
			} else {
				GUIManager.GetManager().Message("ジャンプとか 無理っす・・・");
			}
			break;
		case 3: // rest
			if( m_currentPlayer.CanRest() ) {
				GUIManager.GetManager().HideTopWindow();	// this window
				m_currentPlayer.DoRest();
			} 
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

		//Debug.Log ("Selecting Item!!");
		Rect curSepectionPosSize = m_selectionIndicator.positionAndSize;
		m_selectionIndicator.positionTo(new Vector2(curSepectionPosSize.x, curSepectionPosSize.y), 
		                                _GetItemPositionForSelector(m_curIndex), 0.1f);
	}
	
	public override void act (iGUIElement caller) {
		m_curIndex = 0;
		m_selectionIndicator.setPosition(_GetItemPositionForSelector(m_curIndex));

		m_currentPlayer = GameManager.GetManager().currentPlayer;

		m_pickup.passive 	= !m_currentPlayer.CanPickupItemOnGround();
		m_stamp.passive 	= !m_currentPlayer.CanStampAndStill();
		m_jump.passive 		= !m_currentPlayer.CanJump();
		m_rest.passive 	 	= !m_currentPlayer.CanRest();

		UserInput uim = UserInput.GetUserInput();		
		uim.AddToDpadEvent(DpadButton);
		uim.AddToConformCancelEvent(ConfirmButton, CancelButton);		
		uim.AddToActionMenuEvent(CancelButton);
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
