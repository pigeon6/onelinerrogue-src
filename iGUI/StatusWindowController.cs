using UnityEngine;
using System.Collections;
using iGUI;

public class StatusWindowController : iGUI.iGUIAction, WindowController {

	[SerializeField]
	private iGUI.iGUIPanel m_panel;

	[SerializeField]
	private iGUI.iGUIImage m_chara;

	[SerializeField]
	private iGUI.iGUILabel m_charaInfoStatusLabel;
	[SerializeField]
	private iGUI.iGUILabel m_leftStatusLabel;
	[SerializeField]
	private iGUI.iGUILabel m_rightStatusLabel;

	[SerializeField]
	private PlayerActor m_currentPlayer;

	private string nameFormat = @"なまえ：{0}
レベル：{1}
経験値：{2}
次のレベルまで：{3}
{4}";
	private string leftStatusFormat = @"
ＨＰ：　{0, 3} / {1, 3}
満腹度：{2, 3} / {3, 3}
　　　　物/炎/氷/雷/毒/闇/光/心
攻撃力：	{4:00}/{5:00}/{6:00}/{7:00}/{8:00}/{9:00}/{10:00}/{11:00}
防御力：	{12:00}/{13:00}/{14:00}/{15:00}/{16:00}/{17:00}/{18:00}/{19:00}
すばやさ：{20, 3}
武器	：{21}
防具	：{22}";

	public void ShowWindow() {
		m_panel.setEnabled(true);
	}

	public void HideWindow() {
		m_panel.setEnabled(false);
	}

	public void UpdateStatus(PlayerActor currentPlayer) {
//		m_useLabel.label.text = (m_currentPlayer.CanEatItem(currentIndex) ? "たべる" : "つかう");
//		m_equipLabel.label.text = (m_currentPlayer.IsEquippingItem(currentIndex) ? "はずす" : "みにつける");

		string statusStr = string.Empty;
		// TODO:
//		if(currentPlayer.IsPoison) {
//			statusStr += "どく "
//		}

		string nameText = string.Format (nameFormat,	// 5 items
		                                 currentPlayer.charName,
		                                 (int)currentPlayer.level, // level
		                                 (int)currentPlayer.exp, // exp
		                                 (int)(currentPlayer.nextExp - currentPlayer.exp), // nextexp
		                                 statusStr 		//status
		                                 );
		string statusText = string.Format (leftStatusFormat,	// 23 items
		                                 currentPlayer.hp,
		                                 currentPlayer.hpMax,
		                                 currentPlayer.Hunger,
		                                 currentPlayer.hungerMax,
		                                   currentPlayer.AP (ElementType.ET_Physical ),
		                                   currentPlayer.AP (ElementType.ET_Fire	),
		                                   currentPlayer.AP (ElementType.ET_Ice		),
		                                   currentPlayer.AP (ElementType.ET_Electric),
		                                   currentPlayer.AP (ElementType.ET_Poison	),
		                                   currentPlayer.AP (ElementType.ET_Dark	),
		                                   currentPlayer.AP (ElementType.ET_Holy	),
		                                   currentPlayer.AP (ElementType.ET_Psycho	),
		                                   currentPlayer.DP (ElementType.ET_Physical ),
		                                   currentPlayer.DP (ElementType.ET_Fire	),
		                                   currentPlayer.DP (ElementType.ET_Ice		),
		                                   currentPlayer.DP (ElementType.ET_Electric),
		                                   currentPlayer.DP (ElementType.ET_Poison	),
		                                   currentPlayer.DP (ElementType.ET_Dark	),
		                                   currentPlayer.DP (ElementType.ET_Holy	),
		                                   currentPlayer.DP (ElementType.ET_Psycho	),
		                                 currentPlayer.agility,
		                                 (currentPlayer.armedWeapon == null ? "なし" : currentPlayer.armedWeapon.itemName),
		                                 (currentPlayer.armedShield == null ? "なし" : currentPlayer.armedShield.itemName)
		                                 );

		string itemlistText = "";
		foreach(ItemEntity e in currentPlayer.items) {
			itemlistText += e.itemName;
			itemlistText += "\n";
		}

		m_charaInfoStatusLabel.label.text = nameText;
		m_leftStatusLabel.label.text = statusText;
		m_rightStatusLabel.label.text = itemlistText;
	}

	private void _Dismiss(int value) {
		GUIManager.GetManager().HideTopWindow();
	}
	
	public override void act (iGUIElement caller) {
		UserInput uim = UserInput.GetUserInput();		
		uim.AddToStatusMenuEvent(_Dismiss);

		UpdateStatus( GameManager.GetManager().currentPlayer );
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
