using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using ABPaint.Tools.Backend;
using ABPaint.Objects;

namespace ABPaint.Elements
{
    public class Brush : Element
    {      
        public Bitmap brushPoints;
        public Color brushColor = Color.Black;

        public override void ProcessImage(Graphics g)
        {
            BrushDrawing.ChangeGraphicsColor(brushPoints, g, brushColor, DrawAtX, DrawAtY);
        }

        public override void Resize()
        {
            brushPoints = ResizeImage.resizeImageNoAntiAlias(brushPoints, new Size(Width, Height));
        }

        public override void FinishResize()
        {
        }
    }
}
