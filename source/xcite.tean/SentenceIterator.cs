using System;
using System.Collections;
using System.Collections.Generic;

namespace xcite.tean {
    public class SentenceIterator : IEnumerator<Sentence> {
        private const char WS = ' ';
        private const char CR = '\r';
        private const char NL = '\n';
        private static readonly char[] FooSet = {WS, CR, NL};

        private readonly char[] _charSet;
        private readonly char[] _buffer;
        private readonly ILangInfo _lang;
        
        private int p = 0;

        /// <inheritdoc />
        public SentenceIterator(string text, ILangInfo lang) {
            if (text == null) throw new ArgumentNullException(nameof(text));
            _lang = lang ?? throw new ArgumentNullException(nameof(lang));
            _charSet = text.ToCharArray();
            _buffer = new char[_charSet.Length];
        }

        /// <inheritdoc />
        public bool MoveNext() {
            int i = p;

            char[] puncMarks = _lang.PuncMarks;
            
            while (i != _charSet.Length) {
                char c = _charSet[i];
                bool isPuncMark = Array.IndexOf(puncMarks, c) != -1;

                // TODO escape strings
                
                // Sentence ends here, if the punc mark is not part of an URL
                if (isPuncMark && !IsUrl(i)) {
                    int end = i + 1;
                    int length = end - p;
                    Array.Copy(_charSet, p, _buffer, 0, length);
                    string text = new string(_buffer, 0, length);
                    Current = new Sentence(text, p, end);

                    int n = end;
                    
                    // Play forward until we reach the next valuable letter
                    while (n != _charSet.Length && IsFoo(_charSet[n]))
                        n++;
                    
                    p = n;
                    return true;
                }
                
                i++;
            }
            return false;
        }

        private bool IsUrl(int pmIndex) {
            int np1 = pmIndex + 1;
            // If we are at the end of the stream, this is no URL
            if (np1 == _charSet.Length) return false;
            char m = _charSet[np1];

            // If the PM is followed by a whitespace, this is no URL
            if (IsFoo(m)) return false;

            return true;
        }

        private bool IsFoo(char c) {
            return Array.IndexOf(FooSet, c) != -1;
        }

        /// <inheritdoc />
        public void Reset() {
            p = 0;
        }

        /// <inheritdoc />
        object IEnumerator.Current 
            => Current;

        /// <inheritdoc />
        public void Dispose() {
            
        }

        /// <inheritdoc />
        public Sentence Current { get; private set; }
    }
}