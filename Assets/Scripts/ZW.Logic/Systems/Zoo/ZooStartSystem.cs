using UniRx;
using UnityEngine;
using ZW.Data.Interfaces.Managers.Zoo;
using ZW.Logic.Interfaces.Streams.Zoo;
using ZW.Logic.Interfaces.Systems.Zoo;

namespace ZW.Logic.Systems.Zoo
{
    public class ZooStartSystem : IZooStartSystem
    {
        private readonly IZooManager _zooManager;
        private readonly IZooStartedStream _startedStream;

        public ZooStartSystem(IZooManager zooManager, IZooStartedStream startedStream)
        {
            _zooManager = zooManager;
            _startedStream = startedStream;
        }

        public void Start()
        {
            var removed = _zooManager.RestoreIndex();
            if (removed > 0)
            {
                Debug.LogWarning($"Dropped {removed} saved animals whose species no longer exist in game data");
            }

            _startedStream.OnNext(Unit.Default);
        }
    }
}
