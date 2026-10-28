using System;
using Core.JG.Views.UI.Core.Interfaces.Buttons;

namespace Core.JG.Presenters.Core.Interfaces.Systems.Common
{
    public interface IButtonClickSystem
    {
        event Action ButtonClicked;
        void Set(IOnClickHolder holder, Action action);
        void Set(Action<Action> OnClick, Action action);
        void Click();
    }
}
