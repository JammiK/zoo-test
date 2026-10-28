#if !DISABLE_CSV_HELPER
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Core.JG.Extensions.Editor.Extensions
{
    public class ParseCsvExtensions
    {
        public static List<int> LoadCsvAndGetColumnInt(string fileName, string columnName)
        {
            var lst = new List<int>();

            var strings = LoadCsvAndGetColumn(fileName, columnName);

            if (strings == null)
                return lst;

            foreach (var str in strings)
            {
                if (int.TryParse(str, out int val) == false)
                {
                    Debug.LogError($"Failed to parse string:{str}");
                    continue;
                }
                lst.Add(val);
            }

            return lst;
        }

        public static List<string> LoadCsvAndGetColumn(string fileName, string columnName)
        {
            var textFile = (TextAsset)EditorGUIUtility.Load(fileName);
            if (textFile == null || string.IsNullOrEmpty(textFile.text))
            {
                Debug.LogError($"Failed to load csv:{fileName}");
                return null;
            }

            var parsedCsv = CsvExtensions.GetParsedCsv(textFile.text);
            if (parsedCsv.TryGetValue(columnName, out var column) == false)
            {
                Debug.LogError($"Failed to get column:{columnName} from csv:{fileName}");
                return null;
            }

            Debug.Log($"Csv {fileName} column {columnName} parsed succsessfully");

            return column;
        }

        public static string[] LoadCsvAndGetColumnArray(string fileName, string columnName)
        {
            return LoadCsvAndGetColumn(fileName, columnName).ToArray();
        }

        public static int[] LoadCsvAndGetColumnIntArray(string fileName, string columnName)
        {
            return LoadCsvAndGetColumnInt(fileName, columnName).ToArray();
        }
    }
}
#endif