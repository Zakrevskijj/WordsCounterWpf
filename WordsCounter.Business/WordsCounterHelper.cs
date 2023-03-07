namespace WordsCounter.Business
{
    public class WordsCounterHelper
    {
        private string _filePath;
        public WordsCounterHelper(string filePath)
        {
            _filePath = filePath;
        }

        public Dictionary<string, int> ParseFileAndCountWords(Action<int> setProgressMax, Action iterateProgressTracker, CancellationToken cancellationToken = default)
        {
            var fileText = File.ReadAllText(_filePath);
            //ToDo: Error handling

            var entries = fileText.Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);

            setProgressMax(entries.Length);

            var dict = new Dictionary<string, int>();
            for (int i = 0; i < entries.Length; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    throw new TaskCanceledException();
                }
                if (dict.TryGetValue(entries[i], out int curValue))
                {
                    dict[entries[i]] = curValue + 1;
                }
                else
                {
                    dict.Add(entries[i], 1);
                }
                iterateProgressTracker();
            }
            return dict;
        }

        public Dictionary<string, int> ParseFileAndCountWords()
        {
            var fileText = File.ReadAllText(_filePath);
            //ToDo: Error handling

            var entries = fileText
                .Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);

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