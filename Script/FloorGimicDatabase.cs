using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FloorGimicDatabase : MonoBehaviour {

	enum RarityType {
		Constant,
		LinearDecrease,
		LinearIncrease,
		ExpoDecrease,
		ExpoIncrease
	}

	[SerializeField]
	private FloorGimicData m_fgData;

	[SerializeField]
	private FloorGimicSetData m_fgSetData;

	[SerializeField]
	private QuestRarityData m_rarityData;

	[System.Serializable]
	public class FloorGimicGraphic {
		public string 		name;
		public GameObject 	fab;
	}

	[SerializeField]
	private GameObject 	m_generalFgfab;

	[SerializeField]
	private FloorGimicGraphic[] m_floorGimicGraphics;

	/*
	 *  Create FloorGimic with Graphic
	 */
	private FloorGimic _CreateGimic(FloorGimicData.Param p) {
		GameObject fab = null;
		foreach(FloorGimicGraphic fg in m_floorGimicGraphics) {
			if( fg.name == p.graphic ) {
				fab = fg.fab;
			}
		}
		if(fab == null) {
			fab = m_generalFgfab;
		}

		GameObject go = GameObject.Instantiate(fab) as GameObject;
		FloorGimic gimic = go.GetComponent<FloorGimic>() as FloorGimic;

		gimic.Initialize(p, this);
		return gimic;
	}
	public FloorGimic CreateGimic(string questName, int step) {
		List<QuestRarityData.Param> rl = _GetRarityList(questName);
		
		int acc = _AccumrateRarity(rl, step);
		int val = Random.Range(1, acc);
		
		foreach(QuestRarityData.Param p in rl) {
			if( p.kind != "floorgimic" ) {
				continue;
			}
			val -= _CalcurateRarity(step, p);
			if(val < 0) {
				return _CreateGimic( GenerateFloorGimicDirect(p.id) );
			}
		}
		Debug.LogError("[FATAL/LOGIC ERROR]GenerateNextFloorGimic() couldn't found generating item. should not happen:" + step);
		return null;
	}
	public FloorGimic CreateGimic(int fgid) {
		return _CreateGimic( GenerateFloorGimicDirect(fgid) );
	}

	/*
	 * create enemy by enemyId
	 */
	private FloorGimicData.Param GenerateFloorGimicDirect(int fgId) {
		foreach(FloorGimicData.Param p in m_fgData.sheets[0].list) {
			if(p.id == fgId) {
				return p;
			}
		}
		Debug.LogError("[FATAL]GenerateFloorGimicDirect() couldn't found generating item. id="+fgId);
		return null;
	}

	public int GetMinimumFloorGimicStep(string questName) {

		List<QuestRarityData.Param> rl = _GetRarityList(questName);

		int minStep = int.MaxValue;
		
		foreach(QuestRarityData.Param p in rl) {
			if( p.kind != "floorgimic" ) {
				continue;
			}
			minStep = Mathf.Min (minStep, p.minStep);
		}

		return minStep;
	}
	
	private int _AccumrateRarity(List<QuestRarityData.Param> rl, int currentStep) {
		int acc = 0;
		foreach(QuestRarityData.Param p in rl) {
			if( p.kind != "floorgimic" ) {
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

		Debug.LogError ("[FloorGimicDB] unknown quest:" + questName);
		return m_rarityData.sheets[0].list;
	}
}
