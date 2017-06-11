using System.Collections.Generic;

namespace xcite.collections {
    /// <summary>
    /// Provides methods extensions for implementations of <see cref="ICollection{T}"/>.
    /// </summary>
    public static class CollectionExtensionMethods {
        /// <summary>
        /// Adds the given <paramref name="item"/> to the given <paramref name="collection"/> and 
        /// returns it.
        /// </summary>
        /// <typeparam name="TItem">Type of managed items</typeparam>
        /// <param name="collection">Collection the item is added</param>
        /// <param name="item">Item to add</param>
        /// <returns>Added item</returns>
        public static TItem Add<TItem>(this ICollection<TItem> collection, TItem item) {
            collection.Add(item);
            return item;
        }
    }
}