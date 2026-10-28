using UnityEngine;
using ZW.Data.Models.Types;

namespace ZW.Presenters.Extensions
{
    public static class PlanarVectorWorldExtensions
    {
        public static Vector3 ToWorld(this PlanarVector vector, float height = 0f)
        {
            return new Vector3(vector.X, height, vector.Z);
        }

        public static PlanarVector ToPlanar(this Vector3 vector)
        {
            return new PlanarVector(vector.x, vector.z);
        }
    }
}
