using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace xcite.logging;

/// <summary> Provides methods to modify the log context. </summary>
public static class LogContext {

    private static readonly AsyncLocal<IDictionary<string, string>> _contextStorage = new();

    /// <summary>
    /// Adds that <paramref name="value"/> for that <paramref name="key"/> to the context.
    /// Note, any existing value is overridden.
    /// When you dispose the returned <see cref="ILogContextProperty"/>, the related value is removed from the context.
    /// </summary>
    public static ILogContextProperty Add(string key, string value) {
        IDictionary<string, string> storage = _contextStorage.Value;
        if (storage is null) {
            _contextStorage.Value = storage = new Dictionary<string, string>(1);
        }

        storage[key] = value;
        return new LogContextProperty(key, value);
    }

    /// <summary> Removes the property for that <paramref name="key"/> from the context. </summary>
    public static void Remove(string key)
        => _contextStorage.Value?.Remove(key);

    /// <summary> Returns <c>TRUE</c> when the context currently contains a value for that <paramref name="key"/>. </summary>
    internal static bool TryGetValue(string key, out string value) {
        value = null;
        
        IDictionary<string, string> storage = _contextStorage.Value;
        return storage is not null 
               && storage.TryGetValue(key, out value);
    }

    /// <summary> Implementation of <see cref="ILogContextProperty"/>. </summary>
    sealed class LogContextProperty(string key, string value) : ILogContextProperty {
        private bool _disposed;

        /// <inheritdoc />
        public void Dispose() {
            if (_disposed) return;
            _disposed = true;
            
            Remove(key);
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync() {
            Dispose();
            return ValueTask.CompletedTask;
        }

        /// <inheritdoc />
        public override string ToString()
            => $"{{Key='{key}' Value='{value}'}}";

    }
    
}