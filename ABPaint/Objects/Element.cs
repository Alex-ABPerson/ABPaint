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

        public int Top { get { return Y; } }
        public int Bottom { get { return Y + Height; } }

        public int Left { get { return X; } }
        public int Right { get { return X + Width; } }

        public abstract Bitmap ProcessImage();

        public abstract void Resize(int newWidth, int newHeight);
    }
}
