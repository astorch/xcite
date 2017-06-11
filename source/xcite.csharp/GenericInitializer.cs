using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace xcite.csharp {
    /// <summary>
    /// Defines an expression to create a new object of the given type with the 
    /// given constructor parameters.
    /// </summary>
    /// <typeparam name="T">Type of the instance that is created</typeparam>
    /// <param name="args">Constructor arguments</param>
    /// <returns>Instance of the new type</returns>
    public delegate T ObjectInitializer<out T>(params object[] args);

    /// <summary>
    /// Provides method extensions to create new instances of a given type without using 
    /// <see cref="Activator.CreateInstance(System.Type)"/>.
    /// The main advantages are a better performance and a simplified API. For futher informations visit 
    /// http://rogeralsing.com/2008/02/28/linq-expressions-creating-objects/
    /// </summary>
    public static class GenericInitializer {
        /// <summary>
        /// Creates a new object initializer based of the given constructor meta data object.
        /// </summary>
        /// <typeparam name="T">Type of the object initializer</typeparam>
        /// <param name="ctor">Constructor meta data object</param>
        /// <returns>Object initializer</returns>
        public static ObjectInitializer<T> GetInitializer<T>(ConstructorInfo ctor) {
            // Type type = ctor.DeclaringType;
            ParameterInfo[] paramsInfo = ctor.GetParameters();

            //create a single param of type object[]
            ParameterExpression param = Expression.Parameter(typeof(object[]), "args");
            Expression[] argsExp = new Expression[paramsInfo.Length];

            //pick each arg from the params array 
            //and create a typed expression of them
            for (int i = -1; ++i < paramsInfo.Length;) {
                Expression index = Expression.Constant(i);
                Type paramType = paramsInfo[i].ParameterType;
                Expression paramAccessorExp = Expression.ArrayIndex(param, index);
                Expression paramCastExp = Expression.Convert(paramAccessorExp, paramType);
                argsExp[i] = paramCastExp;
            }

            //make a NewExpression that calls the
            //ctor with the args we just created
            NewExpression newExp = Expression.New(ctor, argsExp);

            //create a lambda with the New
            //Expression as body and our param object[] as arg
            LambdaExpression lambda = Expression.Lambda(typeof(ObjectInitializer<T>), newExp, param);

            //compile it
            ObjectInitializer<T> compiled = (ObjectInitializer<T>) lambda.Compile();
            return compiled;
        }

        /// <summary>
        /// Creates a new instance of the given type with the given constructor parameters.
        /// </summary>
        /// <typeparam name="T">Type of the new instance</typeparam>
        /// <param name="ctorValues">Constructor parameters</param>
        /// <returns>New instance of the type</returns>
        public static T NewInstance<T>(params object[] ctorValues) {
            return NewInstance<T>(false, ctorValues);
        }
        
        /// <summary>
        /// Creates a new instance of the given type with the given constructor parameters.
        /// </summary>
        /// <typeparam name="T">Type of the new instance</typeparam>
        /// <param name="type">Type of the object that shall be constructed</param>
        /// <param name="ctorValues">Constructor parameters</param>
        /// <returns>New instance of the type</returns>
        public static T NewInstance<T>(this Type type, params object[] ctorValues) {
            return type.NewInstance<T>(false, ctorValues);
        }

        /// <summary>
        /// Creates a new instance of the given type with the given constructor parameters.
        /// </summary>
        /// <typeparam name="T">Type of the new instance</typeparam>
        /// <param name="nonPublic">if TRUE a non public constructor can be used if needed</param>
        /// <param name="ctorValues">Constructor parameters</param>
        /// <returns>New instance of the type</returns>
        public static T NewInstance<T>(bool nonPublic, params object[] ctorValues) {
            Type type = typeof(T);
            return type.NewInstance<T>(nonPublic, ctorValues);
        }

        /// <summary>
        /// Creates a new instance of the given type with the given constructor parameters.
        /// </summary>
        /// <typeparam name="T">Type of the new instance</typeparam>
        /// <param name="type">Type of the object that shall be constructed</param>
        /// <param name="nonPublic">if TRUE a non public constructor can be used if needed</param>
        /// <param name="ctorValues">Constructor parameters</param>
        /// <returns>New instance of the type</returns>
        public static T NewInstance<T>(this Type type, bool nonPublic, params object[] ctorValues) {
            if (type == null) throw new ArgumentNullException("type");
            if (!typeof(T).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
                throw new InvalidCastException($"Current type '{type}' is not assignable to desired type '{typeof(T)}'");
            ConstructorInfo ctor = type.GetTypeInfo().DeclaredConstructors.FirstOrDefault(_ => _.GetParameters().Length == ctorValues.Length);

//            if (ctor == null && nonPublic)
//                ctor = type.GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
//                    .FirstOrDefault(_ => _.GetParameters().Length == ctorValues.Length);

            if (ctor == null) throw new InvalidOperationException($"Could not resolve an constructor of type '{type}'");
            ObjectInitializer<T> initializer = GetInitializer<T>(ctor);
            return initializer(ctorValues);
        }
    }
}