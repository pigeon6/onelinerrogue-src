using UnityEngine;
using System.Collections;

public class HideCursor : MonoBehaviour {
 	void Awake () {
#if !UNITY_EDITOR
		Screen.showCursor = false;
#endif
	}	
}
