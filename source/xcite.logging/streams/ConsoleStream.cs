using System;

namespace xcite.logging.streams {
    /// <summary> Inherits <see cref="AbstractStream"/> to print records onto the console. </summary>
    public class ConsoleStream : AbstractStream {
        
        /// <inheritdoc />
        protected override void OnDispose(bool disposing) {
            // Nothing to do here
        }

        /// <inheritdoc />
        protected override void Write(string value) 
            => Console.Out.Write(value);
        
    }
}