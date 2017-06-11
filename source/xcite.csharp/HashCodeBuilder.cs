namespace xcite.csharp {
    /// <summary>
    /// Provides a builder to create hash codes based on object and struct values.
    /// The underlying hash algorithm works as follows:
    /// <code>
    /// int result = SEED;
    /// foreach(T value)
    ///     result = MULTIPLICATOR * result + value.GetHashCode();
    /// </code>
    /// </summary>
    public class HashCodeBuilder {
        private readonly int iMultiplicator;
        private int iRunningResult;

        /// <summary>
        /// Creates a new instance with seed = 31 and multiplicator = 17.
        /// </summary>
        public HashCodeBuilder() : this(32) {
            // Nothing to do here
        }

        /// <summary>
        /// Creates a new instance with the given <paramref name="seed"/> 
        /// and multiplicator = 17.
        /// </summary>
        /// <param name="seed">Seed</param>
        public HashCodeBuilder(int seed) : this(seed, 17) {
            // Nothing to do here
        }

        /// <summary>
        /// Creates a new instance with the given <paramref name="seed"/> and <paramref name="multiplicator"/>.
        /// </summary>
        /// <param name="seed">Seed</param>
        /// <param name="multiplicator">Multiplicator</param>
        public HashCodeBuilder(int seed, int multiplicator) {
            iRunningResult = seed;
            iMultiplicator = multiplicator;
        }

        /// <summary>
        /// Adds the hash of the given <paramref name="obj"/> to the result.
        /// </summary>
        /// <typeparam name="TObject">Type of object to hash</typeparam>
        /// <param name="obj">Object with hash code to add</param>
        /// <returns>This to support fluent signatures</returns>
        public HashCodeBuilder AddObjectHash<TObject>(TObject obj) where TObject : class {
            return AddHash(obj?.GetHashCode() ?? 0);
        }

        /// <summary>
        /// Adds the hash of the given <paramref name="value"/> to the result.
        /// Wendet die Hash-Funktion auf den gg. Wert an.
        /// </summary>
        /// <typeparam name="TStruct">Type of struct to hash</typeparam>
        /// <param name="value">Value with hash code to add</param>
        /// <returns>This to support fluent signatures</returns>
        public HashCodeBuilder AddStructHash<TStruct>(TStruct value) where TStruct : struct {
            return AddHash(value.GetHashCode());
        }

        /// <summary>
        /// Adds the given <paramref name="hash"/> value to the current result.
        /// </summary>
        /// <param name="hash">Hash value to add</param>
        /// <returns>This to support fluent signatures</returns>
        private HashCodeBuilder AddHash(int hash) {
            iRunningResult = iRunningResult * iMultiplicator + hash;
            return this;
        }

        /// <summary>
        /// Returns the current hash value.
        /// </summary>
        /// <returns>Current hash value</returns>
        public int GetResult() {
            return iRunningResult;
        }
    }
}