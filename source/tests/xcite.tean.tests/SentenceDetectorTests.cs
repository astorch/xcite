using System.Linq;
using NUnit.Framework;
using xcite.tean.lang;

namespace xcite.tean.tests {
    [TestFixture]
    public class SentenceDetectorTests {
        [Test]
        public void DetectMixedIn() {
            // Arrange
            string text = "Hr. Müller, bitte überweisen Sie den mtl. Betrag i. H. v. 200 EUR " +
                          "bis zum 31.10.2018 auf das Admiraldirekt.de Konto. Wir werden den " +
                          "Eingang unmittelbar bestätigen. Wenn der o. g. bis 10:00 Uhr bei uns eingeht, " +
                          "dann erhalten sie einen zusätzl. Rabatt von rd. 2 %.";

            // Act
            Sentence[] sentences = new SentenceDetector(GermanLangInfo.Instance).Detect(text).ToArray();

            // Assert
            Assert.AreEqual(3, sentences.Length);

            Sentence s1 = sentences[0];
            Assert.AreEqual(text.Substring(s1.Begin, s1.Length), s1.Text);
            Assert.AreEqual("Hr. Müller, bitte überweisen Sie den mtl. Betrag i. H. v. 200 EUR " +
                            "bis zum 31.10.2018 auf das Admiraldirekt.de Konto.", s1.Text);

            Sentence s2 = sentences[1];
            Assert.AreEqual(text.Substring(s2.Begin, s2.Length), s2.Text);
            Assert.AreEqual("Wir werden den " +
                            "Eingang unmittelbar bestätigen.", s2.Text);

            Sentence s3 = sentences[2];
            Assert.AreEqual(text.Substring(s3.Begin, s3.Length), s3.Text);
            Assert.AreEqual("Wenn der o. g. bis 10:00 Uhr bei uns eingeht, " +
                            "dann erhalten sie einen zusätzl. Rabatt von rd. 2 %.", s3.Text);
        }
    }
}