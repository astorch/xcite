namespace xcite.collections {
    /// <summary> Defines callbacks to listen to collection changes. </summary>
    public interface IEnumerableListener {
        /// <summary>
        /// Is invoked when the given <paramref name="item"/> has been added to the given <paramref name="collection"/>.
        /// </summary>
        /// <param name="collection">Modified item collection</param>
        /// <param name="item">Added item</param>
        void OnItemAdded(IObservableEnumerable collection, object item);

        /// <summary>
        /// Is invoked when the given <paramref name="item"/> has been removed from the given <paramref name="collection"/>.
        /// </summary>
        /// <param name="collection">Modified item collection</param>
        /// <param name="item">Removed item</param>
        void OnItemRemoved(IObservableEnumerable collection, object item);

        /// <summary>
        /// Is invoked when the given <paramref name="collection"/> has been cleared.
        /// </summary>
        /// <param name="collection">Modified item collection</param>
        void OnCleared(IObservableEnumerable collection);
    }
}