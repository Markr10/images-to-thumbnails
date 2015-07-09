/* Copyright (c) 2011 Nathanael Jones. See license.txt */
using System.Drawing;
using System.IO;

namespace ImageResizer.Encoding
{
    /// <summary>
    /// An image encoder. Exposes methods for encoding.
    /// </summary>
    public interface IEncoder
    {
        /// <summary>
        /// Encodes the image to the specified stream 
        /// </summary>
        /// <param name="i"></param>
        /// <param name="s"></param>
        void Write(Image i, Stream s);

        /// <summary>
        /// Returns a file extension appropriate for the output format as currently configured, without a leading dot.
        /// </summary>
        string Extension { get; }
    }
}
