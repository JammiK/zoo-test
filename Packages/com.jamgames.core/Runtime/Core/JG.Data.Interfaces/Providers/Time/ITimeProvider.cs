namespace Core.JG.Data.Interfaces.Providers.Time
{
    public interface ITimeProvider
    {
        long GetTime();
        int GetTimeS();
    }
}