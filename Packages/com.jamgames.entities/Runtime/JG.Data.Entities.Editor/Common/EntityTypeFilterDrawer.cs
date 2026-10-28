using UnityEditor;
using UnityEngine;

namespace JG.Data.Entities.Editor.Common
{
    [CustomPropertyDrawer(typeof(EntityTypeFilterAttribute))]
    public class EntityTypeFilterDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var entityType = ((EntityTypeFilterAttribute)attribute).EntityType;
            label.tooltip = entityType.Name;

            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.BeginChangeCheck();
            var value = EditorGUI.ObjectField(position, label, property.objectReferenceValue, typeof(SimpleEditorEntityBase), false);
            if (EditorGUI.EndChangeCheck())
            {
                if (value == null || entityType.IsInstanceOfType(value))
                {
                    property.objectReferenceValue = value;
                }
                else
                {
                    Debug.LogWarning($"{value.name} is not {entityType.Name}");
                }
            }

            EditorGUI.EndProperty();
        }
    }
}
