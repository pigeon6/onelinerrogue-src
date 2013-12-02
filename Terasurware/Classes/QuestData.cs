using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestData : ScriptableObject
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
		public int step;
		public string condition;
		public bool once;
		public string format;
		public string kind;
		public string dialog;
		public string[] gimic;
	}
}

