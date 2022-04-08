namespace xcite.tdn {
    /// <summary> Describes elementary TDN element types. </summary>
    public enum ETdnDataType {
        /// <summary> The data type is not known. </summary>
        Unknown,

        /// <summary> 64 bit integer (long) </summary>
        Int64,

        /// <summary> Double </summary>
        Double,

        /// <summary> Decimal </summary>
        Decimal,

        /// <summary> String </summary>
        String,

        /// <summary> Boolean </summary>
        Bool,

        /// <summary> NULL </summary>
        Null
    }
}