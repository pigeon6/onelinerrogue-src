using UnityEngine;
using System.Collections;

public class QuestMenuItemController : MonoBehaviour {

	[SerializeField]
	private iGUI.iGUIContainer m_container;

	[SerializeField]
	private iGUI.iGUILabel m_questName;

	[SerializeField]
	private iGUI.iGUIImage m_questIcon;

	[SerializeField]
	private QuestDescription.Param m_qd;

	[SerializeField]
	private string m_episodeFormat;

	public iGUI.iGUIContainer Panel {
		get { return m_container; }
	}

	public QuestDescription.Param Description {
		get { return m_qd; }
	}

	public void Initialize(QuestDescription.Param qd, bool bClearedAlready) {
		m_qd = qd;
		m_questName.label.text = string.Format (m_episodeFormat, qd.episodeNum, qd.questName);

		//		m_item = e;
		//		m_itemName.label.text = m_item.itemName;
		//		m_equipLabel.setEnabled(isEquipped);
		//		m_itemGraphic.image = GUIManager.GetManager().GetIconGraphicOfItem(e);
	}
}
