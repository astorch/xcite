using System.Diagnostics;

namespace xcite.tean {
    /// <summary> Area within a string that contains specific text. </summary>
    [DebuggerDisplay("Begin: {Begin} End: {End} Text: {Text,nq}")]
    public abstract class Span {
        
        /// <summary> Initializes the new instance. </summary>
        /// <param name="text">Text</param>
        /// <param name="begin">Begin</param>
        protected Span(string text, int begin) {
            Text = text;
            Begin = begin;
            End = begin + text.Length;
        }

        /// <summary> Text </summary>
        public virtual string Text { get; }
        
        /// <summary> Begin </summary>
        public virtual int Begin { get; }
        
        /// <summary> End </summary>
        public virtual int End { get; }

        /// <summary> Length </summary>
        public virtual int Length
            => Text.Length;
    }
}