using System;
using System.Collections.Generic;

namespace xcite.collections {
    /// <summary>
    /// Provides a debug view that can be applied to implementations of <see cref="ICollection{TItem}"/>.
    /// </summary>
    /// <typeparam name="TItem">Tyepe of managed items</typeparam>
    public class CollectionDebugView<TItem> {
        private readonly ICollection<TItem> _collection;

        /// <summary>
        /// Creates a new instance for the given <paramref name="collection"/>.
        /// </summary>
        /// <param name="collection">Collection</param>
        public CollectionDebugView(ICollection<TItem> collection) {
            _collection = collection ?? throw new ArgumentNullException(nameof(collection));
        }

        /// <summary>
        /// Returns all items of the collection as array.
        /// </summary>
        public TItem[] Items {
            get {
                TItem[] resultArray = new TItem[_collection.Count];
                _collection.CopyTo(resultArray, 0);
                return resultArray;
            }
        }

        /// <summary> Returns the underlying collection type name. </summary>
        public string CollectionType => _collection.GetType().Name;
    }
}