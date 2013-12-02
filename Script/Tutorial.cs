using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {

	[SerializeField]
	private Texture2D m_howToPlay;

	[SerializeField]
	private bool m_bProceed;

	public void RegisterTutorialCommands(CommandChain cc) {
		cc.InturrptAdd(new CommandChain.Command(_AfterFadeInHowToPlay));
		cc.InturrptAdd(new CommandChain.Command(_BeginTutorial));
	}

	private void _BeginTutorial(object[] args, CommandChain cc) {
		Debug.Log ("[Tutorial] BeginTutorial");
		GUIManager.GetManager().FadeInImage(m_howToPlay, 0.5f, 1.0f, cc);
	}
	private void _AfterFadeInHowToPlay(object[] args, CommandChain cc) {
		Debug.Log ("[Tutorial] _AfterFadeInHowToPlay");
		StartCoroutine(_HowToPlay(cc));
	}

	public void TutorialButton_Proceed(int value) {
		m_bProceed = true;
	}

	//	private void _AfterFadeInHowToPlay(object[] args, CommandChain cc) {
//		Debug.Log ("[Tutorial] _AfterFadeInHowToPlay");
//		StartCoroutine(_HowToPlay(cc));
//	}

	private IEnumerator _HowToPlay(CommandChain cc) {

		Debug.Log ("[Howtoplay] started");

		yield return new WaitForSeconds(0.5f);

		m_bProceed = false;
		UserInput.GetUserInput().AddToConformCancelEvent(TutorialButton_Proceed,TutorialButton_Proceed);

		Debug.Log ("[Howtoplay] Waiting for key in");
		while ( !m_bProceed ) {
			yield return new WaitForEndOfFrame();
		}

		UserInput.GetUserInput().RemoveFromConformCancelEvent(TutorialButton_Proceed,TutorialButton_Proceed);

		Debug.Log ("[Howtoplay] calling fadeout");
		GUIManager.GetManager().Fadeout(0.5f, 0.0f, cc);

		Destroy (gameObject, 1.0f);
		yield return null;
	}
}
