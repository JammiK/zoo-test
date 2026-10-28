using UnityEngine;
using ZW.Data.Models.Types;
using ZW.Logic.Interfaces.Services.Spawns;
using ZW.Presenters.Extensions;
using ZW.Views.Interfaces.Actors.World;

namespace ZW.Presenters.Services.Spawns
{
    public class PhysicsSpawnPointValidator : ISpawnPointValidator
    {
        private const float ProbeCenterHeight = 0.6f;
        private const float ProbeHalfHeight = 0.45f;

        private readonly IPhysicsProbeView _probeView;

        public PhysicsSpawnPointValidator(IPhysicsProbeView probeView)
        {
            _probeView = probeView;
        }

        public bool IsFree(PlanarVector point, float radius)
        {
            return _probeView.IsBoxFree(point.ToWorld(ProbeCenterHeight), new Vector3(radius, ProbeHalfHeight, radius));
        }
    }
}
