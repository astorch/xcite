using System;

namespace xcite.tean {
    /// <summary> Provides common methods to check aspects of a text (fragment). </summary>
    public class TextKit {
        private readonly char[] _biasSet;

        /// <summary> Initilaizes the new instance. </summary>
        /// <param name="biasSet">Set of characters declared </param>
        public TextKit(char[] biasSet) {
            _biasSet = biasSet;
        }

        /// <summary>
        /// Increments <see cref="n"/> until the given <paramref name="charSet"/> contains
        /// a valuable character at n-th index. A character is valuable if it's not declared
        /// as bias.
        /// </summary>
        /// <param name="charSet">Set of characters to iterate</param>
        /// <param name="n">Index pointer</param>
        public void PlayForward(char[] charSet, ref int n) {
            while (n != charSet.Length && IsBias(charSet[n]))
                n++;
        }

        /// <summary>
        /// Returns TRUE if the punctuation mark at the specified index of the given <paramref name="charSet"/>
        /// indicates a special language construction like an URL, date or time instant.
        /// </summary>
        /// <param name="charSet">Character set</param>
        /// <param name="pmIndex">Punctuation mark index</param>
        /// <param name="lang">Language info</param>
        /// <returns>TRUE or FALSE</returns>
        public bool IsSpecialCon(char[] charSet, int pmIndex, ILangInfo lang) {
            if (IsUrl(charSet, pmIndex)) return true;
            if (IsDate(charSet, pmIndex, lang.DateSeparator)) return true;
            if (IsTime(charSet, pmIndex, lang.TimeSeparator)) return true;
            
            return false;
        }

        /// <summary>
        /// Returns TRUE if the punctuation <paramref name="mark"/> at the specified index of the given <paramref name="charSet"/>
        /// indicates a time.
        /// </summary>
        /// <param name="charSet">Character set</param>
        /// <param name="pmIndex">Punctuation mark index</param>
        /// <param name="mark">Punctuation mark</param>
        /// <returns>TRUE or FALSE</returns>
        private bool IsTime(char[] charSet, int pmIndex, char mark) {
            // Only the specified mark is relevant
            if (charSet[pmIndex] != mark) return false;

            int np1 = pmIndex + 1;
            // If we've reached the end of the stream, this is no date
            if (np1 == charSet.Length) return false;
            char m = charSet[np1];
            
            // The mark is followed by a bias token, this is no date
            if (IsBias(m)) return false;
            
            return true;
        }
        
        /// <summary>
        /// Returns TRUE if the punctuation <paramref name="mark"/> at the specified index of the given <paramref name="charSet"/>
        /// indicates a date.
        /// </summary>
        /// <param name="charSet">Character set</param>
        /// <param name="pmIndex">Punctuation mark index</param>
        /// <param name="mark">Punctuation mark</param>
        /// <returns>TRUE or FALSE</returns>
        private bool IsDate(char[] charSet, int pmIndex, char mark) {
            // Only the specified mark is relevant
            if (charSet[pmIndex] != mark) return false;

            int np1 = pmIndex + 1;
            
            // If we've reached the end of the stream, this is no date
            if (np1 == charSet.Length) return false;
            char m = charSet[np1];
            
            // The mark is followed by a bias token, this is no date
            if (IsBias(m)) return false;
            
            return true;
        }
        
        /// <summary>
        /// Returns TRUE if the punctuation mark at the specified index of the given <paramref name="charSet"/>
        /// indicates an URL.
        /// </summary>
        /// <param name="charSet">Character set</param>
        /// <param name="pmIndex">Punctuation mark index</param>
        /// <returns>TRUE or FALSE</returns>
        private bool IsUrl(char[] charSet, int pmIndex) {
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
        public bool IsBias(char c)
            => IsBias(_biasSet, c);

        /// <summary> Returns TRUE if the specified character is known as bias. </summary>
        /// <param name="biasSet">Set of characters declared as bias</param>
        /// <param name="c">Character to check</param>
        /// <returns>TRUE or FALSE</returns>
        public static bool IsBias(char[] biasSet, char c) 
            => Array.IndexOf(biasSet, c) != -1;
    }
}