using System;
using System.Collections.Generic;

namespace xcite.collections {
    /// <summary>
    /// Provides an implementation of <see cref="IObservableCollection{TItem}"/> that is based on a <see cref="LinearList{TItem}"/>.
    /// </summary>
    /// <typeparam name="TItem">Data type of managed items</typeparam>
    public class ObservableCollection<TItem> : LinearList<TItem>, IObservableCollection<TItem> {
        private readonly LinearList<ICollectionListener<TItem>> _registeredListener = new LinearList<ICollectionListener<TItem>>();

        /// <summary> Initializes the new instance. </summary>
        public ObservableCollection() {
            // Currently nothing to do here
        }

        /// <summary>
        /// Creates a new instance that adds all items of the given <paramref name="enumerable"/>.
        /// </summary>
        /// <param name="enumerable">Enumerable with items to add</param>
        public ObservableCollection(IEnumerable<TItem> enumerable) : base(enumerable) {
            // Currently nothing to do here
        }

        /// <summary>
        /// Subscribes the given <paramref name="listener"/> to this instance.
        /// </summary>
        /// <param name="listener">Listener</param>
        public void AddCollectionListener(ICollectionListener<TItem> listener) {
            if (listener == null) return;
            lock (_registeredListener) {
                _registeredListener.Add(listener);
            }
        }

        /// <summary>
        /// Unsubscribes the given <paramref name="listener"/> from this instance.
        /// </summary>
        /// <param name="listener">Listener</param>
        public void RemoveCollectionListener(ICollectionListener<TItem> listener) {
            if (listener == null) return;
            lock (_registeredListener) {
                _registeredListener.Remove(listener);
            }
        }

        /// <summary>Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        public override void Add(TItem item) {
            base.Add(item);
            RaiseAddedEvent(item);
        }

        /// <summary>Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <returns>true if <paramref name="item" /> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1" />; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1" />.</returns>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1" />.</param>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only.</exception>
        public override bool Remove(TItem item) {
            if (!base.Remove(item)) return false;
            RaiseRemovedEvent(item);
            return true;
        }

        /// <summary>Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1" />.</summary>
        /// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.Generic.ICollection`1" /> is read-only. </exception>
        public override void Clear() {
            base.Clear();
            RaiseClearedEvent();
        }

        /// <summary>
        /// Raises the <see cref="ICollectionListener{TItem}.OnItemAdded"/> event for the given <paramref name="item"/>.
        /// </summary>
        /// <param name="item">Affected item</param>
        protected virtual void RaiseAddedEvent(TItem item) {
            DispatchEvent(lstnr => lstnr.OnItemAdded(this, item));
        }

        /// <summary>
        /// Raises the <see cref="ICollectionListener{TItem}.OnItemRemoved"/> event for the given <paramref name="item"/>.
        /// </summary>
        /// <param name="item">Affected item</param>
        protected virtual void RaiseRemovedEvent(TItem item) {
            DispatchEvent(lstnr => lstnr.OnItemRemoved(this, item));
        }

        /// <summary>
        /// Raises the <see cref="ICollectionListener{TItem}.OnCleared"/> event.
        /// </summary>
        protected virtual void RaiseClearedEvent() {
            DispatchEvent(lstnr => lstnr.OnCleared(this));
        }

        /// <summary>
        /// Invokes the given <paramref name="dispatchEvent"/> delegate for each registered listener.
        /// </summary>
        /// <param name="dispatchEvent">Action to invoke</param>
        private void DispatchEvent(Action<ICollectionListener<TItem>> dispatchEvent) {
            ICollectionListener<TItem>[] itemSet;
            lock (_registeredListener) {
                itemSet = _registeredListener.ToArray();
            }

            for (int i = -1; ++i != itemSet.Length;) {
                ICollectionListener<TItem> listenerInfo = itemSet[i];
                dispatchEvent(listenerInfo);
            }
        }
    }
}