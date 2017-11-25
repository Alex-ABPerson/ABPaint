using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPaint.Tools.Backend
{
    public static class ImageCropping
    {
        public static Bitmap CropImage(Image source, int x, int y, int width, int height)
        {
            Rectangle crop = new Rectangle(x, y, width, height);

            if (crop.Width == 0)
                crop.Width = 1;
            if (crop.Height == 0) crop.Height = 1;

            var bmp = new Bitmap(crop.Width, crop.Height);
            using (var gr = Graphics.FromImage(bmp))
            {
                gr.DrawImage(source, new Rectangle(0, 0, bmp.Width, bmp.Height), crop, GraphicsUnit.Pixel);
            }
            return bmp;
        }

        public static Bitmap CropToContent(Bitmap oldBmp)
        {
            try
            {
                Rectangle currentRect = new Rectangle();
                bool IsFirstOne = true;

                // Get a base color

                for (int y = 0; y < oldBmp.Height; y++)
                {
                    for (int x = 0; x < oldBmp.Width; x++)
                    {
                        Color debug = oldBmp.GetPixel(x, y);
                        if (oldBmp.GetPixel(x, y) != Color.Transparent)
                        {
                            // We need to interpret this!

                            // Check if it is the first one!

                            if (IsFirstOne)
                            {
                                currentRect.X = x;
                                currentRect.Y = y;
                                currentRect.Width = 1;
                                currentRect.Height = 1;
                                IsFirstOne = false;
                            }
                            else
                            {

                                if (!currentRect.Contains(new Point(x, y)))
                                {
                                    // This will run if this is out of the current rectangle

                                    if (x > (currentRect.X + currentRect.Width)) currentRect.Width = x - currentRect.X;
                                    if (x < (currentRect.X))
                                    {
                                        // Move the rectangle over there and extend it's width to make the right the same!
                                        int oldRectLeft = currentRect.Left;

                                        currentRect.X = x;
                                        currentRect.Width += oldRectLeft - x;
                                    }

                                    if (y > (currentRect.Y + currentRect.Height)) currentRect.Height = y - currentRect.Y;

                                    if (y < (currentRect.Y + currentRect.Height))
                                    {
                                        int oldRectTop = currentRect.Top;

                                        currentRect.Y = y;
                                        currentRect.Height += oldRectTop - y;
                                    }
                                }
                            }
                        }
                    }
                }
                return CropImage(oldBmp, currentRect.X, currentRect.Y, currentRect.Width, currentRect.Height);
            }
            catch { return oldBmp; }
        }
    }
}
