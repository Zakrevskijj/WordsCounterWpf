using System.IO.Abstractions.TestingHelpers;
using WordsCounterApp.Business;

namespace WordsCounterApp.UnitTests
{
    public class WordsCounterServiceTests
    {
        private readonly string _filePath= @"c:\myfile.txt";

        [Fact]
        public async Task ParseFileAndCountWords_ThreeWords_CorrectCountsPresent()
        {
            //Arrange
            MockFileSystem fileSystem = SetupFileSystem(_filePath);
            var wordsCounterService = new WordsCounterService(fileSystem);

            // Act
            var wordsCount = wordsCounterService.ParseFileAndCountWords(_filePath);

            //Assert
            Assert.True(wordsCount["Word1"] == 3);
            Assert.True(wordsCount["Word2"] == 2);
            Assert.True(wordsCount["Word3"] == 1);
        }

        [Fact]
        public async Task ParseFileAndCountWords_CancellationRequested_TaskCanceledThrown()
        {
            //Arrange
            MockFileSystem fileSystem = SetupFileSystem(_filePath);
            var wordsCounterService = new WordsCounterService(fileSystem);
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();

            // Act
            // Assert
            Assert.Throws<TaskCanceledException>(() => wordsCounterService.ParseFileAndCountWords(_filePath, cancellationTokenSource.Token));
        }

        private static MockFileSystem SetupFileSystem(string filePath)
        {
            return new MockFileSystem(new Dictionary<string, MockFileData>
            {
                { filePath, new MockFileData("Word1 Word2 .,., Word3 Word1 Word1 Word2") }
            });
        }
    }
}