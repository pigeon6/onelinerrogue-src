using UnityEngine;
using System.Collections;

public class Gauge : MonoBehaviour {

	[SerializeField]
	private tk2dSprite m_gaugeBack;

	[SerializeField]
	private tk2dSprite m_gauge;

	[SerializeField]
	private float m_animLengthSec = 1.0f;

	[SerializeField]
	private float m_prePostAnimIntervalSec = .5f;

	[SerializeField]
	private bool m_animating = false;

	public bool IsAnimating {
		get { return m_animating; }
	}

	void Start() {
		m_gaugeBack.renderer.enabled = false;
		m_gauge.renderer.enabled = false;
		SetGaugeValue(1.0f);
	}

	// gauge value: [0.0-1.0]
	// set value immediately
	public void SetGaugeValue(float v) {
		m_gauge.transform.localScale = new Vector3(v, 1.0f,1.0f);
	}

	// gauge value: [0.0-1.0]
	public void ApplyGaugeValueTo(float from, float to) {
		m_animating = true;
		StartCoroutine(_ApplyGaugeAnim(from, to));
	}

	private static float _easeOut(float v) {
		float sv = 1.0f - Mathf.Sin (Mathf.PI/2.0f * v);
		return 1.0f - (sv*sv);
	}

	private IEnumerator _ApplyGaugeAnim(float fromValue, float toValue) {


		toValue = Mathf.Clamp01(toValue);

		m_gauge.transform.localScale = new Vector3(fromValue, 1.0f,1.0f);
		m_gaugeBack.renderer.enabled = true;
		m_gauge.renderer.enabled = true;

		yield return new WaitForSeconds(m_prePostAnimIntervalSec);

		float tNow = Time.time;

		while( Time.time - tNow < m_animLengthSec ) {
			float rate = Time.time - tNow;
			float easedRate = _easeOut(rate);
			float curValue = Mathf.Lerp (fromValue, toValue, easedRate);
			m_gauge.transform.localScale = new Vector3(curValue, 1.0f, 1.0f);
			yield return new WaitForEndOfFrame();
		}

		m_gauge.transform.localScale = new Vector3(toValue, 1.0f,1.0f);

		yield return new WaitForSeconds(m_prePostAnimIntervalSec);

		m_gaugeBack.renderer.enabled = false;
		m_gauge.renderer.enabled = false;
		m_animating = false;
	}
}
