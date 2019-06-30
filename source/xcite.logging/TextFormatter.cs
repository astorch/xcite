using System;
using System.Collections.Generic;
using System.Threading;

namespace xcite.logging {
    /// <summary> Formats log records based on a specified pattern. </summary>
    public class TextFormatter {
        private readonly Dictionary<string, Plot> _plotter = new Dictionary<string, Plot>(10) {
            {"level", PlotLogLevel},
            {"date", PlotDate},
            {"name", PlotLogName},
            {"text", PlotText},
            {"nl", PlotNewline},
            {"thread", PlotThread}
        };

        /// <summary> Formats the given <paramref name="logData"/> using the specified <paramref name="pattern"/>. </summary>
        public string FormatValue(string pattern, LogData logData) {
            if (string.IsNullOrEmpty(pattern)) return string.Empty;
            
            char[] patternChars = pattern.ToCharArray();
            char[] buffer = new char[500];
            int p = 0;
            for (int i = -1; ++i != patternChars.Length;) {
                char c = patternChars[i];

                if (c == '%') {
                    string token = ReadTokenName(patternChars, i, out string arg, out int end);
                    Plot plot = GetPlotter(token);
                    string tokenValue = plot(arg, logData);

                    if (p + tokenValue.Length >= buffer.Length)
                        Array.Resize(ref buffer, (int) (p + tokenValue.Length * 1.4));
                    
                    for (int j = -1; ++j != tokenValue.Length;)
                        buffer[p++] = tokenValue[j];

                    i = end;
                    continue;
                }

                if (p == buffer.Length)
                    Array.Resize(ref buffer, (int) (buffer.Length * 1.4));

                buffer[p++] = c;
            }

            return new string(buffer, 0, p);
        }

        /// <summary>
        /// Returns the plotter that is related with the specified <paramref name="tokenName"/>.
        /// If there isn't a registered plotter, a default plotter that writes an empty string
        /// is returned.
        /// </summary>
        private Plot GetPlotter(string tokenName) {
            if (_plotter.TryGetValue(tokenName, out Plot plot)) return plot;
            return (s, data) => string.Empty;
        }
        
        /// <summary>
        /// Parses the plot token name from the given <paramref name="patternCharSet"/> beginning
        /// at the specified index <paramref name="b"/>. If the token name is followed by an argument,
        /// the argument value is provided via <paramref name="arg"/>. The token name end index is provided
        /// via <paramref name="e"/>.
        /// </summary>
        private static string ReadTokenName(char[] patternCharSet, int b, out string arg, out int e) {
            char[] buffer = new char[50];
            int p = 0;

            char[] argCharSet = new char[50];
            int l = 0;

            bool argMode = false;
            
            e = b;
            for (; ++e != patternCharSet.Length;) {
                char c = patternCharSet[e];

                if (c == '(') {
                    argMode = true;
                    continue;
                }

                if (c == ')') {
                    argMode = false;
                    continue;
                }
                
                if (!argMode && (c == ' ' || c == '%' || !char.IsLetter(c))) {
                    break;
                }

                if (argMode)
                    argCharSet[l++] = c;
                else
                    buffer[p++] = c;
            }

            e--;
            
            arg = new string(argCharSet, 0, l);
            return new string(buffer, 0, p);
        }

        /// <summary> Signature of a data plotter. </summary>
        /// <param name="arg">(Optional) Plot argument</param>
        /// <param name="logData">Data to plot</param>
        private delegate string Plot(string arg, LogData logData);

        /// <summary> Plots the thread information based on the specified <paramref name="arg"/>. </summary>
        private static string PlotThread(string arg, LogData logData) {
            Thread td = Thread.CurrentThread;
            if (arg == null) return td.ManagedThreadId.ToString();
            if (arg == "name") return td.Name;
            return td.ManagedThreadId.ToString();
        }
        
        /// <summary> Plots a newline string. </summary>
        private static string PlotNewline(string _, LogData logData) {
            return Environment.NewLine;
        }
        
        /// <summary> Plots the log data value. </summary>
        private static string PlotText(string _, LogData logData) {
            return logData.value;
        }

        /// <summary> Plots the name of the log based on the specified <paramref name="format"/>. </summary>
        private static string PlotLogName(string format, LogData logData) {
            string logName = logData.name;
            int dotTkIdx = logName.LastIndexOf('.');
            
            // No qualified class name
            if (dotTkIdx == -1) return logName;

            string simpleName = logName.Substring(dotTkIdx + 1);
            
            // No format specified, so we take the simple name as default
            if (string.IsNullOrEmpty(format)) return simpleName;

            // Explicitly full qualified requested
            if (format == "fq") return logName;

            // Any other cases
            return simpleName;
        }

        /// <summary> Plots the current timestamp with the specified <paramref name="format"/>. </summary>
        private static string PlotDate(string format, LogData logData) {
            return DateTime.Now.ToString(format);
        }

        /// <summary> Plots the current log level. </summary>
        private static string PlotLogLevel(string arg, LogData data) {
            return data.level.ToString();
        }
    }
}