using System;
using System.Collections.Generic;
using Core.JG.Data.Interfaces.Providers.Time;
using Core.JG.Presenters.Core.Interfaces.Views.Timers;
using Core.JG.Views.UI.Core.Interfaces.Timers;
using Cysharp.Threading.Tasks;

namespace Core.JG.Presenters.Core.Views.Timers
{
    public class TimerPresenter : ITimerPresenter
    {
        readonly ITimeProvider _timeProvider;

        readonly List<ITimerView> _timers = new List<ITimerView>();
        readonly List<ITimerView> _timersForRemove = new List<ITimerView>();
        
        public TimerPresenter(ITimeProvider timeProvider,
            ITimerView[] timerViews)
        {
            _timeProvider = timeProvider;
            foreach (var timerView in timerViews)
            {
                Register(timerView);
            }
            
            UpdateTimer();
        }

        async UniTaskVoid UpdateTimer()
        {
            while (true)
            {
                if (_timers.Count == 0)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(1));
                }

                var time = _timeProvider.GetTime();
                for (var i = 0; i < _timers.Count; i++)
                {
                    var timerHolder = _timers[i];
                    UpdateTimerHolder(timerHolder, time);
                }

                for (var i = _timers.Count - 1; i >= 0; i--)
                {
                    if (_timersForRemove.Contains(_timers[i]))
                    {
                        _timers.RemoveAt(i);
                    }
                }
                
                _timersForRemove.Clear();
                await UniTask.Delay(TimeSpan.FromSeconds(1));
            }
        }

        void UpdateTimerHolder(ITimerView timerHolder, long time)
        {
            if (!timerHolder.Enabled || timerHolder.TargetTime <= 0)
            {
                return;
            }
            
            var timerTime = ((timerHolder.TargetTime - time) / 1000);
            long t1;
            long t2;
            if (timerTime <= 0 && timerHolder.Finished)
            {
                return;
            }

            timerTime = Math.Max(timerTime, 0);
            if (timerTime < 3600)
            {
                t1 = timerTime / 60;
                t2 = timerTime % 60;
            }
            else
            {
                timerTime /= 60;
                t1 = timerTime / 60;
                t2 = timerTime % 60;
            }

            timerHolder.Finished = timerTime <= 0;
            timerHolder.SetTimer($"{t1:00}:{t2:00}");
        }

        public void Register(ITimerView timerView)
        {
            timerView.SetOnDestroy(timer => _timersForRemove.Add(timer));
            timerView.SetOnEnable(timer =>
            {
                var time = _timeProvider.GetTime();
                UpdateTimerHolder(timer, time);
            });
            
            _timers.Add(timerView);
            var currentTime = _timeProvider.GetTime();
            UpdateTimerHolder(timerView, currentTime);
        }
    }
}