namespace xcite.csharp {
    /// <summary> Announces classes taking part of the serialization process of an object. </summary>
    public interface ICustomSerializer {
        /// <summary> Custom serialization mode </summary>
        ECustomSerializationMode CustomSerializationMode { get; }
    }
}