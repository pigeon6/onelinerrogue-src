using UnityEngine;
using System.Collections;

public enum DamageKind {
	EnemyDamage,
	PlayerDamage,
	Cure,
}

public class DamageNumber : MonoBehaviour {

	[SerializeField]
	private tk2dTextMesh m_textMesh;

	[SerializeField]
	private Color[] m_damageColor;

	[SerializeField]
	private float m_damageDisplaySec = 1.0f;

	[SerializeField]
	private Vector3 m_distAmount = new Vector3(1.0f,1.0f, 2.0f);

	[SerializeField]
	private float m_coeff = 1.0f;

	[SerializeField]
	public int damage;

	[SerializeField]
	public DamageKind kind;

	private static GameObject s_dFab;

	// Use this for initialization
	IEnumerator Start () {
		m_textMesh.text = damage.ToString();
		m_textMesh.color = m_damageColor[(int)kind];

		float dir = Random.Range (0, 2) == 0 ? -1.0f : 1.0f;
		Color c = m_textMesh.color;

		float tNow = Time.time;
		while( Time.time - tNow < m_damageDisplaySec ) {
			float rate = (Time.time - tNow) / m_damageDisplaySec;

			float x = m_distAmount.x * rate * rate * dir;
			float y = Mathf.Sin (rate * m_coeff) * m_distAmount.y;
			float z = m_distAmount.z;

			c.a = 1.0f - rate*rate;
			m_textMesh.color = c;
			m_textMesh.transform.localPosition = new Vector3(x,y,z);
			m_textMesh.Commit();

			yield return new WaitForEndOfFrame();
		}
		c.a = 0.0f;
		m_textMesh.color = c;

		Destroy(gameObject);
	}
}
