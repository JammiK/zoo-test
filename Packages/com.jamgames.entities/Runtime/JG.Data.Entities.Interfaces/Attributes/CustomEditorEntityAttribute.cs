using System;
using System.Linq;

namespace JG.Data.Entities.Interfaces.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property)]
    public class CustomEditorEntityAttribute : Attribute
    {
        public Type EditorEntityType;
        public String ConvertSnippet;

        public CustomEditorEntityAttribute(Type editorEntityType, string convertSnippet = "?.ToEntity()")
        {
            EditorEntityType = editorEntityType;
            ConvertSnippet = convertSnippet;
        }
        
        public CustomEditorEntityAttribute(string editorEntityType, string convertSnippet = "?.ToEntity()")
        {
#if UNITY_EDITOR
            ConvertSnippet = convertSnippet;
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetTypes().FirstOrDefault(t => t.FullName.Contains(editorEntityType));
                if (type != null)
                {
                    EditorEntityType = type;
                    return;
                }
            }

            throw new Exception($"Unable to find type {editorEntityType}");
#endif
        }
    }
}