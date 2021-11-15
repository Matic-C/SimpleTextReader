using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.ComponentModel;

namespace TxtReader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly WordCounter _wordCounter;

        public MainWindow()
        {
            InitializeComponent();
            progressBar.Value = 0;
            _wordCounter = new WordCounter();
            _wordCounter.worker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
            _wordCounter.worker.ProgressChanged += backgroundWorker_ProgressChanged;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Open file select window
            Microsoft.Win32.OpenFileDialog openTxtFile = new Microsoft.Win32.OpenFileDialog()
            {
                InitialDirectory = @"C:\Users\%UserProfile%\Desktop",
                Title = "Browse Text Files",
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "txt",
                Filter = "txt files (*.txt)|*.txt",
                FilterIndex = 2,
                RestoreDirectory = true,
                ReadOnlyChecked = true,
                ShowReadOnly = true
            };

            if (openTxtFile.ShowDialog() == true)
            {
                try
                {
                    if ((openTxtFile.OpenFile()) != null)
                    {
                        // Disable start button.
                        start.IsEnabled = false;
                        // Go
                        _wordCounter.worker.RunWorkerAsync(openTxtFile.FileName);
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        // Update progress
        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
            percentage.Text = e.ProgressPercentage.ToString()+" %";
        }

        // Cancel the asynchronous operation.
        private void Cancel_Click(System.Object sender, System.EventArgs e)
        {
            _wordCounter.worker.CancelAsync();
        }

        // Completed operation
        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled != true)
            {
                List<Results> row = new List<Results>();

                var sortedDictionary = (Dictionary<string, int>)e.Result;
                
                foreach (KeyValuePair<string, int> kvp in sortedDictionary.OrderByDescending(key => key.Value))
                {
                    row.Add(new Results() { Word = kvp.Key, Occurrence = kvp.Value });
                }

                wordResults.ItemsSource = row;
                progressBar.Value = 100;
                percentage.Text = "100 %";
                MessageBox.Show("Done");
            }
            else
            {
                MessageBox.Show("Canceled");
            }
            
            // Enble start button.
            start.IsEnabled = true;
        }

        public class Results
        {
            public string Word { get; set; }
            public int Occurrence { get; set; }
        }

    }
}
