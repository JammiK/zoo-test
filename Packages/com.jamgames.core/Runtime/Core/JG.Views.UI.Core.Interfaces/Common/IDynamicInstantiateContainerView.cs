using System;
using System.Collections.Generic;

namespace Core.JG.Views.UI.Core.Interfaces.Common
{
    /// <summary>
    /// Obsolete
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Obsolete("Use IDynamicContainerView")]
    public interface IDynamicInstantiateContainerView<T> : IInstantiateContainerView where T : IDestroyable
    {
        void Register(string id, T view);
        T GetView(string id);
        IEnumerable<T> GetViews();
        void Clear();
        void Destroy(string id);
        void DestroyAllMarkedForRemove();
        void MarkAllForRemove();
    }
}


