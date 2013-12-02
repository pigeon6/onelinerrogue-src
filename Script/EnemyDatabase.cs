using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyDatabase : MonoBehaviour {

	enum RarityType {
		Constant,
		LinearDecrease,
		LinearIncrease,
		ExpoDecrease,
		ExpoIncrease
	}

	[SerializeField]
	private EnemyData m_data;

	[SerializeField]
	private QuestRarityData m_rarityData;

	[SerializeField]
	private tk2dSpriteAnimation[] m_enemyGraphics;

	/*
	 * get enemy graphics animation clip by name
	 */ 
	public tk2dSpriteAnimation GetAnimationFromName(string name) {
		foreach(tk2dSpriteAnimation anim in m_enemyGraphics) {
			if(anim.name.Equals(name)) {
				return anim;
			}
		}
		return m_enemyGraphics[0];
	}

	/*
	 * create enemy by enemyId
	 */
	public EnemyData.Param GenerateEnemyDirect(int enemyId) {
		foreach(EnemyData.Param p in m_data.sheets[0].list) {
			if(p.id == enemyId) {
				return p;
			}
		}
		Debug.LogError("[FATAL]GenerateEnemyDirect() couldn't found generating item. id="+enemyId);
		return null;
	}

	public int GetMinimumEnemyStep(string questName) {

		List<QuestRarityData.Param> rl = _GetRarityList(questName);

		int minStep = int.MaxValue;
		
		foreach(QuestRarityData.Param p in rl) {
			if( p.kind != "enemy" ) {
				continue;
			}
			minStep = Mathf.Min (minStep, p.minStep);
		}

		return minStep;
	}

	/*
	 * select next enemy entry from given floor index
	 */
	public EnemyData.Param GenerateNextEnemy(string questName, int currentStep) {

		List<QuestRarityData.Param> rl = _GetRarityList(questName);

		int acc = _AccumrateRarity(rl, currentStep);
		int val = Random.Range(1, acc);

		foreach(QuestRarityData.Param p in rl) {
			if( p.kind != "enemy" ) {
				continue;
			}
			val -= _CalcurateRarity(currentStep, p);
			if(val < 0) {
				return GenerateEnemyDirect( p.id );
			}
		}
		Debug.LogError("[FATAL/LOGIC ERROR]GenerateNextEnemy() couldn't found generating item. should not happen:" + currentStep);
		return new EnemyData.Param();
	}

	private int _AccumrateRarity(List<QuestRarityData.Param> rl, int currentStep) {
		int acc = 0;
		foreach(QuestRarityData.Param p in rl) {
			if( p.kind != "enemy" ) {
				continue;
			}
			acc += _CalcurateRarity(currentStep, p);
		}
		return acc;
	}

	/*
	 * rarity calcuration
	 */
	private int _CalcurateRarity(int currentStep, QuestRarityData.Param p) {

		if( currentStep < p.minStep || currentStep > p.maxStep ) {
			return 0;
		}

		float floorRate = (float)(currentStep - p.minStep) / (float)(p.maxStep - p.minStep);

		switch( (RarityType)p.rarityType ) {
		case RarityType.LinearDecrease:
			return (int)Mathf.Lerp ((float)p.rarity, p.rarity * 0.1f, floorRate);
		case RarityType.LinearIncrease:
			return (int)Mathf.Lerp ((float)p.rarity * 0.1f, p.rarity, floorRate);
		case RarityType.ExpoDecrease:
			return (int)( (float)p.rarity * Mathf.Cos ( (Mathf.PI/2.0f) * floorRate ) );
		case RarityType.ExpoIncrease:
			return (int)( (float)p.rarity * Mathf.Sin ( (Mathf.PI/2.0f) * floorRate ) );
//		case RarityType.Constant:
//			break;
		}
		// otherwise treat as constant
		return p.rarity;
	}

	private List<QuestRarityData.Param> _GetRarityList(string questName) {

		foreach(QuestRarityData.Sheet sht in m_rarityData.sheets) {
			if( sht.name == questName ) {
				return sht.list;
			}
		}

		Debug.LogError ("[EnemyDB] unknown quest:" + questName);
		return m_rarityData.sheets[0].list;
	}
}
