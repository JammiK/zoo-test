using System;

namespace Core.JG.Logic.Core.Interfaces.Systems.Core
{
    public interface IUpdateSystem
    {
        void Add(Action onUpdate);
        void AddFixed(Action onFixedUpdate);
        void Remove(Action onUpdate);
        void RemoveFixed(Action onFixedUpdate);
    }
}