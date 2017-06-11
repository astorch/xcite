using System;
using System.Linq;
using System.Reflection;

namespace xcite.csharp {
    /// <summary> Provides various extension methods for <see cref="Type"/>. </summary>
    public static class TypeMethodExtensions {
        /// <summary>
        /// Returns all public properties of this <paramref name="type"/>. If the given type is 
        /// an interface, the properties of the parent interfaces will be selected, too.
        /// </summary>
        /// <param name="type">Type which public properties to look up</param>
        /// <returns>All public properties of the given type</returns>
        public static PropertyInfo[] GetPublicProperties(this Type type) {
            if (!type.GetTypeInfo().IsInterface) return type.GetTypeInfo().DeclaredProperties.ToArray();

            return new[] {type}
                .Concat(type.GetTypeInfo().ImplementedInterfaces)
                .SelectMany(i => i.GetTypeInfo().DeclaredProperties).ToArray();
        }
    }
}