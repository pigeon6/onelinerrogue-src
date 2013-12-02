using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using iGUI;

public class ItemListWindowController : iGUI.iGUIAction, WindowController {

	[SerializeField]
	private iGUI.iGUIPanel m_panel;

	[SerializeField]
	private iGUI.iGUILabel m_descriptionLabel;

	[SerializeField]
	private iGUI.iGUIImage m_selectionIndicator;

	[SerializeField]
	private GameObject m_itemContainerFab;

	[SerializeField]
	private PlayerActor m_currentPlayer;

	[SerializeField]
	private List<ItemInMenuController> m_inMenuItems;

	[SerializeField]
	private int m_curItemIndex;

	[SerializeField]
	private ItemActionWindowController m_actionWindow;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

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
		if( m_inMenuItems != null && m_inMenuItems.Count > 0 ) {
			m_actionWindow.UpdateItemTarget(m_currentPlayer, m_curItemIndex);
			GUIManager.GetManager().ShowItemActionWindow();
		}
	}

	public void DpadButton(int x, int y) {
		if( m_inMenuItems.Count > 0 ) {

			int newIndex = m_curItemIndex;
			int itemCount = m_inMenuItems.Count;

			if( x != 0 )  {
				int oldIndex = newIndex;
				newIndex += 4;
				newIndex %= 8;
				if( newIndex >= itemCount ) {
					newIndex = oldIndex;
				}
			}
			// key down
			if( y == -1 ) {
				int oldIndex = newIndex;
				newIndex += 1;
				newIndex = newIndex% 4 + ((oldIndex / 4) * 4);
				if( newIndex >= itemCount ) {
					newIndex = 0 + ((oldIndex / 4) * 4);
				}
			}
			// key up
			if( y == 1 ) {
				int oldIndex = newIndex;
				newIndex = (newIndex + 3) % 4;
				newIndex = (newIndex % 4) + ((oldIndex / 4) * 4);
				if( newIndex >= itemCount ) {
					newIndex = (itemCount - 1) % 4 + ((oldIndex / 4) * 4);
				}
			}

			_SelectItem(newIndex);
		}
	}

	public void CancelButton(int value) {
		GUIManager.GetManager().HideTopWindow();
	}

	private void _SelectItem(int index) {
		if( index >= m_inMenuItems.Count || index < 0 ) {
			Debug.LogError ("tried to select too big item:index " + index + " count:" + m_inMenuItems.Count);
		} else {
			GUIManager.GetManager().PlayGUISE(GUISE.Select);

			m_curItemIndex = index;
			ItemEntity e = m_inMenuItems[m_curItemIndex].ItemEntity;
			m_descriptionLabel.label.text = e.Description;
			
			//Debug.Log ("Selecting Item!!");
			if( index == m_curItemIndex ) {
				m_selectionIndicator.setPosition(_GetItemPositionForSelector(m_curItemIndex));
			} else {
				Rect curSepectionPosSize = m_selectionIndicator.positionAndSize;
				m_selectionIndicator.positionTo(new Vector2(curSepectionPosSize.x, curSepectionPosSize.y), 
				                                _GetItemPositionForSelector(m_curItemIndex), 0.1f);
			}
		}
	}

	/*
	 * Action OnEnable of Window
	 */
	public override void act (iGUIElement caller) {
		UserInput uim = UserInput.GetUserInput();

		uim.AddToItemMenuEvent(ItemMenuButton);
		uim.AddToDpadEvent(DpadButton);
		uim.AddToConformCancelEvent(ConfirmButton, CancelButton);

		// get current player
		m_currentPlayer = GameManager.GetManager().currentPlayer;
		// update item view
		_UpdateItemView();
	}

	private void _UpdateItemView() {
		List<ItemEntity> items = m_currentPlayer.items;
		int itemDiff = items.Count - m_inMenuItems.Count;

		// if there are items than current itemcotainer, add some more
		if( itemDiff > 0) {
			for(int i=0; i < itemDiff; ++i) {
				iGUI.iGUIElement e = m_panel.addSmartObject("ItemInMenu");
				ItemInMenuController imc = e.GetComponent<ItemInMenuController>() as ItemInMenuController;
				if(imc == null) Debug.LogError ("imc is null!");
				m_inMenuItems.Add(imc);
			}
		} 
		// if there are more container than items, remove them
		else if( itemDiff < 0 ) {
			for(int i=items.Count; i < m_inMenuItems.Count; ++i) {
				ItemInMenuController c = m_inMenuItems[i];
				m_panel.removeElement(c.gameObject.GetComponent<iGUI.iGUIElement>());
			}
			m_inMenuItems.RemoveRange(items.Count, -itemDiff);
		}

		if( m_curItemIndex >= items.Count ) {
			m_curItemIndex = 0;
		}

		for(int i = 0; i< items.Count; ++i) {
			Vector2 pos = _GetItemPositionForItemContainer(i);
			m_inMenuItems[i].Panel.setPosition(pos);
			//TODO: real equip flag
			m_inMenuItems[i].Initialize(items[i], m_currentPlayer.IsEquippingItem(i));
		}

		m_selectionIndicator.setEnabled( items.Count > 0 );
		if( items.Count == 0 ) {
			m_descriptionLabel.label.text = "アイテムを なにも もっていません。";
		} else {
			_SelectItem(m_curItemIndex);
		}
	}
	
	private Vector2 _GetItemPositionForSelector(int index) {
		return new Vector2( ((int)(index/4)) * 1.0f, ((int)(index%4)) * 0.25f);
	}

	private Vector2 _GetItemPositionForItemContainer(int index) {
		return new Vector2( ((int)(index/4)) * 1.0f, ((int)(index%4)) * 0.25f);
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
