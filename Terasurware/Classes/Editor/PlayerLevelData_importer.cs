using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class PlayerLevelData_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/Data/PlayerLevelData.xls";
	private static readonly string exportPath = "Assets/Data/PlayerLevelData.asset";
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			PlayerLevelData data = (PlayerLevelData)AssetDatabase.LoadAssetAtPath (exportPath, typeof(PlayerLevelData));
			if (data == null) {
				data = ScriptableObject.CreateInstance<PlayerLevelData> ();
				AssetDatabase.CreateAsset ((ScriptableObject)data, exportPath);
				data.hideFlags = HideFlags.NotEditable;
			}
			
			data.list.Clear ();
			using (FileStream stream = File.Open (filePath, FileMode.Open, FileAccess.Read)) {
				IWorkbook book = new HSSFWorkbook (stream);
				ISheet sheet = book.GetSheetAt (0);
				
				for (int i=1; i< sheet.LastRowNum; i++) {
					IRow row = sheet.GetRow (i);
					ICell cell = null;
					
					PlayerLevelData.Param p = new PlayerLevelData.Param ();
					
					cell = row.GetCell(0); p.level = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.hpMax = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(2); p.hungerMax = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(3); p.agility = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(4); p.exp = (int)(cell == null ? 0 : cell.NumericCellValue);
					p.ApMax = new int[8];
					cell = row.GetCell(5); p.ApMax[0] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(6); p.ApMax[1] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(7); p.ApMax[2] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(8); p.ApMax[3] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(9); p.ApMax[4] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(10); p.ApMax[5] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(11); p.ApMax[6] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(12); p.ApMax[7] = (int)(cell == null ? 0 : cell.NumericCellValue);
					p.DpMax = new int[8];
					cell = row.GetCell(13); p.DpMax[0] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(14); p.DpMax[1] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(15); p.DpMax[2] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(16); p.DpMax[3] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(17); p.DpMax[4] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(18); p.DpMax[5] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(19); p.DpMax[6] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(20); p.DpMax[7] = (int)(cell == null ? 0 : cell.NumericCellValue);
					data.list.Add (p);
				}
			}

			ScriptableObject obj = AssetDatabase.LoadAssetAtPath (exportPath, typeof(ScriptableObject)) as ScriptableObject;
			EditorUtility.SetDirty (obj);
		}
	}
}
