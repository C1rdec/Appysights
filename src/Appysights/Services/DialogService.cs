﻿using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;

namespace Appysights.Services
{
    public class DialogService
    {
        private static readonly IDialogCoordinator Coordinator = DialogCoordinator.Instance;

        private object _context;

        public async Task ShowProgressAsync(string title, string message, Task task)
        {
            var controller = await Coordinator.ShowProgressAsync(_context, title, message);
            controller.SetIndeterminate();

            await task;

            await controller.CloseAsync();
        }

        public void Register(object context)
        {
            _context = context;
        }
    }
}