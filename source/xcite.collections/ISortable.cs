using System;
using System.Collections;

namespace xcite.collections {
    /// <summary> Announces that this type supports sorting of its elements. </summary>
    public interface ISortable {
        /// <summary>
        /// Sorts using the given <paramref name="comparison"/> in ascending order.
        /// </summary>
        /// <param name="comparison">Comparison</param>
        void Sort(Comparison<object> comparison);

        /// <summary>
        /// Sorts using the given <paramref name="comparer"/> in ascending order.
        /// </summary>
        /// <param name="comparer">Comparer</param>
        void Sort(IComparer comparer);

        /// <summary>
        /// Sorts using the property referenced by the given <paramref name="propertyReference"/> 
        /// in ascending order.
        /// </summary>
        /// <typeparam name="TProperty">Type of property</typeparam>
        /// <param name="propertyReference">Property reference</param>
        void Sort<TProperty>(Func<object, TProperty> propertyReference);
    }
}