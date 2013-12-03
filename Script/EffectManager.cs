using UnityEngine;
using System.Collections;

public enum ActorSE {
	Footstep,
	JumpTakeOff,
	JumpLand,
	ItemPickup,
	ItemThrow,
	ItemEat,
}

public class EffectManager : MonoBehaviour {

	[System.Serializable]
	class Effect {
		public string effectName = string.Empty;
		public GameObject effectFab = null;
	}
	[SerializeField]
	private Effect[] m_effects;

	[SerializeField]
	private AudioClip[] m_se;

	[SerializeField]
	private CameraTremble m_ct;

	[SerializeField]
	private GameObject m_dnFab;

	public void PlaySE(Actor a, ActorSE ase) {
		a.audio.PlayOneShot(m_se[(int)ase]);
	}

	public void Spawn(string effectName, Vector3 pos) {
		foreach(Effect e in m_effects){
			if( e.effectName == effectName ) {
				pos.z = -3.0f;
				GameObject.Instantiate(e.effectFab, pos, e.effectFab.transform.rotation);
				break;
			}
		}
	}

	public void Spawn(string effectName, Vector3 actorPos, Vector3 targetPos) {
		foreach(Effect e in m_effects){
			if( e.effectName == effectName ) {
//				Vector3 pos = targetStep.GetActorPos();
				targetPos.z = -3.0f;
				GameObject.Instantiate(e.effectFab, targetPos, e.effectFab.transform.rotation);
				break;
			}
		}
	}

	public void SpawnDamage(int damage, DamageKind k, Vector3 pos) {
		GameObject go = GameObject.Instantiate(m_dnFab, pos, m_dnFab.transform.rotation) as GameObject;
		DamageNumber dn = go.GetComponent<DamageNumber>();
		dn.kind = k;
		dn.damage = damage;
	}

	public void CameraTremble() {
		if(m_ct == null) {
			m_ct = Camera.main.GetComponent<CameraTremble>();
		}
		m_ct.Tremble();
	}
}
