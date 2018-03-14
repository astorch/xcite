using System.Globalization;
using xcite.tean.lang;

namespace xcite.tean {
    /// <summary> Provides access to various language information. </summary>
    public static class LangInfo {
        /// <summary>
        /// Returns the <see cref="ILangInfo"/> that is associated with the given <paramref name="cultureNfo"/>.
        /// </summary>
        /// <param name="cultureNfo">Culture info</param>
        /// <returns>Associated <see cref="ILangInfo"/></returns>
        public static ILangInfo Get(CultureInfo cultureNfo) {
            return GermanLangInfo.Instance;
        }
    }
}