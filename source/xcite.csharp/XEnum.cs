using System;
using System.Linq;
using System.Reflection;

namespace xcite.csharp {
    /// <summary> Describes an abstract enum object type. </summary>
    /// <typeparam name="TEnum">Type of enum</typeparam>
    public abstract class XEnum<TEnum> where TEnum : XEnum<TEnum> //: IComparable, IFormattable, IConvertible 
    {
        private static TEnum[] _values;

        /// <summary> Returns all values of this enumeration. </summary>
        public static TEnum[] Values => _values ?? (_values = ReadEnumFields());

        /// <summary>
        /// Resolves all values of this enumeration type. 
        /// </summary>
        /// <returns>Enumeration values</returns>
        private static TEnum[] ReadEnumFields() {
            Type type = typeof(TEnum);
//            FieldInfo[] fields = type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Static);
            FieldInfo[] fields = type.GetRuntimeFields().ToArray();
            return fields.Select(field => field.GetValue(null)).Cast<TEnum>().ToArray();
        }

        /// <summary>
        /// Protected constructor.
        /// </summary>
        /// <param name="uniqueReference">Unique reference to identify this instance</param>
        protected XEnum(object uniqueReference) {
            UniqueReference = uniqueReference ?? throw new ArgumentNullException(nameof(uniqueReference));
        }

        /// <summary>
        /// Returns a unique id to identify the enum value.
        /// </summary>
        protected object UniqueReference { get; }

        /// <summary>
        /// Returns TRUE if the given object is equal to the current.
        /// </summary>
        /// <param name="obj">Object to proof</param>
        /// <returns>TRUE or FALSE</returns>
        public override bool Equals(object obj) {
            if (ReferenceEquals(this, obj)) return true;
            XEnum<TEnum> enumInstance = obj as XEnum<TEnum>;
            if (enumInstance == null) return false;
            bool equals = Equals(UniqueReference, enumInstance.UniqueReference);
            return equals;
        }

        /// <summary>
        /// Returns the hash code of this instance.
        /// </summary>
        /// <returns>Hash code of this instance</returns>
        public override int GetHashCode() {
            int result = 31;
            result = 17 * result + UniqueReference.GetHashCode();
            return result;
        }

        /// <summary>
        /// Returns a string representation of this object.
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString() {
            string result = string.Format("{0} '{1}'", GetType().Name, UniqueReference);
            return result;
        }

        /// <summary>
        /// Provides an reference equal overload implementation for <see cref="XEnum{TEnum}"/>.
        /// </summary>
        /// <param name="e1">First value to compare</param>
        /// <param name="e2">Second value to compare</param>
        /// <returns>TRUE or FALSE</returns>
        public static bool operator ==(XEnum<TEnum> e1, XEnum<TEnum> e2) {
            return Equals(e1, e2);
        }

        /// <summary>
        /// Provides an reference un-equal overload implemenation for <see cref="XEnum{TEnum}"/>.
        /// </summary>
        /// <param name="e1">First value to compare</param>
        /// <param name="e2">Second value to compare</param>
        /// <returns>TRUE or FALSE</returns>
        public static bool operator !=(XEnum<TEnum> e1, XEnum<TEnum> e2) {
            return !(e1 == e2);
        }

        /// <summary>
        /// Returns an <see cref="int"/> value which represents the given enum <paramref name="value"/>.
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns><see cref="int"/> value</returns>
        public static explicit operator int(XEnum<TEnum> value) {
            return value.GetHashCode();
        }

        /// <summary>
        /// Returns an <see cref="XEnum{TEnum}"/> value which represents the given integer <paramref name="value"/>.
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns><see cref="XEnum{TEnum}"/> value</returns>
        public static explicit operator XEnum<TEnum>(int value) {
            return Values.First(enumValue => ((int) ((XEnum<TEnum>) enumValue)) == value);
        }
    }
}