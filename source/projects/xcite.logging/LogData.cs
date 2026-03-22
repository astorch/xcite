using System;

namespace xcite.logging;

/// <summary> Describes the data to log. </summary>
public readonly ref struct LogData(in string name, in ELogLevel level, in string value, in Exception exception) {

    /// <summary> Returns the log name </summary>
    public readonly ref string name = ref name;

    /// <summary> Returns the log level </summary>
    public readonly ref ELogLevel level = ref level;

    /// <summary> Returns the log message </summary>
    public readonly ref string value = ref value;

    /// <summary> Returns an optional exception </summary>
    public readonly ref Exception exception = ref exception;
    
}