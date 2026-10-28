using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using ZW.Presenters.Pools;

namespace ZW.Presenters.Tests.Pools
{
    public class ObjectPoolBaseTests
    {
        private CountingPool _pool;

        [SetUp]
        public void SetUp()
        {
            _pool = new CountingPool();
        }

        [Test]
        public void EmptyPool_CreatesNewItem()
        {
            var item = Get();

            Assert.IsNotNull(item);
            Assert.AreEqual(1, _pool.Created);
            Assert.IsTrue(item.Active);
        }

        [Test]
        public void ReleasedItem_IsReused()
        {
            var first = Get();
            _pool.Release(first);

            var second = Get();

            Assert.AreSame(first, second);
            Assert.AreEqual(1, _pool.Created);
        }

        [Test]
        public void Release_DeactivatesAndStoresItem()
        {
            var item = Get();

            _pool.Release(item);

            Assert.IsFalse(item.Active);
            Assert.AreEqual(1, _pool.FreeCount);
        }

        [Test]
        public void ItemsInUse_AreNotShared()
        {
            var first = Get();
            var second = Get();

            Assert.AreNotSame(first, second);
            Assert.AreEqual(2, _pool.Created);
        }

        private Item Get()
        {
            return _pool.Get(CancellationToken.None).GetAwaiter().GetResult();
        }

        private class Item
        {
            public bool Active { get; set; }
        }

        private class CountingPool : ObjectPoolBase<Item>
        {
            public int Created { get; private set; }

            protected override UniTask<Item> Create(CancellationToken token)
            {
                Created++;
                return UniTask.FromResult(new Item());
            }

            protected override void OnGet(Item item)
            {
                item.Active = true;
            }

            protected override void OnRelease(Item item)
            {
                item.Active = false;
            }
        }
    }
}
