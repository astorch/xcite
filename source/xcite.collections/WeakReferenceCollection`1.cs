using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace xcite.collections {
    /// <summary>
    /// Implements <see cref="ICollection{T}"/> that holds items using <see cref="WeakReference"/>. By accessing the collection, 
    /// each item is checked if it's not collected by the GC. If an item has been garbage collected, it is removed from the collection. 
    /// Therefore, you have to notice that the content of the collection may change continuously.
    /// </summary>
    /// <typeparam name="TItem">Type of managed items</typeparam>
    public class WeakReferenceCollection<TItem> : ICollection<TItem> {
        private readonly LinearList<WeakReference> iWeakReferences = new LinearList<WeakReference>();

        /// <summary>
        /// Removes all items of the collection that have been garbage collected.
        /// </summary>
        public void Trim() {
            lock (iWeakReferences) {
                WeakReference[] weakReferences = iWeakReferences.ToArray();
                for (short i = -1; ++i != weakReferences.Length;) {
                    WeakReference weakReference = weakReferences[i];
                    if (!weakReference.IsAlive)
                        iWeakReferences.Remove(weakReference);
                }
            }
        }

        /// <summary>Returns an enumerator that iterates through the collection.</summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<TItem> GetEnumerator() {
            Trim();
            lock (iWeakReferences) {
                return new WeakReferenceEnumerator(iWeakReferences.GetEnumerator());
            }
        }

        /// <summary>Returns an enumerator that iterates through a collection.</summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        /// <summary>Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        public void Add(TItem item) {
            lock (iWeakReferences) {
                WeakReference weakReference = new WeakReference(item);
                iWeakReferences.Add(weakReference);
            }
        }

        /// <summary>Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only. </exception>
        public void Clear() {
            lock (iWeakReferences) {
                iWeakReferences.Clear();
            }
        }

        /// <summary>Determines whether the <see cref="T:System.Collections.Generic.ICollection`1" /> contains a specific value.</summary>
        /// <returns>true if <paramref name="item" /> is found in the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false.</returns>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        public bool Contains(TItem item) {
            Trim();

            lock (iWeakReferences) {
                return iWeakReferences.Any(weakReference => Equals(item, weakReference.Target));
            }
        }

        /// <summary>Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1" /> to an <see cref="T:System.Array" />, starting at a particular <see cref="T:System.Array" /> index.</summary>
        /// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1" />. The <see cref="T:System.Array" /> must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in <paramref name="array" /> at which copying begins.</param>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="array" /> is null.</exception>
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        /// <paramref name="arrayIndex" /> is less than 0.</exception>
        /// <exception cref="T:System.ArgumentException">The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1" /> is greater than the available space from <paramref name="arrayIndex" /> to the end of the destination <paramref name="array" />.</exception>
        public void CopyTo(TItem[] array, int arrayIndex) {
            Trim();

            TItem[] items;
            lock (iWeakReferences) {
                items = iWeakReferences.Select(weakReference => weakReference.Target).Cast<TItem>().ToArray();
            }

            Array.Copy(items, 0, array, arrayIndex, items.Length);
        }


        /// <summary>Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <returns>true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        public bool Remove(TItem item) {
            lock (iWeakReferences) {
                WeakReference weakReference = iWeakReferences.FirstOrDefault(wr => Equals(wr.Target, item));
                if (weakReference == null) return false;
                return iWeakReferences.Remove(weakReference);
            }
        }

        /// <summary>Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <returns>The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        public int Count {
            get {
                Trim();
                lock (iWeakReferences) {
                    return iWeakReferences.Count;
                }
            }
        }

        /// <summary>Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</summary>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only; otherwise, false.</returns>
        public bool IsReadOnly => false;

        /// <summary>
        /// Implements <see cref="IEnumerator{TItem}"/> for the <see cref="WeakReferenceCollection{TItem}"/>.
        /// </summary>
        class WeakReferenceEnumerator : IEnumerator<TItem> {
            private readonly IEnumerator<WeakReference> iWeakReferenceEnumerator;

            /// <summary>
            /// Creates a new instance utilizing the given <paramref name="weakReferenceEnumerator"/>.
            /// </summary>
            /// <param name="weakReferenceEnumerator">Underlying weak reference enumerator</param>
            public WeakReferenceEnumerator(IEnumerator<WeakReference> weakReferenceEnumerator) {
                iWeakReferenceEnumerator = weakReferenceEnumerator;
            }

            /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
            /// <filterpriority>2</filterpriority>
            public void Dispose() => iWeakReferenceEnumerator.Dispose();

            /// <summary>Advances the enumerator to the next element of the collection.</summary>
            /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            /// <filterpriority>2</filterpriority>
            public bool MoveNext() => iWeakReferenceEnumerator.MoveNext();

            /// <summary>Sets the enumerator to its initial position, which is before the first element in the collection.</summary>
            /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception>
            /// <filterpriority>2</filterpriority>
            public void Reset() => iWeakReferenceEnumerator.Reset();

            /// <summary>Gets the element in the collection at the current position of the enumerator.</summary>
            /// <returns>The element in the collection at the current position of the enumerator.</returns>
            public TItem Current {
                get {
                    WeakReference wr = iWeakReferenceEnumerator.Current;
                    return (TItem) wr.Target;
                }
            }

            /// <summary>Gets the current element in the collection.</summary>
            /// <returns>The current element in the collection.</returns>
            /// <filterpriority>2</filterpriority>
            object IEnumerator.Current => Current;
        }
    }
}