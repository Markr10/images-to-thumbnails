/* Copyright (c) 2011 Nathanael Jones. See license.txt */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ImageResizer.Encoding
{
    /// <summary>
    /// Provides basic encoding functionality for Jpeg, Png, Gif and Bmp output. Allows adjustable Jpeg compression, but doesn't implement indexed PNG files or quantized GIF files.
    /// </summary>
    public class DefaultEncoder : IEncoder
    {
        private ImageFormat _outputFormat = ImageFormat.Jpeg;
        /// <summary>
        /// If you set this to anything other than Jpeg, Png, Gif or Bmp it will throw an exception. Defaults to Jpeg
        /// </summary>
        public ImageFormat OutputFormat
        {
            get { return _outputFormat; }
            set
            {
                if (!IsValidOutputFormat(value)) throw new ArgumentException(value.ToString() + " is not a valid OutputFormat for DefaultEncoder.");
                _outputFormat = value;
            }
        }

        private int quality = 90;
        /// <summary>
        /// 0..100 value. The Jpeg compression quality. 90 is default and the best setting. It has excellent quality and file size.
        /// Defaults to 90 if passed as a negative number. Numbers over 100 are truncated to 100.
        /// Not relevant in Png or Gif compression
        /// </summary>
        public int Quality
        {
            get { return quality; }
            set { quality = GetValidQuality(value); }
        }

        /// <summary>
        /// Constructor that creates an instance of DefaultEncoder with the default values.
        /// Initialises the default output format to Jpeg with a compression quality of 90.
        /// </summary>
        public DefaultEncoder()
        {
        }

        /// <summary>
        /// Constructor that creates an instance of DefaultEncoder with the given output format.
        /// It also initialises the default value for Jpeg compression quality.
        /// </summary>
        /// <param name="outputFormat">Any supported output format</param>
        public DefaultEncoder(ImageFormat outputFormat)
        {
            this.OutputFormat = outputFormat;
        }

        /// <summary>
        /// Constructor that creates an instance of DefaultEncoder with the default output format set to Jpeg and
        /// the given Jpeg compression quality.
        /// </summary>
        /// <param name="jpegQuality">The Jpeg compression quality. It must be a number between 0 and 100.
        /// Defaults to 90 if passed as a negative number. Numbers over 100 are truncated to 100.
        /// </param>
        public DefaultEncoder(int jpegQuality)
        {
            this.Quality = jpegQuality;
        }

        /// <summary>
        /// Constructor that creates an instance of DefaultEncoder with the ImageFormat of the source image/path.
        /// It also initialises the default value for Jpeg compression quality.
        /// </summary>
        /// <param name="original">A string path or the source image that was loaded from a stream</param>
        public DefaultEncoder(object original)
        {
            //What format was the image originally (used as a fallback).
            ImageFormat originalFormat = GetOriginalFormat(original);
            if (!IsValidOutputFormat(originalFormat))
            {
                throw new ArgumentException("No valid info available about the original format.");
            }

            //Ok, we've found our format.
            this.OutputFormat = originalFormat;
        }


        /// <summary>
        /// Returns true if the this encoder supports the specified image format
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public bool IsValidOutputFormat(ImageFormat f)
        {
            return (ImageFormat.Jpeg.Equals(f) || ImageFormat.Png.Equals(f) || ImageFormat.Gif.Equals(f) || ImageFormat.Bmp.Equals(f));
        }

        #region IEncoder methods
        /// <summary>
        /// Writes the specified image to the stream using Quality (if needed) and OutputFormat
        /// </summary>
        /// <param name="image"></param>
        /// <param name="s"></param>
        public void Write(Image image, System.IO.Stream s)
        {
            if (ImageFormat.Jpeg.Equals(OutputFormat)) SaveJpeg(image, s, this.Quality);
            else if (ImageFormat.Png.Equals(OutputFormat)) SavePng(image, s);
            else if (ImageFormat.Gif.Equals(OutputFormat)) SaveGif(image, s);
            else if (ImageFormat.Bmp.Equals(OutputFormat)) SaveBmp(image, s);
        }

        /// <summary>
        /// Returns the default file extension for OutputFormat
        /// </summary>
        public string Extension
        {
            get { return DefaultEncoder.GetExtensionFromImageFormat(OutputFormat); }
        }
        #endregion


        #region Static methods
        /// <summary>
        /// Attempts to determine the ImageFormat of the source image. First attempts to parse the path, if 'original' is a string.
        /// Falls back to using original.RawFormat. Returns null if both 'original' is null.
        /// RawFormat has a bad reputation, so this may return unexpected values, like MemoryBitmap or something in some situations.
        /// </summary>
        /// <param name="original">A string path or the source image that was loaded from a stream</param>
        /// <returns></returns>
        public static ImageFormat GetOriginalFormat(object original)
        {
            if (original == null) return null;
            //Try to parse the original file extension first.
            string path = original as string;

            //We have a path? Parse it!
            if (path != null)
            {
                ImageFormat f = DefaultEncoder.GetImageFormatFromPhysicalPath(path);
                if (f != null) return f; //From the path
            }
            //Ok, I guess it there (a) wasn't a path, or (b), it didn't have a recognizeable extension
            if (original is Image) return ((Image)original).RawFormat;
            return null;
        }

        /// <summary>
        /// Returns the ImageFormat enumeration value based on the extension in the specified physical path. Extensions can lie, just a guess.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ImageFormat GetImageFormatFromPhysicalPath(string path)
        {
            return GetImageFormatFromExtension(Path.GetExtension(path));
        }

        /// <summary>
        /// Returns an string instance from the specfied ImageFormat. First matching entry in imageExtensions is used.
        /// Returns null if not recognized.
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GetExtensionFromImageFormat(ImageFormat format)
        {
            lock (_syncExts)
            {
                foreach (KeyValuePair<string, ImageFormat> p in imageExtensions)
                {
                    if (p.Value.Guid.Equals(format.Guid)) return p.Key;
                }
            }
            return null;
        }


        private static object _syncExts = new object();
        /// <summary>
        /// Returns a dict of (lowercase invariant) image extensions and ImageFormat values
        /// </summary>
        private static IDictionary<String, ImageFormat> _imageExtensions = null;
        private static IDictionary<String, ImageFormat> imageExtensions
        {
            get
            {
                lock (_syncExts)
                {
                    if (_imageExtensions == null)
                    {
                        _imageExtensions = new Dictionary<String, ImageFormat>();
                        addImageExtension("jpg", ImageFormat.Jpeg);
                        addImageExtension("jpeg", ImageFormat.Jpeg);
                        addImageExtension("jpe", ImageFormat.Jpeg);
                        addImageExtension("jif", ImageFormat.Jpeg);
                        addImageExtension("jfif", ImageFormat.Jpeg);
                        addImageExtension("jfi", ImageFormat.Jpeg);
                        addImageExtension("exif", ImageFormat.Jpeg);
                        addImageExtension("png", ImageFormat.Png);
                        addImageExtension("gif", ImageFormat.Gif);
                        addImageExtension("bmp", ImageFormat.Bmp);
                        //"bmp","gif","jpg","jpeg","jpe","jif","jfif","jfi","exif","png"
                    }
                    return _imageExtensions;
                }
            }
        }

        /// <summary>
        /// Returns an ImageFormat instance from the specfied file extension. Extensions lie sometimes, just a guess.
        /// returns null if not recognized.
        /// </summary>
        /// <param name="ext"></param>
        /// <returns></returns>
        public static ImageFormat GetImageFormatFromExtension(string ext)
        {
            if (string.IsNullOrEmpty(ext)) return null;
            lock (_syncExts)
            {
                ext = ext.Trim(' ', '.').ToLowerInvariant();
                if (!imageExtensions.ContainsKey(ext)) return null;
                return imageExtensions[ext];
            }
        }

        /// <summary>
        /// NOT thread-safe! 
        /// </summary>
        /// <param name="extension"></param>
        /// <param name="matchingFormat"></param>
        private static void addImageExtension(string extension, ImageFormat matchingFormat)
        {
            //In case first call is to this method, use the property. Will be recursive, but that's fine, since it won't be null.
            imageExtensions.Add(extension.TrimStart('.', ' ').ToLowerInvariant(), matchingFormat);
        }

        public static void AddImageExtension(string extension, ImageFormat matchingFormat)
        {
            lock (_syncExts)
            {//In case first call is to this method, use the property. Will be recursive, but that's fine, since it won't be null.
                imageExtensions.Add(extension.TrimStart('.', ' ').ToLowerInvariant(), matchingFormat);
            }
        }


        /// <summary>
        /// Returns a valid Jpeg compression quality value.
        /// </summary>
        /// <param name="quality">The Jpeg compression quality. It must be a number between 0 and 100.
        /// Defaults to 90 if passed as a negative number. Numbers over 100 are truncated to 100.
        /// </param>
        /// <returns>A number between 0 and 100</returns>
        public static int GetValidQuality(int quality)
        {
            if (quality < 0)
            {
                return 90; //90 is a very good default to stick with.
            }
            else if (quality > 100)
            {
                return 100;
            }
            else
            {
                return quality;
            }
        }


        /// <summary>
        /// Returns true if quality is a valid Jpeg compression quality value.
        /// </summary>
        /// <param name="quality"></param>
        /// <returns></returns>
        public static bool IsValidQuality(int quality)
        {
            return ((quality >= 0) && (quality <= 100));
        }

        /// <summary>
        /// Returns the first ImageCodeInfo instance with the specified mime type. Returns null if there are no matches.
        /// </summary>
        /// <param name="mimeType"></param>
        /// <returns></returns>
        public static ImageCodecInfo GetImageCodecInfo(string mimeType)
        {
            ImageCodecInfo[] info = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo ici in info)
                if (ici.MimeType.Equals(mimeType, StringComparison.OrdinalIgnoreCase)) return ici;
            return null;
        }


        /// <summary>
        /// Saves the specified image to the specified stream using jpeg compression of the specified quality.
        /// </summary>
        /// <param name="b"></param>
        /// <param name="quality">A number between 0 and 100. Throws an exception when it is not valid.
        /// 90 is a *very* good setting.
        /// </param>
        /// <param name="target"></param>
        public static void SaveJpeg(Image b, Stream target, int quality)
        {
            #region Encoder paramater notes
            //image/jpeg
            //  The parameter list requires 172 bytes.
            //  There are 4 EncoderParameter objects in the array.
            //    Parameter[0]
            //      The category is Transformation.
            //      The data type is Long.
            //      The number of values is 5.
            //    Parameter[1]
            //      The category is Quality.
            //      The data type is LongRange.
            //      The number of values is 1.
            //    Parameter[2]
            //      The category is LuminanceTable.
            //      The data type is Short.
            //      The number of values is 0.
            //    Parameter[3]
            //      The category is ChrominanceTable.
            //      The data type is Short.
            //      The number of values is 0.


            //  http://msdn.microsoft.com/en-us/library/ms533845(VS.85).aspx
            // http://msdn.microsoft.com/en-us/library/ms533844(VS.85).aspx
            // TODO: What about ICC profiles
            #endregion

            //Validate quality
            if (!IsValidQuality(quality))
            {
                throw new ArgumentException(quality.ToString() + " is not a valid Jpeg compression quality value.");
            }

            //Prepare paramater for encoder
            //One pair of parenthesis used for easier reading of the usings.
            using (EncoderParameters p = new EncoderParameters(1))
            using (EncoderParameter ep = new EncoderParameter(Encoder.Quality, (long)quality))
            {
                p.Param[0] = ep;
                //save
                b.Save(target, GetImageCodecInfo("image/jpeg"), p);
            }
        }

        /// <summary>
        /// Saves the image in png form. If Stream 'target' is not seekable, a temporary MemoryStream will be used to buffer the image data into the stream.
        /// </summary>
        /// <param name="img"></param>
        /// <param name="target"></param>
        public static void SavePng(Image img, Stream target)
        {
            if (!target.CanSeek)
            {
                //Write to an intermediate, seekable memory stream (PNG compression requires it)
                using (MemoryStream ms = new MemoryStream(4096))
                {
                    img.Save(ms, ImageFormat.Png);
                    ms.WriteTo(target);
                }
            }
            else
            {
                // The parameter list requires 0 bytes.
                img.Save(target, ImageFormat.Png);
            }
        }

        /// <summary>
        /// Saves the image in gif form.
        /// </summary>
        /// <param name="img"></param>
        /// <param name="target"></param>
        public static void SaveGif(Image img, Stream target)
        {
            // The parameter list requires 0 bytes.
            img.Save(target, ImageFormat.Gif);
        }

        /// <summary>
        /// Saves the image in bmp form.
        /// </summary>
        /// <param name="img"></param>
        /// <param name="target"></param>
        public static void SaveBmp(Image img, Stream target)
        {
            // The parameter list requires 0 bytes.
            img.Save(target, ImageFormat.Bmp);
        }

        #endregion
    }
}
