using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class QuestRarityData_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/Data/QuestRarityData.xls";
	private static readonly string exportPath = "Assets/Data/QuestRarityData.asset";
	private static readonly string[] sheetNames = { "Quest1","Quest2" };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			QuestRarityData data = (QuestRarityData)AssetDatabase.LoadAssetAtPath (exportPath, typeof(QuestRarityData));
			if (data == null) {
				data = ScriptableObject.CreateInstance<QuestRarityData> ();
				AssetDatabase.CreateAsset ((ScriptableObject)data, exportPath);
				data.hideFlags = HideFlags.NotEditable;
			}
			
			data.sheets.Clear ();
			using (FileStream stream = File.Open (filePath, FileMode.Open, FileAccess.Read)) {
				IWorkbook book = new HSSFWorkbook (stream);
				
				foreach(string sheetName in sheetNames) {
					ISheet sheet = book.GetSheet(sheetName);
					if( sheet == null ) {
						Debug.LogError("[QuestData] sheet not found:" + sheetName);
						continue;
					}

					QuestRarityData.Sheet s = new QuestRarityData.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i< sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						QuestRarityData.Param p = new QuestRarityData.Param ();
						
					cell = row.GetCell(0); p.kind = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(1); p.id = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(2); p.name = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(3); p.minStep = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(4); p.maxStep = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(5); p.rarity = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(6); p.rarityType = (int)(cell == null ? 0 : cell.NumericCellValue);
						s.list.Add (p);
					}
					data.sheets.Add(s);
				}
			}

			ScriptableObject obj = AssetDatabase.LoadAssetAtPath (exportPath, typeof(ScriptableObject)) as ScriptableObject;
			EditorUtility.SetDirty (obj);
		}
	}
}
