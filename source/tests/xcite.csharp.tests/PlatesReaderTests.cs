using System;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
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
        }

    }

    public class TestConfiguration {

        public string PropertyString { get; set; }

        public bool PropertyBool { get; set; }

        public int PropertyInt { get; set; }
    }
}