using System.Collections.Generic;

namespace xcite.collections {
    /// <summary>
    /// Defines a collection that publishes events when it has been modified.
    /// </summary>
    /// <typeparam name="TItem">Type of elements managed by this collection</typeparam>
    public interface IObservableCollection<TItem> : ICollection<TItem> {
        /// <summary>
        /// Subscribes the given <paramref name="listener"/> to this instance.
        /// </summary>
        /// <param name="listener">Listener</param>
        void AddCollectionListener(ICollectionListener<TItem> listener);

        /// <summary>
        /// Unsubscribes the given <paramref name="listener"/> from this instance.
        /// </summary>
        /// <param name="listener">Listener</param>
        void RemoveCollectionListener(ICollectionListener<TItem> listener);
    }
}