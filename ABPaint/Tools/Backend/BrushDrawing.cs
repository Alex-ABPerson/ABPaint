using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        /// <param name="color">The color to draw into</param>
        /// <param name="oldX">The start of the line in the X.</param>
        /// <param name="oldY">The start of the line in the Y.</param>
        /// <param name="newX">The end of the line in the X.</param>
        /// <param name="newY">The end of the line in the Y.</param>
        public static void DrawLineOfEllipse(int Thickness, Graphics g, SolidBrush color, int oldX, int oldY, int newX, int newY)
        {
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

        public static void DrawLineOfEllipseOld(int Thickness, Graphics grph, SolidBrush color, int oldX, int oldY, int newX, int newY)
        {
            int amount = (Math.Abs((newX - oldX) / 10) > Math.Abs((newY - oldY) / 10)) ? Math.Abs((newX - oldX) / 10) : Math.Abs((newY - oldY) / 10); // Calculate how many ellipse we need to draw!
            for (int i = 1; i < amount + 1; i++)
            {
                int x = ((amount / i) / Math.Abs(newX - oldX)) * Thickness;
                int y = ((amount / i) / Math.Abs(newY - oldY)) * Thickness;

                grph.FillEllipse(color, ((amount / i) / Math.Abs(newX - oldX)) * Thickness, ((amount / i) / Math.Abs(newY - oldY)) * Thickness, Thickness, Thickness);
            }
        }
    }
}
