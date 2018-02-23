using ABPaint.Objects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPaint.Elements
{
    public class Ellipse : Element
    {
        // The two variables below have capitals at the start UNLIKE the rectangle so that the Serializer (The thing that saves it to a file)
        // Knows whether the data Serialized is an ellipse or rectangle.
        public Color FillColor;
        public Color BorderColor;
        public int BorderSize;
        public bool IsFilled = false;

        public override void ProcessImage(Graphics g)
        {
            if (IsFilled) g.FillEllipse(new SolidBrush(FillColor), DrawAtX, DrawAtY, Math.Abs(Width), Math.Abs(Height)); // Fill

            g.DrawEllipse(new Pen(BorderColor, BorderSize), (BorderSize / 2) + DrawAtX, (BorderSize / 2) + DrawAtY, Math.Abs(Width - (BorderSize)), Math.Abs(Height - (BorderSize)));
        }

        public override void Resize(int newWidth, int newHeight)
        {
            // Do Nothing
        }
    }
}
