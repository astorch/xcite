namespace xcite.logging.streams {
    /// <summary> Inherits <see cref="AbstractStream"/> to print records onto the .NET debug environment. </summary>
    /// <seealso cref="System.Diagnostics.Debug"/>
    public class DebugStream : AbstractStream {

        /// <inheritdoc />
        protected override void OnDispose(bool disposing) {
            // Nothing to do here
        }

        /// <inheritdoc />
        protected override void Write(string value) 
            => System.Diagnostics.Debug.Write(value);
        
    }
}