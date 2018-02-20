using System.Diagnostics;

namespace xcite.tean {
    /// <inheritdoc />
    /// <summary> Area within a string that can be interpreted as sentence. </summary>
    [DebuggerDisplay("Sentence: {Begin}-{End} Text: '{Text}'")]
    public class Sentence : Span {
        /// <inheritdoc />
        public Sentence(string text, int begin, int end) : base(text, begin, end) {
            // Accomplish base initializer
        }
    }
}