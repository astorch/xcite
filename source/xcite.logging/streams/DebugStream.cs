namespace xcite.logging.streams {
    /// <summary> Implements <see cref="ILogStream"/> to print records onto the .NET debug environment. </summary>
    /// <seealso cref="System.Diagnostics.Debug"/>
    public class DebugStream : ILogStream {
        /// <inheritdoc />
        public virtual void Dispose() {
            // Nothing to do here
        }

        /// <inheritdoc />
        public virtual void Write(string value) 
            => System.Diagnostics.Debug.Write(value);
    }
}