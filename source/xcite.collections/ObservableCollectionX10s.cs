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
        public static IObservableCollectionSubset<TElement> Where<TElement>(this IObservableCollection<TElement> collection,
            Predicate<TElement> wherePredicate) {
            if (collection == null) throw new NullReferenceException();
            if (wherePredicate == null) throw new ArgumentNullException(nameof(wherePredicate));
            
            return new ObervableCollectionSubset<TElement>(collection, wherePredicate);
        }

        /// <summary>
        /// Implements <see cref="IObservableCollection{TItem}"/>.
        /// </summary>
        /// <typeparam name="TElement">Type of managed elements</typeparam>
        class ObervableCollectionSubset<TElement> : IObservableCollectionSubset<TElement> {
            private readonly LinearList<IEnumerableListener<TElement>> _genericListener = new LinearList<IEnumerableListener<TElement>>();
            private readonly LinearList<IEnumerableListener> _listener = new LinearList<IEnumerableListener>();
            private readonly IObservableCollection<TElement> _originSet;
            private readonly Predicate<TElement> _wherePredicate;
            private LinearList<TElement> _elementCache;

            /// <summary>
            /// Initializes the new instance.
            /// </summary>
            /// <param name="originSet">Origin set to iterate</param>
            /// <param name="wherePredicate">Predicate used for filtering</param>
            public ObervableCollectionSubset(IObservableCollection<TElement> originSet, Predicate<TElement> wherePredicate) {
                _originSet = originSet ?? throw new ArgumentNullException(nameof(originSet));
                _wherePredicate = wherePredicate ?? throw new ArgumentNullException(nameof(wherePredicate));

                // If the collection has been changed, we need to clear the cache
                _originSet.AddListener(new CollectionListenerAdapter<TElement>(OnOriginSetChanged));
            }

            /// <summary>
            /// Is invoked when the origin set has been changed.
            /// </summary>
            /// <param name="collection">Modified collection</param>
            /// <param name="args">Event arguments</param>
            private void OnOriginSetChanged(IObservableEnumerable<TElement> collection, CollectionChangedEventArgs<TElement> args) {
                if (_elementCache == null) return; // Elements haven't been request yet

                // If the origin set has been clear, there can't be any elements
                if (args.Reason == ECollectionChangeReason.Cleared) {
                    _elementCache.Clear();
                    return;
                }

                // An item has been added or removed, but is it relevant for this subset?
                bool isRelevant = _wherePredicate(args.Item);
                if (!isRelevant) return;
                
                if (args.Reason == ECollectionChangeReason.Added)
                    _elementCache.Add(args.Item);
                else
                    _elementCache.Remove(args.Item);
            }

            /// <inheritdoc />
            IEnumerator IEnumerable.GetEnumerator() 
                => GetEnumerator();

            /// <inheritdoc />
            public IEnumerator<TElement> GetEnumerator() {
                if (_elementCache != null) return _elementCache.GetEnumerator();

                // Copy all matching items into the cache
                _elementCache = new LinearList<TElement>();
                using (FilterIterator itr = new FilterIterator(_originSet.GetEnumerator(), _wherePredicate)) {
                    while (itr.MoveNext()) {
                        _elementCache.Add(itr.Current);
                    }
                }

                return _elementCache.GetEnumerator();
            }

            /// <inheritdoc />
            public void AddListener(IEnumerableListener<TElement> listener) {
                _originSet.AddListener(listener);
                _genericListener.Add(listener);
            }

            /// <inheritdoc />
            public void RemoveListener(IEnumerableListener<TElement> listener) {
                _genericListener.Remove(listener);
                _originSet.RemoveListener(listener);
            }

            /// <inheritdoc />
            void IObservableEnumerable.AddListener(IEnumerableListener listener) {
                _originSet.AddListener(listener);
                _listener.Add(listener);
            }

            /// <inheritdoc />
            void IObservableEnumerable.RemoveListener(IEnumerableListener listener) {
                _listener.Remove(listener);
                _originSet.RemoveListener(listener);
            }

            /// <inheritdoc />
            public void Add(TElement item)
                => Add(item, true);

            /// <inheritdoc />
            public void Add(TElement item, bool modifySource) {
                if (modifySource) {
                    _originSet.Add(item);
                } else {
                    RaiseFakeCollectionChangedEvent(true, item);
                }
            }

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
                => Remove(item, true);

            /// <inheritdoc />
            public bool Remove(TElement item, bool modifySource) {
                if (modifySource) return _originSet.Remove(item);

                RaiseFakeCollectionChangedEvent(false, item);
                return true;
            }

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

            /// <summary>
            /// Raises a fake collection changed event.
            /// </summary>
            /// <param name="add">TRUE if an add event shall be raised</param>
            /// <param name="item">Item that has been affected</param>
            private void RaiseFakeCollectionChangedEvent(bool add, TElement item) {
                Action<IEnumerableListener<TElement>> genericAction;
                Action<IEnumerableListener> rawAction;

                if (add) {
                    genericAction = l => l.OnItemAdded(this, item);
                    rawAction = l => l.OnItemAdded(this, item);
                } else {
                    genericAction = l => l.OnItemRemoved(this, item);
                    rawAction = l => l.OnItemRemoved(this, item);
                }

                _listener.ForEach(rawAction);
                _genericListener.ForEach(genericAction);
            }

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