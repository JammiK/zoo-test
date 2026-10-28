using System;
using System.Collections.Generic;

namespace Core.JG.Presenters.Core.Interfaces.ViewContexts
{
    /// <summary>
    /// Интерфейс, который позволяет запоминать компоненты на сцене
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IActorViewContext<T>
    {
        void Add(T entity);

        bool Remove(T entity);
        int RemoveAll();
        int RemoveAll(Predicate<T> predicate);

        T FindBy(Func<T, bool> predicate);
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAll(Predicate<T> predicate);
        int GetCount();
    }
}