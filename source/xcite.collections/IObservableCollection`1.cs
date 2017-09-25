using System.Collections.Generic;

namespace xcite.collections {
    /// <summary>
    /// Defines a collection that publishes events when it has been modified.
    /// </summary>
    /// <typeparam name="TItem">Type of elements managed by this collection</typeparam>
    public interface IObservableCollection<TItem> : ICollection<TItem>, IObservableEnumerable<TItem> {

    }
}