using System;
using ZW.Data.Models.Types;

namespace ZW.Logic.Extensions
{
    public static class PlanarVectorExtensions
    {
        public static PlanarVector Add(this PlanarVector a, PlanarVector b)
        {
            return new PlanarVector(a.X + b.X, a.Z + b.Z);
        }

        public static PlanarVector Subtract(this PlanarVector a, PlanarVector b)
        {
            return new PlanarVector(a.X - b.X, a.Z - b.Z);
        }

        public static PlanarVector Scale(this PlanarVector vector, float factor)
        {
            return new PlanarVector(vector.X * factor, vector.Z * factor);
        }

        public static float Magnitude(this PlanarVector vector)
        {
            return (float)Math.Sqrt(vector.X * vector.X + vector.Z * vector.Z);
        }

        public static PlanarVector Normalized(this PlanarVector vector)
        {
            var magnitude = vector.Magnitude();
            return magnitude > float.Epsilon ? vector.Scale(1f / magnitude) : default;
        }

        public static PlanarVector FromAngle(float radians)
        {
            return new PlanarVector((float)Math.Cos(radians), (float)Math.Sin(radians));
        }

        public static PlanarVector Rotate(this PlanarVector vector, float radians)
        {
            var cos = (float)Math.Cos(radians);
            var sin = (float)Math.Sin(radians);
            return new PlanarVector(vector.X * cos - vector.Z * sin, vector.X * sin + vector.Z * cos);
        }

        public static float Dot(this PlanarVector a, PlanarVector b)
        {
            return a.X * b.X + a.Z * b.Z;
        }
    }
}
