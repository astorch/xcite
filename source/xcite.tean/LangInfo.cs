namespace xcite.tean {
    public class LangInfo : ILangInfo {
        public static readonly ILangInfo German = new LangInfo {
            PuncMarks = new[]{'.','!','?'}
        };

        /// <inheritdoc />
        private LangInfo() {
            // Prevent external initialization
        }

        /// <inheritdoc />
        public char[] PuncMarks { get; private set; }
    }
}