using Arma3BE.Client.Infrastructure.Commands;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Arma3BE.Client.Infrastructure.Models
{
    public abstract class ServerMonitorBaseViewModel<T, TK> : ViewModelBase where T : class where TK : class
    {
        private readonly IEqualityComparer<TK> _comparer;
        protected IEnumerable<TK> _data;
        protected ObservableCollection<TK> FilteredData;
        private TK _selectedItem;

        protected ServerMonitorBaseViewModel(ICommand refreshCommand, IEqualityComparer<TK> comparer)
        {
            _comparer = comparer;
            RefreshCommand = refreshCommand;
            FilterCommand = new ActionCommand(UpdateData);
        }

        public TK SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TK> Data
        {
            get { return FilteredData; }
        }

        public string Filter { get; set; }

        public int DataCount
        {
            get { return _data == null ? 0 : Data.Count(); }
        }

        public ICommand RefreshCommand { get; }

        public ICommand FilterCommand { get; private set; }

        public virtual void SetData(IEnumerable<T> initialData)
        {
            _data = RegisterData(initialData);

            UpdateData();
        }

        public virtual void UpdateData()
        {
            var newData = FilterData(_data).ToArray();
            UpdateFilteredData(newData);

            OnPropertyChanged(nameof(Data));
            OnPropertyChanged(nameof(DataCount));
        }

        private object _locker = new object();
        private void UpdateFilteredData(TK[] newData)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {

                if (FilteredData == null)
                {
                    FilteredData = new ObservableCollection<TK>(newData);
                    return;
                }

                var exists = FilteredData.ToArray();

                var todelete = exists.Where(x => newData.All(n => _comparer.Equals(n, x) == false)).ToArray();
                var toAdd = newData.Where(x => exists.All(n => _comparer.Equals(n, x) == false)).ToArray();

                foreach (var d in todelete)
                {
                    FilteredData.Remove(d);
                }

                foreach (var a in toAdd)
                {
                    FilteredData.Add(a);
                }

            });
        }

        protected abstract IEnumerable<TK> RegisterData(IEnumerable<T> initialData);

        protected virtual ObservableCollection<TK> FilterData(IEnumerable<TK> initialData)
        {
            if (initialData == null) return null;
            return new ObservableCollection<TK>(initialData.Where(x => F(x, Filter)));
        }

        private bool F(TK element, string initialFilter)
        {
            if (string.IsNullOrEmpty(initialFilter)) return true;
            var filter = initialFilter.ToLower();

            var type = typeof(TK);


            var members = type.GetProperties();
            foreach (var propertyInfo in members)
            {
                var value = propertyInfo.GetValue(element);
                if (value != null)
                {
                    string valueAsString;
                    if (value is string)
                    {
                        valueAsString = value as string;
                    }
                    else
                    {
                        valueAsString = value.ToString();
                    }

                    if (!string.IsNullOrEmpty(valueAsString))
                    {
                        if (valueAsString.ToLower().Contains(filter)) return true;
                    }
                }
            }


            return false;
        }
    }
}