using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Core.JG.Extensions.Editor.Extensions
{
    public static class ObjectExtensions
    {
        public static string GetAddress(this Object value, string groupName = "Default")
        {
            if (value == null)
            {
                return null;
            }

            string path = AssetDatabase.GetAssetPath(value);
            string guid = AssetDatabase.AssetPathToGUID(path);
            var settings = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings;
            var assetEntry = settings.FindAssetEntry(guid);
            if (assetEntry == null)
            {
                var group = settings.FindGroup(groupName) ?? settings.CreateGroup(groupName, false, false, false, null, null);
                assetEntry = settings.CreateOrMoveEntry(guid, group);
                assetEntry.address = path;
                EditorUtility.SetDirty(settings);
                settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, assetEntry, true);
                AssetDatabase.SaveAssets();
            }
            return assetEntry?.address ?? string.Empty;
        }
    }
}