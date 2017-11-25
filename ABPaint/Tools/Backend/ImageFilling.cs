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
        public static Bitmap SafeFloodFill(Bitmap background, int x, int y, Color new_color)
        {
            Color old_color = background.GetPixel(x, y);
            Bitmap bm = new Bitmap(background.Width, background.Height);

            if (old_color != new_color)
            {
                Stack<Point> pts = new Stack<Point>(1000);
                pts.Push(new Point(x, y));
                background.SetPixel(x, y, new_color);
                bm.SetPixel(x, y, new_color);

                while (pts.Count > 0)
                {
                    Point pt = pts.Pop();
                    if (pt.X > 0) SafeCheckPoint(ref bm, ref background, ref pts, pt.X - 1, pt.Y, old_color, new_color);
                    if (pt.Y > 0) SafeCheckPoint(ref bm, ref background, ref pts, pt.X, pt.Y - 1, old_color, new_color);
                    if (pt.X < bm.Width - 1) SafeCheckPoint(ref bm, ref background, ref pts, pt.X + 1, pt.Y, old_color, new_color);
                    if (pt.Y < bm.Height - 1) SafeCheckPoint(ref bm, ref background, ref pts, pt.X, pt.Y + 1, old_color, new_color);
                }
            }

            GC.Collect();

            return bm;
        }

        public static void SafeCheckPoint(ref Bitmap bm, ref Bitmap background, ref Stack<Point> pts, int x, int y, Color old_color, Color new_color)
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
