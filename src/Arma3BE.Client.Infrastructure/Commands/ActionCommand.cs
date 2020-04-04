using System;

namespace Arma3BE.Client.Infrastructure.Commands
{
    public class ActionCommand : BaseCommand
    {
        private readonly Action _action;
        private readonly Func<bool> _canExecute;

        public ActionCommand(Action action, Func<bool> canExecute = null)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public override bool CanExecute(object parameter)
        {
            if (_canExecute == null)
                return base.CanExecute(parameter);
            return _canExecute();
        }

        public override void Execute(object parameter)
        {
            _action();
        }

        public void CanExecuteInvalidate() => this.OnCanExecuteChanged();
    }
}