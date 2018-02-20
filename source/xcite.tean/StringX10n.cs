using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace xcite.tean {
    /// <summary> Provides string extension methods to gain access to the TEAN API. </summary>
    public static class StringX10N {
        /// <summary> Returns a sentence iterator that operates with the current UI culture. </summary>
        /// <param name="text">String to decompose</param>
        /// <returns>A new instance of <see cref="SentenceIterator"/></returns>
        public static SentenceIterator GetSentenceIterator(this string text) {
            return GetSentenceIterator(text, CultureInfo.CurrentUICulture);
        }

        /// <summary> Returns a sentence iterator that operates with the specified culture. </summary>
        /// <param name="text">String to decompose</param>
        /// <param name="cultureNfo">Culture to use for decomposing</param>
        /// <returns>A new instance of <see cref="SentenceIterator"/></returns>
        public static SentenceIterator GetSentenceIterator(this string text, CultureInfo cultureNfo) {
            return new SentenceIterator(text, LangInfo.Get(cultureNfo));
        }

        /// <summary> Returns an enumerable sentence sequence created with the current UI culture. </summary>
        /// <param name="text">String to decompose</param>
        /// <returns>Sequence of sentences</returns>
        public static IEnumerable<Sentence> Sentences(this string text) {
            return Sentences(text, CultureInfo.CurrentUICulture);
        }

        /// <summary> Returns an enumerable sentence sequence created with the specified culture. </summary>
        /// <param name="text">String to decompose</param>
        /// <param name="cultureNfo">Culture to use for decomposing</param>
        /// <returns>Sequence of sentences</returns>
        public static IEnumerable<Sentence> Sentences(this string text, CultureInfo cultureNfo) {
            using (SentenceIterator itr = GetSentenceIterator(text, cultureNfo)) {
                while (itr.MoveNext()) {
                    yield return itr.Current;
                }
            }
        }

        /// <summary> Returns all sentences of the given text as array using the current UI culture. </summary>
        /// <param name="text">String to decompose</param>
        /// <returns>Array of sentences</returns>
        public static Sentence[] ToSentences(this string text) {
            return ToSentences(text, CultureInfo.CurrentUICulture);
        }

        /// <summary> Returns all sentences of the given text as array using the specified culture. </summary>
        /// <param name="text">String to decompose</param>
        /// <param name="cultureNfo">Culture used for decomposing</param>
        /// <returns>Array of sentences</returns>
        public static Sentence[] ToSentences(this string text, CultureInfo cultureNfo) {
            return Sentences(text, cultureNfo).ToArray();
        }
    }
}