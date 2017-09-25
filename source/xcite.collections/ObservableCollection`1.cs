using System;
using System.Collections;
using System.Collections.Generic;

namespace xcite.collections {
    /// <summary>
    /// Provides an implementation of <see cref="T:xcite.collections.IObservableCollection`1" /> that is based on a <see cref="T:xcite.collections.LinearList`1" />.
    /// </summary>
    /// <typeparam name="TItem">Data type of managed items</typeparam>
    public class ObservableCollection<TItem> : LinearList<TItem>, IObservableCollection<TItem>, IObservableCollection {
        private readonly LinearList<object> _registeredListener = new LinearList<object>();

        /// <summary> Initializes an empty instance. </summary>
        public ObservableCollection() {
            // Currently nothing to do here
        }

        /// <summary>
        /// Initializes the new instance with all items of the given <paramref name="enumerable"/>.
        /// </summary>
        /// <param name="enumerable">Enumerable with items to add</param>
        public ObservableCollection(IEnumerable<TItem> enumerable) : base(enumerable) {
            // Currently nothing to do here
        }

        /// <inheritdoc />
        public void AddListener(IEnumerableListener<TItem> listener) {
            AddListenerInstance(listener);
        }

        /// <inheritdoc />
        void IObservableEnumerable.AddListener(IEnumerableListener listener) {
            AddListenerInstance(listener);
        }
        
        /// <inheritdoc />
        public void RemoveListener(IEnumerableListener<TItem> listener) {
            RemoveListenerInstance(listener);
        }

        /// <inheritdoc />
        void IObservableEnumerable.RemoveListener(IEnumerableListener listener) {
            RemoveListenerInstance(listener);
        }

        /// <inheritdoc cref="ICollection{T}.Add" />
        public override void Add(TItem item) {
            base.Add(item);
            OnItemAdded(item);
        }
        
        /// <inheritdoc cref="ICollection{T}.Remove" />
        public override bool Remove(TItem item) {
            if (!base.Remove(item)) return false;
            OnItemRemoved(item);
            return true;
        }

        /// <inheritdoc cref="ICollection{T}.Clear" />
        public override void Clear() {
            base.Clear();
            OnCollectionCleared();
        }

        /// <inheritdoc />
        void ICollection.CopyTo(Array array, int index) {
            this.ForEach(item => array.SetValue(item, index++));
        }

        /// <inheritdoc />
        bool ICollection.IsSynchronized { get; } = false;

        /// <inheritdoc />
        object ICollection.SyncRoot { get; } = null;

        /// <summary>
        /// Is invoked when an item has been added. Clients may override this method. But it's recommended 
        /// to always call this method, because it raises the 
        /// <see cref="IEnumerableListener{TItem}.OnItemAdded"/> 
        /// event for the given <paramref name="item"/>.
        /// </summary>
        /// <param name="item">Affected item</param>
        protected virtual void OnItemAdded(TItem item) {
            DispatchEvent(AddEvent, item);
        }

        /// <summary>
        /// Is invoked when an item has been removed. Clients may override this method. But it's recommended 
        /// to always call this method, because it raises the 
        /// <see cref="IEnumerableListener{TItem}.OnItemRemoved"/> 
        /// event for the given <paramref name="item"/>.
        /// </summary>
        /// <param name="item">Affected item</param>
        protected virtual void OnItemRemoved(TItem item) {
            DispatchEvent(RemoveEvent, item);
        }

        /// <summary>
        /// Is invoked when the collection has been cleard. Clients may override this method. But it's recommended 
        /// to always call this method, because it raises the <see cref="IEnumerableListener{TItem}.OnCleared"/> event.
        /// </summary>
        protected virtual void OnCollectionCleared() {
            DispatchEvent(ClearEvent, default(TItem));
        }

        ///  <summary>
        ///  Dispatches the specified event to each listener.
        ///  </summary>
        /// <param name="eventType">Event type</param>
        /// <param name="item">Item</param>
        private void DispatchEvent(int eventType, TItem item) {
            object[] listenerSet;
            lock (_registeredListener) {
                listenerSet = _registeredListener.ToArray();
            }

            for (int i = -1; ++i != listenerSet.Length;) {
                object rawObj = listenerSet[i];
                IEnumerableListener rawListener = rawObj as IEnumerableListener;
                IEnumerableListener<TItem> genListener = rawObj as IEnumerableListener<TItem>;

                if (eventType == AddEvent) {
                    rawListener?.OnItemAdded(this, item);
                    genListener?.OnItemAdded(this, item);
                    continue;
                }

                if (eventType == RemoveEvent) {
                    rawListener?.OnItemRemoved(this, item);
                    genListener?.OnItemRemoved(this, item);
                    continue;
                }

                if (eventType == ClearEvent) {
                    rawListener?.OnCleared(this);
                    genListener?.OnCleared(this);
                }
            }
        }

        private const int ClearEvent = 0;
        private const int AddEvent = 1;
        private const int RemoveEvent = 2;

        /// <summary>
        /// Adds the given <paramref name="listener"/> to the internal collection.
        /// </summary>
        /// <param name="listener">Listener to add</param>
        private void AddListenerInstance(object listener) {
            if (listener == null) return;
            lock (_registeredListener) {
                _registeredListener.Add(listener);
            }
        }

        /// <summary>
        /// Removes the given <paramref name="listener"/> from the internal collection.
        /// </summary>
        /// <param name="listener">Listener to remove</param>
        private void RemoveListenerInstance(object listener) {
            if (listener == null) return;
            lock (_registeredListener) {
                _registeredListener.Remove(listener);
            }
        }
    }
}