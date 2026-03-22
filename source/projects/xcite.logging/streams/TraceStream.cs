using System;

namespace xcite.logging.streams;

/// <summary> Inherits <see cref="AbstractStream"/> to print records onto the .NET trace environment. </summary>
/// <seealso cref="System.Diagnostics.Debug"/>
public sealed class TraceStream : AbstractStream {
        
    /// <inheritdoc />
    protected override void OnDispose(bool disposing) {
        System.Diagnostics.Trace.Flush();
    }

    /// <inheritdoc />
    protected override void OnWrite(in ReadOnlySpan<char> value, in LogData logData) {
        string message = new(value);
        switch (logData.level) {
            case var lvl when lvl == ELogLevel.Error:
                System.Diagnostics.Trace.TraceError(message);
                break;
            case var lvl when lvl == ELogLevel.Warn:
                System.Diagnostics.Trace.TraceWarning(message);
                break;
            case var lvl when lvl == ELogLevel.Info:
                System.Diagnostics.Trace.TraceInformation(message);
                break;
            default:
                System.Diagnostics.Trace.Write(message); // We could add a category (log name or level) here
                break;
        }
    }

}