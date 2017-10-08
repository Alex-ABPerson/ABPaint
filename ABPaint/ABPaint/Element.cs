using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ABPaint
{
    public abstract class Element
    {
        public int X, Y;
        public int Width, Height;
        public int zindex;
        public bool Visible = true;

        public abstract Bitmap ProcessImage();
    }
}
