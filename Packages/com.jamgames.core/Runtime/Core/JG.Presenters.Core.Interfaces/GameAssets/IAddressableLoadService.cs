using System;
using System.Threading.Tasks;
using Object = UnityEngine.Object;

namespace Core.JG.Presenters.Core.Interfaces.GameAssets
{
    public interface IAddressableLoadService
    {
        IObservable<T> LoadAsync<T>(string path) where T : Object;
        IObservable<T> LoadScriptableObjectAsync<T>(string path);
        Task<T> Load<T>(string path) where T : Object;
        Object GetFromCache(string path);
    }
}
