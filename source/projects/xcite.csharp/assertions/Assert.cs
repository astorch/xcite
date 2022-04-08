using System;
using System.Linq.Expressions;
using System.Reflection;

namespace xcite.csharp.assertions {
    /// <summary>
    /// Provides various methods to support type- and member-safe assertions at runtime.
    /// </summary>
    public static class Assert {
        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if the given object reference points to NULL. 
        /// Otherwise the value of the object is returned.
        /// </summary>
        /// <typeparam name="TValue">Type of the object value</typeparam>
        /// <param name="objRef">Expression that points to an object</param>
        /// <returns>Value of the object if it's not NULL</returns>
        public static TValue NotNull<TValue>(Expression<Func<TValue>> objRef) where TValue : class {
            object result;
            string memberName;
            if (TryGetValueReference(objRef, out result, out memberName))
                return result as TValue;

            throw new ArgumentNullException(memberName);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if the given value pointer points to NULL. 
        /// Otherweise the value of the value pointer is returned.
        /// </summary>
        /// <typeparam name="TValue">Type of the value</typeparam>
        /// <param name="valueRef">Expression that points to a value</param>
        /// <returns>Value if the pointer points not to NULL</returns>
        public static TValue NotNull<TValue>(Expression<Func<TValue?>> valueRef) where TValue : struct {
            TValue? objValue = valueRef.Compile()();
            if (objValue != null) return objValue.Value;

            return ThrowArgumentNullException<TValue>(valueRef);
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if the given string object reference points to NULL or is empty.
        /// Otherwise the value of the string is returned.
        /// </summary>
        /// <param name="strRef">Expression that points to the string</param>
        /// <returns>String value if it's not NULL or empty</returns>
        public static string NotNullOrEmpty(Expression<Func<string>> strRef) {
            object objValue;
            string memberName;
            if (TryGetValueReference(strRef, out objValue, out memberName)) {
                string strValue = (string) objValue;
                if (strValue.Length != 0) return strValue;
            }

            throw new ArgumentNullException(memberName, "String is NULL or empty");
        }

        /// <summary>
        /// Throws an <see cref="ArgumentNullException"/> if the given object array reference points to NULL or 
        /// to an empty array. Otherwise the array itself is returned.
        /// </summary>
        /// <typeparam name="TObject">Type of object array</typeparam>
        /// <param name="arrayRef">Expression that points to an object array</param>
        /// <returns>Object array it it's not NULL or empty</returns>
        public static TObject[] NotNullOrEmpty<TObject>(Expression<Func<TObject[]>> arrayRef) {
            object objValue;
            string memberName;
            if (TryGetValueReference(arrayRef, out objValue, out memberName)) {
                TObject[] objectArray = (TObject[]) objValue;
                if (objectArray.Length != 0) return objectArray;
            }

            throw new ArgumentNullException(memberName, "Array is NULL or empty");
        }

        /// <summary>
        /// Throws an <see cref="ArgumentException"/> if the given reference points to a value that is not greater 
        /// than the given <paramref name="compareValue"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the value</typeparam>
        /// <param name="valueRef">Value pointer</param>
        /// <param name="compareValue">Value to compare</param>
        /// <returns>Value if it's greater than given value to compare</returns>
        public static TValue GreaterThan<TValue>(Expression<Func<TValue>> valueRef, TValue compareValue)
            where TValue : IComparable {
            object value;
            string memberName;
            if (TryGetValueReference(valueRef, out value, out memberName)) {
                TValue unboxedValue = (TValue) value;
                int compareResult = unboxedValue.CompareTo(compareValue);
                if (compareResult >= 0) return unboxedValue;
            }

            throw new ArgumentException("Must be greater than", memberName);
        }

        /// <summary>
        /// Tries to resolve the value reference targeted by the given <paramref name="lambdaExpression"/>. If reference targets NULL FALSE is 
        /// returned. The resolved value is returned by <paramref name="value"/>. Additionally, the name of the targeted property or field 
        /// is returned by <paramref name="memberName"/>.
        /// </summary>
        /// <param name="lambdaExpression">Lambda expression to process</param>
        /// <param name="value">Resolved value reference</param>
        /// <param name="memberName">Name of the referenced parameter</param>
        /// <returns>FALSE if the value points to NULL</returns>
        private static bool TryGetValueReference(LambdaExpression lambdaExpression, out object value,
            out string memberName) {
            MemberExpression memberExpression = (MemberExpression) lambdaExpression.Body;
            MemberInfo memberInfo = memberExpression.Member;
            ConstantExpression expression = (ConstantExpression) memberExpression.Expression;

            memberName = memberInfo.Name;
            object runtimeObject = expression.Value;
            Type runtimeObjectType = runtimeObject.GetType();
            FieldInfo field = runtimeObjectType.GetRuntimeField(memberName);
            value = field.GetValue(runtimeObject);
            return value != null;
        }

        /// <summary>
        /// Throws the <see cref="ArgumentNullException"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the return value</typeparam>
        /// <param name="expression">Expression that points to the property</param>
        /// <returns>Never</returns>
        private static TValue ThrowArgumentNullException<TValue>(LambdaExpression expression) {
            string paramName = expression.GetPropertyName();
            throw new ArgumentNullException(paramName);
        }

        /// <summary>
        /// Throws the <see cref="ArgumentException"/>.
        /// </summary>
        /// <typeparam name="TValue">Type of the return value</typeparam>
        /// <param name="expression">Expression that points to the property</param>
        /// <param name="message">Detailed exception message</param>
        /// <returns>Never</returns>
        private static TValue ThrowArgumentException<TValue>(LambdaExpression expression, string message) {
            string paramName = expression.GetPropertyName();
            throw new ArgumentException(message, paramName);
        }
    }
}