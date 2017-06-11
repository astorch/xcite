using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace xcite.csharp {
    /// <summary>
    /// Provides methods to generically dispatch events and invoke corresponding handlers.
    /// </summary>
    public static class EventDispatcher {
        /// <summary>
        /// Raises the event that is described by the given <paramref name="eventHandler"/> and notifies its handlers. If <paramref name="eventHandler"/> 
        /// is NULL, nothing will happen. The given <paramref name="eventArgs"/> are used as event arguments for the handlers. 
        /// If there is an exception during the dispatching of the event, it will be ignored! 
        /// If there is more than one event handler, each is being tried to by notified even in the case of an error. Only when 
        /// a <see cref="ArgumentException"/> or <see cref="TargetParameterCountException"/> occurs, the invocation is being stopped and an the exception 
        /// is re-thrown.
        /// </summary>
        /// <param name="eventHandler">Event handler</param>
        /// <param name="eventArgs">Event arguments to dispatch</param>
        public static void Dispatch(this Delegate eventHandler, object[] eventArgs) {
            Dispatch(eventHandler, eventArgs, null);
        }

        /// <summary>
        /// Raises the event that is described by the given <paramref name="eventHandler"/> and notifies its handlers. If <paramref name="eventHandler"/> 
        /// is NULL, nothing will happen. The given <paramref name="eventArgs"/> are used as event arguments for the handlers. 
        /// If there is an exception during the dispatching of the event, the given <paramref name="exceptionHandler"/> is notified for each handler 
        /// individually. If there is more than one event handler, each is being tried to by notified even in the case of an error. Only when 
        /// a <see cref="ArgumentException"/> or <see cref="TargetParameterCountException"/> occurs, the invocation is being stopped and an the exception 
        /// is re-thrown.
        /// </summary>
        /// <param name="eventHandler">Event handler</param>
        /// <param name="exceptionHandler">Exception handler</param>
        /// <param name="eventArgs">Event arguments to dispatch</param>
        public static void Dispatch(this Delegate eventHandler, object[] eventArgs,
            Action<Exception, Delegate> exceptionHandler) {
            if (eventHandler == null) return;

            Delegate[] subscribers;
            lock (eventHandler) {
                subscribers = eventHandler.GetInvocationList();
            }

            for (int i = -1; ++i != subscribers.Length;) {
                Delegate subscriber = subscribers[i];
                try {
                    subscriber.DynamicInvoke(eventArgs);
                } catch (TargetParameterCountException) {
                    throw;
                } catch (ArgumentException) {
                    throw;
                } catch (Exception ex) {
                    // If there is a handler notify it, else ignore
                    exceptionHandler?.Invoke(ex, subscriber);
                }
            }
        }

        /// <summary>
        /// Executes the given <paramref name="eventInvocation"/> on each element of the given <paramref name="listenerCollection"/>. If 
        /// <paramref name="eventInvocation"/> is NULL, nothing will happen.
        /// If there is an exception during the dispatching of the event, it will be ignored! 
        /// If there is more than one event handler, each is being tried to by notified even in the case of an error.
        /// </summary>
        /// <typeparam name="TListener">Type of event listeners</typeparam>
        /// <param name="listenerCollection">Collection of listeners to notify</param>
        /// <param name="eventInvocation">Event invocator</param>
        public static void Dispatch<TListener>(IEnumerable<TListener> listenerCollection,
            Action<TListener> eventInvocation) {
            Dispatch(listenerCollection, eventInvocation, null);
        }

        /// <summary>
        /// Executes the given <paramref name="eventInvocation"/> on each element of the given <paramref name="listenerCollection"/>. If 
        /// <paramref name="eventInvocation"/> is NULL, nothing will happen.
        /// If there is an exception during the dispatching of the event, the given <paramref name="exceptionHandler"/> is notified for each handler 
        /// individually. If there is more than one event handler, each is being tried to by notified even in the case of an error.
        /// </summary>
        /// <typeparam name="TListener">Type of event listeners</typeparam>
        /// <param name="listenerCollection">Collection of listeners to notify</param>
        /// <param name="eventInvocation">Event invocator</param>
        /// <param name="exceptionHandler">Exception handler</param>
        public static void Dispatch<TListener>(IEnumerable<TListener> listenerCollection,
            Action<TListener> eventInvocation, Action<Exception, TListener> exceptionHandler) {
            if (listenerCollection == null) return;
            if (eventInvocation == null) return;

            TListener[] listeners;
            lock (listenerCollection) {
                listeners = listenerCollection.ToArray();
            }

            for (int i = -1; ++i != listeners.Length;) {
                TListener listener = listeners[i];
                try {
                    eventInvocation(listener);
                } catch (Exception ex) {
                    // If there is a handler notify it, else ignore
                    exceptionHandler?.Invoke(ex, listener);
                }
            }
        }
    }
}