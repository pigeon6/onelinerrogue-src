using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyData : ScriptableObject
{	
	public List<Sheet> sheets = new List<Sheet> ();

	[System.SerializableAttribute]
	public class Sheet
	{
		public string name = string.Empty;
		public List<Param> list = new List<Param>();
	}

	[System.SerializableAttribute]
	public class Param
	{
		
		public int id;
		public string name;
		public string graphic;
		public int level;
		public int hpMax;
		public bool flying;
		public string attackEffect;
		public int agility;
		public int exp;
		public int earnExp;
		public int throwDistance;
		public int[] ApMax;
		public int[] DpMax;
		public int[] dropitem;
	}
}

