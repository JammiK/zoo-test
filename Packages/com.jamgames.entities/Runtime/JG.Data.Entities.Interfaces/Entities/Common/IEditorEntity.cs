namespace JG.Data.Entities.Interfaces.Entities.Common
{
    public interface IEditorEntity<T> : IEntity 
        where T : IEntity
    {
        bool Disabled { get; }
        
        T ToEntity();
    }
}