using UnityEngine;
using System.Collections;

public class EnemyActor : Actor {

	private int m_value;

	public int id;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Initialize(string questname, int currentStep, EnemyDatabase db, ItemDatabase itemdb) {
		Initialize(db.GenerateNextEnemy(questname, currentStep), db, itemdb);
	}

	public void Initialize(int enemyId, EnemyDatabase db, ItemDatabase itemdb) {
		Initialize(db.GenerateEnemyDirect(enemyId), db, itemdb);
	}

	/*
	 * initialize enemy actor with enemy param
	 */
	public void Initialize(EnemyData.Param p, EnemyDatabase db, ItemDatabase itemdb) {

		//tk2dSpriteAnimation
		m_sprite.Library = db.GetAnimationFromName(p.graphic);

		id = p.id;
		charName = p.name;
		hp = p.hpMax;
		hpMax = p.hpMax;
		hunger = 0;
		hungerMax = 0;
		agility = (float)p.agility;
		doesGetHungry = false;
		for(int i =0; i < (int)ElementType.ET_MAX; ++i) {
			ap[i] 		= (float)p.ApMax[i];
			apMax[i] 	= (float)p.ApMax[i];
			def[i] 		= (float)p.DpMax[i];
			defMax[i] 	= (float)p.DpMax[i];
		}
		canDie = true;
		race = Race.Creature;

		level = p.level;
		exp = p.exp;
		earnExp = p.earnExp;

		attackEffect = p.attackEffect;

		// TODO: item
	}

	protected override void PerformTurnAction(GameManager gm) {

		m_value = (m_value == 1) ? -1 : 1;

		Actor naborLeft  = this.step.GetStep(-1).actorOnStep;
		Actor naborRight = this.step.GetStep(1).actorOnStep;

		if(naborLeft != this && naborLeft != null && naborLeft.race == Race.Human) {
			GUIManager.GetManager().DebugMessage("[Actor][Attack]" + gameObject.name + " attacks " + naborLeft.gameObject.name);
			StartCoroutine(Action_Attack (naborLeft, ElementType.ET_Physical));
		}
		else if(naborRight != this && naborRight != null && naborRight.race == Race.Human) {
			GUIManager.GetManager().DebugMessage("[Actor][Attack]" + gameObject.name + " attacks " + naborRight.gameObject.name);
			StartCoroutine(Action_Attack (naborRight, ElementType.ET_Physical));
		}
		else if(CanMove(m_value)) {
			StartCoroutine(Action_Move (m_value));
		} else {
			m_isOnActionNow = false;
		}
	}	

	void OnPostDie() {
		// enemy can be destroyed
		Destroy (gameObject);
	}
}
