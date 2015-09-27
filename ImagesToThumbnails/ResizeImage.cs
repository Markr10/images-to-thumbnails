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

        private void button1_Click(object sender, EventArgs e)
        {
            // When the user selected a folder try to process all files
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
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
                StartProcessingFiles(folderBrowserDialog.SelectedPath, boxSize, fitMode, cbOverwriteExistingfiles.Checked, taskName, numberOfThreads);
            }
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
            string[] files = Directory.GetFiles(directoryPath);

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
            for (int i = 0, filesIndex = 0; i < numberOfThreads; i++)
            {
                // Number of files for the last thread
                // REMARK Verdeel de files eerlijk/evenrediger? over de threads
                // Voorkomt ingewikkelde code die out of bounds gaat 
                if (numberOfThreads - (files.Length % numberOfThreads) == i)
                {
                    filesPerThread++;
                }

                // Create array with file paths to process
                string[] threadFiles = new string[filesPerThread];
                // REMARK Copy array method?
                // filesIndex wordt automatisch berekend omdat deze nu bewaard blijft
                for (int j = 0; j < filesPerThread; j++, filesIndex++)
			    {
			        threadFiles[j] = files[filesIndex];
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
                ProcessFiles(threadFiles, boxSize, fitMode, overwriteExistingfiles, taskName, "thread " + (i + 1));
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
                tbOutput.AppendText(taskName + " - The thumbnails are created successfully.\r\n");
            }

            tbOutput.AppendText(taskName + " finished.\r\n");
        }

        /// <summary>
        /// // REMARK
        /// </summary>
        /// <param name="files"></param>
        /// <param name="boxSize"></param>
        /// <param name="fitMode"></param>
        /// <param name="overwriteExistingfiles"></param>
        private void ProcessFiles(string[] files, Size boxSize, FitMode fitMode, bool overwriteExistingfiles, string taskName, string threadName)
        {
            foreach (string filePath in files)
            {
                try
                {
                    CreateAndSaveImage(filePath, boxSize, fitMode, overwriteExistingfiles);
                }
                catch (Exception exception)
                {
                    // TODO Let set the completion method to an error when this occurs
                    if (noErrors)
                    {
                        noErrors = false;
                    }

                    // Show the problem immediately to the user.
                    ProcessingFilesUpdate(taskName, threadName, filePath, exception);
                }
            }
        }

        private void CreateAndSaveImage(string filePath, Size boxSize, FitMode fitMode, bool overwriteExistingfiles)
        {
            // Avoid unnecessary converting of files.
            // Therefore use the output encoder to detect if the file extension is supported.
            // In the end it determines whether the file can be saved and it supports less formats then the Bitmap class.
            DefaultEncoder encoder = new DefaultEncoder(filePath);
            
            using (Bitmap newImage = CreateImage(filePath, boxSize, fitMode))
            {
                SaveImage(filePath, boxSize, fitMode, newImage, overwriteExistingfiles, encoder);
            }
        }

        private Bitmap CreateImage(string filePath, Size boxSize, FitMode fitMode)
        {
            // Create the variable for the new image
            Bitmap newImage;
            // Don't use Image.FromFile because it locks the file. Use instead a stream and copy it to another stream to release it early.
            // Furthermore according KB814675 keep the (second/memory) stream open for the lifetime of the image.
            // That means that there must be a reference to it between methods or the bitmap must be 'pixel perfect' copied (not Cloned).
            // Use using (with for example Graphics, Bitmap or Stream objects) to dispose the memory earlier.
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Allow early disposal of the stream of the original image
                using (FileStream sourceStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    sourceStream.CopyTo(memoryStream);
                }

                using (Bitmap originalImage = new Bitmap(memoryStream))
                {
                    // Calculate the new size
                    Size newImageSize = ToNewSize(originalImage.Size, boxSize, fitMode);

                    // Create and draw the new image
                    newImage = new Bitmap(newImageSize.Width, newImageSize.Height, originalImage.PixelFormat);
                    using (Graphics graphics = Graphics.FromImage(newImage))
                    {
                        // Set better graphics defaults
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic; // Ensure high quality images. HQ Bicubic is 2 pass. It's only 30% slower than low quality.
                        graphics.SmoothingMode = SmoothingMode.HighQuality; // Ensure crisp edges
                        graphics.PixelOffsetMode = PixelOffsetMode.HighQuality; // Prevent artifacts at the edges
                        graphics.CompositingQuality = CompositingQuality.HighQuality; // Ensure that matted PNGs look decent
                        graphics.CompositingMode = CompositingMode.SourceOver; // Prevent really ugly transparency issues

                        using (ImageAttributes imageAttributes = new ImageAttributes())
                        {
                            // Fixes the 50% gray border issue on (mainly bright white or dark) images.
                            imageAttributes.SetWrapMode(WrapMode.TileFlipXY);

                            graphics.DrawImage(originalImage, ToPointArray(newImageSize), ToRectangle(originalImage.Size), GraphicsUnit.Pixel, imageAttributes);
                        }

                        graphics.Flush(FlushIntention.Flush);  // REMARK Research parameter - Not sure if this is still needed
                    }
                }
            }

            return newImage;
        }

        private void SaveImage(string originalFilePath, Size boxSize, FitMode fitMode, Bitmap newImage, bool overwriteExistingfiles, IEncoder encoder)
        {
            // REMARK Move to method comments
            // Encoder only needed to write files, so the interface provides enough methods.
            // Checks if it support the file format in the given path, so create this object before everything else.
            // If you doesn't do this it is for example possible to create empty/overwrite existing files
            // because then a (new) empty file is created with the file stream method.

            // Creates, if needed, the sub directory
            string dirName = Path.Combine(Path.GetDirectoryName(originalFilePath), boxSize.Width + "x" + boxSize.Height + " - " + fitMode.ToSentenceCase());
            Directory.CreateDirectory(dirName);

            // Create a platform independent file path
            string newFilePath = Path.Combine(dirName, Path.GetFileNameWithoutExtension(originalFilePath)) +
            "(" + newImage.Width + "x" + newImage.Height + ")" + Path.GetExtension(originalFilePath);

            // REMARK
            // Set whether files should be overwritten.
            FileMode fileMode;
            if (overwriteExistingfiles)
            {
                // Overwrite existing files
                fileMode = FileMode.Create;
            }
            else
            {
                // Don't overwrite existing files
                fileMode = FileMode.CreateNew;
            }

            // Save image to the new location using a stream
            using (FileStream newImageStream = new FileStream(newFilePath, fileMode, FileAccess.Write))
            {
                //Save to stream
                encoder.Write(newImage, newImageStream);
                newImageStream.Flush(true);
            }
        }

        private static Size ToNewSize(Size originalSize, Size boxSize, FitMode fitMode)
        {
            // REMARK
            if (fitMode == FitMode.Fit)
            {
                // Aspect ratio of the source image
                double imageRatio = (double)originalSize.Width / (double)originalSize.Height;
                // Aspect ratio of the bounding box
                double boxRatio = (double)boxSize.Width / (double)boxSize.Height;

                if (boxRatio > imageRatio)
                {
                    if (imageRatio < (1 / (double)boxSize.Height))
                    {
                        // Aspect ratio can not be maintained.
                        throw new ArgumentException("Aspect ratio can not be maintained. Height of image is too large.");
                    }

                    // Bound by height. Otherwise (so when you bound by width) the height will be out of bounds.
                    return new Size((int)Math.Round((imageRatio * boxSize.Height)), boxSize.Height);
                }
                else if (imageRatio < boxSize.Width) // Prevents that it aspect ratio can not be maintained.
                {
                    // Bound by width. Otherwise (so when you bound by height) the width will be out of bounds.
                    // Or the aspect ratios are identical.
                    return new Size(boxSize.Width, (int)Math.Round((boxSize.Width / imageRatio)));
                }
                else
                {
                    // Aspect ratio can not be maintained.
                    throw new ArgumentException("Aspect ratio can not be maintained. Width of image is too large.");
                }
            }
            else if (fitMode == FitMode.Stretch)
            {
                return boxSize;
            }
            else
            {
                // REMARK Spelling
                throw new ArgumentException("Fit mode is not supported.");
            }
        }

        /// <summary>
        /// Converts the new size of an image to a point array with three points that forming a scalene triangle.
        /// This point array could be used as the destPoints parameter for drawing the new image.
        /// That needs three points of a parallelogram. The fourth point will be extrapolated from the first three.
        /// </summary>
        /// <param name="newImageSize">The size of the new image</param>
        /// <returns>A point array with three points that forming a scalene triangle</returns>
        private static Point[] ToPointArray(Size newImageSize)
        {
            // HARCODED VALUES
            Point[] returnPointArray = new Point[3];
            returnPointArray[0] = new Point(0, 0); // Upper-left corner (x, y)
            returnPointArray[1] = new Point(newImageSize.Width, 0); // Upper-right corner (width, y)
            returnPointArray[2] = new Point(0, newImageSize.Height); // Lower-left corner (x, height)
            
            return returnPointArray;
        }

        /// <summary>
        /// Converts the size of an image to a rectangle object.
        /// </summary>
        /// <param name="imageSize">The size of the image</param>
        /// <returns>A rectangle object based on the provided values</returns>
        private static Rectangle ToRectangle(Size imageSize)
        {
            // HARCODED VALUES
            return new Rectangle(0, 0, imageSize.Width, imageSize.Height);
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
