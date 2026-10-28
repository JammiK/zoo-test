using System;
using System.Collections.Generic;
using System.Linq;
using Core.JG.Presenters.Core.Interfaces.ViewContexts;

namespace Core.JG.Presenters.Core.ViewContexts
{
    public class ActorViewContext<T> : IActorViewContext<T>
    {
        private readonly List<T> _entities;

        public ActorViewContext(List<T> entities)
        {
            _entities = entities;
        }

        public void Add(T entity)
        {
            _entities.Add(entity);
        }

        public bool Remove(T entity)
        {
            return _entities.Remove(entity);
        }

        public int RemoveAll()
        {
            _entities.Clear();
            return _entities.Count;
        }

        public int RemoveAll(Predicate<T> predicate)
        {
            return _entities.RemoveAll(predicate);
        }

        public T FindBy(Func<T, bool> predicate)
        {
            foreach (var entity in _entities)
            {
                if (predicate(entity))
                {
                    return entity;
                }
            }

            return default;
        }

        public IEnumerable<T> GetAll()
        {
            return _entities;
        }

        public IEnumerable<T> GetAll(Predicate<T> predicate)
        {
            return _entities.Where(e => predicate(e));
        }

        public int GetCount()
        {
            return _entities.Count;
        }
    }
}