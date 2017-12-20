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
    class Pencil : Element
    {       
        public Bitmap pencilPoints;
        public Color pencilColor = Color.Blue;

        public override Bitmap ProcessImage()
        {
            return BrushDrawing.ChangeImageColor(pencilPoints, pencilColor);
        }

        public override void Resize(int newWidth, int newHeight)
        {
            pencilPoints = ResizeImage.resizeImageNoAntiAlias(pencilPoints, new Size(newWidth, newHeight));
        }
    }
}
