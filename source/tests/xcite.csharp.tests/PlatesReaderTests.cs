using System.IO;
using NUnit.Framework;

namespace xcite.csharp.tests {
    [TestFixture]
    public class PlatesReaderTests {
        [Test]
        public void ReadFromFile() {
            // Arrange
            var ctx = TestContext.CurrentContext;
            string testDir = ctx.TestDirectory;
            string configFile = Path.Combine(testDir, "TestConfiguration.txt");
            
            // Act
            TestConfiguration cfg = PlatesReader.ReadFromFile<TestConfiguration>(configFile);

            // Assert
            Assert.IsNotNull(cfg);
            Assert.AreEqual("Zeichenkettenwert", cfg.PropertyString);
            Assert.AreEqual(true, cfg.PropertyBool);
            Assert.AreEqual(65, cfg.PropertyInt);
            Assert.AreEqual(TestConfiguration.EPropertyEnum.One, cfg.PropertyEnum);
            Assert.AreEqual(null, cfg.PropertyNullableInt1);
            Assert.AreEqual(5, cfg.PropertyNullableInt2);
            Assert.AreEqual(5.24d, cfg.PropertyNullableDouble);
        }

    }

    public class TestConfiguration {

        public string PropertyString { get; set; }

        public bool PropertyBool { get; set; }

        public int PropertyInt { get; set; }

        public int? PropertyNullableInt1 { get; set; }
        
        public int? PropertyNullableInt2 { get; set; }
        
        public double? PropertyNullableDouble { get; set; }

        public EPropertyEnum PropertyEnum { get; set; }
        
        public enum EPropertyEnum {
            None,
            One,
            Two
        }
    }
}