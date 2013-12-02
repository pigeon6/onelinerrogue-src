using UnityEngine;
using System.Collections;

public class StatusGUI : MonoBehaviour {

	[SerializeField]
	iGUI.iGUILabel m_label;

	[SerializeField]
	GameManager m_gm;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		_FormatStatus();
	}

	void _FormatStatus() {

		GameManager gm = GameManager.GetManager();
		if( null != gm ) {
			PlayerActor a = gm.currentPlayer;
			if(null != a) {
				m_label.label.text = System.String.Format("満腹度  {0}/{1}  HP {2}/{3}\n{4}ターン目 {5}メートル進んだ", 
				                                         (int)a.Hunger, 
				                                         (int)a.hungerMax,
				                                         (int)a.hp, 
				                                         (int)a.hpMax,
				                                         (int)EventRecorder.GetManager().TurnOfGame,
				                                         (int)EventRecorder.GetManager().MaxStepsPlayerReached);
			} else {
				m_label.label.text = "";
			}
		}
	}
}
