using System;

namespace JG.Logic.Streams.Interfaces
{
    public interface IStream<T> : IObservable<T>, IDisposable
    {
        void OnNext(T obj);
    }
}