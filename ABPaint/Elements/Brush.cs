using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using ABPaint.Tools.Backend;

namespace ABPaint.Elements
{
    public class Brush : Element
    {      
        public Bitmap brushPoints;
        public Color brushColor = Color.Black;

        public override Bitmap ProcessImage()
        {
            return BrushDrawing.ChangeImageColor(brushPoints, brushColor);
        }

        public override void Resize(int newWidth, int newHeight)
        {
            brushPoints = ResizeImage.resizeImageNoAntiAlias(brushPoints, new Size(newWidth, newHeight));
        }
    }
}
