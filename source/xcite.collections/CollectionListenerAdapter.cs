namespace xcite.collections {
    /// <summary>
    /// Defines the signature of a collection changed handler.
    /// </summary>
    /// <param name="collection">Collection that has been modified</param>
    /// <param name="args">Event arguments</param>
    public delegate void CollectionChangedHandler(IObservableEnumerable collection, CollectionChangedEventArgs<object> args);

    /// <summary>
    /// Implements an adapter for <see cref="IEnumerableListener"/> that raises a single 
    /// event when a collection changed event occurred.
    /// </summary>
    public class CollectionListenerAdapter : IEnumerableListener {

        /// <summary> Is raised when a collection changed event occurred. </summary>
        public event CollectionChangedHandler CollectionChanged;

        /// <inheritdoc />
        public void OnItemAdded(IObservableEnumerable collection, object item) {
            CollectionChanged?.Invoke(collection, new CollectionChangedEventArgs<object>(ECollectionChangeReason.Added, item));
        }

        /// <inheritdoc />
        public void OnItemRemoved(IObservableEnumerable collection, object item) {
            CollectionChanged?.Invoke(collection, new CollectionChangedEventArgs<object>(ECollectionChangeReason.Removed, item));
        }

        /// <inheritdoc />
        public void OnCleared(IObservableEnumerable collection) {
            CollectionChanged?.Invoke(collection, new CollectionChangedEventArgs<object>(ECollectionChangeReason.Cleared, null));
        }
    }
}