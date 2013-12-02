using UnityEngine;
using System.Collections;

public class ItemEntity : ScriptableObject {

	public enum Kind {
		Food,
		Weapon,
		Shield
	}

	public string itemName;
	public string icon;
	public Kind kind;
	public float gainHunger;
	public float gainHp;
	public float[] ap;
	public float[] def;
	public float statPoison;
	public float statParalize;
	public int id;
	public string attackEffect;
	public string description;
	private static string[] s_kindString = { "食べもの", "ぶき", "ぼうぐ" };

	public void Initialize(ItemData.Param p) {
		id = p.id;
		itemName = p.name;
		icon = p.icon;
		gainHp = p.gainHitpoint;
		gainHunger = p.gainHunger;
		ap 		= new float[(int)ElementType.ET_MAX];
		def 	= new float[(int)ElementType.ET_MAX];

		for(int i = 0; i < ap.Length; ++i) {
			ap[i] 	= p.ap[i];
			def[i] 	= p.def[i];
		}

		statPoison = p.statPoison;
		statParalize = p.statParalize;

		if( p.kind == "food" 	) kind = Kind.Food;
		if( p.kind == "weapon" 	) kind = Kind.Weapon;
		if( p.kind == "shield" 	) kind = Kind.Shield;

		attackEffect = p.attackEffect;
		description = string.Format("[{0}]: {1}", s_kindString[(int)kind], p.description);
	}

	public bool IsUsable {
		get { return kind == Kind.Food; }
	}

	/*
	 * returns if use next time will consume(break) item
	 */
	public bool WillUseConsumeItem {
		get { return kind == Kind.Food; }
	}

	public bool IsEquipable {
		get { return IsWeapon || IsShield; }
	}

	public bool IsThrowable {
		get { return true; }
	}

	public bool IsWeapon {
		get { return kind == Kind.Weapon; }
	}

	public bool IsShield {
		get { return kind == Kind.Shield; }
	}

	public bool IsFood {
		get { return kind == Kind.Food; }
	}

	public Kind ItemKind {
		get { return kind; }
	}

	public string Description {
		get {
			return description;
		}
	}

	/*
	 * ItemEntry instance may be overrided by a few unique items
	 */
	public virtual void EquipBy(Actor a) {

		if(kind == Kind.Weapon) {
			a.armedWeapon = this;
		}
		else if(kind == Kind.Shield) {
			a.armedShield = this;
		}

		GUIManager.GetManager().Message(a.charName + " は " + itemName + " を みにつけた！" );
	}

	/*
	 * ItemEntry instance may be overrided by a few unique items
	 */
	public virtual void UseBy(Actor a) {
		if( kind == Kind.Food ) {
			GUIManager.GetManager().Message(a.charName + " は " + itemName + " を たべた！" );
			a.ApplyHpGain((int)gainHp);
			a.hunger = Mathf.Clamp (a.hunger + gainHunger, 0.0f, a.hungerMax);
		} else {
			GUIManager.GetManager().Message(a.charName + " は " + itemName + " を つかった！" );
		}

		if( statPoison > 0.0f ) {
			float prob = Random.Range (0.0f, 1.0f);
			Debug.Log ("[Item][Poison] " + prob + "/" + statPoison);
			if( prob <= statPoison ) {
				a.ResolveStatus(StatusModifier.Kind.Poison);
			}
		}
		else if( statPoison < 0.0f ) {
			float prob = Random.Range (0.0f, 1.0f);
			Debug.Log ("[Item][Poison] " + prob + "/" + Mathf.Abs(statPoison));
			if( prob <= Mathf.Abs(statPoison) ) {
				a.AttachStatus(StatusModifier.Kind.Poison);
			}
		}

		if( statParalize > 0.0f ) {
			float prob = Random.Range (0.0f, 1.0f);
			Debug.Log ("[Item][Paralize] " + prob + "/" + statParalize);
			if( prob <= statParalize ) {
				a.ResolveStatus(StatusModifier.Kind.Paralized);
			}
		}
		else if( statParalize < 0.0f ) {
			float prob = Random.Range (0.0f, 1.0f);
			Debug.Log ("[Item][Paralize] " + prob + "/" + Mathf.Abs(statParalize));
			if( prob <= Mathf.Abs(statParalize) ) {
				a.AttachStatus(StatusModifier.Kind.Paralized);
			}
		}

		if(WillUseConsumeItem) {
			Destroy (this);
		}
	}

	/*
	 * ItemEntry instance may be overrided by a few unique items
	 * 
	 * if item is equiped by attacker and hit target.
	 */
	public static bool AttackBy(ItemEntity item, Actor target, Actor attacker, float jumpCoeff) {

		if( item != null ) {
			return item.AttackBy(target, attacker, jumpCoeff);
		} else {
			int dmg = attacker.CalcurateAttackDamage(item, target, ElementType.ET_Physical, jumpCoeff);
			
			if( dmg > 0 ) {
				GUIManager.GetManager().Message(target.charName + " に " + dmg + " の ダメージあたえた！" );
				return target.ApplyDamage(dmg);
			} else {
				GUIManager.GetManager().Message(target.charName + " に ダメージをあたえられない！" );
				return true;
			}
		}
	}


	/*
	 * ItemEntry instance may be overrided by a few unique items
	 * 
	 * if item is equiped by attacker and hit target.
	 */
	public virtual bool AttackBy(Actor target, Actor attacker, float jumpCoeff) {

		int dmg = attacker.CalcurateAttackDamage(this, target, ElementType.ET_Physical, jumpCoeff);
		
		if( dmg > 0 ) {
			GUIManager.GetManager().Message(target.charName + " に " + dmg + " の ダメージあたえた！" );
			return target.ApplyDamage(dmg);
		} else {
			GUIManager.GetManager().Message(target.charName + " に ダメージをあたえられない！" );
			return true;
		}
	}

	/*
	 * ItemEntry instance may be overrided by a few unique items
	 * 
	 * if item is thrown by attacker and hit target.
	 */
	public virtual bool HitBy(Actor target, Actor thrower) {

		if( statPoison > 0.0f ) {
			float prob = Random.Range (0.0f, 1.0f);
			Debug.Log ("[Item][Poison] " + prob + "/" + statPoison);
			if( prob <= statPoison ) {
				target.ResolveStatus(StatusModifier.Kind.Poison);
			}
		}
		else if( statPoison < 0.0f ) {
			float prob = Random.Range (0.0f, 1.0f);
			Debug.Log ("[Item][Poison] " + prob + "/" + Mathf.Abs(statPoison));
			if( prob <= Mathf.Abs(statPoison) ) {
				target.AttachStatus(StatusModifier.Kind.Poison);
			}
		}
		
		if( statParalize > 0.0f ) {
			float prob = Random.Range (0.0f, 1.0f);
			Debug.Log ("[Item][Paralize] " + prob + "/" + statParalize);
			if( prob <= statParalize ) {
				target.ResolveStatus(StatusModifier.Kind.Paralized);
			}
		}
		else if( statParalize < 0.0f ) {
			float prob = Random.Range (0.0f, 1.0f);
			Debug.Log ("[Item][Paralize] " + prob + "/" + Mathf.Abs(statParalize));
			if( prob <= Mathf.Abs(statParalize) ) {
				target.AttachStatus(StatusModifier.Kind.Paralized);
			}
		}

		int dmg = thrower.CalcurateThrowDamageOfItem(this, target, ElementType.ET_Physical);

		if( dmg > 0 ) {
			GUIManager.GetManager().Message(target.charName + " に " + dmg + " の ダメージあたえた！" );
			return target.ApplyDamage(dmg);
		} else {
			GUIManager.GetManager().Message(target.charName + " に ダメージをあたえられない！" );
			return true;
		}
	}
}
