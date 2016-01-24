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
        private ThreadSafeDictionary<string, Thread> tasks;
        /// <summary>
        /// Keeps track of the number for the name of the next task.
        /// This number is unrelated to the running tasks.
        /// </summary>
        private int tasksCounter;
        private bool noErrors;
        private string SelectedPath;

        /// <summary>
        /// Constructor
        /// </summary>
        public ResizeImage()
        {
            InitializeComponent();
            tasks = new ThreadSafeDictionary<string, Thread>();
            tasksCounter = 0;

            // Open automatically the My Pictures/Pictures folder
            folderBrowserDialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            // TODO Shutdown all (foreground) threads on application shutdown!
        }

        /// <summary>
        /// Output textbox change event
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event arguments</param>
        private void tbOutput_TextChanged(object sender, EventArgs e)
        {
            // Enable the scrollbar when it is needed. There is no need to disable it again because the text will only increase.
            if (tbOutput.ScrollBars == ScrollBars.None && tbOutput.Lines.Length > 15)
            {
                tbOutput.ScrollBars = ScrollBars.Vertical;
            }
        }

        /// <summary>
        /// Automatically calculation of threads checkbox checked changed event
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event arguments</param>
        private void cbAutoCalcThreads_CheckedChanged(object sender, EventArgs e)
        {
            // Disable number of threads control when they should be automatically calculated
            nudThreads.Enabled = !cbAutoCalcThreads.Checked;
        }

        /// <summary>
        /// Folder button click event
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event arguments</param>
        private void btnFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                SelectedPath = folderBrowserDialog.SelectedPath;
            }
        }

        /// <summary>
        /// Resize button click event
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event arguments</param>
        private void btnResize_Click(object sender, EventArgs e)
        {
            // When the user selected a folder try to process all files
            int numberOfThreads = -1;
            if (!cbAutoCalcThreads.Checked)
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
            else if (!rbSize100x100.Checked)
            {
                throw new ArgumentException("Size is not selected or not supported.");
            }

            // Grab fitMode
            // Initialise with default value to prevent use of unassigned local variable and convert to null problem.
            FitMode fitMode = FitMode.Fit;
            if (rbFitModeStretch.Checked)
            {
                fitMode = FitMode.Stretch;
            }
            else if (!rbFitModeFit.Checked)
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
            // Debug output:: Console.WriteLine("------------------------");
            // Debug output:: Console.WriteLine("Just made a list of files! Here they are:");
            string[] files = Directory.GetFiles(directoryPath);
            // Debug output:: foreach (string element in files)
            // Debug output:: {
            // Debug output::     if (element != null) // Avoid NullReferenceException
            // Debug output::     {
            // Debug output::         Console.WriteLine(element);
            // Debug output::     }
            // Debug output:: }
            // Debug output:: Console.WriteLine("------------------------");

            // Calculate the number of files per thread
            // Don't create threads with no images to process
            // REMARK // Stel gelijk de standaard instelling in omdat anders het twee keer zal gebeuren
            int filesPerThread = 1;
            if (numberOfThreads > 0 && numberOfThreads < files.Length)
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

            // Create an array to store the tasks in, same length as files
            Task[] taskArray = new Task[files.Length];
            int threadNumber = 0;

            // Show how many files, threads and filesPerThread
            tbOutput.AppendText("Number of files: " + files.Length + " Number of threads to be made: " + numberOfThreads + " Tasks per thread: " + filesPerThread + "\r\n");
            // Debug output:: Console.WriteLine("Number of files: " + files.Length + " Number of threads to be made: " + numberOfThreads + " Tasks per thread: " + filesPerThread);

            // Go through all the files
            for (int i = 0; i < files.Length; i++)
            {
                // Create a task object for every file and put this in the task array
                string taskNumber = "task" + i;
                Task task = new Task(files[i], boxSize, fitMode, overwriteExistingfiles, taskNumber);
                taskArray[i] = task;
            }

            // Initiate all variables, Index is current index in taskArray, countodwn is number of files left to process, 
            // uneven is to make sure all files are being processed even if the result of files.Length / filesPerThread doesnt give a round number
            // leftover is the number of files thats left over after files.Length / filesPerThread
            int taskIndex = 0;
            int countdown = taskArray.Length;
            bool uneven = false;
            int leftover = taskArray.Length % filesPerThread;
            // Debug output:: Console.WriteLine("LEFTOVER: " + leftover);
            if (leftover > 0)
            { uneven = true; }
            // This for loop goes through all tasks
            for (int c = 0; c < taskArray.Length; c++)
            {
                // First part processes all files.
                // First thread processes the leftover too, rest does regular filesPerThread amounts
                if (uneven == true)
                {
                    Threads mythread = new Threads(threadNumber, filesPerThread + leftover);
                    for (int x = 0; x < (filesPerThread + leftover); x++)
                    {
                        mythread.addTask(taskArray[taskIndex]);
                        countdown--;
                        taskIndex++;
                    }
                    uneven = false;
                    c = c + leftover + filesPerThread - 1;
                    Thread newThread = new Thread(new ThreadStart(mythread.startThreadProcess));
                    newThread.Start();
                    threadNumber++;
                }
                // If no leftover, process all threads like regular filesPerThread amounts
                else
                {
                    Threads mythread = new Threads(threadNumber, filesPerThread);
                    for (int t = 0; t < filesPerThread; t++)
                    {
                        mythread.addTask(taskArray[taskIndex]);
                        countdown--;
                        taskIndex++;
                    }
                    c = c + filesPerThread - 1;
                    Thread newThread = new Thread(new ThreadStart(mythread.startThreadProcess));
                    newThread.Start();
                    threadNumber++;
                }
                // Debug output:: Console.WriteLine(taskIndex + ": current index");
            }

            // TODO Use the CountdownEvent to wait to all threads are done and then call this method.
            FinishProcessingFiles(taskName, noErrors);
        }

        /// <summary>
        /// Updates the progress when there is an error.
        /// </summary>
        /// <param name="taskName">The name of the task</param>
        /// <param name="threadName">The name of the thread</param>
        /// <param name="filePath">The file path of the image</param>
        /// <param name="exception">The exception that occurred</param>
        private void ProcessingFilesUpdate(string taskName, string threadName, string filePath, Exception exception)
        {
            // TODO Maybe include an invoke check.
            //      This new method must be a called on an event (callback).
            tbOutput.AppendText(taskName + " at " + threadName + " - Could not successfully create a thumbnail of \"" + Path.GetFileName(filePath) + "\": " + exception.Message + "\r\n");
        }

        /// <summary>
        /// Finishes the task.
        /// </summary>
        /// <param name="taskName">The name of the thread</param>
        /// <param name="noErrors">If there were errors with processing or not</param>
        private void FinishProcessingFiles(string taskName, bool noErrors)
        {
            tasks.Remove("Task " + tasksCounter);

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
    }
}
