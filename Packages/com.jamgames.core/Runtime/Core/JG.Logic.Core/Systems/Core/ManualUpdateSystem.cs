using System;
using System.Collections.Generic;
using Core.JG.Logic.Core.Interfaces.Systems.Core;

namespace Core.JG.Logic.Core.Systems.Core
{
    public class ManualUpdateSystem : IManualUpdateSystem
    {
        readonly IManualTimeProvider _manualTimeProvider;

        public ManualUpdateSystem(IManualTimeProvider manualTimeProvider)
        {
            _manualTimeProvider = manualTimeProvider;
        }

        readonly List<Action> _onUpdateActions = new List<Action>();
        readonly List<Action> _onFixedUpdateActions = new List<Action>();
        public void Add(Action onUpdate)
        {
            _onUpdateActions.Add(onUpdate);
        }

        public void AddFixed(Action onFixedUpdate)
        {
            _onFixedUpdateActions.Add(onFixedUpdate);
        }

        public void Remove(Action onUpdate)
        {
            throw new NotImplementedException();
        }

        public void RemoveFixed(Action onFixedUpdate)
        {
            throw new NotImplementedException();
        }


        public void Increment(int deltaTime)
        {
            _manualTimeProvider.Increment(deltaTime);
            foreach (var onUpdateAction in _onUpdateActions)
            {
                onUpdateAction.Invoke();
            }

            foreach (var onFixedUpdateAction in _onFixedUpdateActions)
            {
                onFixedUpdateAction.Invoke();
            }
        }
    }
}