using System;
using System.Windows.Threading;

namespace AppLurker.Services
{
    public class DebounceService
    {
        private DispatcherTimer _timer;
    
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
    }
}
