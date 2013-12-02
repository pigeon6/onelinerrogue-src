using UnityEngine;
using System.Collections;

/*
 *	Status Modifier is the base of all status changes
 *	- Poison
 *	- Confuse
 *	- 
 */
public class StatusPoison : StatusModifier {

	[SerializeField]
	protected int m_turnLeft;			// attaching actor
	
	public override Kind StatusKind { get { return StatusModifier.Kind.Poison; } }

	public override void ActivateStatus() {
		m_turnLeft = Random.Range (2, 8);
		m_actor.PlayEmotion(Emotion.Kind.Poison);
		GUIManager.GetManager().Message(m_actor.charName + " は どくにおかされた！");
	}
	public override void ResolveStatus() {
		m_turnLeft = 0;
		m_actor.StopEmotion(Emotion.Kind.Poison);
		GUIManager.GetManager().Message(m_actor.charName + " の どくが きえた！");
	}

	public override void UpdateStatus() {
		if(--m_turnLeft <= 0) {
			ResolveStatus();
		} else {
			int damage = Mathf.FloorToInt(m_actor.hp * 0.08f);
			damage = Mathf.Clamp ( damage, 0, (int)m_actor.hp - 1 );
			if( damage > 0 ) {
				GUIManager.GetManager().Message(m_actor.charName + " は どくで  " + damage + " の ダメージをうけた！" );
				m_actor.ApplyDamage(damage);
			}
		}
	}
	
	// return if status is still alive(ongoing)
	public override bool IsStatusAlive() {
		return m_turnLeft > 0;
	}
	
	public override void Initialize(Actor attachingActor) {
		base.Initialize(attachingActor);
	}
}
