using Ariadne.Collections;
using ImageResizer.Encoding;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImagesToThumbnails
{
    public partial class ResizeImage : Form
    {
        public ResizeImage()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        // TODO Method comments
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            // Returns the full filepath
            string filePath = openFileDialog1.FileName;

            // Collect exceptions for the dialog
            ThreadSafeDictionary<string, Exception> exceptions = new ThreadSafeDictionary<string, Exception>();
            try
            {
                CreateAndSaveImage(filePath);
            }
            catch (Exception exception)
            {
                exceptions.Add(Path.GetFileName(filePath), exception);
            }

            // Show dialog after processing all images
            // Create dialog text
            string messageboxTitle = String.Empty;
            string messageboxText = String.Empty;

            if(exceptions.Count > 0)
            {
                messageboxTitle = "Not successfully created all thumbnails";
                messageboxText = "Some thumbnails where not successfully created:\n\n";
                foreach (KeyValuePair<string, Exception> entry in exceptions)
                {
                    messageboxText += entry.Key + "\t" + entry.Value.Message + "\n";
                }
            }
            else
            {
                messageboxTitle = "Successfully created";
                messageboxText = "The thumbnails are successfully created.";
            }
            MessageBox.Show(messageboxText, messageboxTitle);
        }

        private void CreateAndSaveImage(string filePath)
        {
            using (Bitmap newImage = CreateImage(filePath))
            {
                SaveImage(filePath, newImage);
            }
        }

        private Bitmap CreateImage(string filePath)
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
                    Size newImageSize = CalculateSize(originalImage.Size);

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

                            graphics.DrawImage(originalImage, ToPointArray(newImageSize), ToRectangle(newImageSize), GraphicsUnit.Pixel, imageAttributes);
                        }

                        graphics.Flush(FlushIntention.Flush);  // REMARK Research parameter - Not sure if this is still needed
                    }
                }
            }

            return newImage;
        }

        private void SaveImage(string originalFilePath, Bitmap newImage)
        {
            // Encoder only needed to write files, so the interface provides enough methods.
            // Checks if it support the file format in the given path, so create this object before everything else.
            // If you doesn't do this it is for example possible to create empty/overwrite files
            // because then a (new) empty file is created with the file stream method.
            IEncoder encoder = new DefaultEncoder(originalFilePath);

            // REMARK Dynamically use the correct size
            // Creates, if needed, the sub directory
            string dirName = Path.Combine(Path.GetDirectoryName(originalFilePath), "100x100 - Fit");
            Directory.CreateDirectory(dirName);

            // Create a platform independent file path
            string newFilePath = Path.Combine(dirName, Path.GetFileNameWithoutExtension(originalFilePath)) +
            "(" + newImage.Width + "x" + newImage.Height + ")" + Path.GetExtension(originalFilePath);

            // REMARK
            // Set whether files should be overwritten.
            FileMode fileMode;
            if (false)
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

        private Size CalculateSize(Size originalSize)
        {
            Size boxSize = new Size(100, 100);
            // REMARK
            //if()
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
                        throw new ArgumentException("Aspect ratio can not be maintained. Height of image is too large");
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
                    throw new ArgumentException("Aspect ratio can not be maintained. Width of image is too large");
                }
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
        /// Converts the new size of an image to a rectangle object.
        /// </summary>
        /// <param name="newImageSize">The size of the new image</param>
        /// <returns>A rectangle object based on the provided values</returns>
        private static Rectangle ToRectangle(Size newImageSize)
        {
            // HARCODED VALUES
            return new Rectangle(0, 0, newImageSize.Width, newImageSize.Height);
        }
    }
}
