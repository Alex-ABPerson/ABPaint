﻿using System;
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
            g.DrawImage(BrushDrawing.ChangeImageColor(brushPoints, brushColor), DrawAtX, DrawAtY);
        }

        public override void Resize(int newWidth, int newHeight)
        {
            brushPoints = ResizeImage.resizeImageNoAntiAlias(brushPoints, new Size(newWidth, newHeight));
        }
    }
}
