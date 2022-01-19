using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Appysights.Models;
using Caliburn.Micro;
using MahApps.Metro.Controls;

namespace Appysights.ViewModels
{
    public class HamburgerSelectorViewModel : PropertyChangedBase
    {
        #region Fields

        private Action<IMenuItem> _onClick;
        private bool _isOpen;
        private int _index;

        #endregion

        #region Constructors

        public HamburgerSelectorViewModel(IEnumerable<IMenuItem> items, IEnumerable<IMenuItem> options, Action<IMenuItem> onClick)
        {
            _onClick = onClick;
            Items = new ObservableCollection<IMenuItem>(items);
            OptionItems = new ObservableCollection<IMenuItem>(options);
            SelectedItem = Items.FirstOrDefault();
        }

        #endregion

        #region Properties


        public bool IsOpen
        {
            get
            {
                return _isOpen;
            }

            set
            {
                _isOpen = value;
                NotifyOfPropertyChange();
            }
        }

        public bool IsVisible => true;

        public ObservableCollection<IMenuItem> Items { get; set; }

        public ObservableCollection<IMenuItem> OptionItems { get; set; }

        public IMenuItem SelectedItem { get; set; }

        public bool Multiple => true;

        public bool Single => !Multiple;

        public int Index
        {
            get
            {
                return _index;
            }

            set
            {
                _index = value;
                NotifyOfPropertyChange();
            }
        }

        #endregion

        #region Methods

        public void AddItem(IMenuItem item)
        {
            Items.Add(item);
            Index = Items.Count - 1;
            NotifyOfPropertyChange(() => Items);
            NotifyOfPropertyChange(() => IsVisible);
        }

        public void MenuSelectionChanged(object value, ItemClickEventArgs args)
        {
            var item = args.ClickedItem as IMenuItem;
            if (item != null)
            {
                _onClick?.Invoke(item);
                IsOpen = false;
            }
        }

        public void OptionMenuSelectionChanged(object value, ItemClickEventArgs args)
        {
            var iconItem = args.ClickedItem as HamburgerMenuIconItem;
            if (iconItem != null)
            {
                var optionItem = iconItem.Tag as IMenuItem;
                if (optionItem != null)
                {
                    optionItem.OnClick?.Invoke();
                }
            }

            args.Handled = true;
        }

        #endregion
    }
}
