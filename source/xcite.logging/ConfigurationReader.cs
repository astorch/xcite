using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace xcite.logging {
    /// <summary> Provides methods to read log configuration files. </summary>
    public static class ConfigurationReader {
        private static FileSystemWatcher _fileSystemWatcher;

        /// <summary>
        /// Reads the log configuration from a file with the specified <paramref name="fileName"/>.
        /// If <paramref name="watch"/> is set, the configuration is updated
        /// every time the file is modified.
        /// </summary>
        /// <exception cref="ArgumentException">If the specified <paramref name="fileName"/> does not point to valid file.</exception>
        public static LogConfiguration ReadFile(string fileName, bool watch) {
            string fullPath = Path.GetFullPath(fileName);
            if (!File.Exists(fullPath)) throw new ArgumentException($"There is no file located at '{fullPath}'.");

            string fileContent = File.ReadAllText(fileName);
            LogConfiguration logConfig = ReadText(fileContent);
            
            if (watch) {
                string fileDir = Path.GetDirectoryName(fullPath);
                if (fileDir == null) throw new ArgumentException($"Cannot resolve containing folder of file '{fullPath}'.");
            
                string simpleFileName = Path.GetFileName(fullPath);

                void updateLogConfiguration(object sender, FileSystemEventArgs e) {
                    OnFileChanged(logConfig, e);
                }

                if (_fileSystemWatcher != null) {
                    _fileSystemWatcher.Changed -= updateLogConfiguration; // Does this really work?
                    _fileSystemWatcher.Dispose();
                    _fileSystemWatcher = null;                
                }
            
                _fileSystemWatcher = new FileSystemWatcher(fileDir, simpleFileName);
                _fileSystemWatcher.Changed += updateLogConfiguration;
            }

            return logConfig;
        }

        /// <summary>
        /// Reads the log configuration from the given <paramref name="fileContent"/>.
        /// If the <paramref name="fileContent"/> is NULL or empty, a default configuration
        /// is returned. 
        /// </summary>
        public static LogConfiguration ReadText(string fileContent) {
            LogConfiguration fileConfig = new LogConfiguration();

            // No content, no configuration
            if (string.IsNullOrEmpty(fileContent)) return fileConfig;

            Dictionary<string, ILogStream> streamSet = new Dictionary<string, ILogStream>(10);
            
            // Parse lines
            string[] lines = fileContent.Split('\n');
            for (int i = -1; ++i != lines.Length;) {
                string line = lines[i];
                if (string.IsNullOrEmpty(line)) continue; // Empty line
                if (line[0] == '#') continue; // Comment line

                string[] linePair = line.Split('=');
                if (linePair.Length != 2) continue; // Invalid format

                char[] bias = {' ', '\t', '\r'};
                string key = linePair[0]?.Trim(bias).ToLower();
                string value = linePair[1]?.Trim(bias).ToLower();
                
                if (key == null) continue;
                if (value == null) continue;
                
                // Reserved word
                if (key == "level") {
                    ELogLevel.TryParse(value, out ELogLevel level);
                    fileConfig.SetLevel(level);
                    continue;
                }

                if (key == "pattern") {
                    fileConfig.SetPattern(value);
                    continue;
                }
                
                if (key == "streams") {
                    string[] streamNames = value.Split(',');
                    for (int j = -1; ++j != streamNames.Length;) {
                        string streamName = streamNames[j]?.Trim();
                        if (streamName == null) continue;
                        streamSet[streamName] = null;
                    }
                    continue;
                }
                
                // Dynamic content
                int psIdx = key.IndexOf('.');
                string streamId;
                string streamProp;
                ILogStream logStream;
                
                // No dot, no stream property. But the stream kind.
                if (psIdx == -1) {
                    streamId = key;
                    logStream = StreamStore.GetStreamInstance(value);
                    streamSet[streamId] = logStream;
                    fileConfig.AddStream(logStream);
                    continue;
                }

                streamId = key.Substring(0, psIdx);
                streamProp = key.Substring(psIdx + 1);
                if (!streamSet.TryGetValue(streamId, out logStream)) continue;
                SetStreamProperty(logStream, streamProp, value);
            }

            return fileConfig;
        }

        /// <summary>
        /// Sets the <paramref name="property"/> of the given <paramref name="logStream"/>
        /// to the specified <paramref name="value"/>. If the <paramref name="logStream"/> does
        /// not own the declared <paramref name="property"/>, nothing will happen.
        /// </summary>
        private static void SetStreamProperty(ILogStream logStream, string property, string value) {
            if (logStream == null) return;

            string lcProp = property.ToLower();
            PropertyInfo[] logStreamProperties = logStream.GetType().GetProperties();
            for (int i = -1; ++i != logStreamProperties.Length;) {
                PropertyInfo logStreamProperty = logStreamProperties[i];
                string lcStreamProp = logStreamProperty.Name.ToLower();
                if (lcStreamProp != lcProp) continue;

                object val = Convert.ChangeType(value, logStreamProperty.PropertyType);
                logStreamProperty.SetValue(logStream, val);
                return;
            }
        }

        /// <summary> Is invoked when the watched log file has been changed. </summary>
        private static void OnFileChanged(LogConfiguration logConfig, FileSystemEventArgs e) {
            // Currently not implemented
        }
    }
}