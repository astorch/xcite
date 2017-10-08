namespace xcite.collections {
    /// <summary>
    /// Defines a subset a observable collection that has been created 
    /// by applying a where operation. Note, this subset also publishes events 
    /// when itself or its original source has been modified.
    /// </summary>
    /// <typeparam name="TItem">Type of elements managed by this collection</typeparam>
    public interface IObservableCollectionSubset<TItem> : IObservableCollection<TItem> {
        /// <summary>
        /// Removes the first occurrence of a specific object from the collection. However, 
        /// the object is only removed from the subset. If <paramref name="modifySource"/> is 
        /// set to TRUE, the item is removed from the original source.
        /// </summary>
        /// <param name="item">Item to be removed from the subset</param>
        /// <param name="modifySource">TRUE to remove the item from the original source</param>
        /// <returns>TRUE if the item has been removed</returns>
        bool Remove(TItem item, bool modifySource);

        /// <summary>
        /// Adds the specified item to the collection. However, the object is only added to 
        /// the subset. If <paramref name="modifySource"/> is set to TRUE, the item is added 
        /// to the original source.
        /// </summary>
        /// <param name="item">Item to be added to the subset</param>
        /// <param name="modifySource">TRUE to add the item to the original source</param>
        void Add(TItem item, bool modifySource);
    }
}