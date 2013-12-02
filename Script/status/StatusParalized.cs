using UnityEngine;
using System.Collections;

public class StatusParalized : StatusModifier {

	[SerializeField]
	protected int m_turnLeft;			// attaching actor

	public override Kind StatusKind { get { return StatusModifier.Kind.Paralized; } }

	public override void ActivateStatus() {
		m_turnLeft = Random.Range (2, 6);
		m_actor.isParalized = true;
		m_actor.PlayEmotion(Emotion.Kind.Paralize);
		GUIManager.GetManager().Message(m_actor.charName + " は しびれてしまった！");
	}
	public override void ResolveStatus() {
		m_turnLeft = 0;
		m_actor.isParalized = false;
		m_actor.StopEmotion(Emotion.Kind.Paralize);
		GUIManager.GetManager().Message(m_actor.charName + " の しびれが なおった！");
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
