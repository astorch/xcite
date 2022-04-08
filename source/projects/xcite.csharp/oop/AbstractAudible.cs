using System;
using System.Collections.Generic;

namespace xcite.csharp.oop {
    /// <summary>
    /// Provides an abstract implementation of <see cref="IAudible{TListener}"/>. This class is thread-safe.
    /// </summary>
    /// <typeparam name="TListener"></typeparam>
    public abstract class AbstractAudible<TListener> : IAudible<TListener> where TListener : class {
        private readonly HashSet<TListener> _registeredListeners = new HashSet<TListener>();

        /// <summary>
        /// Subscribes the given <paramref name="listener"/> to class specific events.
        /// </summary>
        /// <param name="listener">Listener to subscribe</param>
        public void AddListener(TListener listener) {
            if (listener == null) return;
            lock (_registeredListeners) {
                _registeredListeners.Add(listener);
            }
        }

        /// <summary>
        /// Unsubscribes the given <paramref name="listener"/> from class specific events.
        /// </summary>
        /// <param name="listener">Listener to unsubscribe</param>
        public void RemoveListener(TListener listener) {
            if (listener == null) return;
            lock (_registeredListeners) {
                _registeredListeners.Remove(listener);
            }
        }

        /// <summary>
        /// Invokes the given <paramref name="eventInvocator"/> action on each subscribed lister. If an error occurs, it is dropped. 
        /// The dispatching continuous even if there is an error for one or more listeners.
        /// </summary>
        /// <param name="eventInvocator">Action that is invoked on each subscribed listener</param>
        protected virtual void DispatchEvent(Action<TListener> eventInvocator) {
            DispatchEvent(eventInvocator, null);
        }

        /// <summary>
        /// Invokes the given <paramref name="eventInvocator"/> action on each subscribed lister. If an error occurs, the <paramref name="exceptionHandler"/> 
        /// is called, if it's not NULL. The dispatching continuous even if there is an error for one or more listeners.
        /// </summary>
        /// <param name="eventInvocator">Action that is invoked on each subscribed listener</param>
        /// <param name="exceptionHandler">Exception handler</param>
        protected virtual void DispatchEvent(Action<TListener> eventInvocator,
            Action<Exception, TListener> exceptionHandler) {
            EventDispatcher.Dispatch(_registeredListeners, eventInvocator, exceptionHandler);
        }
    }
}