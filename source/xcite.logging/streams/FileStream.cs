using System;
using System.IO;
using System.Text;

namespace xcite.logging.streams {
    /// <summary>
    /// Implements <see cref="ILogStream"/> that writes into a specified file. Note, the file stream remains
    /// as long open as the file stream instance exists. 
    /// </summary>
    public class FileStream : ILogStream, IDisposable {
        private System.IO.FileStream _fileStream;

        /// <inheritdoc />
        ~FileStream() {
            Dispose(false);
        }

        /// <inheritdoc />
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary> Name of the file the stream writes to. </summary>
        public string FileName { get; set; }
        
        /// <summary> Flag to determine wheter the file should be appended or overwritten. </summary>
        public bool Append { get; set; }

        /// <summary> File locking model. Default is <see cref="ELockingModel.Exclusive"/>. </summary>
        public ELockingModel LockingModel { get; set; } = ELockingModel.Exclusive;

        /// <inheritdoc />
        public virtual void Write(string value) {
            if (string.IsNullOrEmpty(FileName)) return;

            System.IO.FileStream fileStream = GetFileStream();
            if (fileStream == null) return;

            byte[] data = Encoding.UTF8.GetBytes(value);
            fileStream.Write(data, 0, data.Length);
            fileStream.Flush();
        }

        /// <summary>
        /// Returns the already opened file stream or creates a new one. If the stream
        /// cannot be opened, NULL is returned.
        /// </summary>
        protected virtual System.IO.FileStream GetFileStream() {
            if (_fileStream != null) return _fileStream;

            try {
                string fullPath = Path.GetFullPath(FileName);
                _fileStream = Append ? File.OpenWrite(fullPath) : File.Create(fullPath);
                return _fileStream;
            } catch (Exception) {
                return null;
            }
        }

        /// <summary>
        /// Is invoked when the instance is being disposed. Clients may added some
        /// addition disposing steps here.
        /// </summary>
        protected virtual void ReleaseUnmanagedResources() {
            // Clients may override
        }

        /// <summary> Is invoked when instance is being disposed. </summary>
        private void Dispose(bool disposing) {
            ReleaseUnmanagedResources();
            _fileStream?.Dispose();
            _fileStream = null;
        }
    }
}