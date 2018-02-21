using NUnit.Framework;

namespace xcite.tean.tests {
    [TestFixture]
    public class SentenceTests {
        [Test]
        public void UnformattedText() {
            // Arrange
            string text = "Wir freuen uns,\r\nSie als Kunde wieder begrüßen zu dürfen!";
            
            // Act
            Sentence sentence = new Sentence(text, 0);
            string unformattedText = sentence.UnformattedText;

            // Assert
            Assert.IsNotNull(unformattedText);
            Assert.AreNotEqual(unformattedText, text);
            Assert.IsTrue(unformattedText.Length < text.Length);
            Assert.IsFalse(unformattedText.Contains("\r"));

            string expText = text.Replace("\r\n", " ").Replace("\t", " ");
            Assert.AreEqual(expText, unformattedText);
        }

        [Test]
        public void UnformattedTextFull() {
            // Arrange
            string text = "Guten Tag Herr Meier,\r\n\r\nwie geht es Ihnen heute -\tWir hoffen gut!";
            
            // Act
            Sentence sentence = new Sentence(text, 0);
            string unformattedText = sentence.UnformattedText;

            // Assert
            Assert.IsNotNull(unformattedText);
            Assert.AreNotEqual(unformattedText, text);
            Assert.IsTrue(unformattedText.Length < text.Length);
            Assert.IsFalse(unformattedText.Contains("\r"));

            string expText = text.Replace("\r\n\r\n", " ").Replace("\t", " ");
            Assert.AreEqual(expText, unformattedText);
        }

        [Test]
        public void UnformattedTextWithHyphen() {
            // Arrange
            string text = "Bitte achten Sie darauf, dass Ihre neue Versicherung rechtzeitig eine Versicherungsbe-\r\nstätigung hinterlegt.";

            // Act
            Sentence sentence = new Sentence(text, 0);
            string unformattedText = sentence.UnformattedText;

            // Assert
            Assert.IsNotNull(unformattedText);
            Assert.AreNotEqual(unformattedText, text);
            Assert.IsTrue(unformattedText.Length < text.Length);
            Assert.IsFalse(unformattedText.Contains("\r"));

            string expText = text.Replace("\r\n", string.Empty).Replace("\t", " ");
            Assert.AreEqual(expText, unformattedText);
        }
    }
}