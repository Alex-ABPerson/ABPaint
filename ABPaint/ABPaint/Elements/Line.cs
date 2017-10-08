using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPaint.Elements
{
    public class Line : Element
    {
        public Color color;
        public Point EndPoint;
        public Point StartPoint;
        public int Thickness;

        public override Bitmap ProcessImage()
        {
            Bitmap ret = new Bitmap(Math.Abs(Width), Math.Abs(Height));
            Graphics g = Graphics.FromImage(ret);

            g.DrawLine(new Pen(color, Thickness), StartPoint.X, StartPoint.Y, EndPoint.X, EndPoint.Y);

            return ret;
        }
    }
}
