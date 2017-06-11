using System.IO;
using System.IO.Compression;

namespace xcite.io {
    /// <summary>
    /// Provides methods to reduce and expand byte arrays using a <see cref="GZipStream"/>.
    /// </summary>
    public static class ByteCompressor {
        /// <summary>
        /// Applies a <see cref="GZipStream"/> to the given <paramref name="rawData"/> byte array to reduce its size.
        /// </summary>
        /// <param name="rawData">Raw data array</param>
        /// <returns>Compressed data array</returns>
        public static byte[] Compress(this byte[] rawData) {
            byte[] compressesData;
            using (MemoryStream outputStream = new MemoryStream()) {
                using (GZipStream zip = new GZipStream(outputStream, CompressionMode.Compress)) {
                    zip.Write(rawData, 0, rawData.Length);
                }
                //Dont get the MemoryStream data before the GZipStream is closed
                //since it doesn’t yet contain complete compressed data.
                //GZipStream writes additional data including footer information when its been disposed
                compressesData = outputStream.ToArray();
            }

            return compressesData;
        }

        /// <summary>
        /// Applies a <see cref="GZipStream"/> to the given <paramref name="compressedData"/> byte array to expand it (de-compress)
        /// to its original size.
        /// </summary>
        /// <param name="compressedData">Compressed data array</param>
        /// <returns>De-compressed data array</returns>
        public static byte[] Decompress(this byte[] compressedData) {
            byte[] decompressedData;
            using (MemoryStream outputStream = new MemoryStream()) {
                using (MemoryStream inputStream = new MemoryStream(compressedData)) {
                    using (GZipStream zip = new GZipStream(inputStream, CompressionMode.Decompress)) {
                        zip.CopyTo(outputStream);
                    }
                }
                decompressedData = outputStream.ToArray();
            }

            return decompressedData;
        }
    }
}
