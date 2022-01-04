using Caliburn.Micro;
using AppLurker.Services;

namespace AppLurker.ViewModels
{
    public class StatusbarViewModel: PropertyChangedBase
    {
        private FooterService _service;
        private System.Action _onClick;
        private bool _hasExceptions;
        private bool _notInitialize = true;

        public StatusbarViewModel(FooterService service, System.Action onClick)
        {
            _onClick = onClick;
            _service = service;
            service.InitializeAsync().ContinueWith(t =>
            {
                NotInitialize = false;
                _hasExceptions = service.HasException;
            });
        }

        public string StatusbarName => _service.Name;

        public bool NotInitialize
        {
            get
            {
                return _notInitialize;
            }

            set
            {
                _notInitialize = value;
                NotifyOfPropertyChange();
            }
        }

        public bool HasExceptions
        {
            get 
            { 
                return _hasExceptions; 
            }

            set
            {
                _hasExceptions = value;
                NotifyOfPropertyChange();
            }
        }

        public void OnClick()
        {
            _onClick?.Invoke();
        }
    }
}
