using System.IO;

namespace xcite.csharp {
    /// <summary> Describes the API of a class that controls the serialization of an extern object. </summary>
    public interface IExcoSerializer : ICustomSerializer {
        
        /// <summary>
        /// Notifies the instance to serialize the specified <paramref name="obj"/> using the
        /// given stream <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">Stream writer</param>
        /// <param name="obj">Object to serialize</param>
        void WriteToStream(BinaryWriter writer, object obj);

        /// <summary>
        /// Notifies the instance to deserialize the specified <paramref name="obj"/> using the
        /// given stream <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">Stream reader</param>
        /// <param name="obj">Object to deserialize</param>
        void ReadFromStream(BinaryReader reader, object obj);
    }
}