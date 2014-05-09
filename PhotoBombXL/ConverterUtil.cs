using System;
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
                        // we need to get the file size in bytes from our profile, which is saved as mB or kB
                        // we use rounded off versions of mB and kB
                        int maxFileSize = (int) (usedProfile.indicator == Profile.fileSizeIndicator.kb ? usedProfile.fileSize * 1000 : usedProfile.fileSize * 10000);

                        Bitmap bmp = new Bitmap(image);
                        JPEGQuality qualityChanger = new JPEGQuality();
                        ImageCodecInfo jgpEncoder = qualityChanger.GetEncoder(ImageFormat.Jpeg);
                        System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;

                        // this will detirmine the quality indicator (0 to 100 I think) to get the image below
                        // our specified size
                        // it will return -1 if it can't be reduced to the requested fileSize
                        long saveQuality = qualityChanger.sizeToQuality(bmp, maxFileSize, 0, 100);

                        EncoderParameters myEncoderParameters = new EncoderParameters(1);

                        if (saveQuality == -1)
                        {
                            MessageBox.Show("Unable to reduce jpeg quality");
                            saveQuality = 0;
                        }
                        EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, saveQuality);
                        myEncoderParameters.Param[0] = myEncoderParameter;

                        bmp.Save(extentionlessFilePath + ".jpg", jgpEncoder, myEncoderParameters);

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
