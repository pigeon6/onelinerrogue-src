using UnityEngine;
using System.Collections;

public enum BGMTrack {
	TitleScene,
	Quest,
	QuestClear,
}

public class SoundManager : MonoBehaviour {

	[SerializeField]
	private AudioSource[] m_sources;

	[SerializeField]
	private AudioSource m_current;

	[SerializeField]
	private AudioClip[] m_tracks;

	[SerializeField]
	[Range(0.0f,1.0f)]
	private float m_volume;

	private BGMTrack m_currentTrack;

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

		m_sources = new AudioSource[2];
		for(int i=0; i<2;++i) {
			m_sources[i] = gameObject.AddComponent<AudioSource>() as AudioSource;
			m_sources[i].volume = 0.0f;
			m_sources[i].loop = true;
		}
		m_current = m_sources[0];
	}

	public void PlayBGM(BGMTrack t) {
		StopBGM();
		m_current.clip = m_tracks[(int)t];
		m_current.volume = m_volume;
		m_current.Play();
		m_currentTrack = t;
	}

	public void PauseBGM(bool bPause) {
		foreach(AudioSource src in m_sources) {
			if(bPause) {
				src.Pause ();
			} else {
				src.Play ();
			}
		}
	}
	
	public void StopBGM() {
		foreach(AudioSource src in m_sources) {
			src.Stop ();
		}
	}

	public void FadeinBGM(float fadeSec = 1.0f) {
		StartCoroutine(_FadeIn(m_currentTrack,fadeSec));
	}

	public void FadeinBGM(BGMTrack t, float fadeSec = 1.0f) {
		if( m_currentTrack != t ) {
			StartCoroutine(_FadeIn(t,fadeSec));
		}
	}

	public void FadeoutBGM(float fadeSec = 1.0f) {
		StartCoroutine(_FadeOut(fadeSec));
	}

	public void CrossFade(BGMTrack t, float fadeSec = 1.0f) {
		StartCoroutine(_CrossFade(t, fadeSec));
	}

	private IEnumerator _FadeOut(float fadeSec) {

		float tStart = Time.time;

		AudioSource src = m_current;

		float initialVolume = src.volume;

		while( Time.time - tStart < fadeSec ) {
			float rate = (Time.time - tStart) / fadeSec;
			src.volume = initialVolume * (1.0f - rate);
			yield return new WaitForEndOfFrame();
		}
		src.volume = 0.0f;
		src.Pause ();
	}

	private IEnumerator _FadeIn(BGMTrack t, float fadeSec) {
		if( m_current == null ) {
			m_current = m_sources[0];
		}
		AudioSource src = m_current;

		src.clip = m_tracks[(int)t];
		src.volume = 0.0f;
		src.Play ();

		float tStart = Time.time;
		
		while( Time.time - tStart < fadeSec ) {
			float rate = (Time.time - tStart) / fadeSec;
			src.volume = m_volume * rate;
			yield return new WaitForEndOfFrame();
		}
		m_current.volume = m_volume;
		m_currentTrack = t;
	}

	private IEnumerator _CrossFade(BGMTrack t, float fadeSec) {

		AudioSource fadeout = m_current;
		AudioSource fadein = (m_current == m_sources[0])? m_sources[1]:m_sources[0];

		fadein.clip = m_tracks[(int)t];
		fadein.volume = 0.0f;
		fadein.Play ();
		
		float initialVolume = fadeout.volume;
		float tStart = Time.time;
		
		while( Time.time - tStart < fadeSec ) {
			float rate = (Time.time - tStart) / fadeSec;
			fadein.volume = m_volume * rate;
			fadeout.volume = initialVolume * (1.0f - rate);
			yield return new WaitForEndOfFrame();
		}
		m_current = fadein;
		m_current.volume = m_volume;
		m_currentTrack = t;

		fadeout.Pause();
	}

}
