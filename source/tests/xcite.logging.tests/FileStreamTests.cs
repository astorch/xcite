using System;
using System.IO;
using NUnit.Framework;
using FileStream = xcite.logging.streams.FileStream;

namespace xcite.logging.tests {
    [TestFixture]
    public class FileStreamTests {
        [Test]
        public void Write() {
            // Arrange
            string logFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "_log.txt");
            string logText = "INFO 30.12.2018 19:44:51 FileStreamTests - Something about logging.\r\n";
            FileStream fileStream = new FileStream {FileName = logFilePath};

            // Act
            fileStream.Write(logText);
            fileStream.Dispose();

            // Assert
            string fileText = File.ReadAllText(logFilePath);
            Assert.AreEqual(logText, fileText);
        }

        [Test]
        public void WriteDailyRolling() {
            // Arrange
            string[] files = Directory.GetFiles(TestContext.CurrentContext.TestDirectory, "*.txt");
            for (int i = -1; ++i != files.Length;)
                File.Delete(files[i]);
            
            string logFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "_log.txt");
            string logTextYesterday = "INFO 30.12.2018 19:44:51 FileStreamTests - Say hi from yesterday.\r\n";
            string logTextToday = "INFO 31.12.2018 19:44:51 FileStreamTests - Say hi from today.\r\n";
            FileStream fileStream = new FileStream {FileName = logFilePath, DailyRolling = true};
            DateTime today = DateTime.Today;
            DateTime yesterday = today.AddDays(-1);

            // Act
            fileStream.Write(logTextYesterday);
            ((FileStream._Bender) fileStream).SetLastWrite(yesterday);
            fileStream.Write(logTextToday);
            
            fileStream.Dispose();

            // Assert
            string year = yesterday.Year.ToString();
            string month = yesterday.Month.ToString("00");
            string day = yesterday.Day.ToString("00");
            string logFileYesterdayName = $"_log-{year}-{month}-{day}.txt";

            string logFileYesterday = Path.Combine(TestContext.CurrentContext.TestDirectory, logFileYesterdayName);
            string logFileToday = Path.Combine(TestContext.CurrentContext.TestDirectory, "_log.txt");
            
            Assert.IsTrue(File.Exists(logFileYesterday));
            Assert.IsTrue(File.Exists(logFileToday));
            
            string todayFileText = File.ReadAllText(logFileToday);
            Assert.AreEqual(logTextToday, todayFileText);
            
            string yesterdayFileText = File.ReadAllText(logFileYesterday);
            Assert.AreEqual(logTextYesterday, yesterdayFileText);
        }
    }
}