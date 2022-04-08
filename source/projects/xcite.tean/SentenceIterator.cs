using System;
using System.Collections;
using System.Collections.Generic;

namespace xcite.tean {
    /// <inheritdoc />
    /// <summary> Iterates sentences of a text. </summary>
    public class SentenceIterator : IEnumerator<Sentence> {
        private readonly char[] _charSet;
        private readonly ILangInfo _lang;
        private readonly char[] _puncMarks;
        private readonly TextKit _textKit;
        
        private int _p;

        /// <summary> Initializes the new instance. </summary>
        /// <param name="text">Text to iterate</param>
        /// <param name="lang">Language info</param>
        /// <exception cref="ArgumentNullException">If any argument is NULL</exception>
        public SentenceIterator(string text, ILangInfo lang) {
            if (text == null) throw new ArgumentNullException(nameof(text));
            _lang = lang ?? throw new ArgumentNullException(nameof(lang));
            _charSet = text.ToCharArray();
            _puncMarks = _lang.PuncMarks;
            _textKit = new TextKit(new[] {SpecChars.WS, SpecChars.CR, SpecChars.NL});
        }

        /// <inheritdoc />
        public Sentence Current { get; private set; }

        /// <inheritdoc />
        object IEnumerator.Current 
            => Current;

        /// <inheritdoc />
        public bool MoveNext() {
            int i = _p;
            bool escapeMode = false;
            while (i != _charSet.Length) {
                char c = _charSet[i];

                // Toggle escape mode
                if (c == SpecChars.DQ || c == SpecChars.SQ)
                    escapeMode = !escapeMode;
                
                // In escape mode, we have nothing to do
                if (escapeMode) goto NextRound;
                
                // Determine if we have a punctuation mark
                bool isPuncMark = Array.IndexOf(_puncMarks, c) != -1;

                if (isPuncMark) {
                    // Get next valueable character
                    int nx = i + 1;
                    _textKit.PlayForward(_charSet, ref nx);

                    // Sentence ends if there is no additional data or the next character is upper case
                    bool closeSentence;
                    if (nx == _charSet.Length) {
                        closeSentence = true;
                    } else {
                        char cx = _charSet[nx];
                        closeSentence = char.IsUpper(cx);
                    }              

                    // Shift and create sentence
                    if (closeSentence) {
                        int end = i + 1;
                        int length = end - _p;
                        string text = new string(_charSet, _p, length);
                        Current = new Sentence(text, _p);

                        _p = nx;
                        return true;
                    }
                }
                
                // Sentence ends here, if the punc mark does not indiciate a special construction
//                if (isPuncMark && !_textKit.IsSpecialTerm(_charSet, i, _lang)) {
//                    int end = i + 1;
//                    int length = end - _p;
//                    string text = new string(_charSet, _p, length);
//                    Current = new Sentence(text, _p);
//
//                    // Play forward until we reach the next valuable letter
//                    int n = end;
//                    _textKit.PlayForward(_charSet, ref n);
//                    
//                    _p = n;
//                    return true;
//                }
                
                NextRound:
                i++;
            }
            return false;
        }

        /// <inheritdoc />
        public void Reset() {
            _p = 0;
        }

        /// <inheritdoc />
        public void Dispose() {
            // Nothing to do here
        }
    }
}