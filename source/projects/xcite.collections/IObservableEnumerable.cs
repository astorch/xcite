using System.Collections;

namespace xcite.collections {
    /// <summary> Defines an enumerable that publishes events when it has been modified. </summary>
    public interface IObservableEnumerable : IEnumerable {

        /// <summary>
        /// Subscribes the given <paramref name="listener"/> to this instance.
        /// </summary>
        /// <param name="listener">Listener</param>
        void AddListener(IEnumerableListener listener);

        /// <summary>
        /// Unsubscribes the given <paramref name="listener"/> from this instance.
        /// </summary>
        /// <param name="listener">Listener</param>
        void RemoveListener(IEnumerableListener listener);
    }
}