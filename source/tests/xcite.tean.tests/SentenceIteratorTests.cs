using System;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace xcite.tean.tests {
    [TestFixture]
    public class SentenceIteratorTests {
        
        [Test]
        public void IterateSingle() {
            // Arrange
            string text = "Guten Tag Herr Storch,\r\n\r\nvielen Dank für Ihre Mitteilung an uns.";
            
            // Act
            Sentence[] sentences = text.ToSentences();

            // Assert
            Assert.AreEqual(1, sentences.Length);
            
            Sentence sentence = sentences[0];
            Assert.AreEqual(0, sentence.Begin);
            Assert.AreEqual(65, sentence.End);
            Assert.AreEqual(text.Substring(0, 65), sentence.Text);
        }
        
        [Test]
        public void IterateNoUrl() {
            // Arrange
            string text = "Guten Tag Herr Storch,\r\n\r\nvielen Dank für Ihre Mitteilung an AdmiralDirekt.de.";
            
            // Act
            Sentence[] sentences = text.ToSentences();

            // Assert
            Assert.AreEqual(1, sentences.Length);
            
            Sentence sentence = sentences[0];
            Assert.AreEqual(0, sentence.Begin);
            Assert.AreEqual(78, sentence.End);
            Assert.AreEqual(text.Substring(0, 78), sentence.Text);
        }

        [Test]
        public void IterateNoDate() {
            // Arrange
            string text = "Hiermit bestätigen wir, dass wir Ihren Vertrag zum 17.02.2018, 24:00 Uhr beenden.";
            
            // Act
            Sentence[] sentences = text.ToSentences();

            // Assert
            Assert.AreEqual(1, sentences.Length);
            
            Sentence sentence = sentences[0];
            Assert.AreEqual(0, sentence.Begin);
            Assert.AreEqual(81, sentence.End);
            Assert.AreEqual(text.Substring(0, 81), sentence.Text);
        }

        [Test]
        public void Iterate2Sentences() {
            // Arrange
            string text = "Guten Tag Herr Storch,\r\n\r\nvielen Dank für Ihre Mitteilung an uns. Schade, " +
                          "dass Sie uns verlassen möchten!";
            
            // Act
            Sentence[] sentences = text.ToSentences();

            // Assert
            Assert.AreEqual(2, sentences.Length);
            
            Sentence s1 = sentences[0];
            Assert.AreEqual(0, s1.Begin);
            Assert.AreEqual(65, s1.End);
            Assert.AreEqual(text.Substring(s1.Begin, s1.Length), s1.Text);

            Sentence s2 = sentences[1];
            Assert.AreEqual(66, s2.Begin);
            Assert.AreEqual(105, s2.End);
            Assert.AreEqual(text.Substring(s2.Begin, s2.Length), s2.Text);
        }
        
        [Test]
        public void IterateMultiple() {
            // Arrange
            string text = LoadSample("text-sample1.txt");
            
            // Act
            Sentence[] sentences = text.ToSentences();

            // Assert
            Assert.AreEqual(4, sentences.Length);

            Sentence s3 = sentences[2];
            Assert.AreEqual(141, s3.Begin);
            Assert.AreEqual(180, s3.End);
            Assert.AreEqual(text.Substring(s3.Begin, s3.Length), s3.Text);
        }

        private string LoadSample(string name) {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string fqn = $"{assembly.GetName().Name}.samples.{name}";
            using (Stream stream = assembly.GetManifestResourceStream(fqn)) {
                using (StreamReader reader = new StreamReader(stream)) {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}