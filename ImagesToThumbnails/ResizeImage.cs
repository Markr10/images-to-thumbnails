using Ariadne.Collections;
using Extension;
using ImageResizer.Encoding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImagesToThumbnails
{
    public partial class ResizeImage : Form
    {
        // TODO Use a seperate thread safe counter class to keep track of the running threads
        // TODO Make thread safe methods for increasing and decreasing, if possible use 'this' for the lock in that class.
        private ThreadSafeDictionary<string, Thread> tasks;
        /// <summary>
        /// Keeps track of the number for the name of the next task.
        /// This number is unrelated to the running tasks.
        /// </summary>
        private int tasksCounter;
        // TODO Encapsulation in a class?
        private bool noErrors;
        private string SelectedPath;
        

        public ResizeImage()
        {
            InitializeComponent();
            tasks = new ThreadSafeDictionary<string, Thread>();
            tasksCounter = 0;

            // Open automatically the My Pictures/Pictures folder
            // TODO Uncomment
            //folderBrowserDialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            // TODO Shutdown all (foreground) threads on application shutdown!
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                SelectedPath = folderBrowserDialog.SelectedPath;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // When the user selected a folder try to process all files
                int numberOfThreads = -1;
                if (! cbAutoCalcThreads.Checked)
                {
                    numberOfThreads = (int)nudThreads.Value;
                }

                // Grab size
                // Initialise with default value to prevent use of unassigned local variable and convert to null problem.
                Size boxSize = new Size(100, 100);
                if (rbSize200x200.Checked)
                {
                    boxSize = new Size(200, 200);
                }
                else if (! rbSize100x100.Checked)
                {
                    throw new ArgumentException("Size is not selected or not supported.");
                }

                // Grab fitMode
                // Initialise with default value to prevent use of unassigned local variable and convert to null problem.
                FitMode fitMode = FitMode.Fit;
                if (rbFitModeFitHeight.Checked)
                {
                    fitMode = FitMode.FitHeight;
                }
                else if (rbFitModeFitWidth.Checked)
                {
                    fitMode = FitMode.FitWidth;
                }
                else if (rbFitModeStretch.Checked)
                {
                    fitMode = FitMode.Stretch;
                }
                else if(! rbFitModeFit.Checked)
                {
                    throw new ArgumentException("Fit mode is not selected or not supported.");
                }

                // Start task as thread
                string taskName = "Task " + ++tasksCounter;

                // TODO Create thread, maybe as backgroundworker
                //      So create a custom backgroundworker class with a property to set the name (NamedBackgroundWorker?)
                // TODO Set the do worker event handler of Backgroundworker as a lambda Expressions?
                //      Pass the parameters of the StartProcessingFiles call below.
                // TODO Replace with commented line: remove next line and uncomment the second next line
                // TODO Then replace it with the newly created increase method of the counter class 
                tasks.Add(taskName, null);
                //tasks.Add(taskName, backgroundworker);

                // TODO Start thread/backgroundworker
                //      Maybe remove taskName
                // Start processing with selected arguments
                StartProcessingFiles(SelectedPath, boxSize, fitMode, cbOverwriteExistingfiles.Checked, taskName, numberOfThreads);
        }

        // TODO Rename method and change parameters to come in accordance with the backgroundworker
        private void StartProcessingFiles(string directoryPath, Size boxSize, FitMode fitMode, bool overwriteExistingfiles, string taskName, int numberOfThreads)
        {
            // REMARK
            // Validate arguments
            // There is no need to check fitMode because it is an not nullable enum. That means that this argument will never be null, and will contain a valid FitMode value.
            if (boxSize == null)
            {
                throw new ArgumentNullException("boxSize");
            }
            else if (numberOfThreads < 1 && numberOfThreads != -1)
            {
                throw new ArgumentException("numberOfThreads should be -1 or > 0.");
            }

            // TODO Remove the next line or place it to another location
            noErrors = true;

            // Change cursor to notify the user that we are busy.
            if (Cursor.Current != Cursors.WaitCursor)
            {
                Cursor.Current = Cursors.WaitCursor;
                // REMARK maybe needed to add to show the cursor - Application.DoEvents();
            }

            // TODO Replace with commented line: remove next line and uncomment the second next line
            tbOutput.AppendText(taskName + " started...\r\n");
            //tbOutput.AppendText(NamedBackgroundWorker)sender.Name + " - Started...\r\n");

            // Get all files
            Console.WriteLine("------------------------");
            Console.WriteLine("Just made a list of files! Here they are:");
            string[] files = Directory.GetFiles(directoryPath);
            foreach (string element in files)
            {
                if (element != null) // Avoid NullReferenceException
                {
                    Console.WriteLine(element);
                }
            }
            Console.WriteLine("------------------------");

            // Calculate the number of files per thread
            // Don't create threads with no images to process
            // REMARK // Stel gelijk de standaard instelling in omdat anders het twee keer zal gebeuren
            int filesPerThread = 1;
            if(numberOfThreads > 0 && numberOfThreads <= files.Length)
            {

                // REMARK spelling and calculation
                // Floors integer automatically
                filesPerThread = files.Length / numberOfThreads; 
            }
            else
            {
                // prevent setting value twice
                if (numberOfThreads <= files.Length)
                {
                    // throw message
                }
                // REMARK
                // Automatically calculate threads
                numberOfThreads = files.Length;
            }

            

            // TODO Use CountdownEvent with numberOfThreads to keep track of the finished threads.
            // REMARK

            Task[] taskArray = new Task[files.Length];
            int threadNumber = 0;

            tbOutput.AppendText("Number of files: " + files.Length + " Number of threads to be made: " + numberOfThreads + " Tasks per thread: " + filesPerThread + "\r\n");
            Console.WriteLine("Number of files: " + files.Length + " Number of threads to be made: " + numberOfThreads + " Tasks per thread: " + filesPerThread);

            for (int i = 0; i < numberOfThreads; i++)
            {
                // Number of files for the last thread
                // REMARK Verdeel de files eerlijk/evenrediger? over de threads
                // Voorkomt ingewikkelde code die out of bounds gaat 
                if (numberOfThreads - (files.Length % numberOfThreads) == i)
                {
                    filesPerThread++;
                }

                // Calculate filesIndex before increasing
                // filesIndex // REMARK behoud de juiste index
                // REMARK Verdeel elk bestand zo eerlijk mogelijk over een thread
                // Voorkomt een vergelijking met 0 en -1 om de index te verkrijgen
                //filesIndex += filesPerThread;

                // TODO Setup the custom backgroundworker
                //      Start thread within that method
                //      Maybe remove some parameters
                // REMARK Thread name is in lower case for the style
                string taskNumber = "task" + i;
                Task task = new Task(files[i], boxSize, fitMode, overwriteExistingfiles, taskNumber);
                taskArray[i] = task;

                Threads mythread = new Threads(taskArray[i], threadNumber);
                Thread newThread = new Thread(new ThreadStart(mythread.startThreadProcess));
                newThread.Start();
                threadNumber++;
                
                }

                

            // TODO Use the CountdownEvent to wait to all threads are done and then call this method.
            FinishProcessingFiles(taskName, noErrors);
        }

        // TODO Rename method and change parameters to come in accordance with the backgroundworker
        private void ProcessingFilesUpdate(string taskName, string threadName, string filePath, Exception exception)
        {
            // TODO Maybe include an invoke check.
            //      This new method must be a called on an event (callback).
            tbOutput.AppendText(taskName + " at " + threadName + " - Could not successfully create a thumbnail of \"" + Path.GetFileName(filePath) + "\": " + exception.Message + "\r\n");
        }

        // TODO Rename method and change parameters to come in accordance with the backgroundworker
        private void FinishProcessingFiles(string taskName, bool noErrors)
        {
            // TODO Replace with commented line: remove next line and uncomment the second next line
            // TODO Then replace it with the newly created decrease method of the counter class 
            tasks.Remove("Task " + tasksCounter);
            //tasks.Remove((NamedBackgroundWorker)sender.Name);

            if (tasks.Count == 0)
            {
                // At this point there is no calculation so reset the pointer
                Cursor.Current = Cursors.Default;
            }

            if (noErrors)
            {
                // Report at least once the progress of the task.
                tbOutput.AppendText(taskName + " The thumbnails are created successfully.\r\n");
            }

            tbOutput.AppendText(taskName + " finished.\r\n");
        }

        private void tbOutput_TextChanged(object sender, EventArgs e)
        {
            // Enable the scrollbar when it is needed. No need to disable it again because the text will only increase.
            
            if (tbOutput.ScrollBars == ScrollBars.None && tbOutput.Lines.Length > 15)
            {
                tbOutput.ScrollBars = ScrollBars.Vertical;
            }
        }

        private void cbAutoCalcThreads_CheckedChanged(object sender, EventArgs e)
        {
            // Disable number of threads control when they should be automatically calculated
            nudThreads.Enabled = !cbAutoCalcThreads.Checked;
        }
    }
}
