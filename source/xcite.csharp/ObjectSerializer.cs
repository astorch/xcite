using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace xcite.csharp {
    /// <summary> Provides methods to serialize common objects. </summary>
    public class ObjectSerializer {
        private readonly IDictionary<Type, IExcoSerializer> _excoSerializer;

        /// <summary> Initializes the new instance. This instance does not have any <see cref="T:xcite.csharp.IExcoSerializer" />. </summary>
        public ObjectSerializer() : this(null) {
            // Nothing to do here
        }

        /// <summary>
        /// Initializes the new instance and registers the specified set of <see cref="IExcoSerializer"/>.
        /// </summary>
        /// <param name="excoSerializerSet">Set of <see cref="IExcoSerializer"/></param>
        public ObjectSerializer(IDictionary<Type, IExcoSerializer> excoSerializerSet) {
            _excoSerializer = excoSerializerSet ?? new Dictionary<Type, IExcoSerializer>(0);
        }

        /// <summary> Serializes the given <paramref name="element"/>. Note, if the element is NULL
        /// an empty array is returned.</summary>
        /// <param name="element">Element to serialize</param>
        /// <typeparam name="TObject">Type of element being serialized</typeparam>
        /// <returns>Serialized object</returns>
        public virtual byte[] Serialize<TObject>(TObject element) where TObject : class {
            if (element == null) return new byte[0];
            
            using (MemoryStream memoryStream = new MemoryStream()) {
                using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream)) {
                    // First, we write the full qualified object type
                    Type objType = element.GetType();
                    string objTypeStr = objType.AssemblyQualifiedName;
                    binaryWriter.Write(objTypeStr);

                    // Then, we serialize the object itself
                    ISerializor serializor = GetSerializor(_excoSerializer, element);
                    serializor.WriteToStream(binaryWriter, element);
                }

                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Invokes <see cref="Deserialize"/> and casts the resulting element to the specified
        /// <typeparamref name="TObject"/> type.
        /// </summary>
        /// <param name="data">Data of the object to deserialize</param>
        /// <typeparam name="TObject">Type of element the resulting object is being casted to</typeparam>
        /// <returns>Deserialized and casted object</returns>
        public virtual TObject Deserialize<TObject>(byte[] data) where TObject : class {
            return (TObject) Deserialize(data);
        }

        /// <summary> Deserializes an element from the given <paramref name="data"/>. Note, if
        /// the given data is NULL or empty, NULL is returned.</summary>
        /// <param name="data">Data of the object to deserialize</param>
        /// <returns>Deserialized element</returns>
        public virtual object Deserialize(byte[] data) {
            if (data == null || data.Length == 0) return null;

            object element;
            using (MemoryStream memoryStream = new MemoryStream(data)) {
                using (BinaryReader binaryReader = new BinaryReader(memoryStream)) {
                    // First, we read the full qualified object type and create an object instance
                    string objTypeStr = binaryReader.ReadString();
                    Type objType = Type.GetType(objTypeStr, true);
                    element = objType.NewInstance<object>();
                    
                    // Then, we deserialize the object itself
                    ISerializor serializor = GetSerializor(_excoSerializer, element);
                    serializor.ReadFromStream(binaryReader, element);
                }
            }

            return element;
        }

        /// <summary>
        /// Returns an instance of <see cref="ISerializor"/> that handles the (de)serialization of the specified
        /// <paramref name="element"/>.
        /// </summary>
        /// <param name="excoSerSet">Set of registered <see cref="IExcoSerializer"/></param>
        /// <param name="element">Element to (de)serialize</param>
        /// <returns>Instance of <see cref="ISerializor"/></returns>
        private static ISerializor GetSerializor(IDictionary<Type, IExcoSerializer> excoSerSet, object element) {
            if (excoSerSet.TryGetValue(element.GetType(), out IExcoSerializer excoSer)) return new ExcoSerializor(excoSer);
            if (element is IIncoSerializer incoSer) return new IncoSerializor(incoSer);
            return new DefaultSerializor();
        }

        /// <summary> Default implementation of <see cref="ISerializor"/>. (De)Serializes
        /// all public read- and writable properties.
        /// </summary>
        class DefaultSerializor : AbstractSerializor {

        }
        
        /// <summary>
        /// Extends <see cref="CustomSerializor"/> to support element instances
        /// which implement <see cref="IIncoSerializer"/>.
        /// </summary>
        class IncoSerializor : CustomSerializor {
            private readonly IIncoSerializer _incoSerializer;

            /// <inheritdoc />
            public IncoSerializor(IIncoSerializer incoSerializer) : base(incoSerializer) {
                _incoSerializer = incoSerializer;
            }

            /// <inheritdoc />
            protected override void OnWriteToStream(BinaryWriter writer, object element) 
                => _incoSerializer.WriteToStream(writer);

            /// <inheritdoc />
            protected override void OnReadFromStream(BinaryReader reader, object element) 
                => _incoSerializer.ReadFromStream(reader);
        }
        
        /// <summary>
        /// Extends <see cref="CustomSerializor"/> to support element instances
        /// which are serialized by a <see cref="IExcoSerializer"/>.
        /// </summary>
        class ExcoSerializor : CustomSerializor {
            private readonly IExcoSerializer _excoSerializer;

            /// <inheritdoc />
            public ExcoSerializor(IExcoSerializer excoSerializer) : base(excoSerializer) {
                _excoSerializer = excoSerializer;
            }

            /// <inheritdoc />
            protected override void OnWriteToStream(BinaryWriter writer, object element) 
                => _excoSerializer.WriteToStream(writer, element);

            /// <inheritdoc />
            protected override void OnReadFromStream(BinaryReader reader, object element) 
                => _excoSerializer.ReadFromStream(reader, element);
        }

        /// <summary> Extends <see cref="AbstractSerializor"/> to support <see cref="ICustomSerializer"/>. </summary>
        abstract class CustomSerializor : AbstractSerializor {
            private readonly ICustomSerializer _serializer;

            /// <inheritdoc />
            protected CustomSerializor(ICustomSerializer serializer) {
                _serializer = serializer;
            }

            /// <inheritdoc />
            public override void WriteToStream(BinaryWriter writer, object element) {
                // The serializer fully handles the serialization itself
                if (_serializer.CustomSerializationMode == ECustomSerializationMode.Full) {
                    OnWriteToStream(writer, element);
                    return;
                }
                
                // We serialize the public read- and writable properties
                base.WriteToStream(writer, element);
                
                // Now the serializer can serialize its additional content
                OnWriteToStream(writer, element);
            }

            /// <inheritdoc />
            public override void ReadFromStream(BinaryReader reader, object element) {
                // The serializer fully handles the deserialization itself
                if (_serializer.CustomSerializationMode == ECustomSerializationMode.Full) {
                    OnReadFromStream(reader, element);
                    return;
                }
                
                // We deserialize the public read- and writable properties first
                base.ReadFromStream(reader, element);
                
                // Now the serialize can deserialize its additional content
                OnReadFromStream(reader, element);
            }

            /// <summary> Notifies the instance to handle the customized element serialization. </summary>
            /// <param name="writer">Stream writer</param>
            /// <param name="element">Element instance to serialize</param>
            protected abstract void OnWriteToStream(BinaryWriter writer, object element);

            /// <summary> Notifies the instance to handle the customized element deserialization. </summary>
            /// <param name="reader">Stream reader</param>
            /// <param name="element">Element instance to deserialize</param>
            protected abstract void OnReadFromStream(BinaryReader reader, object element);
        } 
        
        /// <summary>
        /// Abstract implementation of <see cref="ISerializor"/>. This class serializes
        /// all public read- and writable properties.
        /// </summary>
        abstract class AbstractSerializor : ISerializor {
            
            /// <inheritdoc />
            public virtual void WriteToStream(BinaryWriter writer, object element) {
                // We handle all public read- and writable properties
                PropertyInfo[] propSet = GetSerializableProperties(element);
                writer.Write(propSet.Length);
                for (int i = -1; ++i != propSet.Length;) {
                    PropertyInfo property = propSet[i];
                    string propName = property.Name;
                    Type propType = property.PropertyType;
                    object propValue = property.GetValue(element);
                        
                    writer.Write(propName);
                    GetValueWriter(propType)(writer, propValue);
                }
            }

            /// <inheritdoc />
            public virtual void ReadFromStream(BinaryReader reader, object element) {
                // We handle all public read- and writable properties
                PropertyInfo[] propSet = GetSerializableProperties(element);

                int propCount = reader.ReadInt32();
                for (int i = -1; ++i != propCount;) {
                    string propertyName = reader.ReadString();
                    PropertyInfo property = propSet.FirstOrDefault(p => p.Name == propertyName);
                    if (property == null) continue;
                    
                    Type propType = property.PropertyType;
                    object propValue = GetValueReader(propType)(reader);
                    property.SetValue(element, propValue);
                }
            }

            /// <summary>
            /// Returns all public read- and writable properties of the specified <paramref name="element"/>.
            /// </summary>
            /// <param name="element">Element which provides the properties to (de)serialize</param>
            /// <returns>Set of public read- and writable properties</returns>
            private static PropertyInfo[] GetSerializableProperties(object element) {
                Type objType = element.GetType();
                return objType.GetRuntimeProperties().Where(p => p.CanRead && p.GetMethod.IsPublic
                                                              && p.CanWrite && p.SetMethod.IsPublic)
                    .ToArray();
            }

            /// <summary> Returns a method that reads an object value type-safe from a stream. </summary>
            /// <param name="propertyType">Type of property being read</param>
            /// <returns>Method that reads from a stream</returns>
            private static Func<BinaryReader, object> GetValueReader(Type propertyType) {
                // Custom types
    //            if (_customSerializer.TryGetValue(propertyType, out ICustomTypeSerializer serializer)) {
    //                return reader => serializer.Deserialize(reader);
    //            }
    
                // Base types
                if (propertyType == typeof(char))    return reader => reader.ReadChar();
                if (propertyType == typeof(string))  return reader => reader.ReadString();
                if (propertyType == typeof(short))   return reader => reader.ReadInt16();
                if (propertyType == typeof(int))     return reader => reader.ReadInt32();
                if (propertyType == typeof(long))    return reader => reader.ReadInt64();
                if (propertyType == typeof(ushort))  return reader => reader.ReadUInt16();
                if (propertyType == typeof(uint))    return reader => reader.ReadUInt32();
                if (propertyType == typeof(ulong))   return reader => reader.ReadUInt64();
                if (propertyType == typeof(bool))    return reader => reader.ReadBoolean();
                if (propertyType == typeof(byte))    return reader => reader.ReadByte();
                if (propertyType == typeof(float))   return reader => reader.ReadSingle();
                if (propertyType == typeof(double))  return reader => reader.ReadDouble();
                if (propertyType == typeof(decimal)) return reader => reader.ReadDecimal();
    
                // Specific types
                if (propertyType.GetTypeInfo().IsEnum) return ReadEnumValue;
                if (propertyType == typeof(DateTime))  return reader => new DateTime(reader.ReadInt64());
                if (propertyType == typeof(byte[]))    return ReadByteArray;
                if (propertyType == typeof(string[]))  return ReadStringArray;
    
                throw new InvalidOperationException($"Property type '{propertyType}' is not supported");
            }
    
            /// <summary> Reads an enum value from a stream. </summary>
            /// <param name="reader">Stream reader</param>
            /// <returns>Read enum value</returns>
            private static object ReadEnumValue(BinaryReader reader) {
                string enumTypeName = reader.ReadString();
                Type enumType = Type.GetType(enumTypeName, true);
                int enumValue = reader.ReadInt32();
                return Enum.ToObject(enumType, enumValue);
            }
    
            /// <summary> Reads a byte array from a stream. </summary>
            /// <param name="reader">Stream reader</param>
            /// <returns>Read byte array</returns>
            private static object ReadByteArray(BinaryReader reader) {
                int dataSize = reader.ReadInt32();
                return reader.ReadBytes(dataSize);
            }
    
            /// <summary> Reads a string array from a stream. </summary>
            /// <param name="reader">Stream reader</param>
            /// <returns>Read string array</returns>
            private static object ReadStringArray(BinaryReader reader) {
                int strCount = reader.ReadInt32();
                string[] strArray = new string[strCount];
                for (int i = -1; ++i != strCount;) {
                    strArray[i] = reader.ReadString();
                }
    
                return strArray;
            }

            /// <summary> Returns a method that writes an object value type-safe into a stream. </summary>
            /// <param name="propertyType">Type of property being written</param>
            /// <returns>Method that writes into a stream</returns>
            private static Action<BinaryWriter, object> GetValueWriter(Type propertyType) {
                // Custom types
    //            if (_customSerializer.TryGetValue(propertyType, out ICustomTypeSerializer serializer)) {
    //                return (writer, o) => serializer.Serialize(writer, o);
    //            }
    
                // Base types
                if (propertyType == typeof(char))    return (writer, o) => writer.Write((char) o);
                if (propertyType == typeof(string))  return (writer, o) => writer.Write((string) o);
                if (propertyType == typeof(short))   return (writer, o) => writer.Write((short) o);
                if (propertyType == typeof(int))     return (writer, o) => writer.Write((int) o);
                if (propertyType == typeof(long))    return (writer, o) => writer.Write((long) o);
                if (propertyType == typeof(ushort))  return (writer, o) => writer.Write((ushort) o);
                if (propertyType == typeof(uint))    return (writer, o) => writer.Write((uint) o);
                if (propertyType == typeof(ulong))   return (writer, o) => writer.Write((ulong) o);
                if (propertyType == typeof(bool))    return (writer, o) => writer.Write((bool) o);
                if (propertyType == typeof(byte))    return (writer, o) => writer.Write((byte) o);
                if (propertyType == typeof(float))   return (writer, o) => writer.Write((float) o);
                if (propertyType == typeof(double))  return (writer, o) => writer.Write((double) o);
                if (propertyType == typeof(decimal)) return (writer, o) => writer.Write((decimal) o);

                // Specific types
                if (propertyType.GetTypeInfo().IsEnum) return WriteEnumValue;
                if (propertyType == typeof(DateTime))  return (writer, o) => writer.Write(((DateTime) o).Ticks);
                if (propertyType == typeof(byte[]))    return WriteByteArray;
                if (propertyType == typeof(string[]))  return WriteStringArray;
    
                throw new InvalidOperationException($"Property type '{propertyType}' is not supported");
            }

            /// <summary> Writes an enum value into a stream. </summary>
            /// <param name="writer">Stream writer</param>
            /// <param name="val">Enum value</param>
            private static void WriteEnumValue(BinaryWriter writer, object val) {
                string enumTypeName = val.GetType().AssemblyQualifiedName;
                writer.Write(enumTypeName);
                writer.Write((int) val);
            }

            /// <summary> Writes a byte array into a stream. </summary>
            /// <param name="writer">Stream writer</param>
            /// <param name="val">Byte array</param>
            private static void WriteByteArray(BinaryWriter writer, object val) {
                byte[] data = (byte[]) val;
                writer.Write(data.Length);
                writer.Write(data);
            }

            /// <summary> Writes a string array into a stream. </summary>
            /// <param name="writer">Binary writer</param>
            /// <param name="val">String array</param>
            private static void WriteStringArray(BinaryWriter writer, object val) {
                string[] stringSet = (string[]) val;
                writer.Write(stringSet.Length);
                for (int i = -1; ++i != stringSet.Length;)
                    writer.Write(stringSet[i]);
            }

        }
        
        /// <summary> Defines a serializor that fascades the different serialization APIs. </summary>
        interface ISerializor {
            /// <summary>
            /// Notifies the serializor to write the specified <paramref name="element"/> into the
            /// stream.
            /// </summary>
            /// <param name="writer">Stream writer</param>
            /// <param name="element">Element to serialize</param>
            void WriteToStream(BinaryWriter writer, object element);

            /// <summary>
            /// Notifies the serializor to read the properties of the specified <paramref name="element"/>
            /// from the strema.
            /// </summary>
            /// <param name="reader">Stream reader</param>
            /// <param name="element">Element to deserialize</param>
            void ReadFromStream(BinaryReader reader, object element);
        }
    }
}