using System;
using Core.JG.Views.UI.Core.Interfaces.Timers;
using TMPro;
using UnityEngine;

namespace Core.JG.Views.UI.Core.Timers
{
    public class TimerView : MonoBehaviour, ITimerView
    {
        [SerializeField] TextMeshProUGUI _text;

        Action<ITimerView> _onDestroy;
        Action<ITimerView> _onEnable;
        bool _destroyed;
        long _targetTime;


        public long TargetTime
        {
            get => _targetTime;
            set
            {
                Finished = false;
                _targetTime = value;
            }
        }

        public bool Enabled { get; set; }
        public bool Finished { get; set; }

        public void SetTimer(string timer)
        {
            if (!_destroyed && _text != null)
            {
                _text.text = timer;
            }
        }

        public void SetOnDestroy(Action<ITimerView> onDestroy)
        {
            _onDestroy = onDestroy;
        }

        public void SetOnEnable(Action<ITimerView> onEnable)
        {
            _onEnable = onEnable;
        }

        void OnEnable()
        {
            Enabled = true;
            _onEnable?.Invoke(this);
        }

        void OnDisable()
        {
            Enabled = false;
        }

        public void OnDestroy()
        {
            Enabled = false;
            _destroyed = true;
            _onDestroy?.Invoke(this);
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (_text == null)
            {
                _text = GetComponent<TextMeshProUGUI>();
            }
        }
#endif
    }
}