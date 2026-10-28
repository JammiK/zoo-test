using System.Threading;
using UnityEngine;
using ZW.Views.Interfaces.Actors.World;

namespace ZW.Views.Actors.World
{
    public class WorldContainerView : MonoBehaviour, IWorldContainerView
    {
        [SerializeField] private Transform _levelContainer;
        [SerializeField] private Transform _animalsContainer;
        [SerializeField] private Transform _effectsContainer;

        public Transform LevelContainer => _levelContainer;
        public Transform AnimalsContainer => _animalsContainer;
        public Transform EffectsContainer => _effectsContainer;
        public CancellationToken LifetimeToken => destroyCancellationToken;
    }
}
