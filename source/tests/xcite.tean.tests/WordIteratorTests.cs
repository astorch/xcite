using System.Linq;
using NUnit.Framework;

namespace xcite.tean.tests {
    [TestFixture]
    public class WordIteratorTests {
        [Test]
        public void Iterate() {
            // Arrange
            string text = "Guten Tag Herr Storch,\r\n\r\nvielen Dank für Ihre Mitteilung an uns.";

            // Act
            Word[] words = text.ToWords();

            // Assert
            Assert.AreEqual(11, words.Length);
            
            string[] plainWordSet = text.Replace(",", string.Empty)
                                        .Replace(".", string.Empty)
                                        .Replace("\r\n\r\n", " ")
                                        .Split(' ')
                                        ;
            
            CollectionAssert.AreEqual(plainWordSet, words.Select(w => w.Text));
        }

        [Test]
        public void IterateNoUrl() {
            // Arrange
            string text = "Guten Tag Herr Storch,\r\n\r\nvielen Dank für Ihre Mitteilung an AdmiralDirekt.de.";
            
            // Act
            Word[] words = text.ToWords();

            // Assert
            Assert.AreEqual(11, words.Length);
            Assert.AreEqual("AdmiralDirekt.de", words[10].Text);
        }
        
        [Test]
        public void IterateNoDate() {
            // Arrange
            string text = "Hiermit bestätigen wir, dass wir Ihren Vertrag zum 17.02.2018, 24:00 Uhr beenden.";
            
            // Act
            Word[] words = text.ToWords();

            // Assert
            Assert.AreEqual(12, words.Length);
            Assert.AreEqual("17.02.2018", words[8].Text);
            Assert.AreEqual("24:00", words[9].Text);
            Assert.AreEqual("Uhr", words[10].Text);
            Assert.AreEqual("beenden", words[11].Text);
        }
        
        [Test]
        public void Iterate2Sentences() {
            // Arrange
            string text = "Guten Tag Herr Storch,\r\n\r\nvielen Dank für Ihre Mitteilung an uns. Schade, " +
                          "dass Sie uns verlassen möchten!";
            
            // Act
            Word[] words = text.ToWords();

            // Assert
            Assert.AreEqual(17, words.Length);
        }

        [Test]
        public void IterateWithQuotes() {
            // Arrange
            string text = "Vielen Dank für Ihre Buchung des \"Freude im Urlaub. Freude im Leben.\"-Pakets.\r\n" +
                          "Anbei finden Sie die Rechnung zur Ihrer Bestellung.";
            
            // Act
            Word[] words = text.ToWords();
            
            // Assert
            Assert.AreEqual(15, words.Length);
            Assert.AreEqual("\"Freude im Urlaub. Freude im Leben.\"-Pakets", words[6].Text);
        }
    }
}