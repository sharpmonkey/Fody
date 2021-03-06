using System;
using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;

[TestFixture]
public class WeaversXmlHistoryTests
{
    [Test]
    public void AddNewFile()
    {
        var fileName = Path.GetTempFileName();
        try
        {
            var processor = new Processor();
            processor.ConfigFiles.Add(fileName);
            processor.CheckForWeaversXmlChanged();

            Assert.AreEqual(File.GetLastWriteTimeUtc(fileName), Processor.TimeStamps.First().Value);
        }
        finally
        {
            File.Delete(fileName);
            Processor.TimeStamps.Clear();
        }
    }

    [Test]
    public void AddExistingFile()
    {
        var fileName = Path.GetTempFileName();
        try
        {
            var processor = new Processor();
            processor.ConfigFiles.Add(fileName);
            processor.CheckForWeaversXmlChanged();
            processor.CheckForWeaversXmlChanged();

            Assert.AreEqual(File.GetLastWriteTimeUtc(fileName), Processor.TimeStamps.First().Value);
        }
        finally
        {
            File.Delete(fileName);
            Processor.TimeStamps.Clear();
        }
    }

    [Test]
    public void AddChangedFile()
    {
        var fileName = Path.GetTempFileName();
        try
        {
            var expected = File.GetLastWriteTimeUtc(fileName);
            var loggerMock = new Mock<BuildLogger>();
            loggerMock.Setup(x => x.LogInfo(It.IsAny<string>()));

            var processor = new Processor
                {
                    Logger = loggerMock.Object
                };
            processor.ConfigFiles.Add(fileName);
            processor.CheckForWeaversXmlChanged();
            File.SetLastWriteTimeUtc(fileName, DateTime.Now.AddHours(1));
            processor.CheckForWeaversXmlChanged();

            loggerMock.Verify();

            Assert.AreEqual(expected, Processor.TimeStamps.First().Value);
        }
        finally
        {
            File.Delete(fileName);
            Processor.TimeStamps.Clear();
        }
    }
}