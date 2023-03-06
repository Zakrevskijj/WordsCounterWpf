using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WordsCounter
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

        string? filePath = null;
        bool isCounting = false;
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

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
                        var fileText = File.ReadAllText(filePath);
                        var entries = fileText.Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);
                        Dispatcher.Invoke(() =>
                        {
                            parsingStatusProgressBar.Maximum = entries.Length;
                        });

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
                            Dispatcher.Invoke(() =>
                            {
                                parsingStatusProgressBar.Value++;
                            });
                            //Thread.Sleep(3000);
                        }

                        var resultedText = "";
                        foreach (var keyValuePair in dict)
                        {
                            resultedText += keyValuePair.Key + ": " + keyValuePair.Value + Environment.NewLine;
                        }
                        Dispatcher.Invoke(() =>
                        {
                            resultsTextBox.Text = resultedText;
                            ResetCounting();
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
    }
}
