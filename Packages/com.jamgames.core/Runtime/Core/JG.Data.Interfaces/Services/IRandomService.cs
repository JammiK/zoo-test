namespace Core.JG.Data.Interfaces.Services
{
    public interface IRandomService
    {
        /// <summary>
        /// Inclusive [min], exclusive [max]
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        int Range(int min, int max);
        float Range(float min, float max);
        int NextInt();
        float NextFloat();
        void SetSeed(int seed);
    }
}