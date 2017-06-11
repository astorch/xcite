using System.Collections;

namespace xcite.collections {
    /// <summary>
    /// Exposes the enumerator, which supports a simple iteration over a non-generic collection. In addition 
    /// to <see cref="IEnumerable"/> this collections assigns an index to each contained element. The index is 
    /// based on the order of appearance.
    /// </summary>
    public interface IIndexedEnumerable : IEnumerable {

    }
}