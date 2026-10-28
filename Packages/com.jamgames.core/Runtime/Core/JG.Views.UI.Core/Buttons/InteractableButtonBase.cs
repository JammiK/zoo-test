using System;
using Core.JG.Views.UI.Core.Interfaces.Buttons;
using UnityEngine;
using UnityEngine.UI;

namespace Core.JG.Views.UI.Core.Buttons
{
    public class InteractableButtonBase : MonoBehaviour, IOnClickHolder, IInteractableHolder
    {
        [SerializeField] Button _button;

        public void SetOnClick(Action onClick)
        {
            _button.onClick.AddListener(() => onClick?.Invoke());
        }

        public void SetInteractable(bool interactable)
        {
            _button.interactable = interactable;
        }


#if UNITY_EDITOR

        void OnValidate()
        {
            if (_button == null)
            {
                _button = GetComponent<Button>();
            }
        }
#endif
    }
}