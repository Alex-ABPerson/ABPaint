using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ABPaint
{
    public class CanvasBox : PictureBox
    {
        protected override void OnPaint(PaintEventArgs pe)
        {
            if (!Core.fillLock)
            {
                Core.paintLock = true;
                pe.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                pe.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                pe.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
                pe.Graphics.ScaleTransform(Core.MagnificationLevel, Core.MagnificationLevel); // Transform anything drawn to the zoom!
            
                base.OnPaint(pe);
                Core.paintLock = false;
            }
        }
    }
}
