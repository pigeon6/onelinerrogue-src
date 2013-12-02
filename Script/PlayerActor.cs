using UnityEngine;
using System.Collections;

public class PlayerActor : Actor {

	[System.SerializableAttribute]
	class HungerAlert {
		public float threshold = 0.0f;
		public string message = string.Empty;

		public bool ThresholdCrossed(float now, float last) {
			return now < threshold && last >= threshold;
		}
	}

	[SerializeField]
	private HungerAlert[] m_hungerAlert; // alert, warn, danger

	void Start() {
		Debug.Log ("[Player] initialization");
		// initialize Player to initial level
		LevelDatabase ldb = GameManager.GetManager().LevelDB;
		while(ldb.TestLevelUp(this)) {
			Debug.Log ("[Player] level +1");
			ldb.ApplyLevelUp(this);
		}
	}

	protected override void PerformTurnAction(GameManager gm) {
		
		UserInput ui = UserInput.GetUserInput();
		ui.PushActionEventStack();

		_AddOnGameMenuActions(ui);

		ui.AddToMoveEvent(_OnMoveKey);
	}

	public void DoPutItemOnGround(int itemIndex) {
		UserInput ui = UserInput.GetUserInput();
		ui.PopActionEventStack();
		//EventRecorder.GetManager().RecordPlayerItemPutOnGround(this.step, items[itemIndex]);
		StartCoroutine(Action_PutItemOnGround(itemIndex));
	}

	public void DoPickupItemOnGround() {
		UserInput ui = UserInput.GetUserInput();
		ui.PopActionEventStack();
		//EventRecorder.GetManager().RecordPlayerItemPutOnGround(this.step, items[itemIndex]);
		StartCoroutine(Action_PickupItemOnGround());
	}

	public void DoUseItem(int itemIndex) {
		UserInput ui = UserInput.GetUserInput();
		ui.PopActionEventStack();
		//EventRecorder.GetManager().RecordPlayerItemPutOnGround(this.step, items[itemIndex]);
		StartCoroutine(Action_UseItem(itemIndex));
	}

	public void DoEquipItem(int itemIndex) {
		UserInput ui = UserInput.GetUserInput();
		ui.PopActionEventStack();
		//EventRecorder.GetManager().RecordPlayerItemEquipped(this.step, items[itemIndex]);
		StartCoroutine(Action_EquipItem(itemIndex));
	}

	public void DoThrowItem(int itemIndex) {
		UserInput ui = UserInput.GetUserInput();
		ui.PopActionEventStack();
		//EventRecorder.GetManager().RecordPlayerItemEquipped(this.step, items[itemIndex]);
		StartCoroutine(Action_ThrowItem(itemIndex));
	}

	public void DoStampAndStill() {
		UserInput ui = UserInput.GetUserInput();
		ui.PopActionEventStack();
		//EventRecorder.GetManager().RecordPlayerItemEquipped(this.step, items[itemIndex]);
		StartCoroutine(Action_Stamp());
	}

	public void DoJump() {
		UserInput ui = UserInput.GetUserInput();
		ui.PopActionEventStack();
		//EventRecorder.GetManager().RecordPlayerItemEquipped(this.step, items[itemIndex]);
		StartCoroutine(Action_Jump( m_currentDirection == DirectionType.RIGHT ? -1 : 1 ));
	}

	public void DoRest() {
		//todo:
		Debug.Log ("[PlayerActor][Action] Rest ");
	}


	#region Pre/Post action callbacks
	/*
	 * callback on after move event
	 */ 
	private void OnPreMove() {
		effects.PlaySE(this, ActorSE.Footstep);
	}
	private void OnPostMove() {
		PickupItemOnGround();
	}
	private void OnPostJump() {
		PickupItemOnGround();
	}

	public void OnHungerChanged(float lastHunger) {

		foreach(HungerAlert ha in m_hungerAlert) {
			if(ha.ThresholdCrossed(hunger, lastHunger)) {
				GUIManager.GetManager().Message(ha.message);
			}
		}
	}

	private void OnPostDie() {

		// Don't destroy
		SetVisible(false);

		GUIManager.GetManager().Message(charName + " は しんでしまった・・・");

		GameManager.GetManager().ExitGame();
	}
	#endregion

	#region InputHandlers
	private void _AddOnGameMenuActions(UserInput ui) {
		ui.AddToItemMenuEvent(_ItemMenuButton_OnGame);
		ui.AddToActionMenuEvent(_ActionMenuButton_OnGame);
		ui.AddToStatusMenuEvent(_StatusMenuButton_OnGame);
		ui.AddToJumpEvent(_JumpButton_OnGame);
	}
	
	private void _StatusMenuButton_OnGame(int value) {
		GUIManager.GetManager().ShowStatusWindow();
	}
	
	private void _ItemMenuButton_OnGame(int value) {
		GUIManager.GetManager().ShowItemListWindow();
	}
	
	private void _ActionMenuButton_OnGame(int value) {
		GUIManager.GetManager().ShowGeneralActionWindow();
	}

	private void _JumpButton_OnGame(int value) {
		UserInput ui = UserInput.GetUserInput();
		if( CanJump() ) {
			ui.PopActionEventStack();
			//			EventRecorder.GetManager().RecordPlayerStep(this.step, this.step.GetStep(value));
			StartCoroutine(Action_Jump (value));
		}
	}

	private void _OnMoveKey(int value) {
		UserInput ui = UserInput.GetUserInput();
		if( CanMove(value) ) {
			ui.PopActionEventStack();
			EventRecorder.GetManager().RecordPlayerStep(this.step, this.step.GetStep(value));
			StartCoroutine(Action_Move (value));
		} else {
			Actor a = this.step.GetStep(value).actorOnStep;
			if( a != null && a != this ) {
				ui.PopActionEventStack();
				GUIManager.GetManager().DebugMessage("[Actor][Attack]" + gameObject.name + " attacks " + a.gameObject.name);
				StartCoroutine(Action_Attack (a, ElementType.ET_Physical));
			}
		}
	}
	#endregion
}
