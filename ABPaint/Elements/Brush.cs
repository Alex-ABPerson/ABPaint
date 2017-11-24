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
    class Brush : Element
    {
        public Bitmap brushPoints;
        public Color brushColor;

        public override Bitmap ProcessImage()
        {
            Bitmap ret = new Bitmap(Width, Height);

            // This code is basically identical to the Pencil tool because the two are the same! Unlike over tools...
            Graphics g = Graphics.FromImage(ret);

            ColorMap[] colorMap = new ColorMap[1];
            colorMap[0] = new ColorMap();
            colorMap[0].OldColor = Color.FromArgb(1, 0, 1);
            colorMap[0].NewColor = brushColor;
            ImageAttributes attr = new ImageAttributes();
            attr.SetRemapTable(colorMap);
            // Draw using the color map
            Rectangle rect = new Rectangle(0, 0, brushPoints.Width, brushPoints.Height);
            g.DrawImage(brushPoints, rect, 0, 0, rect.Width, rect.Height, GraphicsUnit.Pixel, attr);

            return ret;
        }

        public override void Resize(int newWidth, int newHeight)
        {
            brushPoints = ResizeImage.resizeImage(brushPoints, new Size(newWidth, newHeight));
        }
    }
}
