using UnityEditor;
using UnityEngine;

namespace JG.Data.Entities.Editor.Buttons
{
    public class OpenSaveFolderButton
    {
        [MenuItem("[JG]/Open save folder")]
        public static void Open()
        {
            ShowExplorer(Application.persistentDataPath);
        }

        public static void ShowExplorer(string itemPath)
        {
#if UNITY_EDITOR_WIN
        itemPath = itemPath.Replace(@"/", @"\");   // explorer doesn't like front slashes
        System.Diagnostics.Process.Start("explorer.exe", "/select," + itemPath);
#else
            EditorUtility.RevealInFinder(itemPath);
#endif
        }
    }
}