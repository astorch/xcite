using System;
using CommunityToolkit.HighPerformance.Buffers;

namespace xcite.logging;

/// <summary> Implements the log stream writing based on the current configuration. </summary>
public class LogOperator(LogConfiguration config, TextFormatter textFormatter) {
    // Note, the configuration may has changed. So we have to check the current state on-the-fly (again and again)
    
    /// <summary> Writes the given <paramref name="logData"/> to all registered log streams. </summary>
    public void Write(in LogData logData) {
        if (!logData.level.IsGreaterOrEqual(config.Level)) return;
            
        // Maybe a recyclable memory stream is faster?
        using ArrayPoolBufferWriter<char> bufferWriter = new(1024); // Should be sufficent for the very most cases

        string logPattern = config.Pattern;
        textFormatter.FormatValue(in logPattern, in logData, bufferWriter);

        ReadOnlySpan<char> writtenSpan = bufferWriter.WrittenSpan;
        ILogStream[] logStreams = config.Streams;
        for (int i = -1, ilen = logStreams.Length; ++i != ilen;) {
            logStreams[i].Write(in writtenSpan, in logData);
        }

        if (logData.exception is null) return;

        string exceptionMessage = logData.exception.ToString();
        LogData exLgDt = new(in logData.name, in logData.level, in exceptionMessage, in logData.exception);

        bufferWriter.Clear();
        textFormatter.FormatValue(in logPattern, in exLgDt, bufferWriter);

        ReadOnlySpan<char> exWrittenSpan = bufferWriter.WrittenSpan;
        for (int i = -1, ilen = logStreams.Length; ++i != ilen;) {
            logStreams[i].Write(in exWrittenSpan, in exLgDt);
        }
    }
        
}