using UnityEngine;

namespace ZW.Views.Interfaces.Actors.World
{
    public interface IPhysicsProbeView
    {
        bool IsBoxFree(Vector3 center, Vector3 halfExtents);
    }
}
