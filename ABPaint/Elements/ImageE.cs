using ABPaint.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using ABPaint.Tools.Backend;

namespace ABPaint.Elements
{
    public class ImageE : Element
    {
        public Bitmap image;

        public ImageE(Bitmap img = null)
        {
            image = img;
        }

        public override Bitmap ProcessImage()
        {
            return image;
        }

        public override void Resize(int newWidth, int newHeight)
        {
            image = ResizeImage.resizeImage(image, new Size(newWidth, newHeight));
        }
    }
}
