using UnityEngine;

namespace ZW.Views.Interfaces.Actors.Common
{
    public interface IPositionActor
    {
        Vector3 Position { get; }
        void SetPosition(Vector3 position);
    }
}
