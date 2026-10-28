using UnityEngine;
using ZW.Views.Interfaces.Actors.World;

namespace ZW.Views.Actors.World
{
    public class PhysicsProbeView : MonoBehaviour, IPhysicsProbeView
    {
        [SerializeField] private LayerMask _blockingLayers = ~0;

        public bool IsBoxFree(Vector3 center, Vector3 halfExtents)
        {
            return !Physics.CheckBox(center, halfExtents, Quaternion.identity, _blockingLayers, QueryTriggerInteraction.Ignore);
        }
    }
}
