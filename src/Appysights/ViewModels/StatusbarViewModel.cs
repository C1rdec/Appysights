using Caliburn.Micro;
using Appysights.Services;

namespace Appysights.ViewModels
{
    public class StatusbarViewModel : PropertyChangedBase
    {
        #region Fields

        private StatusbarService _service;
        private System.Action _onClick;
        private bool _selected;
        private bool _notInitialize = true;

        #endregion

        #region Constructors

        public StatusbarViewModel(StatusbarService service, System.Action onClick)
        {
            _onClick = onClick;
            _service = service;
            service.InitializeAsync().ContinueWith(t =>
            {
                NotInitialize = false;
                NotifyOfPropertyChange(() => HasExceptions); 
                NotifyOfPropertyChange(() => HasExceptionsSilenced);
            });
        }

        #endregion

        #region Properties

        public bool Selected
        {
            get
            {
                return _selected;
            }

            set
            {
                _selected = value;
                NotifyOfPropertyChange(() => Selected);
            }
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

        public bool HasExceptions => _service.HasException;

        public bool HasExceptionsSilenced
        {
            get
            {
                if (_service.Silenced)
                {
                    return false;
                }

                return HasExceptions;
            }
        }

        #endregion

        #region Methods

        public void OnClick()
        {
            // We need to invoke before settings selected
            _onClick?.Invoke();
            Selected = !Selected;
        }

        public void Silence()
        {
            _service.Silenced = !_service.Silenced;
            NotifyOfPropertyChange(() => HasExceptions);
            NotifyOfPropertyChange(() => HasExceptionsSilenced);
        }

        #endregion
    }
}
