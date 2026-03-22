using System;
using System.Buffers;
using System.IO;
using System.Text;
using System.Threading;

namespace xcite.logging.streams;

/// <summary>
/// Inherits <see cref="AbstractStream"/> and writes into a specified file. Note, the file stream remains
/// as long open as the file stream instance exists. 
/// </summary>
public class FileStream : AbstractStream, FileStream._Bender {
    private static readonly Encoding _utf8Encoding = new UTF8Encoding(false);
    private AbstractStreamWriter _streamWriter;
    
    /// <inheritdoc />
    protected override void OnDispose(bool disposing) {
        ReleaseUnmanagedResources();
        _streamWriter?.Dispose();
        _streamWriter = null;
    }

    /// <summary> Name of the file the stream writes to. </summary>
    public string FileName { get; init; }

    /// <summary>
    /// Flag to determine whether the file should be appended or overwritten.
    /// Default is TRUE.
    /// </summary>
    public bool Append { get; init; } = true;

    /// <summary> File locking model. Default is <see cref="ELockingModel.Exclusive"/>. </summary>
    public ELockingModel LockingModel { get; init; } = ELockingModel.None;
        
    /// <summary> Flag to determine to create one log file per day. </summary>
    public bool DailyRolling { get; init; }

    /// <inheritdoc />
    protected override void OnWrite(in ReadOnlySpan<char> value, in LogData logData) {
        if (string.IsNullOrEmpty(FileName) || value.IsEmpty) return;
            
        AbstractStreamWriter streamWriter = GetStreamWriter();
        if (streamWriter is null) return;

        byte[] dataBuffer = ArrayPool<byte>.Shared.Rent(value.Length * 2);
        try {
            Span<byte> dataBufferSpan = dataBuffer.AsSpan();
            _utf8Encoding.GetBytes(value, dataBufferSpan);

            int dataLength = dataBufferSpan.Length;
            ReadOnlySpan<byte> dataSpan = dataBufferSpan[..dataLength];
            
            streamWriter.LockStream(dataLength);
            try {
                streamWriter.WriteToStream(in dataSpan);
            } finally {
                streamWriter.UnlockStream(dataLength);
            }
            
        } finally {
            ArrayPool<byte>.Shared.Return(dataBuffer);
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
            
        if (_streamWriter is not null) return _streamWriter;

        switch (LockingModel) {
            case ELockingModel.Exclusive:
                _streamWriter = new ExclusiveLockingStreamWriter();
                break;
            case ELockingModel.Minimal:
                _streamWriter = new MinimalLockingStreamWriter();
                break;
            default:
                _streamWriter = new NonLockingStreamWriter();
                break;
        }

        string fqFilePath = Path.GetFullPath(FileName);
        string fqFolderPath = Path.GetDirectoryName(fqFilePath);
        if (
            !string.IsNullOrEmpty(fqFolderPath)
            && !Directory.Exists(fqFolderPath)
        ) {
            Directory.CreateDirectory(fqFolderPath);
        }
            
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
        if (currentStream is null) return null;

        // When nothing has been written to the stream yet, there is nothing to check
        if (currentStream.LastWrite == default) return currentStream;

        // When the day hasn't changed yet, there is nothing to do
        if (currentStream.LastWrite.Day == DateTime.Today.Day) return currentStream;
            
        // Close stream
        DateTime lastWriteDay = currentStream.LastWrite;
        currentStream.Dispose();
            
        // Calculate new file name
        string year = lastWriteDay.Year.ToString();
        string month = lastWriteDay.Month.ToString("00");
        string day = lastWriteDay.Day.ToString("00");

        int extIdx = FileName.LastIndexOf(".", StringComparison.InvariantCultureIgnoreCase);
        string fnm = FileName[..extIdx];
        string ext = FileName[extIdx..];
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
    void _Bender.SetLastWrite(DateTime value)
        => _streamWriter?.LastWrite = value;

    /// <summary> Defines a locking model aware file stream writer.
    /// </summary>
    protected abstract class AbstractStreamWriter : IDisposable {
        private readonly Lock _writeLock = new();
        private bool _disposed;
        private System.IO.FileStream _fileStream;

        /// <inheritdoc />
        ~AbstractStreamWriter() {
            Dispose(false);
        }
        
        /// <inheritdoc />
        public void Dispose() {
            OnDispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary> Notifies the instance that is is being disposed. </summary>
        protected void Dispose(bool disposing) {
            if (_disposed) return;
            _disposed = true;
            OnDispose(disposing);
        }

        /// <summary> Is invoked when this instance is being disposed. </summary>
        protected virtual void OnDispose(bool disposing) {
            _fileStream?.Flush();
            _fileStream?.Dispose();
            _fileStream = null;
        }

        /// <summary> Instant of time this stream has been written to last. </summary>
        public DateTime LastWrite { get; internal set; }
        
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
        public virtual void LockStream(int size) 
            => OnLockStream(_fileStream, size);

        /// <summary> Writes the given <paramref name="data"/> to the underlying stream. </summary>
        public virtual void WriteToStream(in ReadOnlySpan<byte> data) {
            lock (_writeLock) {
                _fileStream.Write(data);
                // _fileStream.Flush(); // Do not immediately flush to reduce pressure to disk
                LastWrite = DateTime.Now;
            }
        }

        /// <summary>
        /// Notifies the instance to unlock the file stream so that other
        /// processes may access the file.
        /// </summary>
        public virtual void UnlockStream(int size) 
            => OnUnlockStream(_fileStream, size);

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
        protected abstract void OnLockStream(System.IO.FileStream fileStream, int size);

        /// <summary>
        /// Is invoked when <see cref="UnlockStream"/> is called. Sub-classes
        /// can add custom behavior to the stream.
        /// </summary>
        protected abstract void OnUnlockStream(System.IO.FileStream fileStream, int size);
    }

    /// <summary>
    /// Implementation of <see cref="AbstractStreamWriter"/> that does not acquire
    /// a lock for the log file. Other processes can always read the file.
    /// </summary>
    sealed class NonLockingStreamWriter : AbstractStreamWriter {
        
        /// <inheritdoc />
        protected override System.IO.FileStream OnInitStream(string fileName, FileMode fileMode) {
            return new System.IO.FileStream(fileName, fileMode, FileAccess.Write, FileShare.ReadWrite);
        }

        /// <inheritdoc />
        protected override void OnLockStream(System.IO.FileStream fileStream, int size) {
            // No lock
        }

        /// <inheritdoc />
        protected override void OnUnlockStream(System.IO.FileStream fileStream, int size) {
            // No unlock
        }
        
    }
        
    /// <summary>
    /// Implementation of <see cref="AbstractStreamWriter"/> that only acquires
    /// a lock for the log file in the moment of writing. Other processes can
    /// mostly read the log file.
    /// </summary>
    sealed class MinimalLockingStreamWriter : AbstractStreamWriter {
        
        /// <inheritdoc />
        protected override System.IO.FileStream OnInitStream(string fileName, FileMode fileMode) {
            return new System.IO.FileStream(fileName, fileMode, FileAccess.Write, FileShare.Read);
        }

        /// <inheritdoc />
        protected override void OnLockStream(System.IO.FileStream fileStream, int size) {
            // No additional lock required
        }

        /// <inheritdoc />
        protected override void OnUnlockStream(System.IO.FileStream fileStream, int size) {
            // No additional lock required
        }
        
    }

    /// <summary>
    /// Implementation of <see cref="AbstractStreamWriter"/> that acquires
    /// an exclusive lock for the log file. No other process can read or
    /// write to that file.
    /// </summary>
    sealed class ExclusiveLockingStreamWriter : AbstractStreamWriter {
        
        /// <inheritdoc />
        protected override System.IO.FileStream OnInitStream(string fileName, FileMode fileMode) {
            return new System.IO.FileStream(fileName, fileMode, FileAccess.Write, FileShare.None);
        }

        /// <inheritdoc />
        protected override void OnLockStream(System.IO.FileStream fileStream, int size) {
            // No additional lock required
        }

        /// <inheritdoc />
        protected override void OnUnlockStream(System.IO.FileStream fileStream, int size) {
            // No additional unlock required
        }
        
    }
        
    /// <summary> Interface for internal usage </summary>
    // ReSharper disable once InconsistentNaming
    internal interface _Bender {
        
        /// <summary> Sets the last write instant to the given <paramref name="value"/>. </summary>
        void SetLastWrite(DateTime value);
        
    }
    
}