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
    class Fill : Element
    {
        public Color fillColor;
        public Bitmap fillPoints;

        public override Bitmap ProcessImage()
        {
            Bitmap toReturn = new Bitmap(Width, Height);
            Graphics g = Graphics.FromImage(toReturn);

            ColorMap[] colorMap = new ColorMap[1];
            colorMap[0] = new ColorMap();
            colorMap[0].OldColor = Color.FromArgb(1, 0, 1);
            colorMap[0].NewColor = fillColor;
            ImageAttributes attr = new ImageAttributes();
            attr.SetRemapTable(colorMap);
            // Draw using the color map
            Rectangle rect = new Rectangle(0, 0, fillPoints.Width, fillPoints.Height);
            g.DrawImage(fillPoints, rect, 0, 0, rect.Width, rect.Height, GraphicsUnit.Pixel, attr);

            return toReturn;
        }

        public override void Resize(int newWidth, int newHeight)
        {
            fillPoints = ResizeImage.resizeImageNoAntiAlias(fillPoints, new Size(newWidth, newHeight));
        }
    }
}
