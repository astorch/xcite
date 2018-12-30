namespace xcite.logging {
    /// <summary> Defines common log patterns. </summary>
    public static class LogPatterns {
        public static readonly string Standard  = "%level %date(dd.MM.yyyy HH:mm:ss) [%thread] %name - %text%nl";
        public static readonly string StandardMilli = "%level %date(dd.MM.yyyy HH:mm:ss:fff) [%thread] %name - %text%nl";
    }
}