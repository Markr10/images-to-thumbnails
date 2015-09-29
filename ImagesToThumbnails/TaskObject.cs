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
    class TaskObject
    {
        // initiate all values
        string directoryPath;
        Size boxSize;
        FitMode fitMode;
        bool overwriteExistingfiles;
        string taskName;

        // create an object with all values
        public TaskObject(string directoryPath, Size boxSize, FitMode fitMode, bool overwriteExistingfiles, string taskName)
        {
            // store all values in class
            this.directoryPath = directoryPath;
            this.boxSize = boxSize;
            this.fitMode = fitMode;
            this.overwriteExistingfiles = overwriteExistingfiles;
            this.taskName = taskName;
        }

        // return value methods, no set needed because it only serves to transport data
        public string getPath()
        {
            return directoryPath;
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

        public string getTaskName()
        {
            return taskName;
        }

    }
}
