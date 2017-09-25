using System;
using System.Collections;
using System.Collections.Generic;

namespace xcite.collections {
    /// <summary> Implements extension methods for <see cref="ObservableCollection{TItem}"/>. </summary>
    public static class ObservableCollectionX10S {

        /// <summary>
        /// Filters the sequence using the specified <paramref name="wherePredicate"/>.
        /// </summary>
        /// <typeparam name="TElement">Type of elements in the collection</typeparam>
        /// <param name="collection">Collection to filter</param>
        /// <param name="wherePredicate">Predicate used for filtering</param>
        /// <returns>Observable enumerable with filtered elements</returns>
        public static IObservableCollection<TElement> Where<TElement>(this IObservableCollection<TElement> collection,
            Predicate<TElement> wherePredicate) {
            if (collection == null) throw new NullReferenceException();
            if (wherePredicate == null) throw new ArgumentNullException(nameof(wherePredicate));
            
            return new _ObervableCollection<TElement>(collection, wherePredicate);
        }

        /// <summary>
        /// Implements <see cref="IObservableEnumerable{TItem}"/>.
        /// </summary>
        /// <typeparam name="TElement">Type of managed elements</typeparam>
        class _ObervableCollection<TElement> : IObservableCollection<TElement> {
            private readonly IObservableCollection<TElement> _originSet;
            private readonly Predicate<TElement> _wherePredicate;

            /// <summary>
            /// Initializes the new instance.
            /// </summary>
            /// <param name="originSet">Origin set to iterate</param>
            /// <param name="wherePredicate">Predicate used for filtering</param>
            public _ObervableCollection(IObservableCollection<TElement> originSet, Predicate<TElement> wherePredicate) {
                _originSet = originSet ?? throw new ArgumentNullException(nameof(originSet));
                _wherePredicate = wherePredicate ?? throw new ArgumentNullException(nameof(wherePredicate));
            }

            /// <inheritdoc />
            IEnumerator IEnumerable.GetEnumerator() 
                => GetEnumerator();

            /// <inheritdoc />
            public IEnumerator<TElement> GetEnumerator() {
                return new FilterIterator(_originSet.GetEnumerator(), _wherePredicate);
            }

            /// <inheritdoc />
            public void AddListener(IEnumerableListener<TElement> listener) 
                => _originSet.AddListener(listener);

            /// <inheritdoc />
            public void RemoveListener(IEnumerableListener<TElement> listener) 
                => _originSet.RemoveListener(listener);

            /// <inheritdoc />
            void IObservableEnumerable.AddListener(IEnumerableListener listener)
                => _originSet.AddListener(listener);

            /// <inheritdoc />
            public void RemoveListener(IEnumerableListener listener) 
                => _originSet.RemoveListener(listener);

            /// <inheritdoc />
            public void Add(TElement item)
                => _originSet.Add(item);

            /// <inheritdoc />
            public void Clear()
                => _originSet.Clear();

            /// <inheritdoc />
            public bool Contains(TElement item) {
                using (IEnumerator<TElement> itr = GetEnumerator()) {
                    while (itr.MoveNext()) {
                        if (Equals(itr.Current, item)) return true;
                    }
                }

                return false;
            }

            /// <inheritdoc />
            public void CopyTo(TElement[] array, int arrayIndex) {
                using (IEnumerator<TElement> itr = GetEnumerator()) {
                    while (itr.MoveNext()) {
                        array[arrayIndex++] = itr.Current;
                    }
                }
            }

            /// <inheritdoc />
            public bool Remove(TElement item) 
                => _originSet.Remove(item);

            /// <inheritdoc />
            public int Count {
                get {
                    int count = 0;
                    using (IEnumerator<TElement> itr = GetEnumerator()) {
                        while (itr.MoveNext()) {
                            count++;
                        }
                    }

                    return count;
                }
            }

            /// <inheritdoc />
            public bool IsReadOnly { get; } = true;

            /// <summary> Implements a filtering iterator. </summary>
            class FilterIterator : IEnumerator<TElement> {
                private readonly IEnumerator<TElement> _baseIterator;
                private readonly Predicate<TElement> _wherePredicate;

                /// <summary>
                /// Initializes the new instance.
                /// </summary>
                /// <param name="baseIterator">Base iterator</param>
                /// <param name="wherePredicate">Predicate used for filtering</param>
                public FilterIterator(IEnumerator<TElement> baseIterator, Predicate<TElement> wherePredicate) {
                    _baseIterator = baseIterator;
                    _wherePredicate = wherePredicate;
                }
                
                /// <inheritdoc />
                public bool MoveNext() {
                    while (true) {
                        bool moveNext = _baseIterator.MoveNext();
                        if (!moveNext) return false;
                        Current = _baseIterator.Current;
                        if (!_wherePredicate(Current)) continue;
                        return true;
                    }
                }

                /// <inheritdoc />
                public void Reset() {
                    _baseIterator.Reset();
                    Current = default(TElement);
                }

                /// <inheritdoc />
                public TElement Current { get; private set; }

                /// <inheritdoc />
                object IEnumerator.Current 
                    => Current;

                /// <inheritdoc />
                public void Dispose() {
                    _baseIterator.Dispose();
                }
            }
        }
    }
}