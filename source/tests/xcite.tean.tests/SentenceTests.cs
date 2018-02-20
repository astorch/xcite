using NUnit.Framework;

namespace xcite.tean.tests {
    [TestFixture]
    public class SentenceTests {
        [Test]
        public void PlainText() {
            // Arrange
            string text = "Wir freuen uns,\r\nSie als Kunde wieder begrüßen zu dürfen!";
            
            // Act
            Sentence sentence = new Sentence(text, 0);
            string plainText = sentence.PlainText;

            // Assert
            Assert.IsNotNull(plainText);
            Assert.AreNotEqual(plainText, text);
            Assert.IsTrue(plainText.Length < text.Length);
            Assert.IsFalse(plainText.Contains("\r"));

            string expText = text.Replace("\r\n", " ").Replace("\t", " ");
            Assert.AreEqual(expText, plainText);
        }

        [Test]
        public void PlainTextFull() {
            // Arrange
            string text = "Guten Tag Herr Meier,\r\n\r\nwie geht es Ihnen heute -\tWir hoffen gut!";
            
            // Act
            Sentence sentence = new Sentence(text, 0);
            string plainText = sentence.PlainText;

            // Assert
            Assert.IsNotNull(plainText);
            Assert.AreNotEqual(plainText, text);
            Assert.IsTrue(plainText.Length < text.Length);
            Assert.IsFalse(plainText.Contains("\r"));

            string expText = text.Replace("\r\n\r\n", " ").Replace("\t", " ");
            Assert.AreEqual(expText, plainText);
        }
    }
}