using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemData : ScriptableObject
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
		public string kind;
		public string attackEffect;
		public string icon;
		public int selfApplicable;
		public int gainHunger;
		public int gainHitpoint;
		public int[] ap;
		public int[] def;
		public int statPoison;
		public int statParalize;
		public string description;
	}
}

