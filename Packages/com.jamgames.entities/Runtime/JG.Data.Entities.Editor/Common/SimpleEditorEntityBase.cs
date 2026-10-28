using UnityEngine;

namespace JG.Data.Entities.Editor.Common
{
    public abstract class SimpleEditorEntityBase : ScriptableObject
    {
        public abstract string Id { get; set; }
    }
}
