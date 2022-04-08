using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace xcite.collections {
    /// <summary>
    /// Provides an implementation of a linear list.
    /// </summary>
    /// <typeparam name="TItem">Data type of managed items</typeparam>
    [DebuggerTypeProxy(typeof(CollectionDebugView<>))]
    public class LinearList<TItem> : ICollection<TItem>, ISortable<TItem> {
        
        private LinearListItem _head;
        private LinearListItem _tail;

        /// <summary>  Creates a new instance. </summary>
        public LinearList() {
            // Nothing to do here
        }

        /// <summary>
        /// Creates a new instance that adds all items of the given <paramref name="enumerable"/>.
        /// </summary>
        /// <param name="enumerable">Enumerable with items to add</param>
        public LinearList(IEnumerable<TItem> enumerable) {
            if (enumerable == null) throw new ArgumentNullException(nameof(enumerable));
            enumerable.ForEach(Add);
        }

        /// <inheritdoc />
        public virtual void Add(TItem item) {
            if (item == null) return;

            if (_head == null) {
                _head = new LinearListItem(item);
                _tail = _head;
            } else {
                LinearListItem oldTail = _tail;
                _tail = new LinearListItem(item);
                oldTail.Next = _tail;
            }

            Count++;
        }

        /// <inheritdoc />
        public virtual void Clear() {
            _head = null;
            _tail = null;
            Count = 0;
        }

        /// <inheritdoc />
        public virtual bool Contains(TItem item) {
            if (item == null) return false;

            using (IEnumerator<TItem> itr = GetEnumerator()) {
                while (itr.MoveNext()) {
                    TItem listItem = itr.Current;
                    if (Equals(listItem, item)) return true;
                }
            }

            return false;
        }

        /// <inheritdoc />
        public virtual void CopyTo(TItem[] array, int arrayIndex) {
            TItem[] listItems = ToArray();
            if (listItems.Length + arrayIndex > array.Length)
                throw new ArgumentException("Given array does not have enough space");
            Array.Copy(listItems, 0, array, arrayIndex, listItems.Length);
        }

        /// <inheritdoc />
        public virtual int Count { get; private set; }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() 
            => GetEnumerator();

        /// <inheritdoc />
        public virtual IEnumerator<TItem> GetEnumerator() 
            => new LinearListEnumerator(this);

        /// <inheritdoc />
        public virtual bool IsReadOnly
            => false;

        /// <inheritdoc />
        public virtual bool Remove(TItem item) {
            if (item == null) return false;

            LinearListItem precedingItem = null;
            LinearListItem current = _head;

            while (current != null) {
                TItem currentItem = current.Item;

                if (!Equals(currentItem, item)) {
                    precedingItem = current;
                    current = current.Next;
                    continue;
                }

                // We matched the head
                if (current == _head) {
                    _head = current.Next;
                }

                // We matched the tail
                if (current == _tail) {
                    _tail = precedingItem;
                }

                // We matched any item in sequence
                if (precedingItem != null) {
                    precedingItem.Next = current.Next;
                }

                Count--;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the list as array.
        /// </summary>
        /// <returns>Array containing all elements</returns>
        public virtual TItem[] ToArray() {
            TItem[] array = new TItem[Count];
            int i = 0;

            using (IEnumerator<TItem> itr = GetEnumerator()) {
                while (itr.MoveNext()) {
                    TItem item = itr.Current;
                    array[i++] = item;
                }
            }

            return array;
        }

        /// <inheritdoc />
        public override string ToString() {
            string result = $"Count = {Count}";
            return result;
        }

        #region ISortable

        /// <inheritdoc />
        void ISortable.Sort(Comparison<object> comparison) {
            SortInternal((e1, e2) => comparison(e1, e2));
        }

        /// <inheritdoc />
        void ISortable.Sort(IComparer comparer) {
            SortInternal((e1, e2) => comparer.Compare(e1, e2));
        }

        /// <inheritdoc />
        void ISortable.Sort<TProperty>(Func<object, TProperty> propertyReference) {
            SortInternal((e1, e2) => {
                TProperty v1 = propertyReference(e1);
                TProperty v2 = propertyReference(e2);
                return Comparer<TProperty>.Default.Compare(v1, v2);
            });
        }

        /// <inheritdoc />
        public void Sort(Comparison<TItem> comparison) {
            SortInternal(comparison);
        }

        /// <inheritdoc />
        public void Sort(Comparer<TItem> comparer) {
            SortInternal(comparer.Compare);
        }

        /// <inheritdoc />
        public void Sort<TProperty>(Func<TItem, TProperty> propertyReference) {
            SortInternal((e1, e2) => {
                TProperty v1 = propertyReference(e1);
                TProperty v2 = propertyReference(e2);
                return Comparer<TProperty>.Default.Compare(v1, v2);
            });
        }

        /// <summary>
        /// Sorts the item of the collection using the given <paramref name="comparison"/> 
        /// in ascending order. 
        /// </summary>
        /// <param name="comparison">Comparison</param>
        private void SortInternal(Comparison<TItem> comparison) {
            // Note, the sort may be faster, if we use a 'sort by insert' (binary) approach

            LinearListItem currentRef = _head;

            // Iterate until the end
            while (currentRef != null) {
                // Current sublist head
                LinearListItem subListHead = currentRef;

                // Reference to current sublist minimum item
                LinearListItem minSublistItem = subListHead;
                TItem minItem = minSublistItem.Item;

                // Sublist iterator
                LinearListItem sublistCursor = subListHead;
                while (sublistCursor != null) {
                    TItem sublistCursorItem = sublistCursor.Item;

                    // If the current sublist cursor item is lower, we have a new minimum item
                    bool isLower = comparison(sublistCursorItem, minItem) < 0;
                    if (isLower) {
                        minSublistItem = sublistCursor;
                        minItem = sublistCursorItem;
                    }

                    // Move on
                    sublistCursor = sublistCursor.Next;
                }

                // Now we have the lowest item, so we can swap it
                if (minSublistItem != subListHead) { // Swap only if we have a real change
                    TItem buffer = subListHead.Item;
                    subListHead.Item = minSublistItem.Item;
                    minSublistItem.Item = buffer;
                }

                // Move on
                currentRef = currentRef.Next;
            }
        }

        #endregion

        /// <summary> Implements a linear list item. </summary>
        class LinearListItem {
            /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
            public LinearListItem(TItem item) {
                Item = item;
            }

            /// <summary> Stored item </summary>
            public TItem Item; // TODO Check readonly removement

            /// <summary> Next list item. May be NULL. </summary>
            public LinearListItem Next;

            /// <inheritdoc />
            public override string ToString() {
                if (Item == null) return string.Empty;
                return Item.ToString();
            }
        }

        /// <summary>
        /// Provides an implementation of <see cref="IEnumerator{T}"/> for a <see cref="LinearList{TItem}"/>.
        /// </summary>
        class LinearListEnumerator : IEnumerator<TItem> {
            private readonly LinearList<TItem> iLinearList;
            private LinearListItem iCurrent;

            /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
            public LinearListEnumerator(LinearList<TItem> linearList) {
                iLinearList = linearList;

                // Must not be NULL
                iCurrent = new LinearListItem(default(TItem)) {Next = linearList._head};
            }

            /// <inheritdoc />
            public void Dispose() {
                // Currently nothing to do here
            }

            /// <inheritdoc />
            public bool MoveNext() {
                if (iCurrent.Next == null) return false;
                iCurrent = iCurrent.Next;
                return true;
            }

            /// <inheritdoc />
            public void Reset() 
                => iCurrent = iLinearList._head;

            /// <inheritdoc />
            public TItem Current 
                => iCurrent.Item;

            /// <inheritdoc />
            object IEnumerator.Current 
                => Current;
        }
    }
}