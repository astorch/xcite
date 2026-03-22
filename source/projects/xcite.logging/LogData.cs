using System;

namespace xcite.logging;

/// <summary> Describes the data to log. </summary>
public readonly ref struct LogData(in string name, in ELogLevel level, in string value, in Exception exception) {

    /// <summary> Returns the log name </summary>
    public readonly string name = name;

    /// <summary> Returns the log level </summary>
    public readonly ELogLevel level = level;

    /// <summary> Returns the log message </summary>
    public readonly string value = value;

    /// <summary> Returns an optional exception </summary>
    public readonly Exception exception = exception;
    
}