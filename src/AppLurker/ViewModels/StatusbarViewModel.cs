﻿using Caliburn.Micro;
using AppLurker.Services;

namespace AppLurker.ViewModels
{
    public class StatusbarViewModel: PropertyChangedBase
    {
        #region Fields

        private StatusBatService _service;
        private System.Action _onClick;
        private bool _selected;
        private bool _hasExceptions;
        private bool _notInitialize = true;

        #endregion

        #region Constructors

        public StatusbarViewModel(StatusBatService service, System.Action onClick)
        {
            _onClick = onClick;
            _service = service;
            service.InitializeAsync().ContinueWith(t =>
            {
                NotInitialize = false;
                _hasExceptions = service.HasException;
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

        #endregion

        #region Methods

        public void OnClick()
        {
            Selected = true;
            _onClick?.Invoke();
        }

        #endregion
    }
}
