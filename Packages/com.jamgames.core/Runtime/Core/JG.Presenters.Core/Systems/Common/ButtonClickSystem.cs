using System;
using System.Collections.Generic;
using Core.JG.Presenters.Core.Interfaces.Systems.Common;
using Core.JG.Views.UI.Core.Interfaces.Buttons;

namespace Core.JG.Presenters.Core.Systems.Common
{
    public class ButtonClickSystem : IButtonClickSystem
    {
        public event Action ButtonClicked;

        HashSet<Action<Action>> _actions;
        
        public ButtonClickSystem()
        {
            _actions = new HashSet<Action<Action>>();
        }

        public void Set(IOnClickHolder holder, Action action)
        {
            Set(holder.SetOnClick, action);
        }

        public void Set(Action<Action> OnClick, Action action)
        {
            if (_actions.Contains(OnClick))
            {
                // Log.Error($"Action OnClick already subscribed");
                // OnClick(() =>
                // {
                //     action?.Invoke();
                // });

                return;
            }

            _actions.Add(OnClick);

            OnClick(() =>
            {
                Click();
                action?.Invoke();
            });
        }

        public void Click()
        {
            ButtonClicked?.Invoke();
        }
    }
}
