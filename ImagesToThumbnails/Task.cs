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
    class Task
    {
        // initiate all values
        string filePath;
        Size boxSize;
        FitMode fitMode;
        bool overwriteExistingfiles;
        string taskNumber;

        // create an object with all values
        public Task(string filePath, Size boxSize, FitMode fitMode, bool overwriteExistingfiles, string taskNumber)
        {
            // store all values in class
            this.filePath = filePath;
            this.boxSize = boxSize;
            this.fitMode = fitMode;
            this.overwriteExistingfiles = overwriteExistingfiles;
            this.taskNumber = taskNumber;
        }

        // return value methods, no set needed because it only serves to transport data
        public string getPath()
        {
            return filePath;
        }

        public Size getSize()
        {
            return boxSize;
        }

        public FitMode getFitMode()
        {
            return fitMode;
        }

        public bool getOverwrite()
        {
            return overwriteExistingfiles;
        }

        public string getTaskNumber()
        {
            return taskNumber;
        }

    }
}
