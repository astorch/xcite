using System;

namespace xcite.logging.streams {
    /// <summary> Implements <see cref="ILogStream"/> to print records onto the console. </summary>
    public class ConsoleStream : ILogStream {
        /// <inheritdoc />
        public virtual void Write(string value) {
            Console.Out.Write(value);
        }
    }
}