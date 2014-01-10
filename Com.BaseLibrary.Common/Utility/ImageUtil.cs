using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Com.BaseLibrary.Utility
{
    public static class ImageUtil
    {
        //public static void ScaleImage(Image originalImage, double width, double height, bool isScale)
        //{
        //    double originalImageWidth = originalImage.Width;
        //    double originalImageHeight = originalImage.Height;


        //    double bitmapWidth = width;
        //    double bitmapHeight = height;

        //    double X = 0;
        //    double Y = 0;

        //    if (isScale)
        //    {
        //        bitmapHeight = height = (width / originalImageWidth) * originalImageHeight;

        //    }
        //    else
        //    {
        //        if (bitmapHeight * originalImageWidth > bitmapWidth * originalImageHeight)
        //        {
        //            bitmapHeight = originalImageHeight * width / originalImageWidth;
        //            Y = (height - bitmapHeight) / 2;
        //        }
        //        else
        //        {
        //            bitmapWidth = originalImageWidth * height / originalImageHeight;
        //            X = (width - bitmapWidth) / 2;
        //        }
        //    }

        //    if (originalImageWidth == width)
        //    {

        //    }
        //    else
        //    {
        //        Bitmap bmp = new Bitmap(Convert.ToInt32(width), Convert.ToInt32(height));
        //        Graphics g = Graphics.FromImage(bmp);

        //        g.Clear(Color.White);
        //        g.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //        g.SmoothingMode = SmoothingMode.HighQuality;

        //        g.DrawImage(originalImage,
        //            new Rectangle(Convert.ToInt32(X), Convert.ToInt32(Y), bitmapWidth, bitmapHeight),
        //            new Rectangle(0, 0, originalImageWidth, originalImageHeight),
        //            GraphicsUnit.Pixel);


        //        originalImage.Dispose();
        //        bmp.Dispose();
        //        g.Dispose();

        //    }
        //}

        public static Image ScaleCompressImageByWidth(Image originalImage, double width)
        {
            double originalImageWidth = originalImage.Width;
            double originalImageHeight = originalImage.Height;

            if (originalImageWidth <= width)
            {
                return originalImage;
            }

            double bitmapWidth = width;
            double bitmapHeight = (originalImageHeight / originalImageWidth) * width;

            Bitmap bmp = new Bitmap(Convert.ToInt32(bitmapWidth), Convert.ToInt32(bitmapHeight));
            Graphics g = Graphics.FromImage(bmp);

            g.Clear(Color.White);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = SmoothingMode.HighQuality;

            g.DrawImage(originalImage,
                new Rectangle(0, 0, Convert.ToInt32(originalImageWidth), Convert.ToInt32(originalImageHeight)),
                new Rectangle(0, 0, Convert.ToInt32(bitmapWidth), Convert.ToInt32(bitmapHeight)),
                GraphicsUnit.Pixel);
            originalImage.Dispose();
            g.Dispose();

            return bmp;

        }
    }
}
