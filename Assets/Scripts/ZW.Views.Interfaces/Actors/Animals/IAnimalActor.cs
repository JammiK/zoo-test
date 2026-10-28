using System;
using UnityEngine;
using ZW.Views.Interfaces.Actors.Common;
using ZW.Views.Interfaces.Common;

namespace ZW.Views.Interfaces.Actors.Animals
{
    public interface IAnimalActor : IPositionActor, IMotionActor, IPoolableView
    {
        void SetCollisionListener(Action<IAnimalActor> onCollided);
        void SetObstacleListener(Action<Vector3> onObstacleHit);
    }
}
