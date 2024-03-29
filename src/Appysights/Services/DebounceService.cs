﻿using System;
using System.Windows.Threading;

namespace Appysights.Services
{
    public class DebounceService
    {
        #region Fields

        private DispatcherTimer _timer;

        #endregion

        #region Methods

        public void Debounce(int interval, Action action)
        {
            _timer?.Stop();
            _timer = null;

            _timer = new DispatcherTimer(TimeSpan.FromMilliseconds(interval), DispatcherPriority.ApplicationIdle, (s, e) =>
            {
                if (_timer == null)
                {
                    return;
                }

                _timer?.Stop();
                _timer = null;
                action.Invoke();
            }, Dispatcher.CurrentDispatcher);

            _timer.Start();
        }

        #endregion
    }
}
