using System;
using System.IO;

namespace xcite.io {
    /// <summary> Provides method extensions for operating with files. </summary>
    public static class FileUtil {
        /// <summary>
        /// Creates a file target by the given <paramref name="absolutePath"/> and any parent folder if desired.
        /// If the file already exists, nothing will happen.
        /// </summary>
        /// <param name="absolutePath">Absolute path of the file</param>
        /// <param name="includingParents">If TRUE any missing parent folder is created too</param>
        public static void CreateFile(string absolutePath, bool includingParents = true) {
            if (string.IsNullOrWhiteSpace(absolutePath)) throw new ArgumentException("Must not be NULL or empty", nameof(absolutePath));
            CreateFile(new FileInfo(absolutePath), includingParents);
        }

        /// <summary>
        /// Creates the file and any parent folder if desired. If the file already exists, nothing will happen.
        /// </summary>
        /// <param name="fileInfo">File reference</param>
        /// <param name="includingParents">If TRUE any missing parent folder is created too</param>
        public static void CreateFile(this FileInfo fileInfo, bool includingParents = true) {
            if (fileInfo == null) throw new ArgumentNullException(nameof(fileInfo));
            if (fileInfo.Exists) return;

            using (CreateFileStream(fileInfo, includingParents)) {
                // Nothing to do here
            }

            // Refresh to update the internal state
            fileInfo.Refresh();
        }

        /// <summary>
        /// Creates the file and any parent folder if desired and opens a file stream. 
        /// If the file already exists, nothing will happen.
        /// </summary>
        /// <param name="fileInfo">File reference</param>
        /// <param name="includingParents">If TRUE any missing parent folder is created too</param>
        /// <returns>File stream to the newly created file</returns>
        public static FileStream CreateFileStream(this FileInfo fileInfo, bool includingParents = true) {
            if (fileInfo == null) throw new ArgumentNullException(nameof(fileInfo));
            if (fileInfo.Exists) return fileInfo.Create();

            string fullName = fileInfo.FullName;
            if (string.IsNullOrWhiteSpace(fullName)) throw new InvalidOperationException($"Full name of the given file info is null or blank! file: '{fileInfo}'");
            if (includingParents)
                Directory.CreateDirectory(Path.GetDirectoryName(fullName));

            return fileInfo.Create();
        }

        /// <summary>
        /// Copies the file into the temp folder using the given <paramref name="fileName"/>. If <paramref name="overrideFile"/> 
        /// is false and the file already exists, an <see cref="IOException"/> is thrown. The default value of <paramref name="overrideFile"/> 
        /// is TRUE.
        /// </summary>
        /// <param name="fileInfo">Info of the file to copy</param>
        /// <param name="fileName">Name of the copied file</param>
        /// <param name="overrideFile">If TRUE an existing file is overriden</param>
        /// <returns><see cref="FileInfo"/> of the resulting file</returns>
        public static FileInfo CopyToTempPath(this FileInfo fileInfo, string fileName, bool overrideFile = true) {
            if (fileInfo == null) throw new ArgumentNullException(nameof(fileInfo));
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentException("Must not be NULL or empty", nameof(fileName));

            string targetFolder = Path.GetTempPath();
            string targetFileName = Path.Combine(targetFolder, fileName);

            File.Copy(fileInfo.FullName, targetFileName, overrideFile);
            return new FileInfo(targetFileName);
        }
    }
}