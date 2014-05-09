using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhotoBombXL
{
    class Profile
    {
        // enum to be used in file type selection
        public enum fileTypes
        {
            JPG,
            GIF,
            PNG,
            BMP,
            TIFF
        };

        // enum to be used in the rotation of photo gained from exif
        public enum exifMaintained
        {
            Yes,
            No
        }

        // enum to be used to determine the file size
        public enum fileSizeIndicator
        {
            mb,
            kb
        }

        // variables for the profile
        public string name { get; set; }
        public int heightInPixels { get; set; }
        public int widthInPixels { get; set; }
        public fileTypes fileType { get; set; }
        public double fileSize { get; set; }
        public fileSizeIndicator indicator { get; set; } 
        public int aspectHeight { get; set; }
        public int aspectWidth { get; set; }
        public bool isExifMaintained { get; set; }

        // profile contstructor
        public Profile(String name, int heightInPixels, int widthInPixels, fileTypes fileType, double fileSize, fileSizeIndicator fsi,int aspectHeight, int aspectWidth, bool isExifMaintained)
        {
            this.name = name;
            this.heightInPixels = heightInPixels;
            this.widthInPixels = widthInPixels;
            this.fileType = fileType;
            this.fileSize = fileSize;
            this.indicator = fsi;
            this.aspectHeight = aspectHeight;
            this.aspectWidth = aspectWidth;
            this.isExifMaintained = isExifMaintained;
        }
    }
}
