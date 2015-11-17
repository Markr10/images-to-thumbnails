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
    class Threads
    {
        // Initiate vars
        Task[] taskArray;
        int taskIndex = 0;
        string filePath;
        Size boxSize;
        FitMode fitMode;
        bool overwriteExistingfiles;
        string taskNumber;
        string threadName;
        
        // Makes a new threead 
        public Threads(int threadNumber, int arrayLength)
        {
            threadName = "Thread " + threadNumber;
            taskArray = new Task[arrayLength];
            Console.WriteLine("New thread is made, " + threadName + " has this many tasks: " + taskArray.Length);
        }

        // Add a task to this thread
        public void addTask(Task task)
        {
            taskArray[taskIndex] = task;
            taskIndex++;
        }

        // Return the tasknumber
        public string getTaskNumber()
        {
            return taskNumber;
        }

        // Resize all tasks currently in this thread
        public void startThreadProcess()
        {
            foreach (Task task in taskArray)
            {
                try
                {
                    filePath = task.getPath();
                    boxSize = task.getSize();
                    fitMode = task.getFitMode();
                    overwriteExistingfiles = task.getOverwrite();
                    taskNumber = task.getTaskNumber();

                    CreateAndSaveImage(filePath, boxSize, fitMode, overwriteExistingfiles);
                    Console.WriteLine(threadName + " is converting " + filePath);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(taskNumber + " at " + threadName + " - Could not successfully create a thumbnail of \"" + Path.GetFileName(filePath) + "\": " + exception.Message + "\r\n");
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
    }
}
