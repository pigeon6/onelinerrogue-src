using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemLevelData : ScriptableObject
{
	
	public List<Param> list = new List<Param> ();
	
	[System.SerializableAttribute]
	public class Param
	{
		
		public int level;
		public int[] ApMax;
		public int[] DpMax;
		public string[] WpPfx;
		public string[] ShdPfx;
	}
}

