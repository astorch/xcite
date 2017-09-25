namespace xcite.collections {
    /// <summary>
    /// Defines callbacks to listen to collection changes.
    /// </summary>
    /// <typeparam name="TItem">Type of managed items</typeparam>
    public interface ICollectionListener<TItem> {
        /// <summary>
        /// Is invoked when the given <paramref name="item"/> has been added to the given <paramref name="itemCollection"/>.
        /// </summary>
        /// <param name="itemCollection">Modified item collection</param>
        /// <param name="item">Added item</param>
        void OnItemAdded(IObservableEnumerable<TItem> itemCollection, TItem item);

        /// <summary>
        /// Is invoked when the given <paramref name="item"/> has been removed from the given <paramref name="itemCollection"/>.
        /// </summary>
        /// <param name="itemCollection">Modified item collection</param>
        /// <param name="item">Removed item</param>
        void OnItemRemoved(IObservableEnumerable<TItem> itemCollection, TItem item);

        /// <summary>
        /// Is invoked when the given <paramref name="itemCollection"/> has been cleared.
        /// </summary>
        /// <param name="itemCollection">Modified item collection</param>
        void OnCleared(IObservableEnumerable<TItem> itemCollection);
    }
}