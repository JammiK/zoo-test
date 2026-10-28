using System;
using Core.JG.Views.UI.Core.Interfaces.Buttons;
using UnityEngine;
using UnityEngine.UI;

namespace Core.JG.Views.UI.Core.Buttons
{
    public class ButtonBase : MonoBehaviour, IOnClickHolder
    {
        [SerializeField] Button _button;
        
        public void SetOnClick(Action onClick)
        {
            _button.onClick.AddListener(() => onClick?.Invoke());
        }


#if UNITY_EDITOR
        void OnValidate()
        {
            if (_button == null)
            {
                _button.GetComponent<Button>();
            }
        }
#endif
    }
}