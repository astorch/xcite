namespace xcite.logging.streams {
    /// <summary> Inherits <see cref="AbstractStream"/> to print records onto the .NET trace environment. </summary>
    /// <seealso cref="System.Diagnostics.Debug"/>
    public class TraceStream : AbstractStream {
        
        /// <inheritdoc />
        protected override void OnDispose(bool disposing) {
            // Nothing to do here
        }

        /// <inheritdoc />
        protected override void Write(string value) 
            => System.Diagnostics.Trace.Write(value);
        
    }
}