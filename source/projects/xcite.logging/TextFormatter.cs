using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Threading;

namespace xcite.logging;

/// <summary> Formats log records based on a specified pattern. </summary>
public class TextFormatter {

    private static readonly FrozenDictionary<string, Plot> _standardPlotter = new Dictionary<string, Plot>(6) {
        { "level",  PlotLogLevel},
        { "date",   PlotDate},
        { "name",   PlotLogName},
        { "text",   PlotText},
        { "nl",     PlotNewline},
        { "thread", PlotThread}
    }.ToFrozenDictionary(StringComparer.InvariantCultureIgnoreCase);
    
    private readonly ConcurrentDictionary<string, IReadOnlyList<PlotPeer>> _plotSequences = new();

    /// <summary> Formats the given <paramref name="logData"/> using the specified <paramref name="pattern"/>. </summary>
    public void FormatValue(in string pattern, in LogData logData, in IBufferWriter<char> bufferWriter) {
        foreach (PlotPeer plotPeer in ToPlotPeerSequence(in pattern)) {
            plotPeer(in logData, in bufferWriter);
        }
    }
    
    /// <summary> Returns a sequence of plot peers for that <paramref name="pattern"/>. </summary>
    private IReadOnlyList<PlotPeer> ToPlotPeerSequence(in string pattern)
        => _plotSequences.GetOrAdd(
            pattern,
            static pttrn => BuildPlotSequence(in pttrn) 
        );

    /// <summary> Builds a sequence of plot peers for that <paramref name="pattern"/>. </summary>
    private static IReadOnlyList<PlotPeer> BuildPlotSequence(in string pattern) {
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
                    PlotPeer plotFill = (in _, in writer) => writer.Write(fillData);
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
            PlotPeer plotFill = (in _, in writer) => writer.Write(fillData);
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
    private static PlotPeer GetPlotPeer(string plotName, string plotArg) {
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

    
    private delegate void PlotPeer(in LogData logData, in IBufferWriter<char> bufferWriter);
    
    private delegate void Plot(in string arg, in LogData logData, in IBufferWriter<char> bufferWriter);
    
    /// <summary> Plots nothing. </summary>
    private static void PlotNull(in string arg, in LogData logData, in IBufferWriter<char> bufferWriter) {
        // Do nothing
    }

    /// <summary> Plots the thread information based on the specified <paramref name="arg"/>. </summary>
    private static void PlotThread(in string arg, in LogData logData, in IBufferWriter<char> bufferWriter) {
        Thread thread = Thread.CurrentThread;
        string txt = arg switch {
            null => thread.ManagedThreadId.ToString(),
            "name" => thread.Name,
            _ => thread.ManagedThreadId.ToString()
        };

        bufferWriter.Write(txt);
    }

    /// <summary> Plots a newline string. </summary>
    private static void PlotNewline(in string _, in LogData logData, in IBufferWriter<char> bufferWriter)
        => bufferWriter.Write(" ");

    /// <summary> Plots the log data value. </summary>
    private static void PlotText(in string _, in LogData logData, in IBufferWriter<char> bufferWriter)
        => bufferWriter.Write(logData.value);

    /// <summary> Plots the name of the log based on the specified <paramref name="format"/>. </summary>
    private static void PlotLogName(in string format, in LogData logData, in IBufferWriter<char> bufferWriter) {
        string logName = logData.name;
        int dotTkIdx = logName.LastIndexOf('.');
            
        // No qualified class name
        if (dotTkIdx == -1) {
            bufferWriter.Write(logName);
            return;
        }

        string simpleName = logName[(dotTkIdx + 1)..];
            
        // No format specified, so we take the simple name as default
        if (string.IsNullOrEmpty(format)) {
            bufferWriter.Write(simpleName);
            return;
        }

        // Explicitly full qualified requested
        if (format == "fq") {
            bufferWriter.Write(logName);
            return;
        }

        // Any other cases
        bufferWriter.Write(simpleName);
    }

    /// <summary> Plots the current log level. </summary>
    private static void PlotLogLevel(in string arg, in LogData data, in IBufferWriter<char> bufferWriter)
        => bufferWriter.Write(data.level.ToString());

    /// <summary> Plots the current timestamp with the specified <paramref name="format"/>. </summary>
    private static void PlotDate(in string format, in LogData _, in IBufferWriter<char> bufferWriter)
        => bufferWriter.Write(DateTime.Now.ToString(format));

}