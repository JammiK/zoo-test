using System;
using Core.JG.Views.UI.Core.Interfaces.Common;
using ZW.Views.Interfaces.Actors.Common;

namespace ZW.Views.Interfaces.Actors.Animals
{
    public interface IAnimalActor : IPositionActor, IMotionActor, ILabelActor, IDestroyable
    {
        void SetCollisionListener(Action<IAnimalActor> onCollided);
    }
}
