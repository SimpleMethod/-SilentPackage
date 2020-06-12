/*
 * Copyright  Michał Młodawski (SimpleMethod)(c) 2020.
 */
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows;

namespace SilentPackage.Controllers
{

    /// <summary>
    /// Class for creating screenshots.
    /// </summary>
    internal class PrintScreenManagement
    {
        /// <summary>
        /// Getting information about the active monitor.
        /// </summary>
        /// <param name="systemMetric">
        /// Specified enum.
        /// </param>
        /// <returns>
        /// Information from the specified enum.
        /// </returns>
        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(SystemMetric systemMetric);
        public enum SystemMetric : int
        {
            SM_CXSCREEN = 0,
            SM_CYSCREEN = 1
        }


        public enum JpegQuality : long
        {
            VLOW = 1L,
            LOW = 25L,
            MEDIUM = 50L,
            HIGH = 75L,
            BEST = 100L
        }

        /// <summary>
        /// Method for making screenshots.
        /// </summary>
        /// <param name="filepath">
        ///  Path for screenshots.
        /// </param>
        /// <param name="fileName">
        /// File name to save.
        /// </param>
        /// <param name="jpegQuality">
        ///
        /// </param>
        public void MakePrintScreen(string filepath, string fileName, JpegQuality jpegQuality)
        {
            if (filepath == null) throw new ArgumentNullException(nameof(filepath));
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            if (!Enum.IsDefined(typeof(JpegQuality), jpegQuality))
                throw new ArgumentOutOfRangeException(nameof(jpegQuality),
                    "Value should be defined in the JpegQuality enum.");

            var widthResolution = GetSystemMetrics(SystemMetric.SM_CXSCREEN);
            var heightResolution = GetSystemMetrics(SystemMetric.SM_CYSCREEN);
            var bmp = new Bitmap(widthResolution, heightResolution, PixelFormat.Format32bppArgb);
            using (var captureGraphic = Graphics.FromImage(bmp))
            {
                captureGraphic.CopyFromScreen(0, 0, 0, 0, bmp.Size);
                ImageCodecInfo encode = GetEncoderInfo("image/jpeg");
                try
                {
                    var param = new EncoderParameters(1);
                    param.Param[0] = new EncoderParameter(Encoder.Quality, (long)jpegQuality);
                    bmp.Save(filepath+fileName, encode, param);
                }
                catch (ArgumentNullException e)
                {
                  //  MessageBox.Show(e.ToString());
                }
            }
        }

        /// <summary>
        /// Specifying file compression method.
        /// </summary>
        /// <param name="mimeType">
        /// File type
        /// </param>
        /// <returns>
        /// Returns a specific type of compression.
        /// </returns>
        /// <remarks>
        /// https://docs.microsoft.com/pl-pl/dotnet/api/system.drawing.imaging.encoder.quality?view=netcore-3.1
        /// </remarks>
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            if (mimeType == null) throw new ArgumentNullException(nameof(mimeType));
            int j;
            var encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }

    }
}
