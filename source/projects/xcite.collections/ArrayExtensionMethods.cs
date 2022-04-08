using System;
using System.Linq;

namespace xcite.collections {
    /// <summary> Provides method extensions for arrays. </summary>
    public static class ArrayExtensionMethods {
        /// <summary>
        /// Copies all elements of the given array collection into a new array which contains all elements sequentially.
        /// </summary>
        /// <typeparam name="TArray">Type of array elements</typeparam>
        /// <param name="valueArray">Collection of arrays</param>
        /// <returns>Array with elements of the array</returns>
        public static TArray[] Join<TArray>(this TArray[][] valueArray) {
            int maxSize = valueArray.Sum(data => data.Length);
            TArray[] resultSet = new TArray[maxSize];
            int p = 0;
            for (int i = -1; ++i != valueArray.Length;) {
                TArray[] srcArray = valueArray[i];
                Array.Copy(srcArray, 0, resultSet, p, srcArray.Length);
                p += srcArray.Length;
            }

            return resultSet;
        }

        /// <summary>
        /// Searches for the specified <paramref name="item"/> and returns the index of the first occurrence 
        /// within the entire <paramref name="array"/>.
        /// </summary>
        /// <typeparam name="TArray">Type of array elements</typeparam>
        /// <param name="array">Array to search</param>
        /// <param name="item">Item to look up</param>
        /// <returns>Index of item or -1</returns>
        public static int IndexOf<TArray>(this TArray[] array, TArray item) {
            return Array.IndexOf(array, item);
        }

        /// <summary>
        /// Extends the array by inserting the given <paramref name="item"/> at the end. 
        /// The resulting array is a new one. There is no resize of the current array.
        /// </summary>
        /// <typeparam name="TArray">Type of array objects</typeparam>
        /// <param name="array">Array the item is appended</param>
        /// <param name="item">Item to be appended</param>
        /// <returns>A new array with the appended item</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="array"/> is NULL</exception>
        public static TArray[] Append<TArray>(this TArray[] array, TArray item) {
            return Append(array, new[] {item});
        }

        /// <summary>
        /// Extends the array by inserting the given <paramref name="items"/> at the end. 
        /// The resulting array is a new one. There is no resize of the current array.
        /// </summary>
        /// <typeparam name="TArray">Type of array objects</typeparam>
        /// <param name="array">Array the items are appended</param>
        /// <param name="items">Items to be appended</param>
        /// <returns>A new array with the appended items</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="array"/> is NULL</exception>
        public static TArray[] Append<TArray>(this TArray[] array, TArray[] items) {
            if (array == null) throw new ArgumentNullException(nameof(array));
            int index = array.Length;
            return InsertAt(array, index, items);
        }

        /// <summary>
        /// Inserts the given <paramref name="item"/> at the first index (zero) into the current array. 
        /// All items will be shifted. The resulting array is a new one. There is no resize of the current array.
        /// </summary>
        /// <typeparam name="TArray">Type of array objects</typeparam>
        /// <param name="array">Array the item is inserted</param>
        /// <param name="item">Item to be inserted</param>
        /// <returns>A new array with the inserted item</returns>
        public static TArray[] InsertFirst<TArray>(this TArray[] array, TArray item) {
            return InsertAt(array, 0, item);
        }

        /// <summary>
        /// Inserts the given <paramref name="item"/> at the last index into the current array. 
        /// The resulting array is a new one. There is no resize of the current array.
        /// </summary>
        /// <typeparam name="TArray">Type of array objects</typeparam>
        /// <param name="array">Array the item is inserted</param>
        /// <param name="item">Item to be inserted</param>
        /// <returns>A new array with the inserted item</returns>
        public static TArray[] InsertLast<TArray>(this TArray[] array, TArray item) {
            return Append(array, new[] {item});
        }

        /// <summary>
        /// Inserts the given <paramref name="item"/> at the given <paramref name="index"/> into 
        /// the current array. All items after the given index will be moved right. The resulting array is a 
        /// new one. There is no resize of the current array!
        /// </summary>
        /// <typeparam name="TArray">Type of array objects</typeparam>
        /// <param name="array">Array the item is inserted</param>
        /// <param name="index">Index where the item is inserted</param>
        /// <param name="item">Item to be inserted</param>
        /// <exception cref="ArgumentNullException">If <paramref name="array"/> is NULL</exception>
        /// <exception cref="IndexOutOfRangeException">If <paramref name="index"/> is greater than the array size</exception>
        /// <returns>A new array with the inserted item</returns>
        public static TArray[] InsertAt<TArray>(this TArray[] array, int index, TArray item) {
            return InsertAt(array, index, new[] {item});
        }

        /// <summary>
        /// Inserts the given item collection <paramref name="items"/> at the given <paramref name="index"/> into 
        /// the current array. All items after the given index will be moved right. The resulting array is a 
        /// new one. There is no resize of the current array!
        /// </summary>
        /// <typeparam name="TArray">Type of array objects</typeparam>
        /// <param name="array">Array the items are inserted</param>
        /// <param name="index">Index where the items are inserted</param>
        /// <param name="items">Items to be inserted</param>
        /// <exception cref="ArgumentNullException">If <paramref name="array"/> is NULL</exception>
        /// <exception cref="IndexOutOfRangeException">If <paramref name="index"/> is greater than the array size</exception>
        /// <returns>A new array with the inserted items</returns>
        public static TArray[] InsertAt<TArray>(this TArray[] array, int index, TArray[] items) {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (items == null || items.Length == 0) return array;
            if (index > array.Length) throw new IndexOutOfRangeException();

            int newSize = array.Length + items.Length;
            TArray[] resultSet = new TArray[newSize];

            Array.Copy(array, 0, resultSet, 0, index);
            Array.Copy(items, 0, resultSet, index, items.Length);
            Array.Copy(array, index, resultSet, index + items.Length, array.Length - index);

            return resultSet;
        }
    }
}
