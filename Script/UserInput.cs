using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void InputActionHandler(int value);
public delegate void DpadInputActionHandler(int x, int y);

public class UserInput : MonoBehaviour {

	[System.Serializable]
	class InputAction {
		public event InputActionHandler moveButtonEvent;
		public event InputActionHandler confirmButtonEvent;
		public event InputActionHandler cancelButtonEvent;
		public event InputActionHandler itemMenuButtonEvent;
		public event InputActionHandler actionMenuButtonEvent;
		public event InputActionHandler statusMenuButtonEvent;
		public event InputActionHandler jumpButtonEvent;
		public event DpadInputActionHandler dpadButtonEvent;

		public bool IsMoveButtonEventValid {
			get { return moveButtonEvent != null; }
		}
		public bool IsConfirmButtonEventValid {
			get { return confirmButtonEvent != null; }
		}
		public bool IsCancelButtonEventValid {
			get { return cancelButtonEvent != null; }
		}
		public bool IsItemMenuButtonEventValid {
			get { return itemMenuButtonEvent != null; }
		}
		public bool IsActionMenuButtonEventValid {
			get { return actionMenuButtonEvent != null; }
		}
		public bool IsDpadButtonEventValid {
			get { return dpadButtonEvent != null; }
		}
		public bool IsStatusMenuButtonEventValid {
			get { return statusMenuButtonEvent != null; }
		}
		public bool IsJumpButtonEventValid {
			get { return jumpButtonEvent != null; }
		}
		public void MoveButtonEvent(int v) 			{ moveButtonEvent(v); }
		public void ConfirmButtonEvent(int v) 		{ confirmButtonEvent(v); }
		public void CancelButtonEvent(int v) 		{ cancelButtonEvent(v); }
		public void ItemMenuButtonEvent(int v) 		{ itemMenuButtonEvent(v); }
		public void ActionMenuButtonEvent(int v) 	{ actionMenuButtonEvent(v); }
		public void DpadButtonEvent(int x, int y) 	{ dpadButtonEvent(x, y); }
		public void StatusMenuButtonEvent(int v) 	{ statusMenuButtonEvent(v); }
		public void JumpButtonEvent(int v) 			{ jumpButtonEvent(v); }
	}

	[SerializeField]
	private Stack<InputAction> m_actionStack;
	private int m_dpadVLastValue;
	private int m_dpadHLastValue;
	private static UserInput s_manager;

	public void EnsureActionStackReady() {
		if(m_actionStack == null) {
			m_actionStack = new Stack<InputAction>();
		}
		if(m_actionStack.Count == 0) {
			PushActionEventStack();
		}
	}
	public void PushActionEventStack() {
		if(m_actionStack == null) {
			m_actionStack = new Stack<InputAction>();
		}
		m_actionStack.Push (new InputAction());
		Debug.Log ("[UserInput] input event stack push: now " + m_actionStack.Count);
	}

	public void PopActionEventStack() {
		if(m_actionStack != null) {
			m_actionStack.Pop ();
		}
		Debug.Log ("[UserInput] input event stack pop: now " + m_actionStack.Count);
	}

	public void AddToMoveEvent(InputActionHandler h) {
		EnsureActionStackReady();
		m_actionStack.Peek ().moveButtonEvent += h;
	}

	public void AddToDpadEvent(DpadInputActionHandler h) {
		EnsureActionStackReady();
		m_actionStack.Peek ().dpadButtonEvent += h;
	}

	public void AddToConformCancelEvent(InputActionHandler confirm, InputActionHandler cancel) {
		EnsureActionStackReady();
		m_actionStack.Peek ().confirmButtonEvent += confirm;
		m_actionStack.Peek ().cancelButtonEvent += cancel;
	}

	public void AddToItemMenuEvent(InputActionHandler h) {
		EnsureActionStackReady();
		m_actionStack.Peek ().itemMenuButtonEvent += h;
	}

	public void AddToActionMenuEvent(InputActionHandler h) {
		EnsureActionStackReady();
		m_actionStack.Peek ().actionMenuButtonEvent += h;
	}

	public void AddToStatusMenuEvent(InputActionHandler h) {
		EnsureActionStackReady();
		m_actionStack.Peek ().statusMenuButtonEvent += h;
	}

	public void AddToJumpEvent(InputActionHandler h) {
		EnsureActionStackReady();
		m_actionStack.Peek ().jumpButtonEvent += h;
	}

	public void RemoveFromMoveEvent(InputActionHandler h) {
		EnsureActionStackReady();
		m_actionStack.Peek ().moveButtonEvent -= h;
	}
	
	public void RemoveFromConformCancelEvent(InputActionHandler confirm, InputActionHandler cancel) {
		EnsureActionStackReady();
		m_actionStack.Peek ().confirmButtonEvent -= confirm;
		m_actionStack.Peek ().cancelButtonEvent -= cancel;
	}

	public void RemoveFromItemMenuEvent(InputActionHandler h) {
		EnsureActionStackReady();
		m_actionStack.Peek ().itemMenuButtonEvent -= h;
	}
	
	public void RemoveFromActionMenuEvent(InputActionHandler h) {
		EnsureActionStackReady();
		m_actionStack.Peek ().actionMenuButtonEvent -= h;
	}

	public void RemoveFromDpadEvent(DpadInputActionHandler h) {
		EnsureActionStackReady();
		m_actionStack.Peek ().dpadButtonEvent -= h;
	}

	public void RemoveFromStatusMenuEvent(InputActionHandler h) {
		EnsureActionStackReady();
		m_actionStack.Peek ().statusMenuButtonEvent -= h;
	}

	public void RemoveFromJumpEvent(InputActionHandler h) {
		EnsureActionStackReady();
		m_actionStack.Peek ().jumpButtonEvent -= h;
	}

	public static UserInput GetUserInput() {
		if( s_manager == null ) {
			UserInput ui = Component.FindObjectOfType(typeof(UserInput)) as UserInput;
			if(ui) {
				s_manager = ui;
			} else {
				GameObject go = new GameObject("UserInput");
				ui = go.AddComponent<UserInput>() as UserInput;
				s_manager = ui;
			}
		}
		return s_manager;
	}

	void Awake() {
	}

	// Use this for initialization
	void Start () {

		UserInput[] uis = Component.FindObjectsOfType(typeof(UserInput)) as UserInput[];
		if(uis.Length > 1) {
			Debug.LogError("UserInput exists more than one");
			foreach(UserInput ui in uis) {
				Debug.LogError("UserInput:" + ui.gameObject.name);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if( m_actionStack != null && m_actionStack.Count > 0 ) {
			InputAction a = m_actionStack.Peek();

			float h = Input.GetAxisRaw("Horizontal");
			float v = Input.GetAxisRaw("Vertical");
			bool ok = Input.GetButtonDown("OK");
			bool cancel = Input.GetButtonDown("Cancel");
			bool item 	= Input.GetButtonDown("ItemMenu");
			bool action = Input.GetButtonDown("ActionMenu");
			bool jumpLeft	= Input.GetButtonDown("JumpLeft");
			bool jumpRight	= Input.GetButtonDown("JumpRight");
			bool status	= Input.GetButtonDown("StatusMenu");

			// FOR Rokete Show - force reset
			bool exitGame = Input.GetKeyDown(KeyCode.F8);
			if( exitGame ) {
				GameManager.GetManager().ExitGame();
			}

			int intH = 0;
			int intDH = 0;
			int intDV = 0;

			if(h < 0.0f) {
				intH = 1;
				intDH = -1;
			}
			else if(h > 0.0f) {
				intH = -1;
				intDH = 1;
			}
			if(v < 0.0f) {
				intDV = -1;
			}
			else if(v > 0.0f) {
				intDV = 1;
			}

			//Debug.Log ("[Input] intH:" + intH + " intDH:" + intDH + " intDV:" + intDV + " ok:" + ok + " cancel:" + cancel);

			/*
			 *  no event should happen at the same time
			 */
			if(a.IsMoveButtonEventValid && intH != 0) {
				a.MoveButtonEvent(intH);
			}
			else if(a.IsDpadButtonEventValid && 
			   ( (intDH != 0 && m_dpadHLastValue == 0 ) || 
			     (intDV != 0 && m_dpadVLastValue == 0) ) ) 
			{
				a.DpadButtonEvent(intDH, intDV);
			}
			else if(cancel) {
				if(a.IsCancelButtonEventValid) {
					a.CancelButtonEvent(0);
				}
			} 
			
			else if(ok) {
				if(a.IsConfirmButtonEventValid) {
					a.ConfirmButtonEvent(0);
				}
			}

			else if(item) {
				if(a.IsItemMenuButtonEventValid) {
					a.ItemMenuButtonEvent(0);
				}
			} 

			else if(action) {
				if(a.IsActionMenuButtonEventValid) {
					a.ActionMenuButtonEvent(0);
				}
			}

			else if(jumpLeft) {
				if(a.IsJumpButtonEventValid) {
					a.JumpButtonEvent(1);
				}
			}
			else if(jumpRight) {
				if(a.IsJumpButtonEventValid) {
					a.JumpButtonEvent(-1);
				}
			}
			else if(status) {
				if(a.IsStatusMenuButtonEventValid) {
					a.StatusMenuButtonEvent(0);
				}
			}

			m_dpadHLastValue = intDH;
			m_dpadVLastValue = intDV;
		}
	}
}
