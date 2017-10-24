using System;
using System.Collections.Generic;

namespace xcite.collections {
    /// <summary> Provides method extension for <see cref="IEnumerable{T}"/>. </summary>
    public static class EnumerableExtensionMethods {
        /// <summary>
        /// Applies the given <paramref name="action"/> to each element of the given sequence <paramref name="sequence"/>.
        /// </summary>
        /// <typeparam name="TItem">Type of sequence items</typeparam>
        /// <param name="sequence">Collection of items</param>
        /// <param name="action">Action to apply to each element</param>
        public static void ForEach<TItem>(this IEnumerable<TItem> sequence, Action<TItem> action) {
            using (IEnumerator<TItem> itr = sequence.GetEnumerator()) {
                while (itr.MoveNext()) {
                    TItem item = itr.Current;
                    action(item);
                }
            }
        }

        /// <summary>
        /// Returns a newly created array that contains all items of the given sequence <paramref name="sequence"/>.
        /// </summary>
        /// <typeparam name="TItem">Type of sequence items</typeparam>
        /// <param name="sequence">Sequence to process</param>
        /// <returns>Array of all sequence items</returns>
        public static TItem[] ToArray<TItem>(this IEnumerable<TItem> sequence) {
            // Check if it's already an array
            if (sequence is TItem[] itemArray) return itemArray;

            // Check if we need just to perform a copy
            if (sequence is ICollection<TItem> itemCollection) {
                itemArray = new TItem[itemCollection.Count];
                itemCollection.CopyTo(itemArray, 0);
                return itemArray;
            }

            // Process items
            LinearList<TItem> linkedList = new LinearList<TItem>();
            sequence.ForEach(linkedList.Add);
            TItem[] resultingArray = linkedList.ToArray();
            return resultingArray;
        }
    }
}