using IR.Presenters.Interfaces.Services.Resources;
using UnityEngine;

namespace IR.Presenters.Services.Resources
{
    public class ResourcesLoadService : IResourcesLoadService
    {
        public T Load<T>(string path) where T : Object
        {
            return UnityEngine.Resources.Load<T>(path);
        }
    }
}