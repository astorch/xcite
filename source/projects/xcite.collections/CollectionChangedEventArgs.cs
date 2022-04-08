namespace xcite.collections {
    /// <summary>
    /// Collection changed event arguments.
    /// </summary>
    /// <typeparam name="TItem">Type of managed items</typeparam>
    public struct CollectionChangedEventArgs<TItem> {
        /// <summary>
        /// Initializes the new instance.
        /// </summary>
        /// <param name="reason">Collection changed reason</param>
        /// <param name="item">Affected item</param>
        public CollectionChangedEventArgs(ECollectionChangeReason reason, TItem item) {
            Reason = reason;
            Item = item;
        }

        /// <summary> Collection changed reason </summary>
        public ECollectionChangeReason Reason { get; }

        /// <summary> Affected item </summary>
        public TItem Item { get; }
    }
}