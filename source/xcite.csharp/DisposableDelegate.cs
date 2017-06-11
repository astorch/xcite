using System;

namespace xcite.csharp {
    /// <summary>
    /// Provides an implementation of <see cref="IDisposable"/> that invokes a delegate, 
    /// when <see cref="IDisposable.Dispose"/> is executed.
    /// </summary>
    public class DisposableDelegate : IDisposable {
        private readonly Action _handler;

        /// <summary>
        /// Creates a new instance. 
        /// </summary>
        /// <param name="handler">Handler to invoke</param>
        public DisposableDelegate(Action handler) {
            _handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose() {
            _handler();
        }
    }
}