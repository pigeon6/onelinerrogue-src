using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestRarityData : ScriptableObject
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
		
		public string kind;
		public int id;
		public string name;
		public int minStep;
		public int maxStep;
		public int rarity;
		public int rarityType;
	}
}

