using UnityEngine;
using System.Collections;
using iGUI;

public class iGUICode_TeamLogo : MonoBehaviour{
	[HideInInspector]
	public iGUILabel label2;
	[HideInInspector]
	public iGUIImage image1;
	[HideInInspector]
	public iGUIButton button1;
	[HideInInspector]
	public iGUILabel label1;
	[HideInInspector]
	public iGUIRoot root1;

	[SerializeField]
	private float m_logoDisplaySec = 1.0f;
	[SerializeField]
	private float m_logoFadeoutSec = 1.0f;

	[SerializeField]
	private string nextscene;

	[SerializeField]
	private AsyncOperation m_loader;

	[SerializeField]
	private iGUIImage m_teamLogoImage;

	static iGUICode_TeamLogo instance;
	void Awake(){
		instance=this;
	}
	public static iGUICode_TeamLogo getInstance(){
		return instance;
	}

	public void button1_Click(iGUIButton caller){
		PlayGame();
	}

	public void PlayGame() {
		if(m_loader == null) {
//			m_loader = Application.LoadLevelAsync(nextscene);
//			m_loader.allowSceneActivation = false;
			animation.Play("TitleAnimation");
		}
	}

	private void _HideLogoImage() {
		m_teamLogoImage.setEnabled(false);
	}

	private void _SetGameCanStart() {
		animation.Stop();
//		StartCoroutine(_WaitAndStartGame());
		_WaitAndStartGame();
	}

	void _WaitAndStartGame() {
//		yield return new WaitForSeconds(.5f);
//		m_loader.allowSceneActivation = true;
		Application.LoadLevel(nextscene);
	}

	IEnumerator Start() { 
		yield return new WaitForSeconds(m_logoDisplaySec);

		float tStart = Time.time;

		while( Time.time - tStart < m_logoFadeoutSec ) {
			float opacity = 1.0f - (Time.time - tStart) / m_logoFadeoutSec;
			image1.opacity = opacity;
			yield return new WaitForEndOfFrame();
		}
		image1.enabled = false;
	}

	void Update() {
		if(Input.GetButtonDown("OK")) {
			PlayGame();
		}
	}
}
