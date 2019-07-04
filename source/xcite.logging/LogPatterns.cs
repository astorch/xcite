namespace xcite.logging {
    /// <summary> Defines common log patterns. </summary>
    public static class LogPatterns {
        /// <summary> Standard log pattern </summary>
        public static readonly string Standard  = "%level %date(dd.MM.yyyy HH:mm:ss) [%thread] %name - %text%nl";
        
        /// <summary> Standard log pattern that adds milliseconds to the date information </summary>
        public static readonly string StandardMilli = "%level %date(dd.MM.yyyy HH:mm:ss:fff) [%thread] %name - %text%nl";
    }
}