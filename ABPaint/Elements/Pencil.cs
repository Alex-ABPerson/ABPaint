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
    public class Pencil : Element
    {       
        public Bitmap pencilPoints;
        public Color pencilColor = Color.Blue;

        public override void ProcessImage(Graphics g)
        {
            BrushDrawing.ChangeGraphicsColor(pencilPoints, g, pencilColor, DrawAtX, DrawAtY);
        }

        public override void Resize()
        {
            pencilPoints = ResizeImage.resizeImageNoAntiAlias(pencilPoints, new Size(Width, Height));
        }

        public override void FinishResize()
        {
        }
    }
}
