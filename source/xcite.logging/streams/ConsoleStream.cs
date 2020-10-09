using System;

namespace xcite.logging.streams {
    /// <summary> Inherits <see cref="AbstractStream"/> to print records onto the console. </summary>
    public class ConsoleStream : AbstractStream {
        /// <inheritdoc />
        public override void Dispose() {
            // Noting to do here
        }

        /// <inheritdoc />
        public override void Write(string value) 
            => Console.Out.Write(value);
    }
}