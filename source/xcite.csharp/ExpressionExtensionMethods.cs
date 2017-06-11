using System;
using System.Linq.Expressions;

namespace xcite.csharp {
    /// <summary> Provides extension methods for expressions. </summary>
    public static class ExpressionMethodExtensions {
        /// <summary>
        /// Returns the name of the property that is targeted by the given expression.
        /// </summary>
        /// <typeparam name="TProperty">Value type of the property</typeparam>
        /// <param name="expression">Expression that points to a property</param>
        /// <returns>Name of the targeted property</returns>
        public static string GetPropertyName<TProperty>(this Expression<Func<TProperty>> expression) {
            if (expression == null) return null;

            // Check the expression
            LambdaExpression lambdaExpression = expression;
            return GetPropertyName(lambdaExpression);
        }

        /// <summary>
        /// Returns the name of the property that is targeted by the given expression.
        /// </summary>
        /// <param name="expression">Expression that points to a property</param>
        /// <returns>Name of the targeted property</returns>
        public static string GetPropertyName(this LambdaExpression expression) {
            if (expression == null) return null;

            // Check the expression
            MemberExpression memberExpression = expression.Body as MemberExpression;
            if (memberExpression != null) return GetPropertyName(memberExpression);

            UnaryExpression unaryExpression = expression.Body as UnaryExpression;
            if (unaryExpression != null) return GetPropertyName(unaryExpression.Operand as MemberExpression);

            // Cannot handle expression
            return null;
        }

        /// <summary>
        /// Returns the name of the property that is targeted by the given expression.
        /// </summary>
        /// <param name="memberExpression">Expression that points to a property</param>
        /// <returns>Name of the targeted property</returns>
        private static string GetPropertyName(MemberExpression memberExpression) {
            string propertyName = memberExpression.Member.Name;
            return propertyName;
        }
    }
}