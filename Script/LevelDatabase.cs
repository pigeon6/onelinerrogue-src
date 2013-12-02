using UnityEngine;
using System.Collections;

public class LevelDatabase : MonoBehaviour {

	[SerializeField]
	private ItemLevelData m_itemLevel;

	[SerializeField]
	private PlayerLevelData m_playerLevel;

	public bool TestLevelUp(Actor a) {
		if(a.level < m_playerLevel.list.Count) {
			Debug.Log ("[Level] next:" + a.exp + "/" + m_playerLevel.list[a.level].exp);
			return m_playerLevel.list[a.level].exp <= a.exp;
		} else {
			// max level
			return false;
		}
	}

	// @result true if more levels can be applied
	public bool ApplyLevelUp(Actor a) {

		int nextLevel = a.level + 1;
		if( nextLevel >= m_playerLevel.list.Count ) {
			return false; // max level
		}

		PlayerLevelData.Param pld = m_playerLevel.list[nextLevel -1];

		a.level = nextLevel;
		a.hpMax 	+= (int)pld.hpMax;
		a.hungerMax += (int)pld.hungerMax;
		a.agility 	+= (int)pld.agility;

		for(int i = 0; i < a.apMax.Length; ++i) {
			a.apMax[i] 	+= (int)pld.ApMax[i];
			a.defMax[i] += (int)pld.DpMax[i];
		}

		a.hp 	 = a.hpMax;
		a.hunger = a.hungerMax;
		for(int i = 0; i < a.apMax.Length; ++i) {
			a.ap[i]  = a.apMax[i];
			a.def[i] = a.defMax[i];
		}


		int comingLevel = a.level + 1;
		if( comingLevel < m_playerLevel.list.Count ) {
			a.nextExp   = m_playerLevel.list[a.level].exp;
			return (a.nextExp - a.exp) <= 0.0f;
		} else {
			a.nextExp   = 0.0f;
		}
		return false;
	}
	
	public void ApplyLevelUp(ItemEntity e, ElementType t) {
	}
}
