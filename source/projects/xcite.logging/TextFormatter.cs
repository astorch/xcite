using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace xcite.logging;

/// <summary> Formats log records based on a specified pattern. </summary>
public class TextFormatter {

    private readonly FrozenDictionary<string, Plot> _standardPlotter;
    private readonly ConcurrentDictionary<string, IReadOnlyList<PlotPeer>> _plotSequences = new();
    private readonly Encoding _encoding = new UTF8Encoding(false);

    /// <summary> Initializes the new instance </summary>
    public TextFormatter() {
        _standardPlotter = new Dictionary<string, Plot>(6) {
            { "level",  PlotLogLevel},
            { "date",   PlotDate},
            { "name",   PlotLogName},
            { "text",   PlotText},
            { "nl",     PlotNewline},
            { "thread", PlotThread}
        }.ToFrozenDictionary();
    }

    /// <summary> Formats the given <paramref name="logData"/> using the specified <paramref name="pattern"/>. </summary>
    public void FormatValue(in string pattern, in LogData logData, in IBufferWriter<byte> bufferWriter) {
        foreach (PlotPeer plotPeer in ToPlotPeerSequence(in pattern)) {
            plotPeer(in logData, in bufferWriter);
        }
    }
    
    /// <summary> Returns a sequence of plot peers for that <paramref name="pattern"/>. </summary>
    private IReadOnlyList<PlotPeer> ToPlotPeerSequence(in string pattern)
        => _plotSequences.GetOrAdd(
            pattern,
            pttrn => BuildPlotSequence(in pttrn) 
        );

    /// <summary> Builds a sequence of plot peers for that <paramref name="pattern"/>. </summary>
    private IReadOnlyList<PlotPeer> BuildPlotSequence(in string pattern) {
        if (string.IsNullOrWhiteSpace(pattern)) return [];

        List<PlotPeer> plotSequence = new(16);
        
        // "%level %date(dd.MM.yyyy HH:mm:ss) [%thread] %name - %text%nl"

        Span<char> buffer = stackalloc char[64];
        int p = 0;

        Span<char> plotNameBuffer = stackalloc char[64];
        Span<char> plotArgBuffer = stackalloc char[256];
        
        ReadOnlySpan<char> patternChars = pattern.AsSpan();
        for (int i = -1; ++i != patternChars.Length;) {
            char c = patternChars[i];

            if (c == '%') {

                if (p != 0) {
                    string fillData = new(buffer);
                    PlotPeer plotFill = (in _, in writer) => _encoding.GetBytes(fillData, writer);
                    plotSequence.Add(plotFill);
                    
                    buffer.Clear();
                    p = 0;
                }
                
                plotNameBuffer.Clear();
                plotArgBuffer.Clear();
                
                ReadPlotSegment(in patternChars, ref i, in plotNameBuffer, plotArgBuffer);
                
                plotSequence.Add(GetPlotPeer(new string(plotNameBuffer), new string(plotArgBuffer)));
                continue;
            }

            buffer[p++] = c;
        }

        if (p != 0) {
            string fillData = new(buffer);
            PlotPeer plotFill = (in _, in writer) => _encoding.GetBytes(fillData, writer);
            plotSequence.Add(plotFill);
                    
            // No clean-up here
        }

        return plotSequence;
    }

    /// <summary>
    /// Returns the plotter that is related with the specified <paramref name="plotName"/>.
    /// If there isn't a registered plotter, a default plotter that writes an empty string
    /// is returned.
    /// </summary>
    private PlotPeer GetPlotPeer(string plotName, string plotArg) {
        Plot basePlot = _standardPlotter.TryGetValue(plotName, out Plot plot) ? plot : PlotNull;
        return (in logData, in bufferWriter) => basePlot(in plotArg, in logData, in bufferWriter);
    }

    /// <summary>
    /// Parses the plot segment name from the given <paramref name="patternChars"/> beginning
    /// at the specified index <paramref name="i"/>. If the segment name is followed by an argument,
    /// the argument value is provided via <paramref name="plotArgBuffer"/>.
    /// </summary>
    private static void ReadPlotSegment(
        in ReadOnlySpan<char> patternChars, ref int i, 
        in Span<char> plotNameBuffer, in Span<char> plotArgBuffer
    ) {
        bool argMode = false;

        int p1 = 0;
        int p2 = 0;
        for (int ilen = patternChars.Length; ++i != ilen;) {
            char c = patternChars[i];

            switch (c) {
                case '(':
                    argMode = true;
                    continue;
                case ')':
                    argMode = false;
                    continue;
            }
            
            if (!argMode && (c is ' ' or '%' || !char.IsLetter(c))) {
                break;
            }

            switch (argMode) {
                case true:
                    plotArgBuffer[p2++] = c; 
                    continue;
                case false:
                    plotNameBuffer[p1++] = c;
                    continue;
            }
            
        }
    }

    
    private delegate void PlotPeer(in LogData logData, in IBufferWriter<byte> bufferWriter);
    
    private delegate void Plot(in string arg, in LogData logData, in IBufferWriter<byte> bufferWriter);
    
    /// <summary> Plots nothing. </summary>
    private static void PlotNull(in string arg, in LogData logData, in IBufferWriter<byte> bufferWriter) {
        // Do nothing
    }

    /// <summary> Plots the thread information based on the specified <paramref name="arg"/>. </summary>
    private void PlotThread(in string arg, in LogData logData, in IBufferWriter<byte> bufferWriter) {
        Thread thread = Thread.CurrentThread;
        string txt = arg switch {
            null => thread.ManagedThreadId.ToString(),
            "name" => thread.Name,
            _ => thread.ManagedThreadId.ToString()
        };

        _encoding.GetBytes(txt, bufferWriter);
    }

    /// <summary> Plots a newline string. </summary>
    private static void PlotNewline(in string _, in LogData logData, in IBufferWriter<byte> bufferWriter)
        => bufferWriter.Write("\n"u8);

    /// <summary> Plots the log data value. </summary>
    private void PlotText(in string _, in LogData logData, in IBufferWriter<byte> bufferWriter)
        => _encoding.GetBytes(logData.value, bufferWriter);

    /// <summary> Plots the name of the log based on the specified <paramref name="format"/>. </summary>
    private void PlotLogName(in string format, in LogData logData, in IBufferWriter<byte> bufferWriter) {
        string logName = logData.name;
        int dotTkIdx = logName.LastIndexOf('.');
            
        // No qualified class name
        if (dotTkIdx == -1) {
            _encoding.GetBytes(logName, bufferWriter);
            return;
        }

        string simpleName = logName[(dotTkIdx + 1)..];
            
        // No format specified, so we take the simple name as default
        if (string.IsNullOrEmpty(format)) {
            _encoding.GetBytes(simpleName, bufferWriter);
            return;
        }

        // Explicitly full qualified requested
        if (format == "fq") {
            _encoding.GetBytes(logName, bufferWriter);
            return;
        }

        // Any other cases
        _encoding.GetBytes(simpleName, bufferWriter);
    }

    /// <summary> Plots the current log level. </summary>
    private void PlotLogLevel(in string arg, in LogData data, in IBufferWriter<byte> bufferWriter)
        => _encoding.GetBytes(data.level.ToString(), bufferWriter);

    /// <summary> Plots the current timestamp with the specified <paramref name="format"/>. </summary>
    private void PlotDate(in string format, in LogData _, in IBufferWriter<byte> bufferWriter)
        => _encoding.GetBytes(DateTime.Now.ToString(format), bufferWriter);
    
}