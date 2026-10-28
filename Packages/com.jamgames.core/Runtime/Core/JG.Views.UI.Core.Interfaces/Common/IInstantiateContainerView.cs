using UnityEngine;

namespace Core.JG.Views.UI.Core.Interfaces.Common
{
    public interface IInstantiateContainerView
    {
        GameObject Prefab { get; }
        Transform Container { get; }
    }
}