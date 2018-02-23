using ABPaint.Objects;
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

        public override void ProcessImage(Graphics g)
        {
            g.DrawLine(new Pen(color, Thickness), StartPoint.X + DrawAtX, StartPoint.Y + DrawAtY, EndPoint.X + DrawAtX, EndPoint.Y + DrawAtY);
        }

        public override void Resize(int newWidth, int newHeight)
        {
            ////StartPoint.X = BeforeResizeStart.X + (newWidth - BeforeResizeWidth);
            ////StartPoint.Y = BeforeResizeStart.Y + (newHeight - BeforeResizeHeight);

            //EndPoint.X = BeforeResizeEnd.X + (newWidth - BeforeResizeWidth);
            //EndPoint.Y = BeforeResizeEnd.Y + (newHeight - BeforeResizeHeight);

            //StartPoint.Y = BeforeResizeStart.Y;
            ////StartPoint.Y = BeforeResizeHeight + (newHeight - BeforeResizeHeight) - Thickness;
        }
    }
}
