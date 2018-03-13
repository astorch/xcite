using System;
using System.Collections;
using System.Collections.Generic;

namespace xcite.tean {
    /// <inheritdoc />
    /// <summary> Iterates words of a text. </summary>
    public class WordIterator : IEnumerator<Word> {
        private static readonly char[] BiasSet = {SpecChars.CR, SpecChars.NL, SpecChars.TB};
        
        private readonly char[] _charSet;
        private readonly ILangInfo _lang;
        private readonly char[] _buffer;
        private readonly char[] _wordSeparator;
        private readonly TextKit _textKit;

        private int _p;
        
        /// <summary> Initializes the new instance. </summary>
        /// <param name="text">Text that contains the words to iterate</param>
        /// <param name="lang">Language info</param>
        /// <exception cref="ArgumentNullException">If any argument is NULL</exception>
        public WordIterator(string text, ILangInfo lang) {
            if (text == null) throw new ArgumentNullException(nameof(text));
            _lang = lang ?? throw new ArgumentNullException(nameof(lang));
            _charSet = text.ToCharArray();
            _buffer = new char[100];
            _wordSeparator = lang.WordSeparator;
            _textKit = new TextKit(new[] {SpecChars.WS, SpecChars.CR, SpecChars.NL});
        }
        
        /// <inheritdoc />
        public Word Current { get; private set; }

        /// <inheritdoc />
        object IEnumerator.Current 
            => Current;

        /// <inheritdoc />
        public bool MoveNext() {
            int i = _p;
            int j = 0;

            bool escapeMode = false;
            
            while (i != _charSet.Length) {
                char c = _charSet[i];

                // Toggle escape mode
                if (c == SpecChars.DQ || c == SpecChars.SQ)
                    escapeMode = !escapeMode;
                
                // In escape mode, we have nothing to do
                if (escapeMode) goto TakeAndGo;
                
                // Determine if we have a word separator
                bool isWordSeparator = Array.IndexOf(_wordSeparator, c) != -1;
                
                // Is this character a word separator? If so, it must not be a special construction (date, URL, ...)
                if (isWordSeparator && !_textKit.IsSpecialTerm(_charSet, i, _lang)) {
                    string wordStr = new string(_buffer, 0, j);
                    Current = new Word(wordStr, _p);
                    
                    // Play forward until we reach the next valuable letter
                    int n = i + 1;
                    _textKit.PlayForward(_charSet, ref n);
                    
                    _p = n;
                    return true;
                }

                // We ignore bias characters
                if (TextKit.IsBias(BiasSet, c)) goto NextRound;

                TakeAndGo:
                // Character is relevant, so we take it
                _buffer[j++] = c;
                
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