namespace Core.JG.Views.UI.Core.Interfaces.Buttons
{
    public interface IDynamicView<T> : IView<T>
    {
        void SelfDestroy();
    }
}