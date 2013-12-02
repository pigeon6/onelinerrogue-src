using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour {

	[SerializeField]
	private GameObject m_initialPosition;

	[SerializeField]
	private CameraScroller m_minScrollTarget;

	[SerializeField]
	private float m_dumping;

	[SerializeField]
	private Vector3 m_targetPos;

	public bool IsScrolling { get { return Vector3.Distance (transform.position, m_targetPos) > 0.1f; } }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		GameManager gm = GameManager.GetManager();
		if( gm != null && gm.currentPlayer != null ) {
			float playerPosX = gm.currentPlayer.transform.position.x;
			float scrollerPosX = m_minScrollTarget.transform.position.x;
			
			float targetX = Mathf.Min(playerPosX, scrollerPosX);
			targetX = Mathf.Min(targetX, m_initialPosition.transform.localPosition.x);
			Vector3 pos = transform.position;
			m_targetPos = new Vector3(targetX, pos.y, pos.z);
			
			transform.position = Vector3.Lerp(pos, m_targetPos, m_dumping);
		}
	}
}
