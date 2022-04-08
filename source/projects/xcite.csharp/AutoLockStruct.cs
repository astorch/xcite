using System;
using System.Threading;

namespace xcite.csharp {
    /// <summary>
    /// Object that provides methods to perform a synchronized access to an encapsulated struct. 
    /// This class is intended to use for short object lock and value access patterns.
    /// </summary>
    /// <typeparam name="TValue">Type of encapsulated value</typeparam>
    /// <example>
    /// <code>
    /// private readonly AutoLockStruct{bool} _running = new AutoLockStruct{bool});
    /// 
    /// _running.Set(true);
    /// bool isRunning = _running.Get();
    /// 
    /// </code>
    /// 
    /// </example>
    public class AutoLockStruct<TValue> where TValue : struct {
        private TValue _value;

        /// <summary>
        /// Creates a new instance with the given default value.
        /// </summary>
        /// <param name="value">Default value</param>
        public AutoLockStruct(TValue value = default(TValue)) {
            lock (this) {
                _value = value;
            }
        }

        /// <summary>
        /// Sets the value to the given one.
        /// </summary>
        /// <param name="value">Value to set</param>
        public void Set(TValue value) {
            lock (this) {
                _value = value;
            }
        }

        /// <summary>
        /// Returns the currently set value
        /// </summary>
        /// <returns>Set value</returns>
        public TValue Get() {
            lock (this) {
                return _value;
            }
        }

        /// <summary>
        /// Aquires a lock for this object. The lock is alive as long the returned dispose token is. 
        /// To release the lock dispose the token.
        /// </summary>
        /// <returns>Disposable token</returns>
        public IDisposable Lock() {
            Monitor.Enter(this);
            return new DisposableDelegate(OnLockDispose);
        }

        /// <summary>
        /// Is invoked when the lock token has been disposed.
        /// </summary>
        private void OnLockDispose() {
            Monitor.Exit(this);
        }
    }
}