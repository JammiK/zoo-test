using System.Threading;
using UnityEngine;

namespace ZW.Views.Interfaces.Actors.World
{
    public interface IWorldContainerView
    {
        Transform LevelContainer { get; }
        Transform AnimalsContainer { get; }
        Transform EffectsContainer { get; }
        CancellationToken LifetimeToken { get; }
    }
}
