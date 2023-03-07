using System.Threading.Tasks;

namespace WordsCounter.Business
{
    public class WordsCounterHelper
    {
        private string _filePath;
        public WordsCounterHelper(string filePath)
        {
            _filePath = filePath;
        }

        public Dictionary<string, int> ParseFileAndCountWords(CancellationToken cancellationToken)
        {
            var fileText = File.ReadAllText(_filePath);
            if (cancellationToken.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }

            var entries = fileText
                .Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);
            if (cancellationToken.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }

            return CalculateWordCounts(entries);
        }

        private static Dictionary<string, int> CalculateWordCounts(string[] entries)
        {
            return entries
                .GroupBy(i => i)
                .ToDictionary(x => x.Key, x => x.Count());
        }
    }
}