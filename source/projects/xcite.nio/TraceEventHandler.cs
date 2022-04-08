using System;

namespace xcite.nio {
    /// <summary> Signature of a method that handles a trace event. </summary>
    /// <param name="message">Message to trace</param>
    /// <param name="ex">Optional exception to trace</param>
    public delegate void TraceEventHandler(string message, Exception ex = null);
}