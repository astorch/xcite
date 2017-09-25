namespace xcite.collections {
    /// <summary>
    /// Defines the signature of a collection changed handler.
    /// </summary>
    /// <typeparam name="TItem">Type of managed items</typeparam>
    /// <param name="collection">Collection that has been modified</param>
    /// <param name="args">Event arguments</param>
    public delegate void CollectionChangedHandler<TItem>(IObservableEnumerable<TItem> collection, CollectionChangedEventArgs<TItem> args);

    /// <summary>
    /// Implements an adapter for <see cref="IEnumerableListener{TItem}"/> that raises a single 
    /// event when a collection changed event occurred.
    /// </summary>
    /// <typeparam name="TItem">Type of managed items</typeparam>
    public class CollectionListenerAdapter<TItem> : IEnumerableListener<TItem> {

        /// <summary> Is raised when a collection changed event occurred. </summary>
        public event CollectionChangedHandler<TItem> CollectionChanged; 

        /// <inheritdoc />
        public void OnItemAdded(IObservableEnumerable<TItem> itemCollection, TItem item) {
            CollectionChanged?.Invoke(itemCollection, new CollectionChangedEventArgs<TItem>(ECollectionChangeReason.Added, item));
        }

        /// <inheritdoc />
        public void OnItemRemoved(IObservableEnumerable<TItem> itemCollection, TItem item) {
            CollectionChanged?.Invoke(itemCollection, new CollectionChangedEventArgs<TItem>(ECollectionChangeReason.Removed, item));
        }

        /// <inheritdoc />
        public void OnCleared(IObservableEnumerable<TItem> itemCollection) {
            CollectionChanged?.Invoke(itemCollection, new CollectionChangedEventArgs<TItem>(ECollectionChangeReason.Cleared, default(TItem)));
        }
    }
}