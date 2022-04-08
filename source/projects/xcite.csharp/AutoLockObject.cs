using System;

namespace xcite.csharp {
    /// <summary>
    /// Object that provides methods to perform a synchronized access to an encapsulated object. 
    /// This class is intended to use for short object lock and value access patterns.
    /// </summary>
    /// <typeparam name="TValue">Type of encapsulated value</typeparam>
    /// <example>
    /// <code>
    /// private readonly AutoLockObject{List} _Set = new AutoLockObject{List}(new List());
    /// 
    /// _Set.Access(lst => lst.Add("hello");
    /// string first = _Set.Access(lst => lst[0]);
    /// 
    /// </code>
    /// 
    /// </example>
    public class AutoLockObject<TValue> where TValue : class {
        private readonly TValue _value;

        /// <summary>
        /// Creates a new instance that encapsulates and restricts parallel access the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">Value to encapsulate and protect</param>
        public AutoLockObject(TValue value) {
            lock (this) {
                _value = value;
            }
        }

        /// <summary>
        /// Aquires locked access to the managed value and invokes the given delegate.
        /// </summary>
        /// <param name="grantAccess">Delegate that is invoked when the value is accessible</param>
        public void Access(Action<TValue> grantAccess) {
            lock (this) {
                grantAccess(_value);
            }
        }

        /// <summary>
        /// Aquires locked access to the managed value and invokes the given delegate. Returns 
        /// the return value of the delegate.
        /// </summary>
        /// <typeparam name="TReturn">Type of return value</typeparam>
        /// <param name="grantAccess">Delegate that is invoked when the value is accessible</param>
        /// <returns>Value returned from delegate</returns>
        public TReturn Access<TReturn>(Func<TValue, TReturn> grantAccess) {
            lock (this) {
                return grantAccess(_value);
            }
        }
    }
}