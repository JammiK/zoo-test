using System;
using Core.JG.Data.Interfaces.Services;

namespace Core.JG.Data.Services
{
    public class RandomService : IRandomService
    {
        Random _random;
        public RandomService()
        {
            _random = new Random();
        }

        public int Range(int min, int max)
        {
            var value = _random.Next(min, max);
            return value;
        }

        public float Range(float min, float max)
        {
            var value = min + (max - min) * _random.NextDouble();
            return (float)value;
        }

        public int NextInt()
        {
            var value = _random.Next();
            return value;
        }

        public float NextFloat()
        {
            var value = (float) _random.NextDouble();
            return value;
        }

        public void SetSeed(int seed)
        {
            _random = new Random(seed);
        }
    }
}