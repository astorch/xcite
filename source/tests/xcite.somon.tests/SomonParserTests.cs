using System.IO;
using NUnit.Framework;

namespace xcite.somon.tests {
    [TestFixture]
    public class SomonParserTests {
        [Test]
        public void Parse() {
            // Arrange
            
            // Act
            string binFolder = TestContext.CurrentContext.TestDirectory;
            string filePath = Path.Combine(binFolder, "SomonCase1.txt");
            SomonObject sobj = new SomonParser().ParseFile(filePath);

            // Assert
            Assert.IsNotNull(sobj);
            Assert.AreEqual(4, sobj.Properties.Length);

            SomonProperty sprop1 = sobj.Properties[0];
            EnsureProperty(sprop1, "region", "r1", new SomonField("bounds", "0, 0, 300, 300"));

            SomonProperty sprop2 = sobj.Properties[1];
            EnsureProperty(sprop2, "entity", "companyName", new SomonField("regions", "r1"));

            SomonProperty sprop3 = sobj.Properties[2];
            EnsureProperty(sprop3, "fragment", null);

            SomonProperty sprop4 = sobj.Properties[3];
            EnsureProperty(sprop4, "entity", "multiple", 
                new SomonField("regions", "r1, r2"), new SomonField("validtrs", "none"));
        }

        private void EnsureProperty(SomonProperty prop, string kind, string id, params SomonField[] fields) {
            Assert.IsNotNull(prop);
            Assert.AreEqual(kind, prop.Kind);
            Assert.AreEqual(id, prop.Id);
            Assert.AreEqual(fields.Length, prop.Fields.Length);
            
            for (int i = -1; ++i != fields.Length;) {
                var expField = fields[i];
                var actField = prop.Fields[i];
                
                Assert.IsNotNull(actField);
                Assert.AreEqual(expField.Name, actField.Name);
                Assert.AreEqual(expField.Value, actField.Value);
            }
        }
    }
}