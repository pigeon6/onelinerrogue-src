using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class QuestDescription_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/Data/QuestDescription.xls";
	private static readonly string exportPath = "Assets/Data/QuestDescription.asset";
	private static readonly string[] sheetNames = { "QuestDescription" };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			QuestDescription data = (QuestDescription)AssetDatabase.LoadAssetAtPath (exportPath, typeof(QuestDescription));
			if (data == null) {
				data = ScriptableObject.CreateInstance<QuestDescription> ();
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

					QuestDescription.Sheet s = new QuestDescription.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i< sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						QuestDescription.Param p = new QuestDescription.Param ();

						try {
							
					cell = row.GetCell(0); p.id = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.episodeNum = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(2); p.questName = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(3); p.condition = (cell == null ? "" : cell.StringCellValue);
					p.itemFreq = new int[2];
					cell = row.GetCell(4); p.itemFreq[0] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(5); p.itemFreq[1] = (int)(cell == null ? 0 : cell.NumericCellValue);
					p.enemyFreq = new int[2];
					cell = row.GetCell(6); p.enemyFreq[0] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(7); p.enemyFreq[1] = (int)(cell == null ? 0 : cell.NumericCellValue);
					p.gimicFreq = new int[2];
					cell = row.GetCell(8); p.gimicFreq[0] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(9); p.gimicFreq[1] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(10); p.worldScrollBeginTurn = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(11); p.worldScrollSpeed = (cell == null ? 0.0 : cell.NumericCellValue);
						} catch(System.Exception e) {
							Debug.LogWarning ("[WARNING]" + sheetName + " ROW("+i+"): "+ e.Message);
						}						
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
