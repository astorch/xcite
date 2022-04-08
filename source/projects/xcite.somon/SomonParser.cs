using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace xcite.somon {
    /// <summary> Provides methods to parse a SOMON file. </summary>
    public class SomonParser {
        private readonly char[] _buffer;

        /// <summary> Initializes the new instance with a default buffer size. </summary>
        public SomonParser() : this(512) {
            // Nothing to do here
        }
        
        /// <summary>
        /// Initializes the new instance with the specified <paramref name="bufferSize"/>.
        /// </summary>
        /// <param name="bufferSize">Buffer size</param>
        public SomonParser(int bufferSize) {
            _buffer = new char[bufferSize];
        }
        
        /// <summary> Parses the given <paramref name="fileContent"/>. </summary>
        /// <param name="fileContent">File content to parse</param>
        /// <returns>Resulting SOMON object</returns>
        public virtual SomonObject Parse(string fileContent) {
            using (StringReader strReader = new StringReader(fileContent)) {
                return Parse(strReader);
            }
        }

        /// <summary> Parses the file referenced by the specified <paramref name="filePath"/>. </summary>
        /// <param name="filePath">Path to the file to parse</param>
        /// <returns>Resulting SOMON object</returns>
        public virtual SomonObject ParseFile(string filePath) {
            using (FileStream fileStream = File.OpenRead(filePath)) {
                using (StreamReader streamReader = new StreamReader(fileStream)) {
                    return Parse(streamReader);
                }
            }
        }

        /// <summary> Parses the content of a stream provided by the given <paramref name="reader"/>. </summary>
        /// <param name="reader">Stream reader</param>
        /// <returns>Resulting SOMON object</returns>
        public virtual SomonObject Parse(TextReader reader) {
            ICollection<SomonProperty> propertySet = new LinkedList<SomonProperty>();
            
            while (reader.Peek() != -1) {
                SomonProperty somonProperty = ParseProperty(reader, _buffer);
                propertySet.Add(somonProperty);
            }
            
            return new SomonObject(propertySet.ToArray());
        }

        /// <summary> Parses a SOMON property from a stream using the given <paramref name="reader"/>. </summary>
        /// <param name="reader">Read to consume a data stream</param>
        /// <param name="buffer">Buffer to store characters</param>
        /// <returns>Resolved property</returns>
        /// <exception cref="SomonException">When the format is invalid</exception>
        private static SomonProperty ParseProperty(TextReader reader, char[] buffer) {
            // Read property kind
            int wordBeginIdx = ReadUntilWordBegin(reader, buffer);
            if (wordBeginIdx == -1) throw new SomonException("Invalid property kind declaration (begin).");

            char frstLetter = buffer[wordBeginIdx];
            
            int wordEndIdx = ReadUntilWordEnd(reader, buffer);
            if (wordEndIdx == -1) throw new SomonException("Invalid property kind declaration (end).");
            
            // Shift buffer to insert the first letter
            Array.Copy(buffer, 0, buffer, 1, wordEndIdx);
            buffer[0] = frstLetter;
            
            string propertyKind = new string(buffer, 0, wordEndIdx + 1);

            // Read until left curly brace
            int lcbIdx = ReadUntilChar('{', reader, buffer);
            if (lcbIdx == -1) throw new SomonException("Invalid SOMON format. Missing left curly backet.");

            // Parse optional id
            string propertyId = ParsePropertyId(buffer, lcbIdx); 
            
            // Parse fields
            SomonField[] fields = ParseFields(reader, buffer);
            
            return new SomonProperty(propertyKind, propertyId, fields);
        }

        /// <summary> Parses each SOMON field from a stream using the given <paramref name="reader"/>. </summary>
        /// <param name="reader">Read to consume a data stream</param>
        /// <param name="buffer">Buffer to store characters</param>
        /// <returns>Set of resolved fields</returns>
        /// <exception cref="SomonException">When the format is invalid</exception>
        private static SomonField[] ParseFields(TextReader reader, char[] buffer) {
            ICollection<SomonField> fieldSet = new LinkedList<SomonField>();
            int br;
            while ((br = ReadUntil(c => c == '}' || char.IsLetter(c), reader, buffer)) != -1) {
                char delimiter = buffer[br];
                
                // In this case, there are no properties or all have been read
                // So, we have finished
                if (delimiter == '}') return fieldSet.ToArray();
                
                // There is at least one field (left)
                
                // Read until field name end
                int wrdEndIdx = ReadUntilWordEnd(reader, buffer);
                if (wrdEndIdx == -1) throw new SomonException("Invalid field name declaration (end).");
            
                // Shift the array to insert first character
                Array.Copy(buffer, 0, buffer, 1, wrdEndIdx);
                buffer[0] = delimiter;
            
                string fieldName = new string(buffer, 0, wrdEndIdx + 1);
                
                // Read until value delimiter
                br = ReadUntilChar(';', reader, buffer);
                string fieldValue = new string(buffer, 0, br);
                
                fieldSet.Add(new SomonField(fieldName, fieldValue));
            }
            
            throw new SomonException("Invalid SOMON format. Missing right curly bracket.");
        }

        /// <summary>
        /// Parses the optional property id from the given <paramref name="buffer"/> with
        /// the specified content <paramref name="length"/>.
        /// </summary>
        /// <param name="buffer">Buffer to read</param>
        /// <param name="length">Length of available data</param>
        /// <returns>Resolved property id or NULL</returns>
        /// <exception cref="SomonException">When the format is invalid</exception>
        private static string ParsePropertyId(char[] buffer, int length) {
            int begin = -1;
            int end = -1;
            bool stringMode = false;
            for (int i = -1; ++i != length;) {
                char c = buffer[i];
                if (c == '"') {
                    stringMode = !stringMode;
                    if (stringMode)
                        begin = i + 1;
                    else
                        end = i;
                }
            }

            // Check for non-closed string
            if (begin > end) throw new SomonException("Invalid SOMON format. String has not been closed.");
            
            // No id declared
            if (begin == end) return null;
            
            return new string(buffer, begin, end - begin);
        }

        /// <summary>
        /// Consumes each character of the given <paramref name="reader"/> and copies it into
        /// the specified <paramref name="buffer"/> until a consumed character is a letter which
        /// indicates the new beginning of a word.
        /// </summary>
        /// <param name="reader">Reader to consumer the stream</param>
        /// <param name="buffer">Buffer each character is copied to</param>
        /// <returns>Number of read characters</returns>
        private static int ReadUntilWordBegin(TextReader reader, char[] buffer)
            => ReadUntil(char.IsLetter, reader, buffer);
        
        /// <summary>
        /// Consumes each character of the given <paramref name="reader"/> and copies it into
        /// the given <paramref name="buffer"/> until a sequence of characters is followed by
        /// a whitespace which indicates the end of a word.
        /// </summary>
        /// <param name="reader">Reader to consumer the stream</param>
        /// <param name="buffer">Buffer each character is copied to</param>
        /// <returns>Number of read characters</returns>
        private static int ReadUntilWordEnd(TextReader reader, char[] buffer)
            => ReadUntil(char.IsWhiteSpace, reader, buffer);
        
        /// <summary>
        /// Consumes each character of the given <paramref name="reader"/> and copies it into
        /// the given <paramref name="buffer"/> until a consumed character is equal to <paramref name="d"/>.
        /// </summary>
        /// <param name="d">Character to match</param>
        /// <param name="reader">Reader to consumer the stream</param>
        /// <param name="buffer">Buffer each character is copied to</param>
        /// <returns>Number of read characters</returns>
        private static int ReadUntilChar(char d, TextReader reader, char[] buffer) 
            => ReadUntil(c => c == d, reader, buffer);
        
        /// <summary>
        /// Consumes each character of the given <paramref name="reader"/> and copies it into
        /// the given <paramref name="buffer"/> until the specified <paramref name="match"/>
        /// condition succeeds.
        /// </summary>
        /// <param name="match">Break condition</param>
        /// <param name="reader">Reader to consumer the stream</param>
        /// <param name="buffer">Buffer each character is copied to</param>
        /// <returns>Number of read characters</returns>
        private static int ReadUntil(Predicate<char> match, TextReader reader, char[] buffer) {
            int br = -1;
            char c;
            while ((c = (char) reader.Read()) != -1) {
                buffer[++br] = c;
                if (match(c)) return br;
            }

            return -1;
        }
    }
}