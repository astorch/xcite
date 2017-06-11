using System;

namespace xcite.csharp.oop {
    /// <summary>
    /// Provides an implementation of <see cref="IAudible{TListener}"/> that can be used as property of a class. 
    /// This can be helpful, if the class cannot use <see cref="AbstractAudible{TListener}"/> as base class due to inheritance 
    /// issues.
    /// </summary>
    /// <typeparam name="TListener"></typeparam>
    public class AuxiliaryAudible<TListener> : AbstractAudible<TListener> where TListener : class {
        /// <summary>
        /// Invokes the given <paramref name="eventInvocator"/> action on each subscribed lister. If an error occurs, it is dropped. 
        /// The dispatching continuous even if there is an error for one or more listeners.
        /// </summary>
        /// <param name="eventInvocator">Action that is invoked on each subscribed listener</param>
        public void Dispatch(Action<TListener> eventInvocator) {
            DispatchEvent(eventInvocator);
        }

        /// <summary>
        /// Invokes the given <paramref name="eventInvocator"/> action on each subscribed lister. If an error occurs, the <paramref name="exceptionHandler"/> 
        /// is called, if it's not NULL. The dispatching continuous even if there is an error for one or more listeners.
        /// </summary>
        /// <param name="eventInvocator">Action that is invoked on each subscribed listener</param>
        /// <param name="exceptionHandler">Exception handler</param>
        public void Dispatch(Action<TListener> eventInvocator, Action<Exception, TListener> exceptionHandler) {
            DispatchEvent(eventInvocator, exceptionHandler);
        }
    }
}