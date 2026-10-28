using Core.JG.Logic.Core.Interfaces.Systems.Core;

namespace Core.JG.Logic.Core.Systems.Core
{
    public class ManualTimeProvider : IManualTimeProvider
    {
        long _time;
        public long GetTime()
        {
            return _time;
        }

        public int GetTimeS()
        {
            return (int)_time;
        }
        
        public void Increment(int deltaTime)
        {
            _time += deltaTime;
        }

        public void Reset()
        {
            _time = 0;
        }
    }
}