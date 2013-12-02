using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class FloorGimicData_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/Data/FloorGimicData.xls";
	private static readonly string exportPath = "Assets/Data/FloorGimicData.asset";
	private static readonly string[] sheetNames = { "FloorGimic" };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			FloorGimicData data = (FloorGimicData)AssetDatabase.LoadAssetAtPath (exportPath, typeof(FloorGimicData));
			if (data == null) {
				data = ScriptableObject.CreateInstance<FloorGimicData> ();
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

					FloorGimicData.Sheet s = new FloorGimicData.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i< sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						FloorGimicData.Param p = new FloorGimicData.Param ();

						try {
							
					cell = row.GetCell(0); p.id = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.name = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(2); p.graphic = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(3); p.onFloorEffect = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(4); p.onFloorAction = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(5); p.leaveFloorAction = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(6); p.targetedAction = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(7); p.statPoison = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(8); p.statParalize = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(9); p.fixedDamage = (cell == null ? 0.0 : cell.NumericCellValue);
					p.ApMax = new double[8];
					cell = row.GetCell(10); p.ApMax[0] = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(11); p.ApMax[1] = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(12); p.ApMax[2] = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(13); p.ApMax[3] = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(14); p.ApMax[4] = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(15); p.ApMax[5] = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(16); p.ApMax[6] = (cell == null ? 0.0 : cell.NumericCellValue);
					cell = row.GetCell(17); p.ApMax[7] = (cell == null ? 0.0 : cell.NumericCellValue);
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
