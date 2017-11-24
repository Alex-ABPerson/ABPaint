using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ABPaint.Elements
{
    public class Text : Element
    {
        public string mainText;
        public Font fnt;
        public Color clr;

        public override Bitmap ProcessImage()
        {
            Bitmap toReturn = new Bitmap(Width, Height);
            Graphics g = Graphics.FromImage(toReturn);

            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            g.DrawString(mainText, fnt, new SolidBrush(clr), 0, 0);

            return toReturn;
        }

        public static Size MeasureText(string Text, Font Font)
        {
            TextFormatFlags flags
              = TextFormatFlags.Left
              | TextFormatFlags.Top
              | TextFormatFlags.NoPadding
              | TextFormatFlags.NoPrefix;
            Size szProposed = new Size(int.MaxValue, int.MaxValue);
            Size sz1 = TextRenderer.MeasureText(".", Font, szProposed, flags);
            Size sz2 = TextRenderer.MeasureText(Text + ".", Font, szProposed, flags);
            return new Size(sz2.Width - sz1.Width, sz2.Height);
        }

        public override void Resize(int newWidth, int newHeight)
        {
            throw new NotImplementedException();
        }
    }
}
