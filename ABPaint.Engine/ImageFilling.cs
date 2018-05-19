// ***********************************************************************
// Assembly         : ABPaint
// Author           : Alex
// Created          : 11-25-2017
//
// Last Modified By : Alex
// Last Modified On : 03-01-2018
// ***********************************************************************
// <copyright file="ImageFilling.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPaint.Engine
{
    public static class ImageFilling
    {
        public static Point Location;
        public static Point RightLocation;
        /// <summary>
        /// Fills an image with a certain color - based on an x and y.
        /// </summary>
        /// <param name="background">The original image.</param>
        /// <param name="x">The X where the image will be filled from.</param>
        /// <param name="y">The Y where the image will be filled from.</param>
        /// <param name="newColor">The new color that will replace the old.</param>
        /// <returns>A new bitmap that is the filled area.</returns>
        public static Bitmap SafeFloodFill(byte[] background, int x, int y, Color newColor)
        {
            Bitmap newBackground = (Bitmap)ImageFormer.ByteArrayToImage(background);

            Color old_color = newBackground.GetPixel(x, y);
            Bitmap bm = new Bitmap(newBackground.Width, newBackground.Height);

            if (old_color != newColor)
            {
                Stack<Point> pts = new Stack<Point>(1000);
                Point pt;
                pts.Push(new Point(x, y));
                newBackground.SetPixel(x, y, newColor);
                bm.SetPixel(x, y, newColor);
                
                while (pts.Count > 0)
                {
                    pt = pts.Pop();
                    if (pt.X > 0) SafeCheckPoint(ref bm, newBackground, ref pts, pt.X - 1, pt.Y, old_color, newColor);
                    if (pt.Y > 0) SafeCheckPoint(ref bm, newBackground, ref pts, pt.X, pt.Y - 1, old_color, newColor);
                    if (pt.X < bm.Width - 1) SafeCheckPoint(ref bm, newBackground, ref pts, pt.X + 1, pt.Y, old_color, newColor);
                    if (pt.Y < bm.Height - 1) SafeCheckPoint(ref bm, newBackground, ref pts, pt.X, pt.Y + 1, old_color, newColor);
                }
            }

            Location.X -= 10;
            Location.Y -= 10;
            RightLocation.X += 20;
            RightLocation.Y += 20;

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
        /// <param name="oldColor">The color before.</param>
        /// <param name="newColor">The new color.</param>
        public static void SafeCheckPoint(ref Bitmap bm, Bitmap background, ref Stack<Point> pts, int x, int y, Color oldColor, Color newColor)
        {
            Color clr = background.GetPixel(x, y);
            if (clr.Equals(oldColor))
            {
                if (x < Location.X) Location.X = x;
                if (y < Location.Y) Location.Y = y;
                if (x > RightLocation.X) RightLocation.X = x;
                if (y > RightLocation.Y) RightLocation.Y = y;

                pts.Push(new Point(x, y));
                background.SetPixel(x, y, newColor);
                bm.SetPixel(x, y, newColor);
            }
        }
    }
}
