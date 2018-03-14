using System;
using System.Collections.Generic;

namespace xcite.tean {
    /// <summary> Detects sentences within a text based on a specified language. </summary>
    public class SentenceDetector {
        private readonly ILangInfo _lang;

        /// <summary>
        /// Initializes a new instance that operates with the specified <paramref name="lang"/>.
        /// </summary>
        /// <param name="lang">Language information</param>
        public SentenceDetector(ILangInfo lang) {
            _lang = lang ?? throw new ArgumentNullException(nameof(lang));
        }

        /// <summary> Returns a set of detected sentences for the specified <paramref name="input"/>. </summary>
        /// <param name="input">Text to process</param>
        /// <returns>Set of sentences</returns>
        public IEnumerable<Sentence> Detect(string input) {
            if (string.IsNullOrEmpty(input)) yield break;

            Dictionary<string, string> tokens = new Dictionary<string, string>(100);
            TokenFactory tokenFactory = new TokenFactory(tokens);
            string text = input;
            
            // First, we strip time expression
            text = _lang.StripTimeExpression(text, tokenFactory);

            // Next, we strip date expressions
            text = _lang.StripDateExpression(text, tokenFactory);

            // Then we strip abbreviations
            text = _lang.StripAbbreviations(text, tokenFactory);

            // Detect sentences and reverse any replacement
            int shift = 0;
            using (SentenceIterator itr = new SentenceIterator(text, _lang)) {
                while (itr.MoveNext()) {
                    Sentence sentence = itr.Current;
                    string sentTxt = sentence.Text;
                    int sentLgth = sentTxt.Length;
                    int sentBgn = sentence.Begin;

                    string rplTxt = sentTxt;
                    using (Dictionary<string, string>.Enumerator tokItr = tokens.GetEnumerator()) {
                        while (tokItr.MoveNext()) {
                            var entry = tokItr.Current;
                            string token = entry.Key;
                            string expr = entry.Value;
                            rplTxt = rplTxt.Replace(token, expr);
                        }
                    }

                    int rplLgth = rplTxt.Length;
                    int rplDelta = rplLgth - sentLgth;
                    int rplBgn = sentBgn + shift;
                    yield return new Sentence(rplTxt, rplBgn);
                    shift += rplDelta;
                }
            }
        }

        /// <summary> Anonymous implementation of <see cref="ITokenFactory"/>. </summary>
        class TokenFactory : ITokenFactory {
            private readonly IDictionary<string, string> _tokenDictionary;

            /// <inheritdoc />
            public TokenFactory(IDictionary<string, string> tokenDictionary) {
                _tokenDictionary = tokenDictionary;
            }

            /// <summary> Counter value </summary>
            public int Counter { get; set; } = -1;

            /// <summary> Token id </summary>
            public string TokenId { get; set; } = "default";

            /// <inheritdoc />
            public string Consume(string value) {
                string id = $"{{{TokenId}_{++Counter}}}";
                _tokenDictionary.Add(id, value);
                return id;
            }
        }
    }
}