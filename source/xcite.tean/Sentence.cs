using System.Diagnostics;

namespace xcite.tean {
    [DebuggerDisplay("Sentence: {Begin}-{End} Text: '{Text}'")]
    public class Sentence : Span {
        /// <inheritdoc />
        public Sentence(string text, int begin, int end) : base(text, begin, end) {
            // Accomplish base initializer
        }
    }
}