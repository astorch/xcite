﻿using System.Collections.Generic;

namespace xcite.collections {
    /// <summary>
    /// Defines an enumerable that publishes events when it has been modified.
    /// </summary>
    /// <typeparam name="TItem">Type of elements managed by this collection</typeparam>
    public interface IObservableEnumerable<TItem> : IEnumerable<TItem>, IObservableEnumerable {
        /// <summary>
        /// Subscribes the given <paramref name="listener"/> to this instance.
        /// </summary>
        /// <param name="listener">Listener</param>
        void AddListener(IEnumerableListener<TItem> listener);

        /// <summary>
        /// Unsubscribes the given <paramref name="listener"/> from this instance.
        /// </summary>
        /// <param name="listener">Listener</param>
        void RemoveListener(IEnumerableListener<TItem> listener);
    }
}