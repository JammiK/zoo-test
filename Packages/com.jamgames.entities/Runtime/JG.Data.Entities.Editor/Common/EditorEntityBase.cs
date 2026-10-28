using JG.Data.Entities.Interfaces.Entities.Common;
//using NaughtyAttributes;
using UnityEngine;

namespace JG.Data.Entities.Editor.Common
{
    public abstract class EditorEntityBase<T> : SimpleEditorEntityBase, IEditorEntity<T>
        where T : IEntity
    {
        /*[ShowIf("ShowId")]*/ [SerializeField] protected string _id;

        [SerializeField] bool _disabled;

        public override string Id
        {
            get => _id;
            set => _id = value;
        }
        public bool Disabled => _disabled;
        public abstract T ToEntity();


#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (string.IsNullOrEmpty(_id))
            {
                _id = name;
            }
        }

        protected virtual bool ShowId() => true;
#endif
    }
}