using System;
using Arma3BE.Client.Infrastructure.Models;

namespace Arma3BE.Client.Infrastructure.Contracts
{
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

        // TODO: do not forget about cleaning!
        public void Cleanup()
        {
            //base.Cleanup();
            Dispose();
        }
    }
}