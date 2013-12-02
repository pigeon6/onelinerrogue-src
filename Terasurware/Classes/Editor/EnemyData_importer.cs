using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class EnemyData_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/Data/EnemyData.xls";
	private static readonly string exportPath = "Assets/Data/EnemyData.asset";
	private static readonly string[] sheetNames = { "EnemyEntries", };
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			EnemyData data = (EnemyData)AssetDatabase.LoadAssetAtPath (exportPath, typeof(EnemyData));
			if (data == null) {
				data = ScriptableObject.CreateInstance<EnemyData> ();
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

					EnemyData.Sheet s = new EnemyData.Sheet ();
					s.name = sheetName;
				
					for (int i=1; i< sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;
						
						EnemyData.Param p = new EnemyData.Param ();
						
					cell = row.GetCell(0); p.id = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.name = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(2); p.graphic = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(3); p.level = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(4); p.hpMax = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(5); p.flying = (cell == null ? false : cell.BooleanCellValue);
					cell = row.GetCell(6); p.attackEffect = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(7); p.agility = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(8); p.exp = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(9); p.earnExp = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(10); p.throwDistance = (int)(cell == null ? 0 : cell.NumericCellValue);
					p.ApMax = new int[8];
					cell = row.GetCell(11); p.ApMax[0] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(12); p.ApMax[1] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(13); p.ApMax[2] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(14); p.ApMax[3] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(15); p.ApMax[4] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(16); p.ApMax[5] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(17); p.ApMax[6] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(18); p.ApMax[7] = (int)(cell == null ? 0 : cell.NumericCellValue);
					p.DpMax = new int[8];
					cell = row.GetCell(19); p.DpMax[0] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(20); p.DpMax[1] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(21); p.DpMax[2] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(22); p.DpMax[3] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(23); p.DpMax[4] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(24); p.DpMax[5] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(25); p.DpMax[6] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(26); p.DpMax[7] = (int)(cell == null ? 0 : cell.NumericCellValue);
					p.dropitem = new int[2];
					cell = row.GetCell(27); p.dropitem[0] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(28); p.dropitem[1] = (int)(cell == null ? 0 : cell.NumericCellValue);
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
