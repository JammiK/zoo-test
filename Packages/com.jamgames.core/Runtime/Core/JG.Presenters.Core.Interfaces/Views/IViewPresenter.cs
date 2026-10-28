using Core.JG.Views.UI.Core.Interfaces;
using Core.JG.Views.UI.Core.Interfaces.Common;

namespace Core.JG.Presenters.Core.Interfaces.Views
{
    public interface IViewPresenter<in T, in TU>
        where T : IView<TU>
    {
        void Present(T view, TU model);
    }
    
    public interface IViewPresenter<in T> : IViewPresenter
        where T : IViewBase
    {
        void Present(T view);
    }
    
    public interface IViewPresenter
    {
        
    }
}