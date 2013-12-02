using UnityEngine;
using System.Collections;

public interface WindowController {

	void Close();
	bool IsOpen();
	void FocusWindow();
	void UnfocusWindow();
}
