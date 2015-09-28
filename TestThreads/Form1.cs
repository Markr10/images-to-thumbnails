using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace TestThreads
{
    public partial class Form1 : Form
    {
        public int i, o = 0;
        public string name1;
        public ValueObject[] objectArray1 = new ValueObject[10];

        public Form1()
        {
            InitializeComponent();
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // This method runs on the main thread.
            Words.CurrentState state =
                (Words.CurrentState)e.UserState;
            this.LinesCounted.Text = state.LinesCounted.ToString();
            this.WordsCounted.Text = state.WordsMatched.ToString();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // This event handler is called when the background thread finishes. 
            // This method runs on the main thread. 
            if (e.Error != null)
                MessageBox.Show("Error: " + e.Error.Message);
            else if (e.Cancelled)
                MessageBox.Show("Word counting canceled.");
            else
                MessageBox.Show("Finished counting words.");
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
                // This event handler is where the actual work is done. 
                // This method runs on the background thread. 

                // Get the BackgroundWorker object that raised this event.
                System.ComponentModel.BackgroundWorker worker;
                worker = (System.ComponentModel.BackgroundWorker)sender;

                // Get the Words object and call the main method.
                Words WC = (Words)e.Argument;
                WC.CountWords(worker, e);
            }

            private void StartThread()
            {
                // This method runs on the main thread. 
                this.WordsCounted.Text = "0";

                // Initialize the object that the background worker calls.
                Words WC = new Words();
                WC.CompareString = this.CompareString.Text;
                WC.SourceFile = this.SourceFile.Text;

                // Start the asynchronous operation.
                backgroundWorker1.RunWorkerAsync(WC);

            }

            private void Start_Click(object sender, EventArgs e)
            {
                StartThread();
            }

            private void Cancel_Click(object sender, EventArgs e)
            {
                // Cancel the asynchronous operation. 
                this.backgroundWorker1.CancelAsync();
            }

            private void button1_Click(object sender, EventArgs e)
            {
                
                //ThreadSleeper sleepThread = new ThreadSleeper("I'm alive!");
                //for(i = 1; i < 4; i++)
                //{
                //    //string no = i.ToString();
                //    Thread myThread = new Thread(new ThreadStart(sleepThread.mySleepMethod));
                //    myThread.Name = "thread" + i;
                //    System.Diagnostics.Debug.WriteLine(myThread.Name + " is going!!!");
                //    myThread.Start();
                //}
                //// TEST!! Make threads and let them say they live in debug window
                //for (int q = 0; q < 3; q++)
                //{
                //    Thread myThread = new Thread(new ThreadStart(sleepThread.DoSomeWork));
                //    myThread.Name = "Thread 00" + q;
                //    sleepThread.ShareSomeInfo("some parameter stuff");
                //    myThread.Start();
                //}

                // Test; make array to store valueObjects
                


                // Test; make task into object and let thread run it
                for(int d = 0; d < 5; d++)
                {
                    ValueObject task = new ValueObject(d, "task" + d, true);
                    System.Diagnostics.Debug.WriteLine(task.getName());
                    objectArray1[d] = task;
                }

                for(int r = 0; r < 5; r++)
                {
                    
                }
                

                //Show whats in the array
                //int counter = 0;
                //foreach (object element in objectArray1)
                //{
                //    if (element != null) // Avoid NullReferenceException
                //    {
                //        Console.WriteLine(counter +"~"+element.ToString() +" -- "+element.GetType());
                //        counter++;
                //    }
                //}

                //make some threads and let them get names
                for (int q = 0; q < 5; q++)
                {
                    ThreadSleeper sleepThread = new ThreadSleeper(objectArray1[q]);
                    sleepThread.ShareSomeInfo(objectArray1[q].getName());
                    Thread myThread = new Thread(new ThreadStart(sleepThread.DoSomeWork));
                    myThread.Name = "Thread" + q;
                    myThread.Start();
                }
            }
        }
}
