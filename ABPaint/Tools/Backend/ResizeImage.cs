using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPaint.Tools.Backend
{
    public static class ResizeImage
    {
        public static Bitmap resizeImage(Bitmap imgToResize, Size size)
        {
            //Get the image current width
            int sourceWidth = imgToResize.Width;
            //Get the image current height
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;
            //Calulate  width with new desired size
            nPercentW = ((float)size.Width / (float)sourceWidth);
            //Calculate height with new desired size
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;
            //New Width
            int destWidth = (int)(sourceWidth * nPercent);
            //New Height
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((System.Drawing.Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            // Draw image with new width and height
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();
            return b;
        }

        //public static Bitmap resizeImage(int newWidth, int newHeight, Bitmap photo)
        //{
        //    Image imgPhoto = photo;

        //    int sourceWidth = imgPhoto.Width;
        //    int sourceHeight = imgPhoto.Height;

        //    //Consider vertical pics
        //    if (sourceWidth < sourceHeight)
        //    {
        //        int buff = newWidth;

        //        newWidth = newHeight;
        //        newHeight = buff;
        //    }

        //    int sourceX = 0, sourceY = 0, destX = 0, destY = 0;
        //    float nPercent = 0, nPercentW = 0, nPercentH = 0;

        //    nPercentW = ((float)newWidth / (float)sourceWidth);
        //    nPercentH = ((float)newHeight / (float)sourceHeight);
        //    if (nPercentH < nPercentW)
        //    {
        //        nPercent = nPercentH;
        //        destX = System.Convert.ToInt16((newWidth -
        //                  (sourceWidth * nPercent)) / 2);
        //    }
        //    else
        //    {
        //        nPercent = nPercentW;
        //        destY = System.Convert.ToInt16((newHeight -
        //                  (sourceHeight * nPercent)) / 2);
        //    }

        //    int destWidth = (int)(sourceWidth * nPercent);
        //    int destHeight = (int)(sourceHeight * nPercent);


        //    Bitmap bmPhoto = new Bitmap(newWidth, newHeight,
        //                  PixelFormat.Format24bppRgb);

        //    bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
        //                 imgPhoto.VerticalResolution);

        //    Graphics grPhoto = Graphics.FromImage(bmPhoto);
        //    grPhoto.Clear(Color.White);
        //    grPhoto.InterpolationMode =
        //        System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

        //    grPhoto.DrawImage(imgPhoto,
        //        new Rectangle(destX, destY, destWidth, destHeight),
        //        new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
        //        GraphicsUnit.Pixel);

        //    grPhoto.Dispose();
        //    imgPhoto.Dispose();

        //    return bmPhoto;
        //}
    }
}
