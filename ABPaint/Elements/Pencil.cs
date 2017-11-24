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
    class Pencil : Element
    {
        public Bitmap pencilPoints;
        public Color pencilColor = Color.Blue;

        public override Bitmap ProcessImage()
        {
            Bitmap ret = new Bitmap(Width, Height);

            //for (int y = 0; y < pencilPoints.Height; y++)
            //{
            //    for (int x = 0; x < pencilPoints.Width; x++)
            //    {
            //        if (pencilPoints.GetPixel(x, y) == Color.FromArgb(1, 0, 1))
            //            ret.SetPixel(x, y, pencilColor);

            //    }
            //}

            // THE CODE ABOVE IS INCREDIBLY OLD AND SLOW - WE ARE CHANGING THE BITMAP'S COLOR MAPPINGS INSTEAD!
            // Set the image attribute's color mappings

            Graphics g = Graphics.FromImage(ret);

            ColorMap[] colorMap = new ColorMap[1];
            colorMap[0] = new ColorMap();
            colorMap[0].OldColor = Color.FromArgb(1, 0, 1);
            colorMap[0].NewColor = pencilColor;
            ImageAttributes attr = new ImageAttributes();
            attr.SetRemapTable(colorMap);
            // Draw using the color map
            Rectangle rect = new Rectangle(0, 0, pencilPoints.Width, pencilPoints.Height);
            g.DrawImage(pencilPoints, rect, 0, 0, rect.Width, rect.Height, GraphicsUnit.Pixel, attr);

            //Graphics g = Graphics.FromImage(ret);

            return ret;
        }

        public override void Resize(int newWidth, int newHeight)
        {
            pencilPoints = ResizeImage.resizeImage(pencilPoints, new Size(newWidth, newHeight));
        }
    }
}
