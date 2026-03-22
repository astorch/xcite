using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;

namespace xcite.logging.streams;

/// <summary> The <see cref="AbstractStream"/> implements functionality used by all other streams. </summary>
public abstract class AbstractStream : ILogStream {
    private FrozenSet<string> _servedTypes = [];
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
            _servedTypes = []; 
            if (value is null || value.Length == 0) return;

            _servedTypes = new HashSet<string>(value).ToFrozenSet();
            _serveTypes = _servedTypes.Count != 0;
        }
    }

    /// <inheritdoc />
    public virtual void Write(in ReadOnlySpan<char> value, in LogData logData) {
        if (!_serveTypes || _servedTypes.Contains(logData.name)) {
            OnWrite(in value, in logData);
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
    protected abstract void OnWrite(in ReadOnlySpan<char> value, in LogData logData);
        
}