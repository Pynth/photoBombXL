﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PhotoBombXL
{
    class ConverterUtil
    {
        public static void convertFiles(List<ImageFilePathUtil> filesToBeConverted, Profile usedProfile, string destinationPath)
        {
            string profileFolder = usedProfile.name;
            foreach (ImageFilePathUtil file in filesToBeConverted)
            {
                // check to make sure we are using a valid file
                if (isFilePathValid(file.fullPath))
                {
                    string extentionlessFilePath = destinationPath + "\\" + profileFolder + "\\" + file.nameWithoutExtension;
                    Image image = Image.FromFile(file.fullPath);
                    System.IO.Directory.CreateDirectory(destinationPath + "\\" + profileFolder);

                    // choose which kind of file to convert the image to
                    if (usedProfile.fileType == Profile.fileTypes.GIF)
                    {
                        image.Save(extentionlessFilePath + ".gif", System.Drawing.Imaging.ImageFormat.Gif);
                    }
                    else if (usedProfile.fileType == Profile.fileTypes.JPG)
                    {
                        image.Save(extentionlessFilePath + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);
                    }
                    else if (usedProfile.fileType == Profile.fileTypes.PNG)
                    {
                        image.Save(extentionlessFilePath + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    }
                    else if (usedProfile.fileType == Profile.fileTypes.TIFF)
                    {
                        image.Save(extentionlessFilePath + ".tiff", System.Drawing.Imaging.ImageFormat.Tiff);
                    }
                    else if (usedProfile.fileType == Profile.fileTypes.BMP)
                    {
                        image.Save(extentionlessFilePath + ".bmp", System.Drawing.Imaging.ImageFormat.Bmp);
                    }
                }
            }
        }

        // this is to do some last minute checking to make sure we are opporating on a valid file
        public static bool isFilePathValid(string filepath)
        {
            if (File.Exists(filepath) &&
                    
                 (string.Equals(Path.GetExtension(filepath), ".jpg", StringComparison.CurrentCultureIgnoreCase) ||
                 string.Equals(Path.GetExtension(filepath), ".jpeg", StringComparison.CurrentCultureIgnoreCase) ||
                 string.Equals(Path.GetExtension(filepath), ".raw", StringComparison.CurrentCultureIgnoreCase)  ||
                 string.Equals(Path.GetExtension(filepath), ".gif", StringComparison.CurrentCultureIgnoreCase)  ||
                 string.Equals(Path.GetExtension(filepath), ".png", StringComparison.CurrentCultureIgnoreCase)  ||
                 string.Equals(Path.GetExtension(filepath), ".bmp", StringComparison.CurrentCultureIgnoreCase)  ||
                 string.Equals(Path.GetExtension(filepath), ".tiff", StringComparison.CurrentCultureIgnoreCase)  )
               )
            {
                return true;
            }
            return false;
        }
    }
}
