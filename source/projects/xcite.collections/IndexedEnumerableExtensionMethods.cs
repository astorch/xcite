using System;
using System.Collections;
using System.Collections.Generic;

namespace xcite.collections {
    /// <summary>
    /// Provides method extensions for <see cref="IIndexedEnumerable"/>.
    /// </summary>
    public static class IndexedEnumerableExtensionMethods {
        /// <summary>
        /// Indexes all elements of the given <paramref name="sequence"/>.
        /// </summary>
        /// <param name="sequence">Sequence of elements to index</param>
        /// <returns>
        /// An <see cref="IIndexedEnumerable"/> that contains each element of the source sequence 
        /// with an assigned index.
        /// </returns>
        public static IIndexedEnumerable Index(this IEnumerable sequence) {
            return new IndexedEnumerable(sequence);
        }

        /// <summary>
        /// Applies the given <paramref name="action"/> to each element of the given <paramref name="indexedSequence"/>. 
        /// In addition to the element of the sequence the action also gets index of it during this processing. Note that 
        /// the given index must no be equal to its real index within the underlying collection.
        /// </summary>
        /// <param name="indexedSequence">Collection of items</param>
        /// <param name="action">Action to apply to each element</param>
        public static void ForEach(this IIndexedEnumerable indexedSequence, Action<int, object> action) {
            if (indexedSequence == null) throw new NullReferenceException();
            IEnumerator itr = indexedSequence.GetEnumerator();

            try {
                int i = -1;
                while (itr.MoveNext()) {
                    action(++i, itr.Current);
                }
            } finally {
                (itr as IDisposable)?.Dispose();
            }
        }

        /// <summary>
        /// Projects each element of a <paramref name="indexedSequence"/> into a form provided by the given <paramref name="selector"/>. 
        /// In addition to the element of the sequence the action also gets index of it during this processing. Note that 
        /// the given index must no be equal to its real index within the underlying collection.
        /// </summary>
        /// <typeparam name="TResult">Type of object resulting the transform function</typeparam>
        /// <param name="indexedSequence">Collection of items</param>
        /// <param name="selector">Transform function</param>
        /// <returns>Collection of transformed items</returns>
        public static IEnumerable<TResult> Select<TResult>(this IIndexedEnumerable indexedSequence,
            Func<int, object, TResult> selector) {
            int i = -1;
            for (IEnumerator itr = indexedSequence.GetEnumerator(); itr.MoveNext();) {
                yield return selector(++i, itr.Current);
            }
        }

        /// <summary>
        /// Indexes all elements of the given strong-typed <paramref name="sequence"/>.
        /// </summary>
        /// <typeparam name="TItem">Type of elements to iterate</typeparam>
        /// <param name="sequence">Sequence of elements to index</param>
        /// <returns>
        /// An <see cref="IIndexedEnumerable{TItem}"/> that contains each element of the source sequence 
        /// with an assigned index.
        /// </returns>
        public static IIndexedEnumerable<TItem> Index<TItem>(this IEnumerable<TItem> sequence) {
            return new IndexedEnumerable<TItem>(sequence);
        }

        /// <summary>
        /// Applies the given strong-typed <paramref name="action"/> to each element of the given <paramref name="indexedSequence"/>. 
        /// In addition to the element of the sequence the action also gets index of it during this processing. Note that 
        /// the given index must no be equal to its real index within the underlying collection.
        /// </summary>
        /// <typeparam name="TItem">Type of elements to iterate</typeparam>
        /// <param name="indexedSequence">Collection of items</param>
        /// <param name="action">Action to apply to each element</param>
        public static void ForEach<TItem>(this IIndexedEnumerable<TItem> indexedSequence, Action<int, TItem> action) {
            int i = -1;
            using (IEnumerator<TItem> itr = indexedSequence.GetEnumerator()) {
                while (itr.MoveNext()) {
                    action(++i, itr.Current);
                }
            }
        }

        /// <summary>
        /// Projects each strong-typed element a <paramref name="indexedSequence"/> into a form provided by the given <paramref name="selector"/>. 
        /// In addition to the element of the sequence the action also gets index of it during this processing. Note that 
        /// the given index must no be equal to its real index within the underlying collection.
        /// </summary>
        /// <typeparam name="TItem">Type of elements to iterate</typeparam>
        /// <typeparam name="TResult">Type of object resulting the transform function</typeparam>
        /// <param name="indexedSequence">Collection of items</param>
        /// <param name="selector">Transform function</param>
        /// <returns>Collection of transformed items</returns>
        public static IEnumerable<TResult> Select<TItem, TResult>(this IIndexedEnumerable<TItem> indexedSequence,
            Func<int, TItem, TResult> selector) {
            int i = -1;
            using (IEnumerator<TItem> itr = indexedSequence.GetEnumerator()) {
                while (itr.MoveNext()) {
                    yield return selector(++i, itr.Current);
                }
            }
        }

        /// <summary>
        /// Provides an anonymous implementation of <see cref="IIndexedEnumerable"/>.
        /// </summary>
        class IndexedEnumerable : IIndexedEnumerable {
            private readonly IEnumerable _enumerable;

            /// <summary>
            /// Creates a new instance based on the given <paramref name="enumerable"/>.
            /// </summary>
            /// <param name="enumerable">Underlying enumerable</param>
            public IndexedEnumerable(IEnumerable enumerable) => _enumerable = enumerable;

            /// <inheritdoc />
            public IEnumerator GetEnumerator() => _enumerable.GetEnumerator();
        }

        /// <summary>
        /// Provides an anonymous implementation of <see cref="IIndexedEnumerable{TItem}"/>
        /// </summary>
        /// <typeparam name="TItem">Type of objects to enumerate</typeparam>
        class IndexedEnumerable<TItem> : IIndexedEnumerable<TItem> {
            private readonly IEnumerable<TItem> _enumerable;

            /// <summary>
            /// Creates a new instance based on the given <paramref name="enumerable"/>.
            /// </summary>
            /// <param name="enumerable">Underlying enumerable</param>
            public IndexedEnumerable(IEnumerable<TItem> enumerable) => _enumerable = enumerable;

            /// <inheritdoc />
            public IEnumerator<TItem> GetEnumerator() => _enumerable.GetEnumerator();

            /// <inheritdoc />
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}