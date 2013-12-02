using UnityEngine;
using System.Collections;

/*
 *	Status Modifier is the base of all status changes
 *	- Poison
 *	- Confuse
 *	- 
 */
public class StatusSleep : StatusModifier {

	[SerializeField]
	protected int m_turnLeft;			// attaching actor

	public override Kind StatusKind { get { return StatusModifier.Kind.Sleep; } }

	public override void ActivateStatus() {
		m_turnLeft = Random.Range (2, 10);
		m_actor.PlayEmotion(Emotion.Kind.Sleep);
		m_actor.isSleeping = true;
		GUIManager.GetManager().Message(m_actor.charName + " は ねむった！");
	}
	public override void ResolveStatus() {
		m_turnLeft = 0;
		m_actor.isSleeping = false;
		m_actor.StopEmotion(Emotion.Kind.Sleep);
		GUIManager.GetManager().Message(m_actor.charName + " は めざめた！");
	}
	
	public override void UpdateStatus() {
		if(--m_turnLeft <= 0) {
			ResolveStatus();
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
