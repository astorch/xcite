namespace xcite.logging.streams {
    /// <summary> Inherits <see cref="AbstractStream"/> to print records onto the .NET trace environment. </summary>
    /// <seealso cref="System.Diagnostics.Debug"/>
    public class TraceStream : AbstractStream {
        /// <inheritdoc />
        public override void Dispose() {
            // Nothing to do here
        }

        /// <inheritdoc />
        public override void Write(string value) 
            => System.Diagnostics.Trace.Write(value);
    }
}