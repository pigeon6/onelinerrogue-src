using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class FloorGimicSetData_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/Data/FloorGimicSetData.xls";
	private static readonly string exportPath = "Assets/Data/FloorGimicSetData.asset";
	private static readonly string[] sheetNames = { "FloorGimicSet" };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			FloorGimicSetData data = (FloorGimicSetData)AssetDatabase.LoadAssetAtPath (exportPath, typeof(FloorGimicSetData));
			if (data == null) {
				data = ScriptableObject.CreateInstance<FloorGimicSetData> ();
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

					FloorGimicSetData.Sheet s = new FloorGimicSetData.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i< sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						FloorGimicSetData.Param p = new FloorGimicSetData.Param ();

						try {
							
					cell = row.GetCell(0); p.id = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.name = (cell == null ? "" : cell.StringCellValue);
					p.gimic = new string[16];
					cell = row.GetCell(3); p.gimic[0] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(4); p.gimic[1] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(5); p.gimic[2] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(6); p.gimic[3] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(7); p.gimic[4] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(8); p.gimic[5] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(9); p.gimic[6] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(10); p.gimic[7] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(11); p.gimic[8] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(12); p.gimic[9] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(13); p.gimic[10] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(14); p.gimic[11] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(15); p.gimic[12] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(16); p.gimic[13] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(17); p.gimic[14] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(18); p.gimic[15] = (cell == null ? "" : cell.StringCellValue);
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
