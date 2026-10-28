using System;
using Core.JG.Data.Interfaces.Providers.Time;

namespace Core.JG.Data.Providers.Time
{
    public class TimeProvider : ITimeProvider
    {
        public long GetTime()
        {
            var timestamp = new DateTimeOffset(DateTime.UtcNow)
                .ToUnixTimeMilliseconds();
            return timestamp;
        }

        public int GetTimeS()
        {
            return (int)(GetTime() / 1000);
        }
    }
}