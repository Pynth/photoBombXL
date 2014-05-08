using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;

namespace PhotoBombXL
{
    public class JPEGQuality
    {
        public JPEGQuality()
        {
        }

        EncoderParameter qualityParam;
        MemoryStream jpegStream;
        long jL, mid;
        EncoderParameters encoderParams = new EncoderParameters(1);

        private long sizeToQuality(Bitmap img, int maxsizebytes, long lower, long upper)
        {
            ImageCodecInfo jpegCodec = GetEncoder(ImageFormat.Jpeg);

            jpegStream = new MemoryStream();

            // New mid value
            mid = (long)Math.Floor(((float)lower + (float)upper) / 2f);

            // Param setting and a new stream
            qualityParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, mid);
            encoderParams.Param[0] = qualityParam;

            // Save to the stream, get its length
            img.Save(jpegStream, jpegCodec, encoderParams);
            jL = jpegStream.Length;

            // Cleanup
            jpegStream.Dispose();
            qualityParam.Dispose();

            // Return/recursion
            if (lower >= upper) // >= in case something went horribly wrong...
                return (jL <= maxsizebytes) ? lower : lower - 1;
            else
                return (jL <= maxsizebytes) ? sizeToQuality(img, maxsizebytes, mid + 1, upper) : sizeToQuality(img, maxsizebytes, lower, mid);
        }
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }
    }
}
