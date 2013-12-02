using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Emotion : MonoBehaviour {

	public enum Kind {
		None,
		Exclamation,
		Question,
		Berserk,
		Hungry,
		Kirakira,
		Ohana,
		Onpu,
		Paralize,
		Poison,
		Sleep,
		Star,
		Wakatta
	}

	[System.SerializableAttribute]
	class EmotionIcon {
		public tk2dAnimatedSprite emotionChip = null;
		public Kind emotionKind = Kind.None;

		public void SetVisible(bool b) {
			if( emotionChip != null) {
				if( emotionKind == Kind.None ) {
					emotionChip.renderer.enabled = false;
				} else {
					emotionChip.renderer.enabled = b;
				}
			}
		}

		public void Play(Kind k) {
			emotionKind = k;
			if( k == Kind.None ) {
				SetVisible(false);
			} else {
				SetVisible(true);
				string s = k.ToString();
				Debug.Log ("[Emotion] playing " + s);
				emotionChip.Play (s);
			}
		}
		public void Stop() {
			Play (Kind.None);
		}
	}

	[SerializeField]
	private EmotionIcon[] icons;

	[SerializeField]
	private List<Kind> emotionRequests;

	[SerializeField]
	private EmotionIcon onceIcon;

	private const float kEmotionOnceDurationSec = 0.5f; // 3frame in 6fps = 0.5s

	void Start() {
		emotionRequests = new List<Kind>();
		// initialize
		_PlayEmotionOfLastCapables();
		onceIcon.SetVisible(false);
	}

	public void PlayEmotionOnce(Kind k) {
		StartCoroutine(_PlayOnceAnimation(k));
	}

	public void PlayEmotion(Kind k) {

		if( _IsOnceAnimation(k) ) {
			StartCoroutine(_PlayOnceAnimation(k));
		} else {
			if( !emotionRequests.Contains(k) ) {
				emotionRequests.Add(k);
				_PlayEmotionOfLastCapables();
			}
		}
	}

	public void StopEmotion(Kind k) {
		if( emotionRequests.Contains(k) ) {
			emotionRequests.RemoveAll(x => x == k);
			_PlayEmotionOfLastCapables();
		}
	}

	private void _PlayEmotionOfLastCapables() {
		for(int i = emotionRequests.Count; i < icons.Length; ++i) {
			icons[i].Play(Kind.None);
		}
		if( emotionRequests.Count > 0 ) {
			int nEmotions = Mathf.Min (emotionRequests.Count, 3);
			for(int i = emotionRequests.Count - nEmotions, j = 0; i < emotionRequests.Count; ++i, ++j) {
				icons[j].Play (emotionRequests[i]);
			}
		}
	}

	private IEnumerator _PlayOnceAnimation(Kind k) {

		foreach(EmotionIcon icon in icons) {
			icon.SetVisible(false);
		}

		onceIcon.Play (k);
		yield return new WaitForSeconds(kEmotionOnceDurationSec);
		onceIcon.Stop ();

		foreach(EmotionIcon icon in icons) {
			icon.SetVisible(true);
		}
	}

	private bool _IsOnceAnimation (Kind k) {
		switch(k) {
		case Kind.Exclamation:
			return true;
		case Kind.Question:
			return true;
		case Kind.Berserk:
			return false;
		case Kind.Hungry:
			return false;
		case Kind.Kirakira:
			return false;
		case Kind.Ohana:
			return false;
		case Kind.Onpu:
			return true;
		case Kind.Paralize:
			return false;
		case Kind.Poison:
			return false;
		case Kind.Sleep:
			return false;
		case Kind.Star:
			return false;
		case Kind.Wakatta:
			return true;
		}
		return false;
	}
}
