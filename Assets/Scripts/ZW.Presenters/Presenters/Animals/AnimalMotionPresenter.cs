using System;
using UniRx;
using ZW.Logic.Interfaces.Streams.Animals;
using ZW.Logic.Models.Movements;
using ZW.Presenters.Extensions;
using ZW.Presenters.Interfaces.Providers.Animals;

namespace ZW.Presenters.Presenters.Animals
{
    public class AnimalMotionPresenter : IDisposable
    {
        private readonly IAnimalActorProvider _actorProvider;
        private readonly IDisposable _motionSubscription;

        public AnimalMotionPresenter(IAnimalMotionStream motionStream, IAnimalActorProvider actorProvider)
        {
            _actorProvider = actorProvider;
            _motionSubscription = motionStream.Subscribe(ApplyMotion);
        }

        public void Dispose()
        {
            _motionSubscription.Dispose();
        }

        private void ApplyMotion(AnimalMotionCommand command)
        {
            if (_actorProvider.TryGet(command.Uid, out var actor))
            {
                actor.SetMotion(command.Motion.Velocity.ToWorld(), command.Motion.VerticalVelocity);
            }
        }
    }
}
