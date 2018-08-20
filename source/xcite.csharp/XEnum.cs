using System;
using System.Linq;
using System.Reflection;

namespace xcite.csharp {
    /// <summary> Describes an abstract enum object type. </summary>
    /// <typeparam name="TEnum">Type of enum</typeparam>
    public abstract class XEnum<TEnum> where TEnum : XEnum<TEnum> //: IComparable, IFormattable, IConvertible 
    {
        private static FieldInfo[] _fields;
        private static TEnum[] _values;

        /// <summary> Returns all values of this enumeration. </summary>
        public static TEnum[] Values => _values ?? (_values = ReadEnumFields());

        /// <summary> Protected constructor. </summary>
        /// <param name="uniqueReference">Unique reference to identify this instance</param>
        protected XEnum(object uniqueReference) {
            UniqueReference = uniqueReference ?? throw new ArgumentNullException(nameof(uniqueReference));
        }

        /// <summary> Returns a unique id to identify the enum value. </summary>
        protected object UniqueReference { get; }

        /// <inheritdoc />
        public override bool Equals(object obj) {
            if (ReferenceEquals(this, obj)) return true;
            if (!(obj is XEnum<TEnum> enumInstance)) return false;
            return Equals(UniqueReference, enumInstance.UniqueReference);
        }

        /// <inheritdoc />
        public override int GetHashCode() {
            int result = 31;
            result = 17 * result + UniqueReference.GetHashCode();
            return result;
        }

        /// <inheritdoc />
        public override string ToString() 
            => GetEnumFieldName((TEnum) this);

        /// <summary>
        /// Provides an reference equal overload implementation for <see cref="XEnum{TEnum}"/>.
        /// </summary>
        /// <param name="e1">First value to compare</param>
        /// <param name="e2">Second value to compare</param>
        /// <returns>TRUE or FALSE</returns>
        public static bool operator ==(XEnum<TEnum> e1, XEnum<TEnum> e2) 
            => Equals(e1, e2);

        /// <summary>
        /// Provides an reference un-equal overload implemenation for <see cref="XEnum{TEnum}"/>.
        /// </summary>
        /// <param name="e1">First value to compare</param>
        /// <param name="e2">Second value to compare</param>
        /// <returns>TRUE or FALSE</returns>
        public static bool operator !=(XEnum<TEnum> e1, XEnum<TEnum> e2) 
            => !(e1 == e2);

        /// <summary>
        /// Returns an <see cref="int"/> value which represents the given enum <paramref name="value"/>.
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns><see cref="int"/> value</returns>
        public static explicit operator int(XEnum<TEnum> value) 
            => value.GetHashCode();

        /// <summary>
        /// Returns an <see cref="XEnum{TEnum}"/> value which represents the given integer <paramref name="value"/>.
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns><see cref="XEnum{TEnum}"/> value</returns>
        public static explicit operator XEnum<TEnum>(int value)
            => Values.First(enumValue => ((int) ((XEnum<TEnum>) enumValue)) == value);

        /// <summary>
        /// Returns TRUE, if the specified <paramref name="stringValue"/> matches an enum value
        /// of the specified <typeparamref name="TEnum"/> kind. The matched value is
        /// provided via out parameter <paramref name="enumValue"/>.
        /// </summary>
        /// <param name="stringValue">String value to parse</param>
        /// <param name="enumValue">Resolved enum or default value</param>
        /// <returns>TRUE if an enum value matched</returns>
        public static bool TryParse(string stringValue, out TEnum enumValue) {
            enumValue = _values[0];
            for (int i = -1; ++i != _values.Length;) {
                if (_fields[i].Name != stringValue) continue;
                enumValue = _values[i];
                return true;
            }

            return false;
        }

        /// <summary> Returns the name of the enum field. </summary>
        /// <param name="enumValue">Reference of the value to read the field name</param>
        /// <returns>Enum field name</returns>
        private static string GetEnumFieldName(TEnum enumValue) {
            for (int i = -1; ++i != _values.Length;) {
                if (enumValue != _values[i]) continue;
                return _fields[i].Name;
            }

            throw new ArgumentException("enumValue is not known");
        }

        /// <summary> Resolves all values of this enumeration type.  </summary>
        /// <returns>Enumeration values</returns>
        private static TEnum[] ReadEnumFields() {
            FieldInfo[] fields = GetFields();
            TEnum[] enumValues = new TEnum[fields.Length];
            for (int i = -1; ++i != fields.Length;)
                enumValues[i] = (TEnum) fields[i].GetValue(null);

            return enumValues;
        }

        /// <summary> Returns all fields of the specified <typeparamref name="TEnum"/>. </summary>
        /// <returns>Set of all fields</returns>
        private static FieldInfo[] GetFields() {
            return _fields ?? (_fields = typeof(TEnum)
                       .GetFields(BindingFlags.Static | BindingFlags.Public)
//                                            .GetRuntimeFields()
                                            .ToArray());
        }
    }
}