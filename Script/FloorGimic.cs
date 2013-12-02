using UnityEngine;
using System.Collections;

public class FloorGimic : MonoBehaviour {

	// step of this floorgimic
	public Step step;

	[SerializeField]
	private tk2dSpriteAnimator m_spriteAnimator;
	[SerializeField]
	private tk2dSprite m_sprite;

	[SerializeField]
	private FloorGimicData.Param m_fgData;

	public void Initialize(FloorGimicData.Param fgData, FloorGimicDatabase db) {

		Debug.Log ("[FloorGimic] Initialized: "+fgData.name );


		m_fgData = fgData;
		m_sprite.SetSprite(m_fgData.graphic);
		m_spriteAnimator.Play(m_fgData.graphic);
	}

	// actor enter into floor
	public void ActorEnterFloorAction(Actor a) {
		if( m_fgData.fixedDamage > 0 ) {
			int damage = Mathf.Clamp ( (int)m_fgData.fixedDamage, 0, (int)a.hp - 1 );
			if( damage > 0 ) {
				GUIManager.GetManager().Message(a.charName + " は " + damage + " の ダメージをうけた！" );
				a.ApplyDamage(damage);
			}
		}
		//TODO
		if( m_fgData.graphic == "PoisonFloor" ) {
			a.AttachStatus(StatusModifier.Kind.Poison);
		}
		if( m_fgData.graphic == "ParalizeFloor" ) {
			a.AttachStatus(StatusModifier.Kind.Paralized);
		}
		Debug.Log ("[FloorGimic] Enter action ("+m_fgData.onFloorAction+") to:" + a.charName );
	}

	// actor leave from floor
	public void ActorLeaveFloorAction(Actor a) {
		Debug.Log ("[FloorGimic] Enter action ("+m_fgData.leaveFloorAction+") to:" + a.charName );
	}

	// actor take action to floor
	public void ActorActivatedFloorAction(Actor a) {
		Debug.Log ("[FloorGimic] Enter action ("+m_fgData.onFloorEffect+") to:" + a.charName );
	}
}
