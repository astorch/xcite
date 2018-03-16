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
                match => {
                    string input = text;
                    string value = match.Value;
                    int fst = match.Index;
                    int lst = fst + value.Length - 1;
                    char t = input[lst];
                    bool endedByWs = t == ' ';

                    if (endedByWs)
                        t = input[--lst];

                    // Default behaviour (standard case)
                    if (t != '.') return onMatch(match);

                    // Look-ahead
                    // Get next valueable character
                    int n = match.Index + match.Length;
                    while (n != input.Length && !char.IsLetter(input[n])) n++;

                    char c = input[n];

                    // The date is likely(!) within a sentence and not at the end (ISSUE see below)
                    if (!char.IsUpper(c)) return onMatch(match);

                    string coreValue = input.Substring(fst, lst - fst);
                    return tokenFactory.Consume(coreValue) + "." + (endedByWs ? " " : string.Empty);
                });

            /** ISSUE hint
             * The implemented algorithm is correct for sentence (1), but not for (2).
             * (1) Bitte überweisen Sie den fälligen Betrag bis zum 28.02. Wir bestätigen Ihnen umgehend den Eingang.
             * (2) Es gibt lediglich am 28.02. Termine. Bitte suchen Sie sich einen aus.
             */

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

        // see https://www.bundesanzeiger-verlag.de/fileadmin/Fachverlag/Autorenservice/Handout_Abkuerzungen_Z-I_9.9.15.pdf
        private static readonly string[] Abbreviations = {
            // titles
            "Dr.", "Hr.", "Fr.", "Prof.", "Dipl.", "Ing.",

            // Single short words
            "Az.",
            "Bd.",
            "bspw.", "Bspw.",
            "bzw.", "Bzw.",
            "bzgl.", "Bzgl.",
            "ca.", "Ca.",
            "ders.", "Ders.",
            "dgl.", "Dgl.",
            "dt.", "Dt.",
            "etc.",
            "evtl.", "Evtl.",
            " f.", " ff.", // WS
            "Fn.",
            "gem.", "Gem.",
            "ggf.", "Ggf.",
            "mtl.", "Mtl.",
            "mio.", "Mio.",
            "mrd.", "Mrd.",
            "MwSt.", "USt.",
            "nr.", "Nr.",
            "pp.",
            "rd.", "Rd.",
            "str.", "Str.",
            "S.", " s.", // WS
            "usf.",
            "usw.",
            "uvm.", "u.v.m.", "u. v. m.",
            "vgl.",
            "vs.",
            "zzt.", "Zzt.", "zz.", "Zz.",
            "zzgl.", "Zggl.",
            
            // Multiple short words
            "a.A.", "a. A.",
            "a.a.O.", "a. a. O.",
            "a.D.", "a. D.",
            "a.E.", "a. E.",
            "a.F.", "a. F.",
            "d.h.", "d. h.",
            "e.V.", "e. V.",
            "i.A.", "i. A.",
            "i.d.F.", "i. d. F.",
            "i.d.R.", "i. d. R.",
            "i.H.v.", "i. H. v.",
            "i.d.S.", "i. d. S.",
            "i.E.", "i. E.",
            "i.S.d.", "i. S. d.",
            "i.S.v.", "i. S. v.",
            "i.ü.", "i. ü.", "i.Ü.", "i. Ü.",
            "i.V.", "i. V.",
            "m.E.", "m. E.",
            "max.", "Max.",
            "min.", "Min.",
            "n.F.", "n. F.",
            "o.a.", "o. a.",
            "o.Ä.", "o. Ä.", "o.ä.", "o. ä.",
            "o.g.", "o. g.",
            "p.a.", "p. a.",
            "s.a.", "s. a.",
            "s.o.", "s. o.",
            "s.u.", "s. u.",
            "u.a.m.", "u. a. m.",
            "u.a.", "u. a.",
            "u.Ä.", "u. Ä.", "u.ä.", "u. ä.",
            "u.E.", "e. E.",
            "u.U.", "u. U.",
            "v.a.", "v. a.",
            "v.H.", "v. H.",
            "z.Bsp.", "z. Bsp.",
            "z.B.", "z. B.",
            "Bsp.", // due to replacement issue
            "z.Hd.", "z. Hd.",
            "z.T.", "z. T.",
            
            // Long words
            "Abschn.",
            "Abb.",
            "Abs.",
            "Alt.",
            "Anl.",
            "Anm.",
            "Art.",
            "Aufl.",
            "Beschl.v.", "Beschl. v.",
            "grds.", "grundsätzl.",
            "Hrsg.",
            "inkl.",
            "insb.",
            "Pos.",
            "Rspr.",
            "sog.", "Sog.",
            "Tab.",
            "Tel.",
            "Tsd.",
            "Ziff.",
            "zit.",
            "zusätzl.", 
        };
    }
}