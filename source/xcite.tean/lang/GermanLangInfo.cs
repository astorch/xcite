using System.Text.RegularExpressions;

namespace xcite.tean.lang {
    /// <inheritdoc />
    public class GermanLangInfo : ILangInfo {
        /// <summary> Default instance </summary>
        public static readonly GermanLangInfo Instance = new GermanLangInfo();

        /// <inheritdoc />
        public virtual char[] PuncMarks { get; } = {'.', '!', '?', ':'};

        /// <inheritdoc />
        public virtual char DateSeparator { get; } = '.';

        /// <inheritdoc />
        public virtual char TimeSeparator { get; } = ':';

        /// <inheritdoc />
        public virtual char[] WordSeparator { get; } = {' ', ',', '.', '!', '?', ':'};

        /// <inheritdoc />
        public virtual string StripTimeExpression(string text, ITokenFactory tokenFactory) 
            => text;

        /// <inheritdoc />
        public virtual string StripDateExpression(string text, ITokenFactory tokenFactory) {
            // ReSharper disable once InconsistentNaming
            string onMatch(Match match) => tokenFactory.Consume(match.Value);

            // Replace full qualified dates
            text = Regex.Replace(text, 
                "((?<number>[0-9]{1,2}\\.\\s?(?<month>Januar|Februar|März|April|Mai|Juni|Juli|August|September|Oktober|November|Dezember)))",
                onMatch);

            // Replace digit only dates
            text = Regex.Replace(text, 
                "((?<day>[0-9]{1,2})\\.\\s?(?<month>[0-9]{1,2})\\.\\s?(?<year>[0-9]{1,4})?)", 
                onMatch);

            return text;
        }

        /// <inheritdoc />
        public virtual string StripAbbreviations(string text, ITokenFactory tokenFactory) {
            string[] abbrSet = GetAbbreviationSet();
            for (int i = -1; ++i != abbrSet.Length;) {
                string abbr = abbrSet[i];
                if (!text.Contains(abbr)) continue;
                string id = tokenFactory.Consume(abbr);
                text = text.Replace(abbr, id);
            }

            return text;
        }

        /// <summary> Returns a set of commonly known abbreviations. </summary>
        /// <returns>Set of abbreviations</returns>
        protected virtual string[] GetAbbreviationSet() 
            => Abbreviations;

        private static readonly string[] Abbreviations = {
            // titles
            "Dr.", "Hr.", "Fr.", "Prof.", "Dipl.", "Ing.",

            // Single short words
            "bspw.", "Bspw.",
            "bzw.", "Bzw.",
            "evtl.", "Evtl.",
            "ggf.", "Ggf.",
            "mtl.", "Mtl.",
            "mio.", "Mio.",
            "mrd.", "Mrd.",
            "rd.", "Rd.",
            "str.", "Str.",
            "usf.",
            "usw.",
            "zz.", "Zz.",
            
            // Multiple short words
            "i.A.", "i. A.",
            "i.H.v.", "i. H. v.",
            "o.g.", "o. g.",
            "u.a.m.", "u. a. m.",
            "u.a.", "u. a.",

            "z.Bsp.", "z. Bsp.",
            "z.B.", "z. B.",
            
            // Long words
            "inkl.",
            "zusätzl.", 
        };
    }
}