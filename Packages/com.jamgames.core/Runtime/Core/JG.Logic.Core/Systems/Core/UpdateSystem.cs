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
            AddHandler(_handlers, _handlersToRemove, onUpdate);
        }

        public void AddFixed(Action onFixedUpdate)
        {
            AddHandler(_fixedHandlers, _fixedHandlersToRemove, onFixedUpdate);
        }

        public void Remove(Action onUpdate)
        {
            RemoveHandler(_handlersToRemove, onUpdate);
        }

        public void RemoveFixed(Action onFixedUpdate)
        {
            RemoveHandler(_fixedHandlersToRemove, onFixedUpdate);
        }

        static void AddHandler(List<Action> handlers, List<Action> handlersToRemove, Action handler)
        {
            if (handler == null)
                return;

            handlersToRemove.Remove(handler);

            if (!handlers.Contains(handler))
                handlers.Add(handler);
        }

        static void RemoveHandler(List<Action> handlersToRemove, Action handler)
        {
            if (handler != null && !handlersToRemove.Contains(handler))
                handlersToRemove.Add(handler);
        }

        static void ApplyRemovals(List<Action> handlers, List<Action> handlersToRemove)
        {
            for (int i = 0; i < handlersToRemove.Count; i++)
            {
                handlers.Remove(handlersToRemove[i]);
            }

            handlersToRemove.Clear();
        }

        static void Invoke(List<Action> handlers, List<Action> handlersToRemove)
        {
            ApplyRemovals(handlers, handlersToRemove);

            for (int i = handlers.Count - 1; i >= 0; i--)
            {
                if (i >= handlers.Count)
                    continue;

                var handler = handlers[i];
                if (handlersToRemove.Contains(handler))
                    continue;

                try
                {
                    handler();
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }

        void Update()
        {
            Invoke(_handlers, _handlersToRemove);
        }

        void FixedUpdate()
        {
            Invoke(_fixedHandlers, _fixedHandlersToRemove);
        }
    }
}
