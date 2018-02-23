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
        public Bitmap mainImage;

        public ImageE(Bitmap img = null)
        {
            mainImage = img;
        }

        public override void ProcessImage(Graphics g)
        {
            g.DrawImage(mainImage, DrawAtX, DrawAtY);
        }

        public override void Resize(int newWidth, int newHeight)
        {
            mainImage = ResizeImage.resizeImage(mainImage, new Size(newWidth, newHeight));
        }
    }
}
