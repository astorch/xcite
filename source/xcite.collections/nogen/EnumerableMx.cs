using System;
using System.Collections;
using System.Collections.Generic;

namespace xcite.collections.nogen {
    /// <summary> Implements extension methods for <see cref="IEnumerable"/>. </summary>
    public static class EnumerableMx {

        /// <summary>
        /// Returns the index of the specified <paramref name="item"/> in the <paramref name="sequence"/>. 
        /// If the sequence does not contain the element, -1 is returned. Note, the lookup is performed 
        /// using <see cref="object.Equals(object)"/>
        /// </summary>
        /// <param name="sequence">Sequence to process</param>
        /// <param name="item">Item which index to look up</param>
        /// <returns>Index of the item or -1</returns>
        public static int IndexOf(this IEnumerable sequence, object item) {
            return IndexOf(sequence, seqItem => Equals(seqItem, item));
        }

        /// <summary>
        /// Returns the index of the first item that matches the specified <paramref name="predicate"/> in the <paramref name="sequence"/>. 
        /// If the sequence does not contain an element, -1 is returned.
        /// </summary>
        /// <param name="sequence">Sequence to process</param>
        /// <param name="predicate">Predicate to find the desired element</param>
        /// <returns>Index of the item or -1</returns>
        public static int IndexOf(this IEnumerable sequence, Predicate<object> predicate) {
            return Iterate(sequence, itr => {
                int index = -1;
                while (itr.MoveNext()) {
                    index++;
                    if (predicate(itr.Current)) return index;
                }
                return -1;
            });
        }

        /// <summary>
        /// Returns TRUE if the sequence contains any element.
        /// </summary>
        /// <param name="sequence">Sequence to proof</param>
        /// <returns>TRUE or FALSE</returns>
        public static bool Any(this IEnumerable sequence) {
            if (sequence is ICollection coll) return coll.Count != 0;

            return sequence.Iterate(itr => {
                while (itr.MoveNext()) {
                    return true;
                }
                return false;
            });
        }

        /// <summary>
        /// Returns the count of elements in the specified <paramref name="sequence"/>.
        /// </summary>
        /// <param name="sequence">Sequence to proof</param>
        /// <returns>Count of elements</returns>
        public static int Count(this IEnumerable sequence) {
            // Matches array, list, ...?
            if (sequence is ICollection collection) return collection.Count;
            
            if (sequence == null) throw new NullReferenceException();

            int count = 0;
            sequence.ForEach(obj => count++);
            return count;
        }

        /// <summary>
        /// Applies the given <paramref name="action"/> to each element of the given <paramref name="sequence"/>.
        /// </summary>
        /// <param name="sequence">Collection of items</param>
        /// <param name="action">Action to apply to each element</param>
        public static void ForEach(this IEnumerable sequence, Action<object> action) {
            if (sequence == null) throw new NullReferenceException();
            sequence.Iterate(itr => {
                while (itr.MoveNext()) {
                    object item = itr.Current;
                    action(item);
                }
            });
        }

        /// <summary>
        /// Applies the given strong-typed <paramref name="action"/> to each element of the given <paramref name="sequence"/>.
        /// </summary>
        /// <typeparam name="TItem">Type each element of the sequence is casted to</typeparam>
        /// <param name="sequence">Collection of items</param>
        /// <param name="action">Action to apply to each element</param>
        public static void ForEach<TItem>(this IEnumerable sequence, Action<TItem> action) {
            sequence.ForEach(obj => action((TItem) obj));
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

            return sequence.Iterate(itr => {
                while (itr.MoveNext()) {
                    object current = itr.Current;
                    if (predicate(current)) return current;
                }
                return null;
            });
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
            return sequence.Select<TResult>(obj => selector((TItem) obj));
        }

        /// <summary>
        /// Provides access to the iterator of the specified <paramref name="sequence"/>. 
        /// The iterator can be operated using the specified <paramref name="iterate"/> delegate. 
        /// However, after the end of sequence iteration the iterator is disposed carefully.
        /// </summary>
        /// <param name="sequence">Sequence to iterate</param>
        /// <param name="iterate">Iterator delegate</param>
        public static void Iterate(this IEnumerable sequence, Action<IEnumerator> iterate) {
            if (sequence == null) throw new NullReferenceException();
            sequence.Iterate(itr => {
                iterate(itr);
                return true;
            });
        }

        /// <summary>
        /// Provides access to the iterator of the specified <paramref name="sequence"/>. 
        /// The iterator can be operated using the specified <paramref name="iterate"/> delegate. 
        /// However, after the end of sequence iteration the iterator is disposed carefully. 
        /// If the iterator returns a value, its returned by this method call.
        /// </summary>
        /// <param name="sequence">Sequence to iterate</param>
        /// <param name="iterate">Iterator delegate</param>
        /// <returns>Value that has been returned by the iterator delegate</returns>
        public static TResult Iterate<TResult>(this IEnumerable sequence, Func<IEnumerator, TResult> iterate) {
            if (sequence == null) throw new NullReferenceException();

            IEnumerator itr = sequence.GetEnumerator();

            try {
                return iterate(itr);
            } finally {
                (itr as IDisposable)?.Dispose();
            }
        }
    }
}