using System;
using System.Windows.Input;

namespace Arma3BEClient.Commands
{
    public abstract class BaseCommand : ICommand
    {
        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        public abstract void Execute(object parameter);

        public event EventHandler CanExecuteChanged;

        protected virtual void OnCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public void RaiseCanExecuteChanged()
        {
            OnCanExecuteChanged();
        }
    }
}