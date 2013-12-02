using UnityEngine;
using System.Collections;

public class FullscreenEffectController : MonoBehaviour {

	[SerializeField]
	private iGUI.iGUIImage m_image;

	[SerializeField]
	private Texture2D m_white;

	public void FadeoutFromBlack(float tFadeSec, float tFadeDelaySec = 0.0f, CommandChain cc = null) {
		StartCoroutine(_Fade(1.0f,0.0f, m_white, true, Color.black,tFadeSec,tFadeDelaySec, cc));
	}
	public void FadeinToBlack(float tFadeSec, float tFadeDelaySec = 0.0f, CommandChain cc = null) {
		StartCoroutine(_Fade(0.0f,1.0f, m_white, true, Color.black,tFadeSec,tFadeDelaySec, cc));
	}

	public void FadeInImage(Texture img, float tFadeSec, float tFadeDelaySec = 0.0f, CommandChain cc = null) {
		StartCoroutine(_Fade(0.0f,1.0f, img, true, Color.white,tFadeSec,tFadeDelaySec, cc));
	}

	public void Fadeout(float tFadeSec, float tFadeDelaySec = 0.0f, CommandChain cc = null) {
		StartCoroutine(_Fade(1.0f,0.0f, null, false, Color.white, tFadeSec,tFadeDelaySec, cc));
	}

	private IEnumerator _Fade(float from, float to, Texture img, bool applyBgColor, Color bgColor, float tFadeSec, float tFadeDelaySec, CommandChain cc) {


		if( applyBgColor ) {
			m_image.setBackgroundColor(bgColor);
		}
		if(img != null) {
			m_image.image = img;
		}
		m_image.opacity = from;
		m_image.enabled = true;

		if( tFadeDelaySec > 0.0f ) {
			yield return new WaitForSeconds(tFadeDelaySec);
		}

		float tStart = Time.time;

		while( Time.time - tStart < tFadeSec ) {
			float t = (Time.time - tStart) / tFadeSec;
			float v = (to - from) * t;
			if( to < from && v <= 0.0f) v = 1.0f + v; // v is negative
			m_image.opacity = v;
			yield return new WaitForEndOfFrame();
		}

		m_image.opacity = to;
		m_image.enabled = to > 0.0f;

		if(cc) {
			cc.FireCommand();
		}
	}
}
