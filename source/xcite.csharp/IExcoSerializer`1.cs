using System.IO;

namespace xcite.csharp {
    /// <inheritdoc />
    /// <summary> Generic interface of <see cref="T:xcite.csharp.IExcoSerializer" />. </summary>
    /// <typeparam name="TObject">Type of object being serialized</typeparam>
    public interface IExcoSerializer<in TObject> : IExcoSerializer {
        
        /// <summary>
        /// Notifies the instance to serialize the specified <paramref name="obj"/> using the
        /// given stream <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">Stream writer</param>
        /// <param name="obj">Object to serialize</param>
        void WriteToStream(BinaryWriter writer, TObject obj);

        /// <summary>
        /// Notifies the instance to deserialize the specified <paramref name="obj"/> using the
        /// given stream <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">Stream reader</param>
        /// <param name="obj">Object to deserialize</param>
        void ReadFromStream(BinaryReader reader, TObject obj);
    }
}