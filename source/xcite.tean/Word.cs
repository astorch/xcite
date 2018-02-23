using System.Diagnostics;

namespace xcite.tean {
    /// <inheritdoc />
    /// <summary> Area within a string that can be interpreted as word. </summary>
    [DebuggerDisplay("Word: {Begin}-{End} '{Text}'")]
    public class Word : Span {
        
        /// <inheritdoc />
        public Word(string text, int begin) : base(text, begin) {
            // Accomplish base initializer
        }
    }
}