using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class ItemData_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/Data/ItemData.xls";
	private static readonly string exportPath = "Assets/Data/ItemData.asset";
	private static readonly string[] sheetNames = { "Sheet1" };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			ItemData data = (ItemData)AssetDatabase.LoadAssetAtPath (exportPath, typeof(ItemData));
			if (data == null) {
				data = ScriptableObject.CreateInstance<ItemData> ();
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

					ItemData.Sheet s = new ItemData.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i< sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						ItemData.Param p = new ItemData.Param ();

						try {
							
					cell = row.GetCell(0); p.id = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.name = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(2); p.kind = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(3); p.attackEffect = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(4); p.icon = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(5); p.selfApplicable = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(6); p.gainHunger = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(7); p.gainHitpoint = (int)(cell == null ? 0 : cell.NumericCellValue);
					p.ap = new int[8];
					cell = row.GetCell(8); p.ap[0] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(9); p.ap[1] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(10); p.ap[2] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(11); p.ap[3] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(12); p.ap[4] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(13); p.ap[5] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(14); p.ap[6] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(15); p.ap[7] = (int)(cell == null ? 0 : cell.NumericCellValue);
					p.def = new int[8];
					cell = row.GetCell(16); p.def[0] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(17); p.def[1] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(18); p.def[2] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(19); p.def[3] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(20); p.def[4] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(21); p.def[5] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(22); p.def[6] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(23); p.def[7] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(24); p.statPoison = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(25); p.statParalize = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(26); p.description = (cell == null ? "" : cell.StringCellValue);
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
