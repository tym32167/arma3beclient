﻿using Arma3BE.Client.Infrastructure.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

// ReSharper disable MemberCanBeProtected.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable ExplicitCallerInfoArgument
// ReSharper disable UnusedMember.Global

namespace Arma3BE.Client.Infrastructure.Models
{
    public abstract class ServerMonitorBaseViewModel<T, TK> : ViewModelBase where T : class where TK : class
    {
        private readonly IEqualityComparer<TK> _comparer;

        // ReSharper disable once InconsistentNaming
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
                RaisePropertyChanged();
            }
        }

        public TK SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                RaisePropertyChanged();
            }
        }

        public ObservableCollection<TK> Data => FilteredData;

        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Filter { get; set; }

        public int DataCount => Data?.Count ?? 0;

        public virtual string Title => $"({DataCount})";

        public ICommand RefreshCommand { get; }

        public ICommand FilterCommand { get; private set; }

        public virtual async Task SetDataAsync(IEnumerable<T> initialData)
        {
            _data = await RegisterDataAsync(initialData);

            UpdateData();
        }

        public virtual void UpdateData()
        {
            var newData = FilterData(_data).ToArray();
            UpdateFilteredData(newData);

            RaisePropertyChanged(nameof(Data));
            RaisePropertyChanged(nameof(DataCount));
            RaisePropertyChanged(nameof(Title));
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

        protected abstract Task<IEnumerable<TK>> RegisterDataAsync(IEnumerable<T> initialData);

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