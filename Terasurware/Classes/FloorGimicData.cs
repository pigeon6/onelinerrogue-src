using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FloorGimicData : ScriptableObject
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
		public string onFloorEffect;
		public string onFloorAction;
		public string leaveFloorAction;
		public string targetedAction;
		public double statPoison;
		public double statParalize;
		public double fixedDamage;
		public double[] ApMax;
	}
}

