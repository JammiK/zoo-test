using System;
using System.Linq;
using Core.JG.Logic.Core.Interfaces.Systems.Core;
using Cysharp.Threading.Tasks;
using UniRx;
using ZW.Data.Interfaces.Models.Animals;
using ZW.Data.Interfaces.Providers.Animals;
using ZW.Data.Interfaces.Types.Animals;
using ZW.Logic.Interfaces.Streams.Animals;
using ZW.Logic.Interfaces.Systems.FoodChains;
using ZW.Logic.Interfaces.Systems.Movements;
using ZW.Presenters.Extensions;
using ZW.Presenters.Interfaces.Pools;
using ZW.Presenters.Interfaces.Providers.Animals;
using ZW.Views.Interfaces.Actors.Animals;
using ZW.Views.Interfaces.Actors.World;

namespace ZW.Presenters.Presenters.Animals
{
    public class AnimalActorPresenter : IDisposable
    {
        private const float SpawnHeight = 0.5f;

        private readonly IViewPoolService _viewPoolService;
        private readonly IWorldContainerView _worldContainerView;
        private readonly IAnimalActorProvider _actorProvider;
        private readonly IAnimalPositionSyncSystem _positionSyncSystem;
        private readonly IAnimalCollisionSystem _collisionSystem;
        private readonly IAnimalObstacleSystem _obstacleSystem;
        private readonly IUpdateSystem _updateSystem;
        private readonly IDisposable _spawnedSubscription;
        private readonly IDisposable _diedSubscription;

        public AnimalActorPresenter(
            IAnimalProvider animalProvider,
            IAnimalSpawnedStream spawnedStream,
            IAnimalDiedStream diedStream,
            IViewPoolService viewPoolService,
            IWorldContainerView worldContainerView,
            IAnimalActorProvider actorProvider,
            IAnimalPositionSyncSystem positionSyncSystem,
            IAnimalCollisionSystem collisionSystem,
            IAnimalObstacleSystem obstacleSystem,
            IUpdateSystem updateSystem)
        {
            _viewPoolService = viewPoolService;
            _worldContainerView = worldContainerView;
            _actorProvider = actorProvider;
            _positionSyncSystem = positionSyncSystem;
            _collisionSystem = collisionSystem;
            _obstacleSystem = obstacleSystem;
            _updateSystem = updateSystem;

            foreach (var animal in animalProvider.GetAlive())
            {
                SpawnAsync(animal).Forget();
            }

            _spawnedSubscription = spawnedStream.Subscribe(animal => SpawnAsync(animal).Forget());
            _diedSubscription = diedStream.Subscribe(Despawn);
            _updateSystem.AddFixed(SyncPositions);
        }

        public void Dispose()
        {
            _spawnedSubscription.Dispose();
            _diedSubscription.Dispose();
            _updateSystem.RemoveFixed(SyncPositions);
        }

        private async UniTaskVoid SpawnAsync(IAnimalModel animal)
        {
            var actor = await _viewPoolService.Get<IAnimalActor>(
                animal.Entity.Prefab,
                _worldContainerView.AnimalsContainer,
                _worldContainerView.LifetimeToken);

            if (actor == null)
            {
                return;
            }

            if (animal.State != AnimalState.Alive)
            {
                _viewPoolService.Release(animal.Entity.Prefab, actor);
                return;
            }

            actor.SetPosition(animal.Position.ToWorld(SpawnHeight));
            actor.SetCollisionListener(other => OnCollided(animal.Uid, other));
            actor.SetObstacleListener(normal => _obstacleSystem.HitObstacle(animal.Uid, normal.ToPlanar()));
            _actorProvider.Register(animal.Uid, actor);
        }

        private void Despawn(IAnimalModel animal)
        {
            if (!_actorProvider.TryGet(animal.Uid, out var actor))
            {
                return;
            }

            _actorProvider.Unregister(animal.Uid);
            _viewPoolService.Release(animal.Entity.Prefab, actor);
        }

        private void OnCollided(int uid, IAnimalActor other)
        {
            if (_actorProvider.TryGetUid(other, out var otherUid))
            {
                _collisionSystem.Collide(uid, otherUid);
            }
        }

        private void SyncPositions()
        {
            foreach (var (uid, actor) in _actorProvider.All.ToList())
            {
                _positionSyncSystem.Sync(uid, actor.Position.ToPlanar());
            }
        }
    }
}
