using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class QuestData_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/Data/QuestData.xls";
	private static readonly string exportPath = "Assets/Data/QuestData.asset";
	private static readonly string[] sheetNames = { "Quest1","Quest2" };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			QuestData data = (QuestData)AssetDatabase.LoadAssetAtPath (exportPath, typeof(QuestData));
			if (data == null) {
				data = ScriptableObject.CreateInstance<QuestData> ();
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

					QuestData.Sheet s = new QuestData.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i< sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						QuestData.Param p = new QuestData.Param ();

						try {
							
					cell = row.GetCell(0); p.id = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.step = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(2); p.condition = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(3); p.once = (cell == null ? false : cell.BooleanCellValue);
					cell = row.GetCell(4); p.format = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(5); p.kind = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(6); p.dialog = (cell == null ? "" : cell.StringCellValue);
					p.gimic = new string[8];
					cell = row.GetCell(7); p.gimic[0] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(8); p.gimic[1] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(9); p.gimic[2] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(10); p.gimic[3] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(11); p.gimic[4] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(12); p.gimic[5] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(13); p.gimic[6] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(14); p.gimic[7] = (cell == null ? "" : cell.StringCellValue);
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
