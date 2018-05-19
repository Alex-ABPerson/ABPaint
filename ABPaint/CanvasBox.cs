// ***********************************************************************
// Assembly         : ABPaint
// Author           : Alex
// Created          : 02-17-2018
//
// Last Modified By : Alex
// Last Modified On : 03-17-2018
// ***********************************************************************
// <copyright file="CanvasBox.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Windows.Forms;

namespace ABPaint
{
    public class CanvasBox : PictureBox
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            e.Graphics.ScaleTransform(Core.Core.MagnificationLevel, Core.Core.MagnificationLevel); // Transform anything drawn to the zoom!
            
            base.OnPaint(e);
        }
    }
}
