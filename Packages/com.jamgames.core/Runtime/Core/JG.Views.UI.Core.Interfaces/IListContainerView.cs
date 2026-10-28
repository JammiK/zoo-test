using UnityEngine;

namespace Core.JG.Views.UI.Core.Interfaces
{
    public interface IListContainerView
    {
        GameObject Prefab { get; }
        Transform Container { get; }
    }
}