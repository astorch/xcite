using System.Globalization;

namespace xcite.tean {
    /// <inheritdoc />
    /// <summary> Provides access to various language information. </summary>
    public class LangInfo : ILangInfo {

        /// <summary> German language information </summary>
        public static readonly ILangInfo German = new LangInfo {
            PuncMarks = new[] {'.', '!', '?', ':'},
            DateSeparator = '.',
            TimeSeparator = ':',
            WordSeparator = new[] {' ', ',', '.', '!', '?', ':'}
        };

        /// <summary>
        /// Returns the <see cref="ILangInfo"/> that is associated with the given <paramref name="cultureNfo"/>.
        /// </summary>
        /// <param name="cultureNfo">Culture info</param>
        /// <returns>Associated <see cref="ILangInfo"/></returns>
        public static ILangInfo Get(CultureInfo cultureNfo) {
            return German;
        }

        /// <inheritdoc />
        private LangInfo() {
            // Prevent external initialization
        }

        /// <inheritdoc />
        public char[] PuncMarks { get; private set; }

        /// <inheritdoc />
        public char DateSeparator { get; private set; }

        /// <inheritdoc />
        public char TimeSeparator { get; private set; }

        /// <inheritdoc />
        public char[] WordSeparator { get; private set; }
    }
}