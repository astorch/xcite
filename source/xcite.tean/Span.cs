using System.Diagnostics;

namespace xcite.tean {
    /// <summary> Area within a string that contains specific text. </summary>
    [DebuggerDisplay("Begin: {Begin} End: {End} Text: {Text}")]
    public abstract class Span {
        
        /// <summary> Initializes the new instance. </summary>
        /// <param name="text">Text</param>
        /// <param name="begin">Begin</param>
        /// <param name="end">End</param>
        protected Span(string text, int begin, int end) {
            Text = text;
            Begin = begin;
            End = end;
            Length = end - begin;
        }

        /// <summary> Text </summary>
        public string Text { get; }
        
        /// <summary> Begin </summary>
        public int Begin { get; }
        
        /// <summary> End </summary>
        public int End { get; }
        
        /// <summary> Length </summary>
        public int Length { get; }
    }
}