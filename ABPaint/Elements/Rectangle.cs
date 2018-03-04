using ABPaint.Objects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPaint.Elements
{
    public class RectangleE : Element
    {
        public Color FillColor;
        public Color BorderColor;
        public int BorderSize;
        public bool IsFilled = false;
        public bool TmpIsRect = true; // A temp variable just so the TEMPORARY JSON SERIALIZER will work.

        public override void ProcessImage(Graphics g)
        {
            if (IsFilled) g.FillRectangle(new SolidBrush(FillColor), DrawAtX, DrawAtY, Math.Abs(Width), Math.Abs(Height)); // Fill

            g.DrawRectangle(new Pen(BorderColor, BorderSize), (BorderSize / 2) + DrawAtX, (BorderSize / 2) + DrawAtY, Math.Abs(Width - (BorderSize)), Math.Abs(Height - (BorderSize))); // Border
        }

        public override void Resize()
        {
        }

        public override void FinishResize()
        {
        }
    }
}
