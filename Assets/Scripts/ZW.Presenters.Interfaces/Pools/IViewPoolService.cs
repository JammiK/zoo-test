using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using ZW.Views.Interfaces.Common;

namespace ZW.Presenters.Interfaces.Pools
{
    public interface IViewPoolService
    {
        UniTask<T> Get<T>(string address, Transform container, CancellationToken token) where T : class, IPoolableView;
        void Release<T>(string address, T view) where T : class, IPoolableView;
    }
}
