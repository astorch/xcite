using System.IO;
using NUnit.Framework;

namespace xcite.csharp.tests {
    [TestFixture]
    public class ObjectSerializerTests {
        [Test]
        public void SerializeDefault() {
            // Arrange
            Poco pco = new Poco {Name = "PCO", Double = 12.04d, Number = 5};
            
            // Act
            byte[] data = new ObjectSerializer().Serialize(pco);
            
            // Assert
            Assert.IsNotNull(data);
            Assert.IsTrue(data.Length > 0);
            Assert.AreEqual(160, data.Length);
        }

        [Test]
        public void DeserializeDefault() {
            // Arrange
            byte[] data = new ObjectSerializer().Serialize(new Poco {Name = "PCO", Double = 12.04d, Number = 5});
            
            // Act
            Poco pco = new ObjectSerializer().Deserialize<Poco>(data);
            
            // Assert
            Assert.IsNotNull(pco);
            Assert.AreEqual("PCO", pco.Name);
            Assert.AreEqual(12.04d, pco.Double);
            Assert.AreEqual(5, pco.Number);
        }

        [Test]
        public void SerializeInco() {
            // Arrange
            SelfPoco slfP = new SelfPoco {Name = "Self", Data = new byte[] {1, 5, 9, 7}, Number = 13};

            // Act
            byte[] data = new ObjectSerializer().Serialize(slfP);

            // Assert
            Assert.IsNotNull(data);
            Assert.IsTrue(data.Length > 0);
            Assert.AreEqual(165, data.Length);
            Assert.IsTrue(slfP.CustomSerialized);
            Assert.IsTrue(slfP.Is42);
        }

        [Test]
        public void DeserializeInco() {
            // Arrange
            byte[] data = new ObjectSerializer().Serialize(new SelfPoco {Name = "Self", Data = new byte[] {1, 5, 9, 7}, Number = 13});
            
            // Act
            SelfPoco spco = new ObjectSerializer().Deserialize<SelfPoco>(data);

            // Assert
            Assert.IsNotNull(spco);
            Assert.AreEqual("Self", spco.Name);
            CollectionAssert.AreEqual(new byte[] {1, 5, 9, 7}, spco.Data);
            Assert.AreEqual(13, spco.Number);
            Assert.IsTrue(spco.CustomSerialized);
            Assert.IsTrue(spco.Is42);
        }

        class Poco {
            public string Name { get; set; }

            public int Number { get; set; }

            public double Double { get; set; }
        }

        class SelfPoco : IIncoSerializer {
            public string Name { get; set; }

            public byte[] Data { get; set; }
            
            public short Number { get; set; }
            
            public bool CustomSerialized { get; private set; }
            
            public bool Is42 { get; private set; }

            /// <inheritdoc />
            public ECustomSerializationMode CustomSerializationMode { get; } = ECustomSerializationMode.Additional;

            /// <inheritdoc />
            public void WriteToStream(BinaryWriter writer) {
                writer.Write(42);
                Is42 = true;
                CustomSerialized = true;
            }

            /// <inheritdoc />
            public void ReadFromStream(BinaryReader reader) {
                int value = reader.ReadInt32();
                Is42 = value == 42;
                CustomSerialized = true;
            }
        }
    }
}