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
    public class LinearList<TItem> : ICollection<TItem> {
        // ReSharper disable once InconsistentNaming
        private LinearListItem Head;

        // ReSharper disable once InconsistentNaming
        private LinearListItem Tail;

        /// <summary>
        /// Creates a new instance.
        /// </summary>
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

        /// <summary>Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        public virtual void Add(TItem item) {
            if (item == null) return;

            if (Head == null) {
                Head = new LinearListItem(item);
                Tail = Head;
            } else {
                LinearListItem oldTail = Tail;
                Tail = new LinearListItem(item);
                oldTail.Next = Tail;
            }

            Count++;
        }

        /// <summary>Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only. </exception>
        public virtual void Clear() {
            Head = null;
            Tail = null;
            Count = 0;
        }

        /// <summary>Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.</summary>
        /// <returns>true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.</returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
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

        /// <summary>Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.</summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array" /> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex" /> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1" /> is greater than the available space from <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.</exception>
        public virtual void CopyTo(TItem[] array, int arrayIndex) {
            TItem[] listItems = ToArray();
            if (listItems.Length + arrayIndex > array.Length)
                throw new ArgumentException("Given array does not have enough space");
            Array.Copy(listItems, 0, array, arrayIndex, listItems.Length);
        }

        /// <summary>Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        public virtual int Count { get; private set; }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public virtual IEnumerator<TItem> GetEnumerator() => new LinearListEnumerator(this);

        /// <summary>Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</summary>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.</returns>
        public virtual bool IsReadOnly => false;

        /// <summary>Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <returns>true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        public virtual bool Remove(TItem item) {
            if (item == null) return false;

            LinearListItem precedingItem = null;
            LinearListItem current = Head;

            while (current != null) {
                TItem currentItem = current.Item;

                if (!Equals(currentItem, item)) {
                    precedingItem = current;
                    current = current.Next;
                    continue;
                }

                // We matched the head
                if (current == Head) {
                    Head = current.Next;
                }

                // We matched the tail
                if (current == Tail) {
                    Tail = precedingItem;
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

        /// <summary>Returns a string that represents the current object.</summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() {
            string result = string.Format("Count = {0}", Count);
            return result;
        }

        /// <summary>
        /// Implements a linear list item.
        /// </summary>
        class LinearListItem {
            /// <summary>Initializes a new instance of the <see cref="T:System.Object" /> class.</summary>
            public LinearListItem(TItem item) {
                Item = item;
            }

            /// <summary>
            /// Returns the stored item or does set it.
            /// </summary>
            public readonly TItem Item;

            /// <summary>
            /// Returns the next list item. May be NULL.
            /// </summary>
            public LinearListItem Next;

            /// <summary>Returns a string that represents the current object.</summary>
            /// <returns>A string that represents the current object.</returns>
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
                iCurrent = new LinearListItem(default(TItem)) {Next = linearList.Head};
            }

            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            public void Dispose() {
                // Currently nothing to do here
            }

            /// <summary>Advances the enumerator to the next element of the collection.</summary>
            /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public bool MoveNext() {
                if (iCurrent.Next == null) return false;
                iCurrent = iCurrent.Next;
                return true;
            }

            /// <summary>Sets the enumerator to its initial position, which is before the first element in the collection.</summary>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            public void Reset() => iCurrent = iLinearList.Head;

            /// <summary>Gets the element in the collection at the current position of the enumerator.</summary>
            /// <returns>The element in the collection at the current position of the enumerator.</returns>
            public TItem Current => iCurrent.Item;

            /// <summary>Gets the current element in the collection.</summary>
            /// <returns>The current element in the collection.</returns>
            object IEnumerator.Current => Current;
        }
    }
}