namespace xcite.tdn {
    /// <summary> Describes TDN element types. </summary>
    public enum ETdnElementType {
        /// <summary> The element type is not known. </summary>
        Unknown,

        /// <summary> Elementary element type like integer, decimal etc. </summary>
        Elementary,

        /// <summary> Set of elements </summary>
        Array,

        /// <summary> The element has custom (TDN) properties. </summary>
        Structured
    }
}