namespace xcite.logging {
    /// <summary> Describes a log stream. </summary>
    public interface ILogStream {
        /// <summary> Writes the specified <paramref name="value"/> into the stream. </summary>
        void Write(string value);
    }
}