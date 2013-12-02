using UnityEngine;
using System.Collections;

/*
 *	Status Modifier is the base of all status changes
 *	- Poison
 *	- Confuse
 *	- 
 */
public abstract class StatusModifier : ScriptableObject {

	public enum Kind {
		Poison,
		Paralized,
		Sleep
	}

	[SerializeField]
	protected Actor m_actor;			// attaching actor

	public abstract void ActivateStatus();
	public abstract void ResolveStatus();
	public abstract void UpdateStatus();
	public abstract bool IsStatusAlive();
	public abstract Kind StatusKind { get; }

	public virtual void Initialize(Actor attachingActor) {
		m_actor = attachingActor;
	}

	public static StatusModifier CreateStatus(Kind k) {

		switch(k) {
		case Kind.Poison:
			return ScriptableObject.CreateInstance<StatusPoison>();
		case Kind.Paralized:
			return ScriptableObject.CreateInstance<StatusParalized>();
		case Kind.Sleep:
			return ScriptableObject.CreateInstance<StatusSleep>();
		}

		return null;
	}
}
