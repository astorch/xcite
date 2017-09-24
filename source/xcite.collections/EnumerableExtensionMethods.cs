using System;
using System.Collections;
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
        /// Applies the given <paramref name="action"/> to each element of the given <paramref name="sequence"/>.
        /// </summary>
        /// <param name="sequence">Collection of items</param>
        /// <param name="action">Action to apply to each element</param>
        public static void ForEach(this IEnumerable sequence, Action<object> action) {
            if (sequence == null) throw new NullReferenceException();
            IEnumerator itr = sequence.GetEnumerator();

            try {
                while (itr.MoveNext()) {
                    object item = itr.Current;
                    action(item);
                }
            } finally {
                (itr as IDisposable)?.Dispose();
            }
        }

        /// <summary>
        /// Applies the given strong-typed <paramref name="action"/> to each element of the given <paramref name="sequence"/>.
        /// </summary>
        /// <typeparam name="TItem">Type each element of the sequence is casted to</typeparam>
        /// <param name="sequence">Collection of items</param>
        /// <param name="action">Action to apply to each element</param>
        public static void ForEach<TItem>(this IEnumerable sequence, Action<TItem> action) {
            ForEach(sequence, obj => action((TItem) obj));
        }

        /// <summary>
        /// Projects each element of a <paramref name="sequence"/> into a form provided by the given <paramref name="selector"/>.
        /// </summary>
        /// <typeparam name="TResult">Type of object resulting the transform function</typeparam>
        /// <param name="sequence">Collection of items</param>
        /// <param name="selector">Transform function</param>
        /// <returns>Collection of transformed items</returns>
        public static IEnumerable<TResult> Select<TResult>(this IEnumerable sequence, Func<object, TResult> selector) {
            if (sequence == null) throw new NullReferenceException();
            IEnumerator itr = sequence.GetEnumerator();

            try {
                while (itr.MoveNext()) {
                    object element = itr.Current;
                    yield return selector(element);
                }
            } finally {
                (itr as IDisposable)?.Dispose();
            }
        }

        /// <summary>
        /// Projects each strong-typed element a <paramref name="sequence"/> into a form provided by the given <paramref name="selector"/>.
        /// </summary>
        /// <typeparam name="TItem">Type each element of the sequence is casted to</typeparam>
        /// <typeparam name="TResult">Type of object resulting the transform function</typeparam>
        /// <param name="sequence">Collection of items</param>
        /// <param name="selector">Transform function</param>
        /// <returns>Collection of transformed items</returns>
        public static IEnumerable<TResult> Select<TItem, TResult>(this IEnumerable sequence,
            Func<TItem, TResult> selector) {
            return Select(sequence, obj => selector((TItem) obj));
        }

        /// <summary>
        /// Returns a newly created array that contains all items of the given sequence <paramref name="sequence"/>.
        /// </summary>
        /// <param name="sequence">Sequence to process</param>
        /// <returns>Array of all sequence items</returns>
        public static object[] ToArray(this IEnumerable sequence) {
            // Check if it's already an array
            if (sequence is object[] objectArray) return objectArray;

            // Check if we need just to perform a copy
            if (sequence is ICollection objectCollection) {
                objectArray = new object[objectCollection.Count];
                objectCollection.CopyTo(objectArray, 0);
                return objectArray;
            }

            // Process items
            LinearList<object> linkedList = new LinearList<object>();
            sequence.ForEach(linkedList.Add);
            object[] resultingArray = linkedList.ToArray();
            return resultingArray;
        }

        /// <summary>
        /// Returns the first item within the given <paramref name="sequence"/> that matches the given 
        /// <paramref name="predicate"/>. If no item can be found, NULL is returned.
        /// </summary>
        /// <param name="sequence">Sequence to iterate</param>
        /// <param name="predicate">Predicate to identify the desired item</param>
        /// <returns></returns>
        public static object FirstOrDefault(this IEnumerable sequence, Predicate<object> predicate) {
            if (sequence == null) throw new NullReferenceException();
            IEnumerator itr = sequence.GetEnumerator();

            try {
                while (itr.MoveNext()) {
                    object current = itr.Current;
                    if (predicate(current)) return current;
                }
            } finally {
                (itr as IDisposable)?.Dispose();
            }

            return null;
        }
    }
}