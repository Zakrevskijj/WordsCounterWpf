using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

        string fileText = "";

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                fileText = File.ReadAllText(openFileDialog.FileName);
                filePathTextBox.Text = openFileDialog.FileName;
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                await Task.Run(() =>
                {
                    var entries = fileText.Split(Array.Empty<char>(), StringSplitOptions.RemoveEmptyEntries);
                    Dispatcher.Invoke(() =>
                    {
                        parsingStatusProgressBar.Maximum = entries.Length;
                    });
                    
                    var dict = new Dictionary<string, int>();
                    for (int i = 0; i < entries.Length; i++)
                    {
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
                    }

                    var resultedText = "";
                    foreach (var keyValuePair in dict)
                    {
                        resultedText += keyValuePair.Key + ": " + keyValuePair.Value + Environment.NewLine;
                    }
                    Dispatcher.Invoke(() =>
                    {
                        resultsTextBox.Text = resultedText;
                    });
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
