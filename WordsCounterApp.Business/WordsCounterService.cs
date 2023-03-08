using System.IO.Abstractions;

namespace WordsCounterApp.Business
{
    public class WordsCounterService
    {
        private readonly IFileSystem _fileSystem;

        public WordsCounterService(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        /// <summary>
        /// Parse file and calculate occurences of each word
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="TaskCanceledException"></exception>
        public Dictionary<string, int> ParseFileAndCountWords(string filePath, CancellationToken cancellationToken)
        {
            var fileText = _fileSystem.File.ReadAllText(filePath);
            if (cancellationToken.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }

            var words = fileText
                .Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);
            if (cancellationToken.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }

            return CalculateWordCounts(words);
        }

        /// <summary>
        /// Calculates number of occurences for each word in the array and returns it in format of Dictionary, where key is is a word and value is a number of occurences
        /// </summary>
        /// <param name="words">Array with words</param>
        /// <returns>Dictionary, where key is is a word and value is a number of occurences</returns>
        private static Dictionary<string, int> CalculateWordCounts(string[] words)
        {
            return words
                .GroupBy(i => i)
                .ToDictionary(x => x.Key, x => x.Count());
        }
    }
}