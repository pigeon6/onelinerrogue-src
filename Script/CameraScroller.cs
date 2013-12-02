using UnityEngine;
using System.Collections;

public class CameraScroller : AbstractActor {

	[SerializeField]
	private float m_stepOfDeathLimit = 0.0f;

	[SerializeField]
	private float m_deathStepForward = 0.5f;

	[SerializeField]
	private float m_actionOrder = 0.0f;

	[SerializeField]
	private int m_deathStepCamGap = -6;

	public override float ActionOrder {
		get {
			return m_actionOrder;
		}
	}
	public override bool Dead {
		get { 
			return false; 
		} 
	}

	public override void BeginTurnAction(GameManager gm) {
		if( gm && gm.currentMap ) {
			m_stepOfDeathLimit += m_deathStepForward;
			int limitstep  = Mathf.FloorToInt(m_stepOfDeathLimit);
			limitstep = gm.currentMap.ClampIndexToMapSize(limitstep);
			// there are gap between camera focus to the edge of death
			gm.deathLimitStep  = Mathf.Max (0, limitstep + m_deathStepCamGap);
			transform.position = gm.currentMap.GetActorStepPosition(limitstep);
		}
	}
	
	public override bool IsTurnActionEnd() {
		return true;
	}

	void Start() {
		GameManager gm = GameManager.GetManager();
		if( gm && gm.currentMap ) {
			int limitstep = Mathf.FloorToInt(m_stepOfDeathLimit);
			
			limitstep = gm.currentMap.ClampIndexToMapSize(limitstep);
			gm.deathLimitStep = limitstep;
			transform.position = gm.currentMap.GetActorStepPosition(limitstep);
		}
	}
}
