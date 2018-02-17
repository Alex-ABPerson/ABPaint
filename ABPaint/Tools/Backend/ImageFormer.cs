using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ABPaint.Tools.Backend
{
    public static class ImageFormer
    {
        public static byte[] ImageToByteArray(Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }

        public static Image ByteArrayToImage(byte[] imageBytes)
        {
            MemoryStream ms = new MemoryStream(imageBytes, 0,
              imageBytes.Length);

            // Convert byte[] to Image
            ms.Write(imageBytes, 0, imageBytes.Length);
            Image image = Image.FromStream(ms, true);
            return image;
        }

        public static Bitmap RemoveTransparency(Bitmap bitmap, Color background)
        {
            var result = new Bitmap(bitmap.Size.Width, bitmap.Size.Height, PixelFormat.Format24bppRgb);
            var g = Graphics.FromImage(result);

            g.Clear(background);
            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            g.DrawImage(bitmap, 0, 0);

            return result;
        }
    }
}
