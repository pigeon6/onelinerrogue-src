using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerLevelData : ScriptableObject
{
	
	public List<Param> list = new List<Param> ();
	
	[System.SerializableAttribute]
	public class Param
	{
		
		public int level;
		public int hpMax;
		public int hungerMax;
		public int agility;
		public int exp;
		public int[] ApMax;
		public int[] DpMax;
	}
}

