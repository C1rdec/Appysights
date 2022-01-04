using System;
using System.Linq;
using Caliburn.Micro;
using AppLurker.Models;
using AppLurker.Services;

namespace AppLurker.ViewModels
{
    public class MicroServiceViewModel : PropertyChangedBase, IDisposable
    {
        #region Fields

        private MicroService _service;
        private Action<MicroServiceViewModel> _callback;
        private bool _selected;
        private DebounceService _debounceService;


        #endregion

        #region Constructors

        public MicroServiceViewModel(MicroService microService, Action<MicroServiceViewModel> callback)
        {
            _callback = callback;
            _service = microService;
            _debounceService = new DebounceService();
            Name = microService.Name;

            microService.Dev.NewEvent += NewEvent;
            microService.Prod.NewEvent += NewEvent;
        }

        #endregion

        #region Properties

        public string Name { get; set; }

        public int DevErrorCount => _service.Dev.Events.Count();

        public int ProdErrorCount => _service.Prod.Events.Count();

        public string DevDisplayValue => DevErrorCount == 0 ? "-" : DevErrorCount.ToString();

        public string ProdDisplayValue => ProdErrorCount == 0 ? "-" : ProdErrorCount.ToString();

        public bool HasEvents => DevErrorCount != 0 || ProdErrorCount != 0;

        public MicroService Service => _service;

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

        #endregion

        #region Methods

        public void GetLastDay()
        {
            _service.GetLastDay();
        }

        public void GetLastHour()
        {
            _service.GetLastHour();
        }

        public void OnClick()
        {
            _callback(this);
        }

        public void Clear()
        {
            _service.Clear();

            NotifyChange();
        }

        private void NewEvent(object sender, AppInsightEvent e)
        {
            // We need a debounce since the event is raised for every exceptions.
            _debounceService.Debounce(800, () => 
            {
                NotifyChange(); 
            });
        }

        private void NotifyChange()
        {
            NotifyOfPropertyChange(() => DevDisplayValue);
            NotifyOfPropertyChange(() => ProdDisplayValue);
            NotifyOfPropertyChange(() => DevErrorCount);
            NotifyOfPropertyChange(() => ProdErrorCount);
            NotifyOfPropertyChange(() => HasEvents);
        }

        public void Dispose()
        {
            _service.Dev.NewEvent -= NewEvent;
            _service.Prod.NewEvent -= NewEvent;
        }

        #endregion
    }
}
