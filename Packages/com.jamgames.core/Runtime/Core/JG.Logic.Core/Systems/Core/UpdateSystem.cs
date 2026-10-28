using System;
using System.Collections.Generic;
using Core.JG.Logic.Core.Interfaces.Systems.Core;
using UnityEngine;

namespace Core.JG.Logic.Core.Systems.Core
{
    public class UpdateSystem : MonoBehaviour, IUpdateSystem
    {
        readonly List<Action> _handlers = new List<Action>();
        readonly List<Action> _fixedHandlers = new List<Action>();
        readonly List<Action> _handlersToRemove = new List<Action>();
        readonly List<Action> _fixedHandlersToRemove = new List<Action>();

        public void Add(Action onUpdate)
        {
            if (onUpdate == null)
                return;

            if (_handlers.Contains(onUpdate))
                return;
            
            _handlers.Add(onUpdate);
        }

        public void AddFixed(Action onFixedUpdate)
        {
            if (onFixedUpdate == null)
                return;

            if (_fixedHandlersToRemove.Contains(onFixedUpdate))
                _fixedHandlersToRemove.Remove(onFixedUpdate);
            
            _fixedHandlers.Add(onFixedUpdate);
        }

        public void Remove(Action onUpdate)
        {
            if (onUpdate == null)
                return;

            if (!_handlersToRemove.Contains(onUpdate))
                _handlersToRemove.Add(onUpdate);
        }

        public void RemoveFixed(Action onFixedUpdate)
        {
            if (onFixedUpdate == null)
                return;

            if (!_fixedHandlersToRemove.Contains(onFixedUpdate))
                _fixedHandlersToRemove.Add(onFixedUpdate);
        }

        void TryClearHandlers()
        {
            TryClearHandlers(_handlers, _handlersToRemove);
        }

        void TryClearFixedHandlers()
        {
            TryClearHandlers(_fixedHandlers, _fixedHandlersToRemove);
        }

        void TryClearHandlers(List<Action> handlers, List<Action> handlersToRemove)
        {
            if (handlersToRemove.Count == 0)
                return;

            for (int i = 0; i < handlersToRemove.Count; i++)
            {
                Action handlerToRemove = handlersToRemove[i];
                for (int index = handlers.Count - 1; index >= 0; index--)
                {
                    Action handler = handlers[index];
                    if (handler == handlerToRemove)
                    {
                        handlers.Remove(handler);
                        break;
                    }
                }
            }
        }
        
        void Update()
        {
            TryClearHandlers();

            for (int i = _handlers.Count - 1; i >= 0; i--)
            {
                var handler = _handlers[i];
                handler();
            }
        }
        
        void FixedUpdate()
        {
            TryClearFixedHandlers();

            for (int i = _fixedHandlers.Count - 1; i >= 0; i--)
            {
                var handler = _fixedHandlers[i];
                handler();
            }
        }
    }
}