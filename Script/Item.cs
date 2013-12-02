using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour {

	[SerializeField]
	private tk2dSprite 		m_sprite;	// graphic

	[SerializeField]
	private ItemEntity 		m_itemEntry;// item entry

	[SerializeField]
	private Renderer		m_renderer;

	public Step step;	// item position;

	public ItemEntity ItemEntity {
		get { return m_itemEntry; }
	}

	public bool IsVisible {
//		get { return m_renderer.isVisible; }
		get { return true; }
	}

	public string ItemName {
		get { return m_itemEntry.itemName; }
	}

	/*
	 * generate and initialize item
	 */
	public void Initialize(string questName, int step, ItemDatabase db) {
		Initialize(db.GenerateNextItem(questName, step));
	}

	public void Initialize(int itemid, int step, ItemDatabase db) {
		Initialize(db.GenerateItemDirect(itemid));
	}

	public void Initialize(ItemEntity e) {		
		m_itemEntry = e;
		m_sprite.SetSprite(e.icon);
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
