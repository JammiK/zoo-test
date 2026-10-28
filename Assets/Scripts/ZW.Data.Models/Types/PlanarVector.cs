using System;

namespace ZW.Data.Models.Types
{
    [Serializable]
    public struct PlanarVector
    {
        public float X;
        public float Z;

        public PlanarVector(float x, float z)
        {
            X = x;
            Z = z;
        }
    }
}
