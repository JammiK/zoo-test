using System;

namespace Core.JG.Views.UI.Core.Interfaces.Buttons
{
    public interface IOnClickHolder
    {
        void SetOnClick(Action onClick);
    }
}