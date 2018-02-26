namespace xcite.csharp {
    /// <summary> Enumeration of custom serialization modes. </summary>
    public enum ECustomSerializationMode {
        /// <summary> This class fully handles the serialization itself. </summary>
        Full,
        
        /// <summary> This class only performs the serialization of some additional values. </summary>
        Additional
    }
}