using System;
using System.Collections;
using System.Collections.Generic;

namespace xcite.tean {
    /// <inheritdoc />
    /// <summary> Iterates sentences of a text. </summary>
    public class SentenceIterator : IEnumerator<Sentence> {
        private static readonly char[] BiasSet = {SpecChars.WS, SpecChars.CR, SpecChars.NL};

        private readonly char[] _charSet;
        private readonly char[] _buffer;
        private readonly ILangInfo _lang;
        private readonly char[] _puncMarks;
        
        private int _p;

        /// <summary> Initializes the new instance. </summary>
        /// <param name="text">Text to iterate</param>
        /// <param name="lang">Language info</param>
        /// <exception cref="ArgumentNullException"></exception>
        public SentenceIterator(string text, ILangInfo lang) {
            if (text == null) throw new ArgumentNullException(nameof(text));
            _lang = lang ?? throw new ArgumentNullException(nameof(lang));
            _charSet = text.ToCharArray();
            _buffer = new char[_charSet.Length];
            _puncMarks = _lang.PuncMarks;
        }

        /// <inheritdoc />
        public bool MoveNext() {
            int i = _p;
            
            while (i != _charSet.Length) {
                char c = _charSet[i];
                bool isPuncMark = Array.IndexOf(_puncMarks, c) != -1;

                // TODO escape strings
                
                // Sentence ends here, if the punc mark is not part of an URL
                if (isPuncMark && !IsUrl(_charSet, i)) {
                    int end = i + 1;
                    int length = end - _p;
                    Array.Copy(_charSet, _p, _buffer, 0, length);
                    string text = new string(_buffer, 0, length);
                    Current = new Sentence(text, _p, end);

                    int n = end;
                    
                    // Play forward until we reach the next valuable letter
                    while (n != _charSet.Length && IsBias(_charSet[n]))
                        n++;
                    
                    _p = n;
                    return true;
                }
                
                i++;
            }
            return false;
        }

        /// <inheritdoc />
        public void Reset() {
            _p = 0;
        }

        /// <inheritdoc />
        object IEnumerator.Current 
            => Current;

        /// <inheritdoc />
        public void Dispose() {
            // Nothing to do here
        }

        /// <inheritdoc />
        public Sentence Current { get; private set; }

        /// <summary>
        /// Returns TRUE if the punctuation mark at the specified index of the given <paramref name="charSet"/>
        /// indicates an URL.
        /// </summary>
        /// <param name="charSet">Char set</param>
        /// <param name="pmIndex">Punctuation mark index</param>
        /// <returns>TRUE or FALSE</returns>
        private static bool IsUrl(char[] charSet, int pmIndex) {
            // Only a dot is relevant
            if (charSet[pmIndex] != SpecChars.DT) return false;
            
            int np1 = pmIndex + 1;
            
            // If we are at the end of the stream, this is no URL
            if (np1 == charSet.Length) return false;
            char m = charSet[np1];

            // If the PM is followed by a bias token, this is no URL
            if (IsBias(m)) return false;

            return true;
        }

        /// <summary> Returns TRUE if the specified character is known as bias. </summary>
        /// <param name="c">Character to check</param>
        /// <returns>TRUE or FALSE</returns>
        private static bool IsBias(char c) 
            => Array.IndexOf(BiasSet, c) != -1;
    }
}