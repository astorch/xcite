using System;
using System.Collections.Generic;

namespace xcite.collections {
    /// <summary> Announces that this generic type supports sorting of its elements. </summary>
    /// <typeparam name="TElement">Type of managed elements</typeparam>
    public interface ISortable<TElement> : ISortable {

        /// <summary>
        /// Sorts using the given <paramref name="comparison"/>.
        /// </summary>
        /// <param name="comparison">Comparison</param>
        void Sort(Comparison<TElement> comparison);

        /// <summary>
        /// Sorts using the given <paramref name="comparer"/>.
        /// </summary>
        /// <param name="comparer">Comparer</param>
        void Sort(Comparer<TElement> comparer);

        /// <summary>
        /// Sorts using the property referenced by the given <paramref name="propertyReference"/>.
        /// </summary>
        /// <typeparam name="TProperty">Type of property</typeparam>
        /// <param name="propertyReference">Property reference</param>
        void Sort<TProperty>(Func<TElement, TProperty> propertyReference);
    }
}