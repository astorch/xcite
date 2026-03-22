using System;
using System.Collections.Generic;
using System.Linq;

namespace xcite.logging.streams;

/// <summary> The <see cref="AbstractStream"/> implements functionality used by all other streams. </summary>
public abstract class AbstractStream : ILogStream {
    private readonly HashSet<string> _servedTypes = [];
    private bool _disposed;
    private bool _serveTypes;
        
    /// <inheritdoc/>
    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
        
    /// <summary>
    /// The types this streams accepts as source of a logging call.
    /// Calls from all other types will be ignored.
    /// </summary>
    public string[] Types {
        get => _servedTypes.ToArray();

        set {
            _serveTypes = false;
            _servedTypes.Clear();
            if (value is null || value.Length == 0) return;

            for (int i = -1, ilen = value.Length; ++i != ilen;) {
                _servedTypes.Add(value[i]);
            }

            _serveTypes = _servedTypes.Count != 0;
        }
    }

    /// <inheritdoc />
    public virtual void Write(in ReadOnlySpan<byte> value, in LogData logData) {
        if (!_serveTypes || _servedTypes.Contains(logData.name)) {
            Write(in value);
        }
    }

    /// <summary>
    /// Notifies the instance that is being disposed.
    /// When <paramref name="disposing"/> is FALSE,
    /// the call is invoked via GC.
    /// </summary>
    /// <param name="disposing">FALSE when called by GC</param>
    protected void Dispose(bool disposing) {
        if (_disposed) return;
        _disposed = true;
        
        OnDispose(disposing);
    }

    /// <summary> Notifies the instance that is being disposed. </summary>
    /// <param name="disposing">FALSE when called by GC</param>
    protected abstract void OnDispose(bool disposing);

    /// <summary> Writes the specified <paramref name="value"/> into the stream. </summary>
    protected abstract void Write(in ReadOnlySpan<byte> value);
        
}