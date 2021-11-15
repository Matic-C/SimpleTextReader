using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;

namespace TxtReader
{
    class WordCounter
    {
        public readonly BackgroundWorker worker = new BackgroundWorker();

        public WordCounter()
        {
            worker = new BackgroundWorker { WorkerReportsProgress = true };
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += ProcessTxt;
        }

        public void ProcessTxt(object sender, DoWorkEventArgs e)
        {
            // Get the BackgroundWorker
            BackgroundWorker worker = sender as BackgroundWorker;

            var file = e.Argument as string;
            var filename = file; //Open file
            var txtContent = File.ReadAllText(filename).Split(new[] { ' ', '\n', '\r'}, StringSplitOptions.RemoveEmptyEntries); //Split words
            Dictionary<string, int> RepeatedWordCount = new Dictionary<string, int>();

            var j = 0;
            for (int i = 0; i < txtContent.Length; i++) //loop the splited string  
            {
                if (worker.CancellationPending == true)
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    j++;
                    if (RepeatedWordCount.ContainsKey(txtContent[i])) // Check if word already exists  
                    {
                        int value = RepeatedWordCount[txtContent[i]];
                        RepeatedWordCount[txtContent[i]] = value + 1;
                    }
                    else
                    {
                        RepeatedWordCount.Add(txtContent[i], 1);  // Add repeated string 
                    }

                    // Avoid program freezing by decreasing ReportProgress operations
                    if (j > 1000)
                    {
                        if (j % 1000 == 0)
                        {
                            float percent = ((float)(j + 1) / txtContent.Length) * 100;
                            worker.ReportProgress((int)percent);
                        }
                    }
                    else
                    {
                        float percent = ((float)(j + 1) / txtContent.Length) * 100;
                        worker.ReportProgress((int)percent);
                    }
                }   
            }
            e.Result = RepeatedWordCount;
        }
    }
}
