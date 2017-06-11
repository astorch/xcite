namespace xcite.csharp.oop {
    /// <summary>
    /// Signals that a class implementing this interface provides support of listening to specific events.
    /// </summary>
    /// <typeparam name="TListener"></typeparam>
    public interface IAudible<in TListener> where TListener : class {
        /// <summary>
        /// Subscribes the given <paramref name="listener"/> to class specific events.
        /// </summary>
        /// <param name="listener">Listener to subscribe</param>
        void AddListener(TListener listener);

        /// <summary>
        /// Unsubscribes the given <paramref name="listener"/> from class specific events.
        /// </summary>
        /// <param name="listener">Listener to unsubscribe</param>
        void RemoveListener(TListener listener);
    }
}