
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ElementType {
	ET_Physical = 0,		// physical contact
	ET_Fire,				// fire
	ET_Ice,					// ice
	ET_Electric,			// electric
	ET_Poison,				// poison
	ET_Dark,				// dark/curse
	ET_Holy,				// holy
	ET_Psycho,				// mind/psycho/panic
	ET_MAX
}

public enum ActionType {
	Move,
	Attack,
	Jump,
	PutItemOnGround,
	GetItemOnGround,
	UseItem,
	EquipItem,
	StampAndStill,
	Sleep,
	ThrowItem,
	AT_MAX
}

public enum DirectionType {
	LEFT,
	RIGHT,
	BACK,
	STAY,
}


public enum Race {
	Human,
	Creature
}

public class Actor : AbstractActor {

	public string charName;				// character name
	public float hp;					// HP/life
	public float hpMax;					// HP/life
	public float hunger;				// hungry
	public float hungerMax;				// hungry
	public float agility;				// agility
	public float throwDistance;			// distance to throw
	public float throwDistanceMax;		// distance to throw
	public Race  race;					// race
	public float[] ap;					// attack power
	public float[] apMax;				// attack power
	public float[] def;					// defence power
	public float[] defMax;				// defence power
	public float[] hungerPerAction;		// hunger consumption value per action
	public float[] hungerPerActionMax;	// hunger consumption value per action

	public int 		level;
	public float 	exp;
	public float 	nextExp;
	public float 	earnExp;

	public string 	attackEffect;

	public bool doesGetHungry;			// if actor consumes food
	public bool canDie;					// if actor can die or destroy
	public bool dead;					// if Actor is already dead

	public bool isSleeping;				// if Actor is sleeping
	public bool isParalized;			// if Actor is paralized

	public List<ItemEntity> items;		// item carrying
	public List<StatusModifier> status;	// bad status attached
	public ItemEntity armedWeapon;		// weapon equipped
	public ItemEntity armedShield;		// shield equipped

	[SerializeField]
	private Animation spriteAnim;

	[SerializeField]
	private Emotion emotion;

	[SerializeField]
	protected EffectManager effects;

	[SerializeField]
	private Gauge gauge;

	[SerializeField]
	protected tk2dAnimatedSprite m_sprite;
	[SerializeField]
	protected DirectionType m_currentDirection;

	public Step step;					// Map position
	
	static public float actionDurationSec = .25f;
	static public float jumpDurationSec   = actionDurationSec * 1.5f;
	static public float throwSpeedPerStepSec = .1f;
	static public float postActionIntervalSec = 0.1f;
	static public float duckDurationSec = 0.25f;

	static public int kMAX_ITEM_CARRY = 8;

	static public Vector3 chipOffset = new Vector3(-0.75f, 0.75f, 0.0f);
	static public Vector3 effectOffset = new Vector3(-0.75f, 0.75f, -3.0f);

	public bool m_isOnActionNow = false;


	[SerializeField]
	private Renderer m_renderer;

	/*
	 * initialize
	 */ 
	public void Awake() {
		items = new List<ItemEntity>();
	}

	/*
	 * do turn action:
	 * true when action is completed
	 */
	public override void BeginTurnAction(GameManager gm) {
		m_isOnActionNow = true;

		foreach(StatusModifier s in status) {
			s.UpdateStatus();
		}
		status.RemoveAll( x => !x.IsStatusAlive() );

		if( isSleeping ) {
			GUIManager.GetManager().DebugMessage("[Actor][Sleep]" + charName + " is sleeping.");			
			// if character is sleeping and not appearing on screen, no message.
			if( IsVisible() ) {
				GUIManager.GetManager().Message(charName + " は ねむっている・・・" );
			}
			m_isOnActionNow = false;
		} 
		else if( isParalized ) {
			GUIManager.GetManager().DebugMessage("[Actor][Paralized]" + charName + " is sleeping.");			
			GUIManager.GetManager().Message(charName + " しびれて うごけない！" );
			m_isOnActionNow = false;
		} else {
			PerformTurnAction(gm);
		}
	}
	protected virtual void PerformTurnAction(GameManager gm) {
		Debug.Log ("[WARN] Unimplemented Actor action");
		// Implemented by inherited class
		m_isOnActionNow = false;
	}

	public override bool IsTurnActionEnd() {
		return !m_isOnActionNow;
	}

	public override float ActionOrder {
		get {
			return Agility;
		}
	}

	public float Agility {
		get {
			if( isSleeping ) return agility * 0.01f;
			if( isParalized ) return agility * 0.1f;
			return agility;
		}
	}

	public override bool Dead {
		get { return dead; }
	}

	public int Hunger {
		get { return Mathf.CeilToInt(hunger); }
	}

	private string AttackEffect {
		get {
			if(armedWeapon != null) {
				return armedWeapon.attackEffect;
			}
			return attackEffect;
		}
	}

	/*
	 * if not visible from camera, animations can be skipped
	 */
	protected bool IsVisible() {
		return m_renderer != null && m_renderer.isVisible;
	}

	public int AP(ElementType t) {
		if( armedWeapon != null ) {
			return (int)(ap[(int)t] + armedWeapon.ap[(int)t]);
		} else {
			return (int)ap[(int)t];
		}
	}
	public int DP(ElementType t) {
			if( armedShield != null ) {
			return (int)(def[(int)t] + armedShield.def[(int)t]);
			} else {
			return (int)(def[(int)t]);
			}
		}

	//------------------------------------------
	// Check if certain action can be performed 
	//-----------------------------------------

	protected bool CanMove(int offset) {
		Step nextStep = this.step.GetStep(offset);
		return nextStep.actorOnStep == null;
	}

	public bool CanPutItemOnGround(int itemIndex) {
		// TODO: also equipped item should not be able to put
		return step.itemOnStep == null;
	}
	
	public bool CanPickupItemOnGround() {
		return step.itemOnStep != null;
	}
	
	// Ashibumi
	public bool CanStampAndStill() {
		// TODO:
		return true;
	}
	
	// Jump
	public bool CanJump() {
		// TODO:
		return true;
	}
	
	// Rest
	public bool CanRest() {
		// TODO:
		return true;
	}
	
	public bool CanUseItem(int itemIndex) {
		return items[itemIndex].IsUsable;
	}
	
	public bool CanEatItem(int itemIndex) {
		return items[itemIndex].IsFood;
	}

	public bool CanEquipItem(int itemIndex) {
		return items[itemIndex].IsEquipable;
	}
	
	public bool IsEquippingItem(int itemIndex) {
		return 	(armedWeapon != null && items[itemIndex] == armedWeapon) ||
				(armedShield != null && items[itemIndex] == armedShield);
	}
	
	public bool CanThrowItem(int itemIndex) {
		return items[itemIndex].IsThrowable;
	}

	
	//---------------------------------
	// Actions 
	//---------------------------------

	/*
	 * Move Action
	 */ 
	protected IEnumerator Action_Move(int offset) {

		SendMessage("OnPreMove", SendMessageOptions.DontRequireReceiver);

		Step nextStep = this.step.GetStep(offset);
		Vector3 nextPos = nextStep.GetActorPos();
		Vector3 pos = this.step.GetActorPos();
		float tNow = Time.time;
		
		GUIManager.GetManager().DebugMessage("[Actor][Move]" + charName + " " + offset + pos);

		FaceDirection( (offset > 0) ? DirectionType.LEFT : DirectionType.RIGHT);

		if( IsVisible() ) {
			while( Time.time - tNow  <= actionDurationSec ) {
				float rate = (Time.time - tNow) / actionDurationSec;
				transform.position = Vector3.Lerp(pos, nextPos, rate);
				yield return new WaitForEndOfFrame();
			}
		}
		transform.position = nextPos;
		MoveTo(nextStep);
		
		SendMessage("OnPostMove", SendMessageOptions.DontRequireReceiver);

		GetHungerAfterAction(ActionType.Move);

		m_isOnActionNow = false;
		yield return null;
	}

	/*
	 * Jump Action
	 */ 
	protected IEnumerator Action_Jump(int offset) {
		
		SendMessage("OnPreJump", SendMessageOptions.DontRequireReceiver);
		
		Step nextStep = this.step.GetStep(offset * 3); // TODO: jump ampunt
		Vector3 nextPos = nextStep.GetActorPos();
		Vector3 pos = this.step.GetActorPos();
		float tNow = Time.time;
		
		GUIManager.GetManager().DebugMessage("[Actor][Jump]" + charName + " " + offset + pos);

		FaceDirection( (offset > 0) ? DirectionType.LEFT : DirectionType.RIGHT);

		GUIManager.GetManager().Message(charName + " は たかく とんだ！" );

		effects.PlaySE(this, ActorSE.JumpTakeOff);

		if( IsVisible() ) {
			while( Time.time - tNow  <= jumpDurationSec ) {
				float rate = (Time.time - tNow) / jumpDurationSec;
				Vector3 targetPos = Vector3.Lerp(pos, nextPos, rate);
				transform.position = new Vector3(targetPos.x, targetPos.y + Mathf.Sin ( Mathf.PI * rate ) * 2.0f, targetPos.z);
				yield return new WaitForEndOfFrame();
			}
		}
		transform.position = nextPos;

		if( nextStep.actorOnStep != null && nextStep.actorOnStep != this ) {
			while( nextStep.actorOnStep != null && nextStep.actorOnStep != this ) {
				Actor target = nextStep.actorOnStep;
				GUIManager.GetManager().Message(charName + " は " + target.charName + " に とびかかった！" );
				
				bool doesHit = TestAttackHit(target, armedWeapon);
				if(doesHit) {
					effects.Spawn(AttackEffect, target.transform.position + effectOffset);
				}
				
				int nextTargetOffset = (m_currentDirection == DirectionType.LEFT) ? -1 : 1;
				
				if( nextStep.GetStep(nextTargetOffset).actorOnStep == null ) {
					nextStep = nextStep.GetStep(nextTargetOffset);
				} else {
					nextStep = nextStep.GetStep(nextTargetOffset*-1);
				}
				
				nextPos = nextStep.GetActorPos();
				pos = transform.position;
				tNow = Time.time;
				
				if( IsVisible() ) {
					while( Time.time - tNow  <= actionDurationSec ) {
						float rate = (Time.time - tNow) / actionDurationSec;
						Vector3 targetPos = Vector3.Lerp(pos, nextPos, rate);
						transform.position = new Vector3(targetPos.x, targetPos.y + Mathf.Sin ( Mathf.PI * rate ), targetPos.z);
						yield return new WaitForEndOfFrame();
					}
				}
				transform.position = nextPos;
				MoveTo(nextStep);
				yield return new WaitForEndOfFrame();
				effects.PlaySE(this, ActorSE.JumpLand);
				
				bool alive = true;
				
				if(doesHit) {
					alive = ItemEntity.AttackBy(armedWeapon, target, this, 1.5f);
					while(target.gauge.IsAnimating) {
						yield return new WaitForEndOfFrame();
					}
					
					if(!alive) {
						_BeatActor(target);
						yield return new WaitForSeconds(0.3f);
					}
				} else {
					target._ActivateDuck();
				}
			}
		} else {
			MoveTo(nextStep);
			yield return new WaitForEndOfFrame();
			effects.PlaySE(this, ActorSE.JumpLand);
		}

		SendMessage("OnPostJump", SendMessageOptions.DontRequireReceiver);
		
		GetHungerAfterAction(ActionType.Jump);
		
		m_isOnActionNow = false;
		yield return null;
	}

	/*
	 * Attack Action
	 */ 
	protected IEnumerator Action_Attack(Actor target, ElementType attackKind) {

		SendMessage("OnPreAttack", SendMessageOptions.DontRequireReceiver);

		GUIManager.GetManager().DebugMessage("[Actor][Attack]" + charName + " attacks " + target.charName );

		GUIManager.GetManager().Message(charName + " は " + target.charName + " に こうげき！" );

		FaceDirection( (target.step.index > step.index) ? DirectionType.LEFT : DirectionType.RIGHT);
		string animName = (target.step.index > step.index) ? "AttackLeft" : "AttackRight";
		spriteAnim.Play(animName);

		yield return new WaitForSeconds(0.3f);

		bool alive = true;

		if(TestAttackHit(target, armedWeapon)) {
			alive = ItemEntity.AttackBy(armedWeapon, target, this, 1.0f);
			effects.Spawn(AttackEffect, target.transform.position + effectOffset);
			while(target.gauge.IsAnimating) {
				yield return new WaitForEndOfFrame();
			}

			if(!alive) {
				_BeatActor(target);
				yield return new WaitForSeconds(0.3f);
			}
		} else {
			target._ActivateDuck();
		}

		SendMessage("OnPostAttack", SendMessageOptions.DontRequireReceiver);

		GetHungerAfterAction(ActionType.Attack);

		m_isOnActionNow = false;
	}


	/*
	 * Item Pickup
	 */ 
	protected IEnumerator Action_PickupItemOnGround() {

		PickupItemOnGround();
		yield return new WaitForSeconds(0.3f);

		GetHungerAfterAction(ActionType.GetItemOnGround);

		m_isOnActionNow = false;
		yield return null;
	}

	/*
	 * Stamp Action
	 */ 
	protected IEnumerator Action_Stamp() {
		
		SendMessage("OnPreStamp", SendMessageOptions.DontRequireReceiver);

		GUIManager.GetManager().DebugMessage("[Actor][Stamp]" + charName + " is waiting... " );		
		GUIManager.GetManager().Message(charName + " は ようすをみている。" );
		yield return new WaitForSeconds(0.3f);

		SendMessage("OnPostStamp", SendMessageOptions.DontRequireReceiver);
		
		GetHungerAfterAction(ActionType.StampAndStill);

		m_isOnActionNow = false;
		yield return null;
	}


	/*
	 * Item Put
	 */ 
	protected IEnumerator Action_PutItemOnGround(int itemIndex) {

		if( itemIndex >= items.Count ) {
			Debug.LogError ("[Actor] tried to put item out of index:" + itemIndex + " items:" + items.Count);
			return false;
		}
		if(step.itemOnStep != null) {
			Debug.LogError ("[Actor] tried to put item where floor is not empty step:" + step.index);
			return false;
		}

		SendMessage("OnPrePutItemOnGround", SendMessageOptions.DontRequireReceiver);

		ItemEntity item = items[itemIndex];
		items.RemoveAt(itemIndex);
		_EnsureItemNotEquipped(item);
		step.SetItem(item);

		GUIManager.GetManager().DebugMessage("[Actor][PutItemOnGround]" + charName + " put item " + item.itemName );		
		GUIManager.GetManager().Message(charName + " は " + item.itemName + " を じめんに おいた。" );
		yield return new WaitForSeconds(0.3f);

		SendMessage("OnPostPutItemOnGround", SendMessageOptions.DontRequireReceiver);

		GetHungerAfterAction(ActionType.PutItemOnGround);

		m_isOnActionNow = false;
	}

	/*
	 * Item Equip
	 */ 
	protected IEnumerator Action_EquipItem(int itemIndex) {
		
		if( itemIndex >= items.Count ) {
			Debug.LogError ("[Actor] tried to equip item out of index:" + itemIndex + " items:" + items.Count);
			return false;
		}

		if( !CanEquipItem(itemIndex) ) {
			Debug.LogError ("[Actor] tried to equip item that isn't equippable:" + itemIndex );
			return false;
		}

		SendMessage("OnPreEquipItem", SendMessageOptions.DontRequireReceiver);
		
		ItemEntity item = items[itemIndex];
		bool isUnequip = false;
		if(item.IsWeapon) {
			if( armedWeapon == item ) {
				isUnequip = true;
				armedWeapon = null;
			} else {
				item.EquipBy(this);
			}
			//TODO: curse action, etc
		} 
		else if(item.IsShield) {
			if( armedShield == item ) {
				isUnequip = true;
				armedShield = null;
			} else {
				item.EquipBy(this);
			}
			//TODO: curse action, etc
		}

		if( isUnequip ) {
			GUIManager.GetManager().DebugMessage("[Actor][Equip]" + charName + " equipped " + item.itemName );		
			GUIManager.GetManager().Message(charName + " は " + item.itemName + " を はずした。" );
		} else {
			GUIManager.GetManager().DebugMessage("[Actor][Equip]" + charName + " equipped " + item.itemName );		
		}
		yield return new WaitForSeconds(0.3f);
		
		SendMessage("OnPostEquipItem", SendMessageOptions.DontRequireReceiver);

		GetHungerAfterAction(ActionType.EquipItem);

		m_isOnActionNow = false;
	}

	/*
	 * Item Equip
	 */ 
	protected IEnumerator Action_UseItem(int itemIndex) {
		
		if( itemIndex >= items.Count ) {
			Debug.LogError ("[Actor] tried to use item out of index:" + itemIndex + " items:" + items.Count);
			return false;
		}
		
		if( !CanUseItem(itemIndex) ) {
			Debug.LogError ("[Actor] tried to use item that isn't usable:" + itemIndex );
			return false;
		}
		
		SendMessage("OnPreUseItem", SendMessageOptions.DontRequireReceiver);
		
		ItemEntity item = items[itemIndex];

		if( item.WillUseConsumeItem ) {
			items.RemoveAt(itemIndex);
			_EnsureItemNotEquipped(item);
		}

		GUIManager.GetManager().DebugMessage("[Actor][Equip]" + charName + " used " + item.itemName );		

		item.UseBy(this);  // may destroy item. don't touch after here

		// message is given in UseBy()
		yield return new WaitForSeconds(0.3f);
		
		SendMessage("OnPostUseItem", SendMessageOptions.DontRequireReceiver);
		GetHungerAfterAction(ActionType.UseItem);

		m_isOnActionNow = false;
	}


	/*
	 * Item Equip
	 */ 
	protected IEnumerator Action_ThrowItem(int itemIndex) {
		
		if( itemIndex >= items.Count ) {
			Debug.LogError ("[Actor] tried to throw item out of index:" + itemIndex + " items:" + items.Count);
			return false;
		}
		
		if( !CanThrowItem(itemIndex) ) {
			Debug.LogError ("[Actor] tried to use item that isn't throwable:" + itemIndex );
			return false;
		}
		
		SendMessage("OnPreThrowItem", SendMessageOptions.DontRequireReceiver);

		ItemEntity e = items[itemIndex];

		GUIManager.GetManager().DebugMessage("[Actor][Throw]" + charName + " throw " + e.itemName );		
		GUIManager.GetManager().Message(charName + " は " + e.itemName + " を なげつけた！" );
		yield return new WaitForSeconds(0.3f);

		Item item = step.map.CreateItem(e);		
		items.RemoveAt(itemIndex);
		_EnsureItemNotEquipped(e);

		int direction = m_currentDirection == DirectionType.RIGHT? -1 : 1;

		effects.PlaySE(this, ActorSE.ItemThrow);
		Step throwTargetStep = step.GetStep(Mathf.FloorToInt(throwDistance * (float)direction));

		Actor hitTarget = null;
		Step nextStep = null;
		float throwDuration = 0.0f;
		for(int i = direction; nextStep != throwTargetStep; i+=direction) {
			throwDuration += throwSpeedPerStepSec;
			nextStep = step.GetStep (i);
			Actor t = nextStep.actorOnStep;
			if( t != null ) {
				if(TestAttackHit(t, e)) {
					hitTarget = t;
					throwTargetStep = nextStep;
					break;
				} else {
					t._ActivateDuck();
				}
			}
		}

		nextStep = throwTargetStep;
		Vector3 pos = step.GetActorPos() + chipOffset; 			// use actor pos for throwing item for now
		Vector3 nextPos = nextStep.GetActorPos() + chipOffset; 		// use actor pos for throwing item for now
		if( hitTarget == null ) {
			nextPos = nextStep.GetItemPos();
		}

		if( item.IsVisible ) {
			float tNow = Time.time;
			throwDuration = Mathf.Max (0.25f, throwDuration);
			while( Time.time - tNow  <= throwDuration ) {
				float rate = (Time.time - tNow) / throwDuration;
				Vector3 t = Vector3.Lerp(pos, nextPos, rate);
				item.transform.position = new Vector3(t.x, t.y + Mathf.Sin ( Mathf.PI * rate ) * 0.45f, t.z);

				yield return new WaitForEndOfFrame();
			}
		}
		item.transform.position = nextPos;

		if( hitTarget != null ) {
			// pop hit effect
			effects.Spawn(e.attackEffect,item.transform.position);
			Destroy (item.gameObject);// destroy housing, not entity
			
			GUIManager.GetManager().Message(hitTarget.charName + " に みごとにあたった！" );
			bool alive = e.HitBy(hitTarget, this);
			while(hitTarget.gauge.IsAnimating) {
				yield return new WaitForEndOfFrame();
			}
			if(!alive) {
				_BeatActor(hitTarget);
				yield return new WaitForSeconds(0.3f);
			}
		} 
		// item didn't hit anything, so now finds where to drop
		else {
			Item itemOnGround = throwTargetStep.itemOnStep;
			if(itemOnGround != null) {
				effects.Spawn("ThrowHitItemOnGround", itemOnGround.transform.position + effectOffset);
				GUIManager.GetManager().Message(itemOnGround.ItemName + " に ぶつかった！" );
				GUIManager.GetManager().Message(itemOnGround.ItemName + " は こわれてしまった。" );
				Destroy (itemOnGround.gameObject);// destroy housing, not entity
			}
			throwTargetStep.SetItem(item);
		}

		SendMessage("OnPostThrowItem", SendMessageOptions.DontRequireReceiver);
		GetHungerAfterAction(ActionType.ThrowItem);
		
		m_isOnActionNow = false;
	}


	//------------------------------------------
	// Damage methods(not actions)
	//-----------------------------------------
	/*
	 *  Apply damage to this actor
	 * used by ItemEntity
	 * @result if Actor is still alive
	 */ 
	public bool ApplyDamage(int damage) {
		float oldHp = hp;
		hp = Mathf.Clamp(hp - damage, 0, hpMax);

		if(race == Race.Human) {
			effects.CameraTremble();
		}
		if( damage != 0 ) {
			DamageKind dk = (race==Race.Human)?DamageKind.PlayerDamage:DamageKind.EnemyDamage;
			effects.SpawnDamage(damage, dk, transform.position + effectOffset);
			gauge.ApplyGaugeValueTo(oldHp/hpMax, hp/hpMax);
		}

		return hp > 0.0f;
	}

	public bool ApplyHpGain(int gain) {
		float oldHp = hp;
		hp = Mathf.Clamp(hp + gain, 0, hpMax);
		
		if( gain != 0 ) {
			if(race == Race.Human && gain < 0) {
				effects.CameraTremble();
			}
			DamageKind dk = (gain>0)?DamageKind.Cure:DamageKind.PlayerDamage;
			effects.SpawnDamage(gain, dk, transform.position + effectOffset);
			gauge.ApplyGaugeValueTo(oldHp/hpMax, hp/hpMax);
		}
		
		return hp > 0.0f;
	}


	/*
	 *  Calcurate damage for the attack
	 * used by ItemEntity
	 * @result if Actor is still alive
	 */ 
	public int CalcurateAttackDamage(ItemEntity e, Actor target, ElementType attackKind, float jumpCoeff) {
		float attack  = ap[(int)attackKind];
		float defence = target.def[(int)attackKind];

		if( e != null ) {
			attack += e.ap[(int)attackKind];
		}
		if( target.armedShield != null ) {
			defence += target.armedShield.def[(int)attackKind];
		}
		
		float damage = (attack - defence) * jumpCoeff;
		
		return Mathf.FloorToInt(damage);
	}

	private void _ActivateDuck() {
		StartCoroutine(_Duck());
	}

	/*
	 * duck animation
	 */ 
	private IEnumerator _Duck() {
		emotion.PlayEmotionOnce(Emotion.Kind.Onpu);
		GUIManager.GetManager().Message(charName + " は かれいに よけた！" );

		float tNowDuck = Time.time;
		Vector3 originalPos = transform.position;
		int dir = (Random.Range (0, 1) == 0 ? -1 : 1);

		while( Time.time - tNowDuck  <= duckDurationSec ) {
			float rate = (Time.time - tNowDuck) / duckDurationSec;
			Vector3 targetPos = originalPos;
			targetPos.x += (Mathf.Sin ( Mathf.PI * rate ) * 0.5f * (float)dir);
			transform.position = targetPos;
			yield return new WaitForEndOfFrame();
		}
		transform.position = originalPos;
	}

	/*
	 *  Calcurate damage for the attack
	 * used by ItemEntity
	 * @result if Actor is still alive
	 */ 
	public int CalcurateThrowDamageOfItem(ItemEntity e, Actor target, ElementType attackKind) {
		float attack  = e.ap[(int)attackKind];
		float defence = target.def[(int)attackKind];
		
		if( target.armedShield != null ) {
			defence += target.armedShield.def[(int)attackKind];
		}
		
		float damage = attack * 2.5f - defence;
		
		return Mathf.FloorToInt(damage);
	}

	//------------------------------------------
	// Logics(not actions)
	//-----------------------------------------

	private void _EnsureItemNotEquipped(ItemEntity e) {
		if( armedWeapon != null && e == armedWeapon ) {
			armedWeapon = null;
		}
		if( armedShield != null && e == armedShield ) {
			armedShield = null;
		}
	}

	private void _BeatActor(Actor target) {
		GUIManager.GetManager().Message(target.charName + " を たおした！" );
		exp += target.earnExp;
		Debug.Log ("[Player] earned + " + target.earnExp);
		// TODO: item drop
		EventRecorder.GetManager().RecordActorKill(target, this);
		target.Die();

		LevelDatabase ldb = GameManager.GetManager().LevelDB;
		while(ldb.TestLevelUp(this)) {
			ldb.ApplyLevelUp(this);
			GUIManager.GetManager().Message(charName + "は レベルが" + level + "に あがった！" );
		}
	}

	/*
	 * Set Actor displayed or not
	 */
	protected void SetVisible(bool bVisible) {
		m_renderer.enabled = bVisible;
	}	

	/*
	 *  Apply hunger to this actor
	 * @result if Actor is still alive
	 */ 
	protected bool ApplyHungerConsumption(ActionType aType) {
		if( doesGetHungry ) {
			float lastHunger = hunger;
			hunger = Mathf.Clamp (hunger - CalcurateHungerConsumption(aType), 0.0f, hungerMax);
			SendMessage("OnHungerChanged", lastHunger, SendMessageOptions.DontRequireReceiver);
			return hunger > 0.0f;
		} else {
			return true;
		}
	}

	protected void GetHungerAfterAction(ActionType aType) {
		if(!ApplyHungerConsumption(aType) ) {
			GUIManager.GetManager().Message(charName + "  は 空腹のあまり ちからつきた。" );
			Die();
		}
	}


	/*
	 *  Test if attack hits to target this time
	 * @result attack hits
	 */ 
	protected bool TestAttackHit(Actor target, ItemEntity item) {

		float defaultSuccessRate = 0.8f;
		float a = Agility - target.Agility;
		float hitRate = 0.0f;

		// [0.05, 0.95]
		hitRate = defaultSuccessRate + Mathf.Clamp ((a / target.Agility) * 0.35f, -0.35f, 0.35f);
		float v = Random.Range(0.0f, 1.0f);
		return v < hitRate;
	}

	/*
	 *  Calcurate hunger per action
	 */ 
	protected float CalcurateHungerConsumption(ActionType aType) {
		float h  = hungerPerAction[(int)aType];
		// TODO:
		return h;
	}

	/*
	 *  Actor move to the step
	 */ 
	protected void MoveTo(Step nextStep) {

		if( step.floorGimicOnStep != null ) {
			step.floorGimicOnStep.ActorLeaveFloorAction(this);
		}

		nextStep.actorOnStep = this;
		step.actorOnStep = null;
		step = nextStep;
		transform.position = nextStep.GetActorPos();

		if( nextStep.floorGimicOnStep != null ) {
			nextStep.floorGimicOnStep.ActorEnterFloorAction(this);
		}
	}


	/*
	 *  Pickup
	 */ 
	protected void PickupItemOnGround() {
		if(CanPickupItemOnGround()) {
			SendMessage("OnPrePickupItemOnGround", SendMessageOptions.DontRequireReceiver);

			Item item = step.itemOnStep;
			GUIManager.GetManager().Message(charName + " は " +item.ItemName + " を みつけた！" );

			if( items.Count < kMAX_ITEM_CARRY ) {
				items.Add(item.ItemEntity);
				Destroy (item.gameObject);
				GUIManager.GetManager().Message(charName + " は " + item.ItemName + "を ひろった！" );
				effects.PlaySE(this, ActorSE.ItemPickup);
			} else {
				GUIManager.GetManager().Message("だが " + charName + " は もちものが いっぱいだ！" );
			}

			SendMessage("OnPostPickupItemOnGround", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void Die() {
		SendMessage("OnPreDie", SendMessageOptions.DontRequireReceiver);
		dead = true;
		SpawnEffect("Die");

		SendMessage("OnPostDie", SendMessageOptions.DontRequireReceiver);
		Destroy (gameObject);
	}

	public void Eat(ItemEntity e) {
		GUIManager.GetManager().Message(charName + " は " + e.itemName + " を たべた！" );
		PlaySE(ActorSE.ItemEat);
		ApplyHpGain((int)e.gainHp);
		hunger = Mathf.Clamp (hunger + e.gainHunger, 0.0f, hungerMax);
	}

	public void SpawnEffect(string effect) {
		effects.Spawn(effect,transform.position + effectOffset);
	}

	public void PlaySE(ActorSE ase) {
		effects.PlaySE(this, ase);
	}

	public void PlayEmotionOnce(Emotion.Kind k) {
		emotion.PlayEmotionOnce(k);
	}

	public void PlayEmotion(Emotion.Kind k) {
		emotion.PlayEmotion(k);
	}

	public void StopEmotion(Emotion.Kind k) {
		emotion.StopEmotion(k);
	}

	public void AttachStatus(StatusModifier.Kind k) {
		Debug.Log ("[Actor]Attaching status:" + k.ToString());
		StatusModifier sm = status.Find( x => x.StatusKind == k);
		if( sm == null ) {
			sm = StatusModifier.CreateStatus(k);
			sm.Initialize(this);
			sm.ActivateStatus();
			status.Add(sm);
		}
	}

	public void ResolveStatus(StatusModifier.Kind k) {
		Debug.Log ("[Actor]Resolving status:" + k.ToString());
		StatusModifier sm = status.Find( x => x.StatusKind == k);
		if( sm != null ) {
			sm.ResolveStatus();
			status.Remove (sm);
		}
	}

	public void FaceDirection(DirectionType d) {
		if(d != m_currentDirection) {
			m_sprite.Play(d.ToString());
			m_currentDirection = d;
		}
	}
	
	public bool HasItem(int itemId) {
		foreach(ItemEntity item in items){
			if( item.id == itemId ) {
				return true;
			}
		}
		return false;
	}
}
