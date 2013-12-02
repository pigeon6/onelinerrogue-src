using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class ItemLevelData_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/Data/ItemLevelData.xls";
	private static readonly string exportPath = "Assets/Data/ItemLevelData.asset";
	
	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;
				
			ItemLevelData data = (ItemLevelData)AssetDatabase.LoadAssetAtPath (exportPath, typeof(ItemLevelData));
			if (data == null) {
				data = ScriptableObject.CreateInstance<ItemLevelData> ();
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
					
					ItemLevelData.Param p = new ItemLevelData.Param ();
					
					cell = row.GetCell(0); p.level = (int)(cell == null ? 0 : cell.NumericCellValue);
					p.ApMax = new int[8];
					cell = row.GetCell(1); p.ApMax[0] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(2); p.ApMax[1] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(3); p.ApMax[2] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(4); p.ApMax[3] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(5); p.ApMax[4] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(6); p.ApMax[5] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(7); p.ApMax[6] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(8); p.ApMax[7] = (int)(cell == null ? 0 : cell.NumericCellValue);
					p.DpMax = new int[8];
					cell = row.GetCell(9); p.DpMax[0] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(10); p.DpMax[1] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(11); p.DpMax[2] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(12); p.DpMax[3] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(13); p.DpMax[4] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(14); p.DpMax[5] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(15); p.DpMax[6] = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(16); p.DpMax[7] = (int)(cell == null ? 0 : cell.NumericCellValue);
					p.WpPfx = new string[8];
					cell = row.GetCell(17); p.WpPfx[0] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(18); p.WpPfx[1] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(19); p.WpPfx[2] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(20); p.WpPfx[3] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(21); p.WpPfx[4] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(22); p.WpPfx[5] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(23); p.WpPfx[6] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(24); p.WpPfx[7] = (cell == null ? "" : cell.StringCellValue);
					p.ShdPfx = new string[8];
					cell = row.GetCell(25); p.ShdPfx[0] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(26); p.ShdPfx[1] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(27); p.ShdPfx[2] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(28); p.ShdPfx[3] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(29); p.ShdPfx[4] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(30); p.ShdPfx[5] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(31); p.ShdPfx[6] = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(32); p.ShdPfx[7] = (cell == null ? "" : cell.StringCellValue);
					data.list.Add (p);
				}
			}

			ScriptableObject obj = AssetDatabase.LoadAssetAtPath (exportPath, typeof(ScriptableObject)) as ScriptableObject;
			EditorUtility.SetDirty (obj);
		}
	}
}
