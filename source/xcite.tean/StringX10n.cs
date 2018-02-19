using System.Collections.Generic;
using System.Linq;

namespace xcite.tean {
    public static class StringX10N {
        public static SentenceIterator GetSentenceIterator(this string text) {
            return GetSentenceIterator(text, LangInfo.German);
        }

        public static SentenceIterator GetSentenceIterator(this string text, ILangInfo lang) {
            return new SentenceIterator(text, lang);
        }

        public static Sentence[] ToSentences(this string text) {
            return ToSentences(text, LangInfo.German);
        }

        public static Sentence[] ToSentences(this string text, ILangInfo lang) {
            LinkedList<Sentence> set = new LinkedList<Sentence>();
            using (SentenceIterator itr = GetSentenceIterator(text, lang)) {
                while (itr.MoveNext()) {
                    Sentence sentence = itr.Current;
                    set.AddLast(sentence);
                }
            }
            return set.ToArray();
        }
    }
}