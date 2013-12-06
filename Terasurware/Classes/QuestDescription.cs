using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestDescription : ScriptableObject
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
		public double episodeNum;
		public string questName;
		public string condition;
		public int[] itemFreq;
		public int[] enemyFreq;
		public int[] gimicFreq;
		public double worldScrollBeginTurn;
		public double worldScrollSpeed;
	}
}

