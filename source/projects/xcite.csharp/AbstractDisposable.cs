using System;

namespace xcite.csharp {
    /// <summary>
    /// Provides an abstract implementation of <see cref="IDisposable"/>. Implements the dispose pattern.
    /// </summary>
    public abstract class AbstractDisposable : IDisposable {
        private bool _disposed;

        /// <summary>Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.</summary>
        ~AbstractDisposable() {
            OnDispose(false);
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public virtual void Dispose() {
            if (_disposed) return;
            _disposed = true;
            GC.SuppressFinalize(this);

            OnDispose(true);
        }

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if this instance has been disposed.
        /// </summary>
        /// <exception cref="ObjectDisposedException"/>
        protected virtual void ThrowIfDisposed() {
            if (!_disposed) return;
            throw new ObjectDisposedException("Instance has been disposed");
        }

        /// <summary>
        /// Is invoked when the instance is disposed. This method is intended to be overriden by clients. 
        /// But clients should call base.OnDispose().
        /// </summary>
        /// <param name="dispose">FALSE if the method is invoked from the GC</param>
        protected abstract void OnDispose(bool dispose);
    }
}