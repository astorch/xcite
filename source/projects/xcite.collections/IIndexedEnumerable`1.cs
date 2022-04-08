using System.Collections.Generic;

namespace xcite.collections {
    /// <summary>
    /// Exposes the enumerator, which supports a simple iteration over a collection of a specified type. In addition 
    /// to <see cref="IEnumerable{T}"/> this collections assigns an index to each contained element. The index is 
    /// based on the order of appearance.
    /// </summary>
    /// <typeparam name="TItem">
    /// The type of objects to enumerate. This type parameter is covariant. That is, you can use either the type you 
    /// specified or any type that is more derived. For more information about covariance and contravariance, 
    /// see Covariance and Contravariance in Generics.
    /// </typeparam>
    public interface IIndexedEnumerable<out TItem> : IIndexedEnumerable, IEnumerable<TItem> {

    }
}