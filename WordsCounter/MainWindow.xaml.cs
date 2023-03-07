using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WordsCounter.Business;

namespace WordsCounterApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        string filePath;
        bool isCounting = false;
        CancellationTokenSource cancellationTokenSource;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                filePath = openFileDialog.FileName;
                filePathTextBox.Text = filePath;
                startCountingBtn.IsEnabled = true;
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (isCounting)
            {
                cancellationTokenSource.Cancel();
                ResetCounting();
            }
            else
            {
                isCounting = true;
                startCountingBtn.Content = "Stop Counting";

                cancellationTokenSource = new CancellationTokenSource();
                var cancellationToken = cancellationTokenSource.Token;
                try
                {
                    await Task.Run(() =>
                    {
                        var stopWatch = new Stopwatch();
                        stopWatch.Start();
                        var wordsCounterHelper = new WordsCounterHelper(filePath);
                        var dict = wordsCounterHelper.ParseFileAndCountWords(
                            (int maxProgressValue) => Dispatcher.Invoke(() =>
                            {
                                parsingStatusProgressBar.Maximum = maxProgressValue;
                            }),
                            () => Dispatcher.Invoke(() =>
                            {
                                parsingStatusProgressBar.Value++;
                            }));

                        var wordCounts = dict
                            .Select(x => new WordCount
                            {
                                Word = x.Key,
                                Occurences = x.Value
                            });

                        Dispatcher.Invoke(() =>
                        {
                            resultsDataGrid.ItemsSource = wordCounts;
                            ResetCounting();
                            stopWatch.Stop();
                            durationMsLabel.Content = stopWatch.ElapsedMilliseconds;
                        });
                    }, cancellationToken);
                }
                catch (TaskCanceledException) { }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void ResetCounting()
        {
            isCounting = false;
            startCountingBtn.Content = "Start Counting";
            parsingStatusProgressBar.Value = 0;
        }

        public class WordCount
        {
            public string Word { get; set; }
            public int Occurences { get; set; }
        }
    }
}
