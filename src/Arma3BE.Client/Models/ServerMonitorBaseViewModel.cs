using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Arma3BEClient.Commands;
using GalaSoft.MvvmLight;

namespace Arma3BEClient.Models
{
    public abstract class ServerMonitorBaseViewModel<T, TK> : ViewModelBase where T:class where TK:class 
    {
        private readonly ICommand _refreshCommand;

        protected ServerMonitorBaseViewModel(ICommand refreshCommand)
        {
            _refreshCommand = refreshCommand;
            FilterCommand = new ActionCommand(UpdateData);
        }

        protected IEnumerable<TK> _data;

        public IEnumerable<TK> Data
        {
            get { return FilterData(_data); }
        }

        public string Filter { get; set; }
       

        public virtual void SetData(IEnumerable<T> initialData)
        {
            _data = RegisterData(initialData);

            UpdateData();
        }

        public virtual void UpdateData()
        {
            RaisePropertyChanged("Data");
            RaisePropertyChanged("DataCount");
        }

        protected abstract IEnumerable<TK> RegisterData(IEnumerable<T> initialData);

        public int DataCount
        {
            get { return _data == null ? 0 : Data.Count(); }
        }

        public ICommand RefreshCommand
        {
            get { return _refreshCommand; }
        }

        public ICommand FilterCommand { get; private set; }


        protected virtual IEnumerable<TK> FilterData(IEnumerable<TK> initialData)
        {
            if (initialData == null) return initialData;
            return initialData.Where(x=>F(x, Filter));
        }

        private bool F(TK element, string initialFilter)
        {
            if (string.IsNullOrEmpty(initialFilter)) return true;
            var filter = initialFilter.ToLower();

            var type = typeof (TK);



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