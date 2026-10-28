using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using ZW.Presenters.Interfaces.Pools;
using ZW.Presenters.Pools;
using ZW.Views.Interfaces.Common;

namespace ZW.Presenters.Tests.Pools
{
    public class ViewPoolServiceTests
    {
        private FakePoolFactory _factory;
        private ViewPoolService _service;

        [SetUp]
        public void SetUp()
        {
            _factory = new FakePoolFactory();
            _service = new ViewPoolService(_factory);
        }

        [Test]
        public void SameAddress_UsesOnePool()
        {
            var view = Get("frog");
            _service.Release("frog", view);

            Assert.AreSame(view, Get("frog"));
            Assert.AreEqual(1, _factory.Addresses.Count);
        }

        [Test]
        public void DifferentAddresses_UseSeparatePools()
        {
            var frog = Get("frog");
            _service.Release("frog", frog);

            var snake = Get("snake");

            Assert.AreNotSame(frog, snake);
            CollectionAssert.AreEqual(new[] { "frog", "snake" }, _factory.Addresses);
        }

        private FakeView Get(string address)
        {
            return _service.Get<FakeView>(address, null, CancellationToken.None).GetAwaiter().GetResult();
        }

        private class FakeView : IPoolableView
        {
            public void SetActive(bool active)
            {
            }
        }

        private class FakePool<T> : ObjectPoolBase<T> where T : class, IPoolableView
        {
            protected override UniTask<T> Create(CancellationToken token)
            {
                return UniTask.FromResult((T)(object)new FakeView());
            }
        }

        private class FakePoolFactory : IViewPoolFactory
        {
            public List<string> Addresses { get; } = new();

            public IObjectPool<T> Create<T>(string address, Transform container) where T : class, IPoolableView
            {
                Addresses.Add(address);
                return new FakePool<T>();
            }
        }
    }
}
