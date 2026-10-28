namespace Core.JG.Logic.Core.Interfaces.Systems.Core
{
    public interface IManualTimeProvider
    {
        void Increment(int deltaTime);
        void Reset();
    }
}