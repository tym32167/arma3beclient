using Arma3BE.Client.Infrastructure.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Arma3BE.Client.Infrastructure.Models
{
    public abstract class ServerMonitorBaseViewModel<T, TK> : ViewModelBase where T : class where TK : class
    {
        private readonly IEqualityComparer<TK> _comparer;
        protected IEnumerable<TK> _data;

        private TK _selectedItem;
        private bool _waitingForEvent = true;
        protected ObservableCollection<TK> FilteredData;

        protected ServerMonitorBaseViewModel(ICommand refreshCommand, IEqualityComparer<TK> comparer)
        {
            _comparer = comparer;
            RefreshCommand = new CommandWrapper(refreshCommand, this);
            FilterCommand = new ActionCommand(UpdateData);
        }

        public bool WaitingForEvent
        {
            get { return _waitingForEvent; }
            set
            {
                _waitingForEvent = value;
                OnPropertyChanged();
            }
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
            get { return Data?.Count ?? 0; }
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


        protected virtual void UpdateExisting(TK old, TK @new)
        {
            var props = typeof(TK).GetProperties().Where(x => x.SetMethod != null && x.GetMethod != null);
            foreach (var prop in props)
            {
                var value = prop.GetValue(@new);
                prop.SetValue(old, value);
            }
        }

        private void UpdateFilteredData(TK[] newData)
        {
            if (Application.Current.Dispatcher.CheckAccess())
            {
                if (FilteredData == null)
                {
                    FilteredData = new ObservableCollection<TK>(newData);
                    return;
                }

                var exists = FilteredData.ToArray();

                var todelete = exists.Where(x => newData.All(n => _comparer.Equals(n, x) == false)).ToArray();
                var toAdd = newData.Where(x => exists.All(n => _comparer.Equals(n, x) == false)).ToArray();

                foreach (var dataItem in exists)
                {
                    var newItem = newData.FirstOrDefault(x => _comparer.Equals(dataItem, x));
                    if (newItem != null)
                    {
                        UpdateExisting(dataItem, newItem);
                    }
                }

                foreach (var d in todelete)
                {
                    FilteredData.Remove(d);
                }

                foreach (var a in toAdd)
                {
                    FilteredData.Add(a);
                }
            }
            else
            {
                Application.Current.Dispatcher.Invoke(() => UpdateFilteredData(newData));
            }
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

        private class CommandWrapper : ICommand
        {
            private readonly ICommand _command;
            private readonly ServerMonitorBaseViewModel<T, TK> _viewModel;

            public CommandWrapper(ICommand command, ServerMonitorBaseViewModel<T, TK> viewModel)
            {
                _command = command;
                _viewModel = viewModel;
            }

            public bool CanExecute(object parameter)
            {
                return _command.CanExecute(parameter);
            }

            public void Execute(object parameter)
            {
                _viewModel.WaitingForEvent = true;
                _command.Execute(parameter);
            }

            public event EventHandler CanExecuteChanged
            {
                add { _command.CanExecuteChanged += value; }
                remove { _command.CanExecuteChanged -= value; }
            }
        }
    }
}