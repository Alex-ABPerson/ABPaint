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
        // The two variables below have capitals at the start UNLIKE the rectangle so that the Serializer (The thing that saves it to a file)
        // Knows whether the data Serialized is an ellipse or rectangle.
        public Color FillColor;
        public Color BorderColor;
        public int BorderSize;
        public bool IsFilled;

        public override Bitmap ProcessImage()
        {
            Bitmap ret = new Bitmap(Math.Abs(Width), Math.Abs(Height));
            Graphics g = Graphics.FromImage(ret);

            // Now let's draw this rectangle!

            //Point startPoint = new Point(0, 0);

            //if (Width < 0) startPoint = new Point(Width, startPoint.Y);
            //if (Height < 0) startPoint = new Point(startPoint.X, Height);

            //if (width < 0) currentDrawingElement.Width = 1;
            //if (height < 0) currentDrawingElement.Height = 1;

            if (IsFilled) g.FillEllipse(new SolidBrush(FillColor), 0, 0, Math.Abs(Width), Math.Abs(Height)); // Fill

            g.DrawEllipse(new Pen(BorderColor, BorderSize), BorderSize / 2, BorderSize / 2, Math.Abs(Width - (BorderSize)), Math.Abs(Height - (BorderSize)));

            //g.FillRectangle(new SolidBrush(borderColor), 0, 0, BorderSize, Height); // Left border
            //g.FillRectangle(new SolidBrush(borderColor), 0, 0, Width, BorderSize); // Top border
            //g.FillRectangle(new SolidBrush(borderColor), Width - BorderSize, 0, BorderSize, Height); // Right border
            //g.FillRectangle(new SolidBrush(borderColor), 0, Height - BorderSize, Width, BorderSize); // Bottom border

            return ret;
        }

        public override void Resize(int newWidth, int newHeight)
        {
            // Do Nothing
        }
    }
}
