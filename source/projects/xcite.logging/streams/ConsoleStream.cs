using System;

namespace xcite.logging.streams;

/// <summary> Inherits <see cref="AbstractStream"/> to print records onto the console. </summary>
public sealed class ConsoleStream : AbstractStream {
        
    /// <inheritdoc />
    protected override void OnDispose(bool disposing) {
        Console.Out.Flush();
        Console.Error.Flush();
    }

    /// <inheritdoc />
    protected override void OnWrite(in ReadOnlySpan<char> value, in LogData logData) {
        switch (logData.level) {
            case var lvl when lvl == ELogLevel.Error:
                Console.Error.Write(value);
                break;
            default:
                Console.Out.Write(value);
                break;
        }
    }

}