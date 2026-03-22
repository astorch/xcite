using System;

namespace xcite.logging.streams;

/// <summary> Inherits <see cref="AbstractStream"/> to print records onto the .NET debug environment. </summary>
/// <seealso cref="System.Diagnostics.Debug"/>
public sealed class DebugStream : AbstractStream {

    /// <inheritdoc />
    protected override void OnDispose(bool disposing) {
        System.Diagnostics.Debug.Flush();
    }

    /// <inheritdoc />
    protected override void OnWrite(in ReadOnlySpan<char> value, in LogData logData) {
        System.Diagnostics.Debug.Write(new string(value));
    }

}