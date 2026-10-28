using Core.JG.Data.Interfaces.Services;

namespace ZW.Logic.Tests.Fakes
{
    public class FakeRandomService : IRandomService
    {
        public float Value { get; set; }

        public int Range(int min, int max) => min;
        public float Range(float min, float max) => min + (max - min) * Value;
        public int NextInt() => 0;
        public float NextFloat() => Value;
        public void SetSeed(int seed) { }
    }
}
