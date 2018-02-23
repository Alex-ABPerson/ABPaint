using ABPaint.Objects;
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

        public override void ProcessImage(Graphics g)
        {
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            g.DrawString(mainText, fnt, new SolidBrush(clr), DrawAtX, DrawAtY);
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
            SizeF RealSize = MeasureText(mainText, fnt);
            float HeightScaleRatio = newHeight / RealSize.Height;
            float WidthScaleRatio = newWidth / RealSize.Width;
            float ScaleRatio = (HeightScaleRatio < WidthScaleRatio) ? ScaleRatio = HeightScaleRatio : ScaleRatio = WidthScaleRatio;
            float ScaleFontSize = fnt.Size * ScaleRatio;

            //fnt = new Font(fnt.FontFamily, Convert.ToSingle(Height / 2.5) + Convert.ToSingle(Width / 2.5), fnt.Style);
            //fnt = new Font(fnt.FontFamily, ScaleFontSize - 5, fnt.Style);
            fnt = new Font(fnt.FontFamily, ((ScaleFontSize - 5) > 0) ? ScaleFontSize - 5 : fnt.Size , fnt.Style);

            Program.mainForm.cmbSize.Text = fnt.Size.ToString();
        }
    }
}
