using System.IO;

namespace xcite.csharp {
    /// <inheritdoc />
    /// <summary> Announces a class that serializes (full or only a part of it) itself. </summary>
    public interface IIncoSerializer : ICustomSerializer {
        
        /// <summary> Notifies the instance to write its values into the given stream. </summary>
        /// <param name="writer">Stream writer</param>
        void WriteToStream(BinaryWriter writer);

        /// <summary> Notifies the instance to read its values from the given stream. </summary>
        /// <param name="reader">Stream reader</param>
        void ReadFromStream(BinaryReader reader);
    }
}