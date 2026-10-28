using UnityEngine;
using ZW.Views.Interfaces.Common;

namespace ZW.Presenters.Interfaces.Pools
{
    public interface IViewPoolFactory
    {
        IObjectPool<T> Create<T>(string address, Transform container) where T : class, IPoolableView;
    }
}
