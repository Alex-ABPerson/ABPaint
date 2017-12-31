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
    public static class BrushDrawing
    {
        /// <summary>
        /// Draws a line of ellipses. Used for the brush tool.
        /// </summary>
        /// <param name="Thickness">The size of each ellipse</param>
        /// <param name="grph">The graphics to draw to.</param>
        /// <param name="color">The color to draw into.</param>
        /// <param name="oldX">The start of the line in the X.</param>
        /// <param name="oldY">The start of the line in the Y.</param>
        /// <param name="newX">The end of the line in the X.</param>
        /// <param name="newY">The end of the line in the Y.</param>
        public static void DrawLineOfEllipse(int Thickness, Graphics g, SolidBrush color, int oldX, int oldY, int newX, int newY)
        {
            g.FillEllipse(color, newX, newY, Thickness, Thickness);
            float length = (float)Math.Sqrt(Math.Pow(newX - oldX, 2) + Math.Pow(newY - oldY, 2));
            float dx = (newX - oldX) / length;
            float dy = (newY - oldY) / length;
            float x = oldX;
            float y = oldY;

            for (int i = 1; i < length; i++)
            {
                g.FillEllipse(color, x, y, Thickness, Thickness);
                x += dx;
                y += dy;
            }
        }

        public static Bitmap ChangeImageColor(Bitmap bmp, Color clr)
        {
            Bitmap ret = new Bitmap(bmp.Width, bmp.Height);

            Graphics g = Graphics.FromImage(ret);

            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            
            ColorMap[] colorMap = new ColorMap[1];
            colorMap[0] = new ColorMap();
            colorMap[0].OldColor = Color.FromArgb(1, 0, 1);
            colorMap[0].NewColor = clr;
            ImageAttributes attr = new ImageAttributes();
            attr.SetRemapTable(colorMap);
            // Draw using the color map

            g.DrawImage(bmp, rect, 0, 0, rect.Width, rect.Height, GraphicsUnit.Pixel, attr);

            return ret;
        }
    }
}
