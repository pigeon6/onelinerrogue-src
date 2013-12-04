using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GUISE {
	ButtonConfirm,
	ButtonCancel,
	WindowOpen,
	WindowClose,
	ButtonNotAvailable,
	Select,
	QuestClear,
	Terop
}

public class GUIManager : MonoBehaviour {

	public StatusGUI 	statusGui;
	public MessageGUI 	messageGui;
	public MenuGUI 		menuGui;
	public DebugMessageGUI debugMessageGui;

	[SerializeField]
	private FullscreenEffectController m_fsControl; 

	[SerializeField]
	private AudioClip[] m_guiSE;
	
	[System.SerializableAttribute]
	class MenuItemIcon {
		public string 			icon  = string.Empty;
		public Texture2D		image = null;
	}
	
	[SerializeField]
	private MenuItemIcon[] m_itemIcons;

	static private GUIManager s_manager;
	
	public static GUIManager GetManager() {
		if( s_manager == null ) {
			GUIManager obj = Component.FindObjectOfType(typeof(GUIManager)) as GUIManager;
			if(obj) {
				s_manager = obj;
			} else {
				Debug.LogError("GUIManager does not exist in scene");
			}
		}
		return s_manager;
	}
	
	void Awake() {
		GUIManager[] gms = Component.FindObjectsOfType(typeof(GUIManager)) as GUIManager[];
		if(gms.Length > 1) {
			Debug.LogError("GUIManager exists more than one");
			foreach(GUIManager gm in gms) {
				Debug.LogError("[GUIManager] Name: " + gm.gameObject.name);
			}
		}
		if(s_manager != null && s_manager != this) {
			Debug.Log("[DYING] GUIManager:" + gameObject.name + " destroyed");
			Destroy(gameObject);
		}
	}

	public Texture GetIconGraphicOfItem(ItemEntity e) {

		foreach(MenuItemIcon i in m_itemIcons) {
			if(i.icon == e.icon) {
				return i.image;
			}
		}

		return m_itemIcons[0].image;
	}

	public void PlayGUISE(GUISE se) {
		audio.PlayOneShot(m_guiSE[(int)se]);
	}
	
	public void Message(string msg) {
		messageGui.AppendMessage(msg);
	}

	public void MessageAndWait(string msg, InputActionHandler h = null) {
		messageGui.AppendMessage(msg);
		menuGui.ShowIndicatorAndWait(h);
		// pbcontrol.show&wait
	}

	public void DebugMessage(string msg) {
		debugMessageGui.AppendMessage(msg);
	}

	public void HideTopWindow() {
		if( !audio.isPlaying ) {
			PlayGUISE(GUISE.WindowClose);
		}
		menuGui.HideTopWindow();
	}
	
	public bool IsAnyMenuWindowOpen() {
		return menuGui.IsAnyMenuWindowOpen();
	}
	
	public void ShowItemListWindow() {
		PlayGUISE(GUISE.WindowOpen);
		menuGui.ShowItemListWindow();
	}
	
	public void ShowGeneralActionWindow() {
		PlayGUISE(GUISE.WindowOpen);
		menuGui.ShowGeneralActionWindow();
	}
	
	public void ShowItemActionWindow() {
		PlayGUISE(GUISE.WindowOpen);
		menuGui.ShowItemActionWindow();
	}

	public void ShowStatusWindow() {
		PlayGUISE(GUISE.WindowOpen);
		menuGui.ShowStatusWindow();
	}

	public void ShowIndicatorAndWait() {
		PlayGUISE(GUISE.WindowOpen);
		menuGui.ShowIndicatorAndWait();
	}
	
	public void FadeoutFromBlack(float tFadeSec, float tFadeDelaySec = 0.0f, CommandChain cc = null) {
		m_fsControl.FadeoutFromBlack(tFadeSec, tFadeDelaySec, cc);
	}

	public void FadeinToBlack(float tFadeSec, float tFadeDelaySec = 0.0f, CommandChain cc = null) {
		m_fsControl.FadeinToBlack(tFadeSec, tFadeDelaySec, cc);
	}

	public void FadeInImage(Texture img, float tFadeSec, float tFadeDelaySec = 0.0f, CommandChain cc = null) {
		m_fsControl.FadeInImage(img, tFadeSec, tFadeDelaySec, cc);
	}

	public void Fadeout(float tFadeSec, float tFadeDelaySec = 0.0f, CommandChain cc = null) {
		m_fsControl.Fadeout(tFadeSec, tFadeDelaySec, cc);
	}
	public void DoCutsceneWithMessages(List<string> msgs,InputActionHandler h) {
		menuGui.DoCutsceneWithMessages(msgs,h);
	}
	public void DoTerop(string msg,InputActionHandler h) {
		menuGui.DoTerop(msg,h);
	}
	public void DoQuestClear(string questName,InputActionHandler h) {
		menuGui.DoQuestClear(questName,h);
	}
}
