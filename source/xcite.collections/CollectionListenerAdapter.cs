using System;

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
        /// <summary> Initializes the new instance. </summary>
        public CollectionListenerAdapter() {
            // Nothing to do here
        }

        /// <summary>
        /// Initializes the new instance and adds the specified <paramref name="handler"/> 
        /// to the <see cref="CollectionChanged"/> event.
        /// </summary>
        /// <param name="handler">Handler to add</param>
        public CollectionListenerAdapter(CollectionChangedHandler handler) {
            CollectionChanged += handler ?? throw new ArgumentNullException(nameof(handler));
        }

        /// <summary> Is raised when a collection changed event occurred. </summary>
        public event CollectionChangedHandler CollectionChanged;

        /// <summary> Removes all handlers from the <see cref="CollectionChanged"/> event. </summary>
        public void Clear() {
            CollectionChanged = null;
        }

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