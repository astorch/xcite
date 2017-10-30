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

        /// <summary>
        /// Returns the index of the specified <paramref name="item"/> in the <paramref name="sequence"/>. 
        /// If the sequence does not contain the element, -1 is returned. Note, the lookup is performed 
        /// using <see cref="object.Equals(object)"/>
        /// </summary>
        /// <typeparam name="TItem">Type of managed items</typeparam>
        /// <param name="sequence">Sequence to process</param>
        /// <param name="item">Item which index to look up</param>
        /// <returns>Index of the item or -1</returns>
        public static int IndexOf<TItem>(this IEnumerable<TItem> sequence, TItem item) {
            return IndexOf(sequence, seqItem => Equals(seqItem, item));
        }

        /// <summary>
        /// Returns the index of the first item that matches the specified <paramref name="predicate"/> in the <paramref name="sequence"/>. 
        /// If the sequence does not contain an element, -1 is returned.
        /// </summary>
        /// <typeparam name="TItem">Type of managed items</typeparam>
        /// <param name="sequence">Sequence to process</param>
        /// <param name="predicate">Predicate to find the desired element</param>
        /// <returns>Index of the item or -1</returns>
        public static int IndexOf<TItem>(this IEnumerable<TItem> sequence, Predicate<TItem> predicate) {
            if (sequence == null) throw new ArgumentNullException();

            int index = -1;
            using (var itr = sequence.GetEnumerator()) {
                while (itr.MoveNext()) {
                    index++;
                    if (predicate(itr.Current)) return index;
                }
            }

            return -1;
        }
    }
}