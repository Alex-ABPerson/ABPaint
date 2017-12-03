using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPaint.Tools.Backend
{
    public static class ImageFilling
    {
        /// <summary>
        /// Fills an image with a certain color - based on an x and y.
        /// </summary>
        /// <param name="background">The original image.</param>
        /// <param name="x">The X where the image will be filled from.</param>
        /// <param name="y">The Y where the image will be filled from.</param>
        /// <param name="new_color">The new color that will replace the old.</param>
        /// <returns>A new bitmap that is the filled area.</returns>
        public static Bitmap SafeFloodFill(byte[] background, int x, int y, Color new_color)
        {
            Bitmap newBackground = (Bitmap)ImageFormer.ByteArrayToImage(background);

            Color old_color = newBackground.GetPixel(x, y);
            Bitmap bm = new Bitmap(newBackground.Width, newBackground.Height);

            if (old_color != new_color)
            {
                Stack<Point> pts = new Stack<Point>(1000);
                pts.Push(new Point(x, y));
                newBackground.SetPixel(x, y, new_color);
                bm.SetPixel(x, y, new_color);

                while (pts.Count > 0)
                {
                    Point pt = pts.Pop();
                    if (pt.X > 0) SafeCheckPoint(ref bm, newBackground, ref pts, pt.X - 1, pt.Y, old_color, new_color);
                    if (pt.Y > 0) SafeCheckPoint(ref bm, newBackground, ref pts, pt.X, pt.Y - 1, old_color, new_color);
                    if (pt.X < bm.Width - 1) SafeCheckPoint(ref bm, newBackground, ref pts, pt.X + 1, pt.Y, old_color, new_color);
                    if (pt.Y < bm.Height - 1) SafeCheckPoint(ref bm, newBackground, ref pts, pt.X, pt.Y + 1, old_color, new_color);
                }
            }

            GC.Collect();

            return bm;
        }

        /// <summary>
        /// Checks if this pixel should be filled.
        /// </summary>
        /// <param name="bm">The new bitmap that is being created</param>
        /// <param name="background">The original bitmap.</param>
        /// <param name="pts">A stack of which pixels are left.</param>
        /// <param name="x">The pixel to check's X.</param>
        /// <param name="y">The pixel to check's Y.</param>
        /// <param name="old_color">The color before.</param>
        /// <param name="new_color">The new color.</param>
        public static void SafeCheckPoint(ref Bitmap bm, Bitmap background, ref Stack<Point> pts, int x, int y, Color old_color, Color new_color)
        {
            Color clr = background.GetPixel(x, y);
            if (clr.Equals(old_color))
            {
                var DrawingMin = Program.mainForm.DrawingMin;
                var DrawingMax = Program.mainForm.DrawingMax;

                if (x < DrawingMin.X) DrawingMin.X = x;
                if (y < DrawingMin.Y) DrawingMin.Y = y;
                if (x > DrawingMax.X) DrawingMax.X = x;
                if (y > DrawingMax.Y) DrawingMax.Y = y;

                Program.mainForm.DrawingMin = DrawingMin;
                Program.mainForm.DrawingMax = DrawingMax;

                pts.Push(new Point(x, y));
                background.SetPixel(x, y, new_color);
                bm.SetPixel(x, y, new_color);
            }
        }
    }
}
