using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPaint.Elements
{
    class Ellipse : Element
    {
        public Color fillColor;
        public Color borderColor;
        public int BorderSize;
        public bool IsFilled;
        public int OriginalX;
        public int OriginalY;

        public override Bitmap ProcessImage()
        {
            Bitmap ret = new Bitmap(Math.Abs(Width), Math.Abs(Height));
            Graphics g = Graphics.FromImage(ret);

            // Now let's draw this rectangle!

            //Point startPoint = new Point(0, 0);

            //if (Width < 0) startPoint = new Point(Width, startPoint.Y);
            //if (Height < 0) startPoint = new Point(startPoint.X, Height);

            int heightamount = 0;
            int widthamount = 0;
            if (Width < 0)
                widthamount = Math.Abs(Width);
            else
                widthamount = 0;

            if (Height < 0)
                heightamount = Math.Abs(Height);
            else
                heightamount = 0;

            //if (width < 0) currentDrawingElement.Width = 1;
            //if (height < 0) currentDrawingElement.Height = 1;

            X = OriginalX - widthamount;
            Y = OriginalY - heightamount;

            if (IsFilled) g.FillEllipse(new SolidBrush(fillColor), BorderSize, BorderSize, Math.Abs(Width) - (BorderSize * 2), Math.Abs(Height) - (BorderSize * 2)); // Fill

            g.DrawEllipse(new Pen(borderColor, BorderSize), BorderSize, BorderSize, Math.Abs(Width) - (BorderSize * 2), Math.Abs(Height) - (BorderSize * 2));

            //g.FillRectangle(new SolidBrush(borderColor), 0, 0, BorderSize, Height); // Left border
            //g.FillRectangle(new SolidBrush(borderColor), 0, 0, Width, BorderSize); // Top border
            //g.FillRectangle(new SolidBrush(borderColor), Width - BorderSize, 0, BorderSize, Height); // Right border
            //g.FillRectangle(new SolidBrush(borderColor), 0, Height - BorderSize, Width, BorderSize); // Bottom border



            return ret;
        }
    }
}
