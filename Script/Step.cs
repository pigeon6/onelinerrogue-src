using UnityEngine;
using System.Collections;

public class Step : ScriptableObject {

	public Actor 	actorOnStep;
	public Item 	itemOnStep;
	public FloorGimic floorGimicOnStep;
	public int 		index;
	public Map 		map;

	public Step GetStep(int offset) {
		int pos = Mathf.Clamp (index + offset, 0, map.steps.Length-1);
		return map.steps[pos];
	}

	public Actor GetActor(int offset) {
		int pos = Mathf.Clamp (index + offset, 0, map.steps.Length-1);
		return map.steps[pos].actorOnStep;
	}

	public void SetActor(Actor a) {
		actorOnStep = a;
		actorOnStep.step = this;
		actorOnStep.transform.position = GetActorPos();
	}

	public void SetFloorGimic(FloorGimic fg) {
		floorGimicOnStep = fg;
		floorGimicOnStep.step = this;
		floorGimicOnStep.transform.position = GetFloorGimicPos();
	}

	public Item GetItem(int offset) {
		int pos = Mathf.Clamp (index + offset, 0, map.steps.Length-1);
		return map.steps[pos].itemOnStep;
	}

	public void SetItem(Item i) {
		itemOnStep = i;
		itemOnStep.step = this;
		itemOnStep.transform.position = GetItemPos();
	}

	public void SetItem(ItemEntity e) {
		itemOnStep = map.CreateItem(e);
		itemOnStep.step = this;
		itemOnStep.transform.position = GetItemPos();
	}

	public Vector3 GetActorPos() {
		return map.GetActorStepPosition(index);
	}

	public Vector3 GetItemPos() {
		return map.GetItemStepPosition(index);
	}

	public Vector3 GetFloorGimicPos() {
		return map.GetFloorGimicStepPosition(index);
	}
}
