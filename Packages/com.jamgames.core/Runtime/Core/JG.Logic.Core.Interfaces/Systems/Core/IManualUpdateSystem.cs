namespace Core.JG.Logic.Core.Interfaces.Systems.Core
{
    public interface IManualUpdateSystem : IUpdateSystem
    {
        void Increment(int deltaTime);
    }
}