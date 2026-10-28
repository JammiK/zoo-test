using Core.JG.Presenters.Core.Interfaces.Systems.Common;
using Core.JG.Views.UI.Core.Interfaces.Buttons;

namespace Core.JG.Presenters.Core.Buttons
{

    public abstract class ButtonPresenterBase<T> where T : IButtonHolder
    {
        readonly T[] _buttons;

        protected ButtonPresenterBase(T[] buttons, IButtonClickSystem buttonClickSystem)
        {
            _buttons = buttons;
            for (var index = 0; index < buttons.Length; index++)
            {
                var onClickHolder = buttons[index];
                buttonClickSystem.Set(onClickHolder, OnClick);
            }
        }

        protected abstract void OnClick();

        protected void SetInteractable(bool interactable)
        {
            for (var i = 0; i < _buttons.Length; i++)
            {
                _buttons[i].SetInteractable(interactable);
            }
        }
    }
}