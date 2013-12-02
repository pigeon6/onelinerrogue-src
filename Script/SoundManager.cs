using UnityEngine;
using System.Collections;

public enum BGMTrack {
	TitleScene,
	Quest
}

public class SoundManager : MonoBehaviour {

	[SerializeField]
	private AudioSource m_bgm;

	static private SoundManager s_manager;
	
	public static SoundManager GetManager() {
		if( s_manager == null ) {
			SoundManager obj = Component.FindObjectOfType(typeof(SoundManager)) as SoundManager;
			if(obj) {
				s_manager = obj;
			} else {
				GameObject go = new GameObject("SoundManager");
				obj = go.AddComponent<SoundManager>() as SoundManager;
				s_manager = obj;
			}
		}
		return s_manager;
	}
	
	void Awake() {
		SoundManager[] gms = Component.FindObjectsOfType(typeof(SoundManager)) as SoundManager[];
		if(gms.Length > 1) {
			Debug.LogError("SoundManager exists more than one");
			foreach(SoundManager gm in gms) {
				Debug.LogError("[SoundManager] Name: " + gm.gameObject.name);
			}
		}
		if(s_manager != null && s_manager != this) {
			Debug.Log("[DYING] SoundManager:" + gameObject.name + " destroyed");
			Destroy(gameObject);
		} else {
			s_manager = this;
		}
	}

	public int PlayBGM(BGMTrack t) {
		// TODO:
		m_bgm.Play();
		return 0;
	}

	public void PauseBGM(int t) {
		// TODO:
		if(t==0)
			m_bgm.Pause();
	}

	public void StopBGM(int t) {
		// TODO:
		if(t==0)
			m_bgm.Stop();
	}
}
