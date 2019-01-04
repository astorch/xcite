using System;
using xcite.logging.streams;

namespace xcite.logging {
    /// <summary> Provides access to all available instance of <see cref="ILogStream"/>. </summary>
    public static class StreamStore {
        private static readonly Type[] StreamTypes = {
            typeof(ConsoleStream), typeof(DebugStream), typeof(FileStream),
            typeof(TraceStream)
        };
        
        /// <summary>
        /// Returns a new instance of <see cref="ILogStream"/> that owns
        /// the specified <paramref name="shortName"/>. 
        /// </summary>
        public static ILogStream GetStreamInstance(string shortName) {
            if (string.IsNullOrEmpty(shortName)) return null;
            string nmlc = shortName.ToLower();
            
            for (int i = -1; ++i != StreamTypes.Length;) {
                Type streamType = StreamTypes[i];
                string fnlc = streamType.FullName?.ToLower();
                if (fnlc == null) continue;

                if (fnlc.EndsWith(nmlc)) return (ILogStream) Activator.CreateInstance(streamType);
            }

            return null;
        }
    }
}