using UnityEngine;

namespace ZW.Views.Interfaces.Actors.Common
{
    public interface IMotionActor
    {
        void SetMotion(Vector3 horizontalVelocity, float verticalVelocity);
    }
}
