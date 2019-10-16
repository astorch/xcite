using System.Collections.Generic;

namespace xcite.logging {
    /// <summary> Signature of a method that handles <see cref="LogConfiguration"/> changed events. </summary>
    public delegate void LogConfigurationChangedHandler(LogConfiguration sender);
    
    /// <summary> Describes the logging configuration. </summary>
    public class LogConfiguration {
        private readonly List<ILogStream> _logStreams = new List<ILogStream>(10);

        /// <inheritdoc />
        public LogConfiguration() {
            Reset();
        }

        /// <summary> Is invoked when the <see cref="Pattern"/> has been changed. </summary>
        public event LogConfigurationChangedHandler PatternChanged;

        /// <summary> Is invoked when the <see cref="Level"/> has been changed. </summary>
        public event LogConfigurationChangedHandler LevelChanged;

        /// <summary> Is invoked when the <see cref="Streams"/> set has been changed. </summary>
        public event LogConfigurationChangedHandler StreamsChanged;
        
        /// <summary> Log record format pattern </summary>
        public string Pattern { get; private set; }

        /// <summary> Log record level </summary>
        public ELogLevel Level { get; private set; }

        /// <summary> Log record streams </summary>
        public ILogStream[] Streams 
            => _logStreams.ToArray();

        /// <summary> Sets the <see cref="Pattern"/> to the specified <paramref name="value"/>. </summary>
        public LogConfiguration SetPattern(string value) {
            Pattern = value ?? string.Empty;
            PatternChanged?.Invoke(this);
            return this;
        }

        /// <summary> Sets the <see cref="Level"/> to the specified <paramref name="value"/>. </summary>
        public LogConfiguration SetLevel(ELogLevel value) {
            Level = value ?? ELogLevel.None;
            LevelChanged?.Invoke(this);
            return this;
        }

        /// <summary> Adds the specified type of <typeparamref name="TStream"/> to the configuration. </summary>
        /// <typeparam name="TStream">Type of stream to add</typeparam>
        public LogConfiguration AddStream<TStream>() where TStream : class, ILogStream, new() {
            return AddStream(new TStream());
        }

        /// <summary> Adds the given <paramref name="logStream"/> instance to the configuration. </summary>
        public LogConfiguration AddStream(ILogStream logStream) {
            if (logStream == null) return this;
            
            _logStreams.Add(logStream);
            StreamsChanged?.Invoke(this);
            return this;
        }

        /// <summary> Resets the configuration to its default values. </summary>
        public LogConfiguration Reset() {
            Pattern = LogPatterns.Standard;
            Level = ELogLevel.Info;
            
            for (int i = -1; ++i != _logStreams.Count;)
                _logStreams[i].Dispose();
            
            _logStreams.Clear();
            return this;
        }
    }
}