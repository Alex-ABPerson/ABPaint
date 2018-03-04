using ABPaint.Objects;
using ABPaint.Tools.Backend;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPaint.Elements
{
    public class Fill : Element
    {
        public Color fillColor;
        public Bitmap fillPoints;

        public override void ProcessImage(Graphics g)
        {
            g.DrawImage(BrushDrawing.ChangeImageColor(fillPoints, fillColor), DrawAtX, DrawAtY);
        }

        public override void Resize()
        {
            fillPoints = ResizeImage.resizeImageNoAntiAlias(fillPoints, new Size(Width, Height));
        }

        public override void FinishResize()
        {
        }
    }
}
