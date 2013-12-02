using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemDatabase : MonoBehaviour {

	enum RarityType {
		Constant,
		LinearDecrease,
		LinearIncrease,
		ExpoDecrease,
		ExpoIncrease
	}

	[SerializeField]
	private ItemData m_data;

	[SerializeField]
	private QuestRarityData m_rarityData;

	public ItemEntity GenerateItemDirect(int itemId) {
		foreach(ItemData.Param p in m_data.sheets[0].list) {
			if( p.id == itemId ) {
				return _GenerateItem(p);
			}
		}
		Debug.LogError("[FATAL]GenerateItemDirect() couldn't found generating item: id=" + itemId);
		return null;
	}

	public int GetMinimumItemStep(string questName) {
		List<QuestRarityData.Param> rl = _GetRarityList(questName);
		
		int minStep = int.MaxValue;
		
		foreach(QuestRarityData.Param p in rl) {
			if( p.kind != "item" ) {
				continue;
			}
			minStep = Mathf.Min (minStep, p.minStep);
		}

		return minStep;
	}

	public ItemEntity GenerateNextItem(string questName, int currentStep) {

		List<QuestRarityData.Param> rl = _GetRarityList(questName);

		int acc = _AccumrateRarity(rl, currentStep);
		int val = Random.Range(1, acc);

		foreach(QuestRarityData.Param p in rl) {
			if( p.kind != "item" ) {
				continue;
			}
			val -= _CalcurateRarity(currentStep, p);
			if(val < 0) {
				return GenerateItemDirect(p.id);
			}
		}
		Debug.LogError("[FATAL/LOGIC ERROR]GenerateNextItem() couldn't found generating item. should not happen.");
		return null;
	}

	private ItemEntity _GenerateItem(ItemData.Param p) {
		ItemEntity e = ScriptableObject.CreateInstance<ItemEntity>() as ItemEntity;
		e.Initialize(p);
		return e;
	}

	private int _AccumrateRarity(List<QuestRarityData.Param> rl, int currentStep) {
		int acc = 0;
		foreach(QuestRarityData.Param p in rl) {
			if( p.kind != "item" ) {
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
