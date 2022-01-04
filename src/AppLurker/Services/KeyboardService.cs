using System;
using System.Diagnostics;
using Winook;

namespace AppLurker.Services
{
    public class KeyboardService
    {
        private KeyboardHook _hook;

        public KeyboardService()
        {
            var process = Process.GetCurrentProcess();
            _hook = new KeyboardHook(process.Id);
            _hook.AddHandler(KeyCode.Left, (o, e) => LeftPressed?.Invoke(this, EventArgs.Empty));
            _hook.AddHandler(KeyCode.Right, (o, e) => RightPressed?.Invoke(this, EventArgs.Empty));
            _ = _hook.InstallAsync();
        }

        public event EventHandler LeftPressed;

        public event EventHandler RightPressed;
    }
}
