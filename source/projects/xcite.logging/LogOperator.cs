using System;
using CommunityToolkit.HighPerformance.Buffers;

namespace xcite.logging;

/// <summary> Implements the log stream writing based on the current configuration. </summary>
public class LogOperator(LogConfiguration config, TextFormatter textFormatter) {
    private readonly ELogLevel _thresholdLevel = config.Level;
    private readonly string _logPattern = config.Pattern;
    private readonly ILogStream[] _logStreams = config.Streams;
    
    /// <summary> Writes the given <paramref name="logData"/> to all registered log streams. </summary>
    public void Write(in LogData logData) {
        if (!logData.level.IsGreaterOrEqual(_thresholdLevel)) return;
            
        // Maybe a recyclable memory stream is faster?
        using ArrayPoolBufferWriter<char> bufferWriter = new(1024); // Should be sufficent for the very most cases
        
        textFormatter.FormatValue(in _logPattern, in logData, bufferWriter);

        ReadOnlySpan<char> writtenSpan = bufferWriter.WrittenSpan;
        for (int i = -1, ilen = _logStreams.Length; ++i != ilen;) {
            _logStreams[i].Write(in writtenSpan, in logData);
        }

        if (logData.exception is null) return;

        string exceptionMessage = logData.exception.ToString();
        LogData exLgDt = new(in logData.name, in logData.level, in exceptionMessage, in logData.exception);

        bufferWriter.Clear();
        textFormatter.FormatValue(in _logPattern, in exLgDt, bufferWriter);

        ReadOnlySpan<char> exWrittenSpan = bufferWriter.WrittenSpan;
        for (int i = -1, ilen = _logStreams.Length; ++i != ilen;) {
            _logStreams[i].Write(in exWrittenSpan, in exLgDt);
        }
    }
        
}