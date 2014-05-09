﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace PhotoBombXL
{
    class ConverterUtil
    {
        public static void convertFiles(List<ImageFilePathUtil> filesToBeConverted, Profile usedProfile, string destinationPath, ProgressBar progressBar)
        {
            progressBar.Minimum = 0;
            progressBar.Maximum = filesToBeConverted.Count;
            progressBar.Step = 1;
            progressBar.Value = 0;

            string profileFolder = usedProfile.name;
            foreach (ImageFilePathUtil file in filesToBeConverted)
            {
                // check to make sure we are using a valid file
                if (isFilePathValid(file.fullPath))
                {
                    string extentionlessFilePath = destinationPath + "\\" + profileFolder + "\\" + file.nameWithoutExtension;
                    Image image = Image.FromFile(file.fullPath);
                    int rotation = GetExifOrienAsInt(image);
                    if (rotation == 2) image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    else if (rotation == 3) image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    else if (rotation == 4) image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    else if (rotation == 5) image.RotateFlip(RotateFlipType.Rotate270FlipX);
                    else if (rotation == 6) image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    else if (rotation == 7) image.RotateFlip(RotateFlipType.Rotate90FlipX);
                    else if (rotation == 8) image.RotateFlip(RotateFlipType.Rotate270FlipNone);

                    System.IO.Directory.CreateDirectory(destinationPath + "\\" + profileFolder);

                    // resizing the image
                    Size adjustedSize = image.Size;
                    if (image.Size.Height > usedProfile.heightInPixels || image.Size.Width > usedProfile.heightInPixels)
                    {
                        adjustedSize = getCorrectSize(image.Size, usedProfile);
                    }

                    Size newSize = new Size(usedProfile.widthInPixels == -1 ? image.Width : adjustedSize.Width, usedProfile.heightInPixels == -1 ? image.Height : adjustedSize.Height);
                    image = resizeImage(image, newSize);

                    // choose which kind of file to convert the image to
                    if (usedProfile.fileType == Profile.fileTypes.GIF)
                    {
                        image.Save(extentionlessFilePath + ".gif", System.Drawing.Imaging.ImageFormat.Gif);
                    }
                    else if (usedProfile.fileType == Profile.fileTypes.JPG)
                    {
                        JPEGQuality qualityChanger = new JPEGQuality();
                        //long saveQuality = qualityChanger.sizeToQuality(image, usedProfile.fileSize, 0, 100);

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
                    progressBar.PerformStep();
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
        // used for resizing an image
        public static Image resizeImage(Image imageToResize, Size size)
        {
            return (Image)(new Bitmap(imageToResize, size));
        }

        public static int GetExifOrienAsInt(Image image)
        {
            int _orienTag = 0x0112;
            int orien = 1; // Default to "horizontal (normal)" in case of failure...

            try
            {
                byte[] orienBytes = image.PropertyItems.Single(x => x.Id == _orienTag).Value;
                if (orienBytes.Length > 0)
                {
                    orien = (int)orienBytes[0];
                }
            }
            catch { };

            return orien;
        }

        public static Size getCorrectSize(Size currentSize, Profile profile)
        {
            double ratio;
            if (currentSize.Height >= currentSize.Width)
            {
                ratio = profile.heightInPixels / (double)currentSize.Height;
                return new Size((int)((double)currentSize.Width * ratio), profile.heightInPixels);
            }
            else
            {
                ratio = profile.heightInPixels / (double)currentSize.Width;
                return new Size(profile.heightInPixels, (int)((double)currentSize.Height * ratio));
            }
        }
    }
}
