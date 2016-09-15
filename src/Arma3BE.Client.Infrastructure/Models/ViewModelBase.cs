using System;
using Prism.Mvvm;

namespace Arma3BE.Client.Infrastructure.Models
{
    public abstract class ViewModelBase : BindableBase
    {

    }

    public abstract class DisposableViewModelBase : ViewModelBase, IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DisposableViewModelBase()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                DisposeManagedResources();
            }
            DisposeUnManagedResources();
            _disposed = true;
        }

        protected virtual void DisposeManagedResources()
        {
        }

        protected virtual void DisposeUnManagedResources()
        {
        }
    }

    public interface ITitledItem
    {
        string Title { get; }
    }
}