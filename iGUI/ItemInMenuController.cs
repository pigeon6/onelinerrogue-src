using UnityEngine;
using System.Collections;

public class ItemInMenuController : MonoBehaviour {

	[SerializeField]
	private iGUI.iGUIContainer m_container;

	[SerializeField]
	private iGUI.iGUILabel m_itemName;

	[SerializeField]
 	private iGUI.iGUILabel m_equipLabel;

	[SerializeField]
	private iGUI.iGUIImage m_itemGraphic;

	[SerializeField]
	private ItemEntity m_item;

	public iGUI.iGUIContainer Panel {
		get { return m_container; }
	}

	public ItemEntity ItemEntity {
		get { return m_item; }
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Initialize(ItemEntity e, bool isEquipped) {
		m_item = e;
		m_itemName.label.text = m_item.itemName;
		m_equipLabel.setEnabled(isEquipped);
		m_itemGraphic.image = GUIManager.GetManager().GetIconGraphicOfItem(e);
	}
}
