using System;
using System.IO;
using System.Text;

namespace xcite.logging.streams {
    /// <summary>
    /// Inherits <see cref="AbstractStream"/> and writes into a specified file. Note, the file stream remains
    /// as long open as the file stream instance exists. 
    /// </summary>
    public class FileStream : AbstractStream, FileStream._Bender {
        private AbstractStreamWriter _streamWriter;

        /// <inheritdoc />
        protected override void OnDispose(bool disposing) {
            ReleaseUnmanagedResources();
            _streamWriter?.Dispose();
            _streamWriter = null;
        }

        /// <summary> Name of the file the stream writes to. </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Flag to determine whether the file should be appended or overwritten.
        /// Default is TRUE.
        /// </summary>
        public bool Append { get; set; } = true;

        /// <summary> File locking model. Default is <see cref="ELockingModel.Exclusive"/>. </summary>
        public ELockingModel LockingModel { get; set; } = ELockingModel.None;
        
        /// <summary> Flag to determine to create one log file per day. </summary>
        public bool DailyRolling { get; set; }

        /// <inheritdoc />
        protected override void Write(string value) {
            if (string.IsNullOrEmpty(FileName) || string.IsNullOrEmpty(value)) return;
            
            AbstractStreamWriter streamWriter = GetStreamWriter();
            if (streamWriter == null) return;
            
            byte[] data = Encoding.UTF8.GetBytes(value);
            
            streamWriter.LockStream();
            try {
                streamWriter.WriteToStream(data);
            } finally {
                streamWriter.UnlockStream();
            }
        }

        /// <summary>
        /// Returns a stream writer to write to.
        /// The instance is either newly created or an already existing
        /// one is returned.
        /// </summary>
        protected virtual AbstractStreamWriter GetStreamWriter() {
            if (DailyRolling) {
                _streamWriter = UpdateFileStream(_streamWriter);
            }
            
            if (_streamWriter != null) return _streamWriter;

            if (LockingModel == ELockingModel.Exclusive)
                _streamWriter = new ExclusiveLockingStreamWriter();
            else if (LockingModel == ELockingModel.Minimal)
                _streamWriter = new MinimalLockingStreamWriter();
            else
                _streamWriter = new NonLockingStreamWriter();

            string fqFilePath = Path.GetFullPath(FileName);
            string fqFolderPath = Path.GetDirectoryName(fqFilePath);
            if (!string.IsNullOrEmpty(fqFolderPath) && !Directory.Exists(fqFolderPath))
                Directory.CreateDirectory(fqFolderPath);
            
            _streamWriter.InitStream(FileName, Append);
            return _streamWriter;
        }

        /// <summary>
        /// Returns an updated file stream when last write date of the <paramref name="currentStream"/>
        /// has another day than today. In this case, the <paramref name="currentStream"/> is closed
        /// and its file is renamed. Finally, NULL is returned.
        /// When the <paramref name="currentStream"/> does not require an update, it is returned
        /// unmodified.
        /// </summary>
        protected virtual AbstractStreamWriter UpdateFileStream(AbstractStreamWriter currentStream) {
            // No stream, nothing to check
            if (currentStream == null) return null;

            // When nothing has been written to the stream yet, there is nothing to check
            if (currentStream.lastWrite == default) return currentStream;

            // When the day hasn't changed yet, there is nothing to do
            if (currentStream.lastWrite.Day == DateTime.Today.Day) return currentStream;
            
            // Close stream
            DateTime lastWriteDay = currentStream.lastWrite;
            currentStream.Dispose();
            
            // Calculate new file name
            string year = lastWriteDay.Year.ToString();
            string month = lastWriteDay.Month.ToString("00");
            string day = lastWriteDay.Day.ToString("00");

            int extIdx = FileName.LastIndexOf(".", StringComparison.InvariantCultureIgnoreCase);
            string fnm = FileName.Substring(0, extIdx);
            string ext = FileName.Substring(extIdx);
            string mvFn = $"{fnm}-{year}-{month}-{day}{ext}";
            
            // Renamed existing file
            File.Move(FileName, mvFn);
            
            // Drop existing file stream
            return null;
        }

        /// <summary>
        /// Is invoked when the instance is being disposed. Clients may added some
        /// addition disposing steps here.
        /// </summary>
        protected virtual void ReleaseUnmanagedResources() {
            // Clients may override
        }

        /// <inheritdoc />
        void _Bender.SetLastWrite(DateTime value) {
            if (_streamWriter == null) return;
            _streamWriter.lastWrite = value;
        }

        /// <summary> Defines a locking model aware file stream writer.
        /// </summary>
        protected abstract class AbstractStreamWriter : IDisposable {
            private System.IO.FileStream _fileStream;
            public DateTime lastWrite;
            
            /// <summary>
            /// Initializes a file stream with the specified <paramref name="fileName"/>.
            /// If <paramref name="append"/> is FALSE, a new file is always created and an
            /// existing file is overridden.. Otherwise an existing file is extended. If
            /// no file exists, a new one is always created. 
            /// </summary>
            public virtual void InitStream(string fileName, bool append) {
                FileMode fileMode = append ? FileMode.OpenOrCreate : FileMode.Create;
                _fileStream = OnInitStream(fileName, fileMode);
                _fileStream.Seek(0, SeekOrigin.End);
            }

            /// <summary>
            /// Notifies the instance to lock the file stream so that no other
            /// process can access the file.
            /// </summary>
            public virtual void LockStream() 
                => OnLockStream(_fileStream);

            /// <summary> Writes the given <paramref name="data"/> to the underlying stream. </summary>
            public virtual void WriteToStream(byte[] data) {
                _fileStream.Write(data, 0, data.Length);
                _fileStream.Flush();
                lastWrite = DateTime.Today;
            }

            /// <summary>
            /// Notifies the instance to unlock the file stream so that other
            /// processes may access the file.
            /// </summary>
            public virtual void UnlockStream() 
                => OnUnlockStream(_fileStream);

            /// <inheritdoc />
            public virtual void Dispose() {
                _fileStream?.Dispose();
                _fileStream = null;
            }

            /// <summary>
            /// Is invoked when <see cref="InitStream"/> is called. Sub-classes
            /// can add custom behavior to the stream.
            /// </summary>
            /// <returns></returns>
            protected abstract System.IO.FileStream OnInitStream(string fileName, FileMode fileMode);

            /// <summary>
            /// Is invoked when <see cref="LockStream"/> is called. Sub-classes
            /// can add custom behavior to the stream.
            /// </summary>
            protected abstract void OnLockStream(System.IO.FileStream fileStream);

            /// <summary>
            /// Is invoked when <see cref="UnlockStream"/> is called. Sub-classes
            /// can add custom behavior to the stream.
            /// </summary>
            protected abstract void OnUnlockStream(System.IO.FileStream fileStream);
        }

        /// <summary>
        /// Implementation of <see cref="AbstractStreamWriter"/> that does not acquire
        /// a lock for the log file. Other processes can always read the file.
        /// </summary>
        private class NonLockingStreamWriter : AbstractStreamWriter {
            /// <inheritdoc />
            protected override System.IO.FileStream OnInitStream(string fileName, FileMode fileMode) {
                return new System.IO.FileStream(fileName, fileMode, FileAccess.Write, FileShare.Read);
            }

            /// <inheritdoc />
            protected override void OnLockStream(System.IO.FileStream fileStream) {
                // No lock
            }

            /// <inheritdoc />
            protected override void OnUnlockStream(System.IO.FileStream fileStream) {
                // No unlock
            }
        }
        
        /// <summary>
        /// Implementation of <see cref="AbstractStreamWriter"/> that only acquires
        /// a lock for the log file in the moment of writing. Other processes can
        /// mostly read the log file.
        /// </summary>
        class MinimalLockingStreamWriter : AbstractStreamWriter {
            /// <inheritdoc />
            protected override System.IO.FileStream OnInitStream(string fileName, FileMode fileMode) {
                return new System.IO.FileStream(fileName, fileMode, FileAccess.Write, FileShare.Read);
            }

            /// <inheritdoc />
            protected override void OnLockStream(System.IO.FileStream fileStream) {
                fileStream.Lock(0, fileStream.Length);
            }

            /// <inheritdoc />
            protected override void OnUnlockStream(System.IO.FileStream fileStream) {
                fileStream.Unlock(0, fileStream.Length);
            }
        }

        /// <summary>
        /// Implementation of <see cref="AbstractStreamWriter"/> that acquires
        /// an exclusive lock for the log file. No other process can read or
        /// write to that file.
        /// </summary>
        class ExclusiveLockingStreamWriter : AbstractStreamWriter {
            /// <inheritdoc />
            protected override System.IO.FileStream OnInitStream(string fileName, FileMode fileMode) {
                return new System.IO.FileStream(fileName, fileMode, FileAccess.Write, FileShare.None);
            }

            /// <inheritdoc />
            protected override void OnLockStream(System.IO.FileStream fileStream) {
                // No additional lock required
            }

            /// <inheritdoc />
            protected override void OnUnlockStream(System.IO.FileStream fileStream) {
                // No additional unlock required
            }
        }
        
        /// <summary> Interface for internal usage </summary>
        // ReSharper disable once InconsistentNaming
        public interface _Bender {
            /// <summary> Sets the last write instant to the given <paramref name="value"/>. </summary>
            void SetLastWrite(DateTime value);
        }
    }
}