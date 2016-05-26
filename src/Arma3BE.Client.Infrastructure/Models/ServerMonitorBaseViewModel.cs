using Arma3BE.Client.Infrastructure.Commands;
using GalaSoft.MvvmLight;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Arma3BE.Client.Infrastructure.Models
{
    public abstract class ServerMonitorBaseViewModel<T, TK> : ViewModelBase where T : class where TK : class
    {
        protected IEnumerable<TK> _data;
        private TK _selectedItem;

        protected ServerMonitorBaseViewModel(ICommand refreshCommand)
        {
            RefreshCommand = refreshCommand;
            FilterCommand = new ActionCommand(UpdateData);
        }

        public IEnumerable<TK> Data
        {
            get { return FilterData(_data); }
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
            RaisePropertyChanged(nameof(Data));
            RaisePropertyChanged(nameof(DataCount));
        }

        protected abstract IEnumerable<TK> RegisterData(IEnumerable<T> initialData);

        protected virtual IEnumerable<TK> FilterData(IEnumerable<TK> initialData)
        {
            if (initialData == null) return initialData;
            return initialData.Where(x => F(x, Filter));
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