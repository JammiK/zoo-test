using UnityEngine;

namespace IR.Presenters.Interfaces.Services.Resources
{
    public interface IResourcesLoadService
    {
        T Load<T>(string path) where T : Object;
    }
}