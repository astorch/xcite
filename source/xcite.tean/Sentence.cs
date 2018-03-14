using System.Diagnostics;

namespace xcite.tean {
    /// <inheritdoc />
    /// <summary> Area within a string that can be interpreted as sentence. </summary>
    [DebuggerDisplay("Sentence: {Begin}-{End} Text: '{Text,nq}'")]
    public class Sentence : Span {
        private string _unformattedText;
        
        /// <inheritdoc />
        public Sentence(string text, int begin) : base(text, begin) {
            // Accomplish base initializer
        }

        /// <summary> <see cref="Span.Text"/> without any linefeeds or tabs. </summary>
        public virtual string UnformattedText => _unformattedText ?? (_unformattedText = SanitizeText(Text));

        /// <summary>
        /// Removes all linefeeds and tabs from the given <paramref name="text"/>.
        /// </summary>
        /// <param name="text">Text to clean</param>
        /// <returns>Cleaned text</returns>
        private static string SanitizeText(string text) {
            char[] buffer = new char[text.Length];
            int j = 0;
            for (int i = -1; ++i != text.Length;) {
                char c = text[i];
                if (c == SpecChars.TB)
                    c = SpecChars.WS;
                
                if (c == SpecChars.CR) {
                    // Do look-up
                    int n = i + 1;
                    while (n != text.Length && (text[n] == SpecChars.CR || text[n] == SpecChars.NL))
                        n++;

                    // If the CR does have a hyphen as predecessor, we don't need a whitespace
                    bool insertWhitespace = !(i != 0 && text[i - 1] == '-');

                    // Insert a single whitespace instead of the stripped symbols
                    if (insertWhitespace)
                        buffer[j++] = SpecChars.WS;

                    i = n - 1;
                    continue;
                }
                
                buffer[j++] = c;
            }
            return new string(buffer, 0, j);
        }
    }
}