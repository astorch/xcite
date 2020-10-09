using System;
using xcite.logging.streams;

namespace xcite.logging {
    /// <summary> Provides access to all available instance of <see cref="ILogStream"/>. </summary>
    public static class StreamStore {
        private static readonly Type[] StreamTypes = {
            typeof(ConsoleStream), typeof(DebugStream), typeof(FileStream),
            typeof(TraceStream)
        };
        
        /// <summary>
        /// Loads the <see cref="ILogStream"/> instance specified by <paramref name="name"/>.
        /// Returns <c>null</c> if the name neither matches a predefined nor a custom implementation (external assembly).
        /// </summary>
        public static ILogStream GetStreamInstance(string name) {
            ILogStream stream =  GetWellKnownStreamInstance(name);
            return stream ?? GetStreamInstanceFromAssembly(name);
        }

        /// <summary>
        /// Returns a new instance of <see cref="ILogStream"/> that owns
        /// the specified <paramref name="shortName"/>. 
        /// </summary>
        private static ILogStream GetWellKnownStreamInstance(string shortName) {
            if (string.IsNullOrEmpty(shortName)) return null;
            string nameLowerCase = shortName.ToLower();
            
            for (int i = -1; ++i != StreamTypes.Length;) {
                Type streamType = StreamTypes[i];
                string fullNameLowerCase = streamType.FullName?.ToLower();
                if (fullNameLowerCase == null) continue;

                if (fullNameLowerCase.EndsWith(nameLowerCase)) return (ILogStream) Activator.CreateInstance(streamType);
            }

            return null;
        }

        /// <summary>
        /// Returns a new instance based on the <paramref name="assemblyQualifiedName"/>,
        /// if it is a subclass of <see cref="ILogStream"/>. Returns <c>null</c> otherwise.
        /// </summary>
        private static ILogStream GetStreamInstanceFromAssembly(string assemblyQualifiedName) {
            bool successful = TryGetType(assemblyQualifiedName, out Type type);
            if (!successful) return null;
            
            if (!typeof(ILogStream).IsAssignableFrom(type)) return null;
            return (ILogStream) Activator.CreateInstance(type);
        }

        /// <summary>
        /// Tries to load the type specified by <paramref name="typeName"/>.
        /// Returns <c>true</c> if successful and <c>false</c> otherwise.
        /// </summary>
        private static bool TryGetType(string typeName, out Type type) {
            type = null;
            try {
                type = Type.GetType(typeName);
                return type != null;
            } catch {
                return false;
            }
        }
    }
}