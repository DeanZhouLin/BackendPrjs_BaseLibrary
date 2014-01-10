using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Net;
using Com.BaseLibrary.Web;

namespace Com.BaseLibrary.Utility
{
    public static class ImageCompressUtil
    {    /// <summary>
        /// 缩略图生成
        /// </summary>
        /// <param name="pathImageFrom">原图地址</param>
        /// <param name="pathImageTo">保存地址</param>
        /// <param name="width">宽度</param>
        /// <param name="height">高度</param>
        /// <param name="isScale">是否等比例缩放,这里为true</param>
        public static bool CompressAndSaveImage(
            string originalImageName,
            string originalImageFullUrl,
            List<string> newImagePathList,
            int width, int height,
            bool isScale,
            ShareLogonUserInfo orgImageServerLogonUser,
            ShareLogonUserInfo newImageServerLogonUser,
            bool orignalImageFromFlile)
        {
            System.Drawing.Image imageFrom = null;

            try
            {
                if (orignalImageFromFlile)
                {
                    imageFrom = GetImageFromFile(originalImageFullUrl, orgImageServerLogonUser);
                }
                else
                {
                    imageFrom = GetImageFromUrl(originalImageFullUrl);
                }
                //imageFrom = GetImageFromFile(originalImageFullUrl, orgImageServerLogonUser);

            }
            catch
            {
            }

            if (imageFrom == null)
            {
                return false;
            }

            int imageFromWidth = imageFrom.Width;
            int imageFromHeight = imageFrom.Height;


            int bitmapWidth = width;
            int bitmapHeight = height;

            int X = 0;
            int Y = 0;

            if (!isScale)
            {
                if (bitmapHeight * imageFromWidth > bitmapWidth * imageFromHeight)
                {
                    bitmapHeight = imageFromHeight * width / imageFromWidth;
                    Y = (height - bitmapHeight) / 2;
                }
                else
                {
                    bitmapWidth = imageFromWidth * height / imageFromHeight;
                    X = (width - bitmapWidth) / 2;
                }
            }
            else
            {
                bitmapHeight = height = Convert.ToInt32(((decimal)width / imageFromWidth) * imageFromHeight);
            }

            if (imageFromWidth == width)
            {
                SaveImage(originalImageName, newImagePathList, imageFrom, newImageServerLogonUser);
                imageFrom.Dispose();
                return true;
            }
            else
            {
                Bitmap bmp = new Bitmap(width, height);
                Graphics g = Graphics.FromImage(bmp);

                g.Clear(Color.White);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.HighQuality;

                g.DrawImage(imageFrom,
                    new Rectangle(X, Y, bitmapWidth, bitmapHeight),
                    new Rectangle(0, 0, imageFromWidth, imageFromHeight),
                    GraphicsUnit.Pixel);

                SaveImage(originalImageName, newImagePathList, bmp, newImageServerLogonUser);
                imageFrom.Dispose();
                bmp.Dispose();
                g.Dispose();
                return true;
            }
        }

        private static Image GetImageFromFile(string fullImageName, ShareLogonUserInfo user)
        {
            if (user == null)
            {
                return Image.FromFile(fullImageName);
            }
            else
            {
                using (WindowsImpersonation imperso = new WindowsImpersonation(user.UserName, user.Password, user.Domain))
                {
                    return Image.FromFile(fullImageName);
                }
            }

        }

        private static void SaveImage(string imageName, List<string> pathList, Image img, ShareLogonUserInfo user)
        {
            if (user == null)
            {
                DoSaveImage(imageName, pathList, img);
            }
            else
            {
                using (WindowsImpersonation imperso = new WindowsImpersonation(user.UserName, user.Password, user.Domain))
                {
                    DoSaveImage(imageName, pathList, img);
                }
            }

        }

        private static void DoSaveImage(string originalImageName, List<string> newImagePathList, Image bmp)
        {
            
            foreach (string path in newImagePathList)
            {
                string newImageFullFile = Path.Combine(path, originalImageName);
                try
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    bmp.Save(newImageFullFile, ImageFormat.Jpeg);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    Console.WriteLine(path);
                }
            }
        }

        private static Image GetImageFromUrl(string originalImageFullUrl)
        {
            WebRequest request = HttpWebRequest.Create(originalImageFullUrl);
            using (WebResponse response = request.GetResponse())
            {
                return new Bitmap(response.GetResponseStream());
            }
        }

        /// <summary>
        /// 剪切图片
        /// </summary>
        /// <param name="ImgFile">原图地址</param>
        /// <param name="sImgPath">保存地址</param>
        /// <param name="PointX">x坐标</param>
        /// <param name="PointY">y坐标</param>
        /// <param name="CutWidth">剪切宽度</param>
        /// <param name="CutHeight">剪切高度</param>
        /// <returns>是否剪切成功</returns>
        public static bool CutImg(string ImgFile, string sImgPath, int PointX, int PointY, int CutWidth, int CutHeight)
        {
            try
            {
                FileStream fs = new FileStream(ImgFile, FileMode.Open);
                BinaryReader br = new BinaryReader(fs);
                byte[] bytes = br.ReadBytes((int)fs.Length);
                br.Close();
                fs.Close();
                MemoryStream ms = new MemoryStream(bytes);
                System.Drawing.Image imgPhoto = System.Drawing.Image.FromStream(ms);
                Bitmap bmPhoto = new Bitmap(CutWidth, CutHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                bmPhoto.SetResolution(72, 72);
                Graphics gbmPhoto = Graphics.FromImage(bmPhoto);
                gbmPhoto.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                gbmPhoto.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                gbmPhoto.DrawImage(imgPhoto, new Rectangle(0, 0, CutWidth, CutHeight), new Rectangle(PointX, PointY, CutWidth, CutHeight), GraphicsUnit.Pixel);
                bmPhoto.Save(sImgPath, System.Drawing.Imaging.ImageFormat.Jpeg);

                imgPhoto.Dispose();
                gbmPhoto.Dispose();
                bmPhoto.Dispose();

                return true;
            }
            catch (Exception err)
            {
                return false;
            }
        }
    }
}
