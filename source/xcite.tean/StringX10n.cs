using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace xcite.tean {
    /// <summary> Provides string extension methods to gain access to the TEAN API. </summary>
    public static class StringX10N {
        /// <summary> Returns a sentence iterator that operates with the current UI culture. </summary>
        /// <param name="text">String to decompose</param>
        /// <returns>A new instance of <see cref="SentenceIterator"/></returns>
        public static SentenceIterator GetSentenceIterator(this string text) 
            => GetSentenceIterator(text, CultureInfo.CurrentUICulture);

        /// <summary> Returns a sentence iterator that operates with the specified culture. </summary>
        /// <param name="text">String to decompose</param>
        /// <param name="cultureNfo">Culture to use for decomposing</param>
        /// <returns>A new instance of <see cref="SentenceIterator"/></returns>
        public static SentenceIterator GetSentenceIterator(this string text, CultureInfo cultureNfo) 
            => new SentenceIterator(text, LangInfo.Get(cultureNfo));

        /// <summary> Returns an enumerable sentence sequence created with the current UI culture. </summary>
        /// <param name="text">String to decompose</param>
        /// <returns>Sequence of sentences</returns>
        public static IEnumerable<Sentence> Sentences(this string text) 
            => Sentences(text, CultureInfo.CurrentUICulture);

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
        public static Sentence[] ToSentences(this string text) 
            => ToSentences(text, CultureInfo.CurrentUICulture);

        /// <summary> Returns all sentences of the given text as array using the specified culture. </summary>
        /// <param name="text">String to decompose</param>
        /// <param name="cultureNfo">Culture used for decomposing</param>
        /// <returns>Array of sentences</returns>
        public static Sentence[] ToSentences(this string text, CultureInfo cultureNfo) 
            => Sentences(text, cultureNfo).ToArray();

        /// <summary> Returns a word iterator that operates with the current UI culture. </summary>
        /// <param name="text">String to decompose</param>
        /// <returns>A new instance of <see cref="WordIterator"/></returns>
        public static WordIterator GetWordIterator(this string text) 
            => GetWordIterator(text, CultureInfo.CurrentUICulture);

        /// <summary> Returns a word iterator that operates with the specified culture. </summary>
        /// <param name="text">String to decompose</param>
        /// <param name="cultureNfo">Culture to use for decomposing</param>
        /// <returns>A new instance of <see cref="WordIterator"/></returns>
        public static WordIterator GetWordIterator(this string text, CultureInfo cultureNfo) 
            => new WordIterator(text, LangInfo.Get(cultureNfo));

        /// <summary> Returns an enumerable word sequence created with the current UI culture. </summary>
        /// <param name="text">String to decompose</param>
        /// <returns>Sequence of words</returns>
        public static IEnumerable<Word> Words(this string text) 
            => Words(text, CultureInfo.CurrentUICulture);

        /// <summary> Returns an enumerable word sequence created with the specified culture. </summary>
        /// <param name="text">String to decompose</param>
        /// <param name="cultureNfo">Culture to use for decomposing</param>
        /// <returns>Sequence of words</returns>
        public static IEnumerable<Word> Words(this string text, CultureInfo cultureNfo) {
            using (WordIterator itr = GetWordIterator(text, cultureNfo)) {
                while (itr.MoveNext()) {
                    yield return itr.Current;
                }
            }
        }

        /// <summary> Returns all words of the given text as array using the current UI culture. </summary>
        /// <param name="text">String to decompose</param>
        /// <returns>Array of words</returns>
        public static Word[] ToWords(this string text) 
            => ToWords(text, CultureInfo.CurrentUICulture);

        /// <summary> Returns all words of the given text as array using the specified culture. </summary>
        /// <param name="text">String to decompose</param>
        /// <param name="cultureNfo">Culture used for decomposing</param>
        /// <returns>Array of words</returns>
        public static Word[] ToWords(this string text, CultureInfo cultureNfo) 
            => Words(text, cultureNfo).ToArray();
    }
}