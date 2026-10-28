using System;
using System.Collections.Generic;

namespace Core.JG.Views.UI.Core.Interfaces.Common
{
    public interface IDynamicContainerView<T, U> : IInstantiateContainerView 
        where T : IDestroyable, IRemovable
        where U : IComparable
    {
        void Register(U id, T view);
        T GetView(U id);
        IEnumerable<T> GetViews();
        void Clear();
        void Destroy(U id);
        void DestroyAllMarkedForRemove();
        void MarkAllForRemove();
    }
}