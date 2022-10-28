using Adita.PlexNet.Core.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Adita.PlexNet.Core.Logging.Test
{
    [TestClass]
    public class FileLoggingTest
    {
        [TestMethod]
        public void CanLogging()
        {
            string defaultDirectory = "D://";

            string? appDirectory = Path.GetDirectoryName(Directory.GetCurrentDirectory());
            string? debugDirectory = Path.GetDirectoryName(appDirectory);
            string? projectDirectory = Path.GetDirectoryName(debugDirectory);
            string directory = Path.Combine(projectDirectory ?? defaultDirectory, "Logs");

            var services = new ServiceCollection().AddLogging(p =>
                p.AddFileLogger(config =>
                {
                    config.Directory = directory;
                    config.FileNamePrefix = "TestLog";
                })

            );

            var serviceProvider = services.BuildServiceProvider();

            var logger = serviceProvider.GetRequiredService<ILogger<DummyModel>>();
            Assert.IsNotNull(logger);

            logger.LogInformation("Information message here!");
            logger.LogWarning("Warning message here!");
            logger.LogError("Error message here!");
        }
    }
}