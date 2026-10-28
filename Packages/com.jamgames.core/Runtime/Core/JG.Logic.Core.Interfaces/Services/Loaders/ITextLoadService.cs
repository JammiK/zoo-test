namespace Core.JG.Logic.Core.Interfaces.Services.Loaders
{
    public interface ITextLoadService
    {
        string Load(string path, bool encrypted, bool optional = false);
    }
}