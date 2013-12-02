using UnityEngine;
using System.Collections;

public abstract class AbstractActor : MonoBehaviour {

	public abstract void BeginTurnAction(GameManager gm);
	public abstract bool IsTurnActionEnd();

	public abstract float ActionOrder {
		get;
	}
	public abstract bool Dead {
		get;
	}
}
