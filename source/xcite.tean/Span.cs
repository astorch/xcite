using System.Diagnostics;

namespace xcite.tean {
    [DebuggerDisplay("Begin: {Begin} End: {End} Text: {Text}")]
    public abstract class Span {
        
        /// <inheritdoc />
        protected Span(string text, int begin, int end) {
            Text = text;
            Begin = begin;
            End = end;
            Length = end - begin;
        }

        public string Text { get; }
        
        public int Begin { get; }
        
        public int End { get; }
        
        public int Length { get; }
    }
}