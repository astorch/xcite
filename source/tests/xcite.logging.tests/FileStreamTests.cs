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
            if (File.Exists(logFilePath))
                File.Delete(logFilePath);

            string logText = "INFO 30.12.2018 19:44:51 FileStreamTests - Something about logging.\r\n";
            FileStream fileStream = new FileStream {FileName = logFilePath};

            // Act
            using (fileStream) {
                fileStream.Write(logText, new LogData());    
            }

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
            fileStream.Write(logTextYesterday, new LogData());
            ((FileStream._Bender) fileStream).SetLastWrite(yesterday);
            fileStream.Write(logTextToday, new LogData());
            
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

        [Test]
        public void WriteToFolderPath() {
            // Arrange
            string recordText = "Some record";
            string logFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "logs", "_log.txt");
            FileInfo fileNfo = new FileInfo(logFilePath);
            if (fileNfo.Exists) {
                DirectoryInfo fileFolder = fileNfo.Directory;
                fileNfo.Delete();
                fileFolder?.Delete(true);
            }

            FileStream fileStream = new FileStream {FileName = logFilePath};
            
            // Act
            using (fileStream) {
                fileStream.Write(recordText, new LogData());
            }
            
            // Assert
            fileNfo.Refresh();
            Assert.IsTrue(fileNfo.Exists);
            Assert.IsTrue(fileNfo.Directory?.Exists);
            
            Assert.AreEqual(recordText, File.ReadAllText(logFilePath));
        }

        [Test]
        public void WriteAppended() {
           // Arrange
           string baseText = "This is the first line\n";
           string recordText = "This is the recorded line.";
           string logFile = Path.Combine(TestContext.CurrentContext.TestDirectory, "fileToAppend.txt");
           if (File.Exists(logFile))
               File.Delete(logFile);
           
           File.WriteAllText(logFile, baseText);

           // Act
           FileStream fileStream = new FileStream {FileName = logFile, Append = true};
           using (fileStream) {
               fileStream.Write(recordText, new LogData());
           }

           // Assert
           Assert.IsTrue(File.Exists(logFile));

           string logFileText = File.ReadAllText(logFile);
           Assert.IsNotEmpty(logFileText);
           Assert.AreEqual(
               baseText + recordText,
               logFileText
           );
        }

        [Test]
        public void WriteNotAppended() {
            // Arrange
            string baseText = "This is the first line\n";
            string recordText = "This is the recorded line.";
            string logFile = Path.Combine(TestContext.CurrentContext.TestDirectory, "fileToAppend.txt");
            if (File.Exists(logFile))
                File.Delete(logFile);
           
            File.WriteAllText(logFile, baseText);

            // Act
            FileStream fileStream = new FileStream {FileName = logFile, Append = false};
            using (fileStream) {
                fileStream.Write(recordText, new LogData());
            }

            // Assert
            Assert.IsTrue(File.Exists(logFile));

            string logFileText = File.ReadAllText(logFile);
            Assert.IsNotEmpty(logFileText);
            Assert.AreEqual(
                recordText,
                logFileText
            );
        }
    }
}