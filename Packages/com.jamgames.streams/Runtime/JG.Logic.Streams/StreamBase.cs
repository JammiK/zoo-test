using System;
using JG.Logic.Streams.Interfaces;
using UniRx;

namespace JG.Logic.Streams
{
    public class StreamBase<T> : IStream<T>
    {
        readonly Subject<T> _subject = new Subject<T>();
        
        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _subject.Subscribe(observer);
        }

        public void OnNext(T obj)
        {
            _subject.OnNext(obj);
        }

        public void Dispose()
        {
            _subject?.Dispose();
        }
    }
}