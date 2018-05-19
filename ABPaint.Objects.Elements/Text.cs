// ***********************************************************************
// Assembly         : ABPaint
// Author           : Alex
// Created          : 10-08-2017
//
// Last Modified By : Alex
// Last Modified On : 03-29-2018
// ***********************************************************************
// <copyright file="Text.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using ABPaint.Objects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Forms;

namespace ABPaint.Objects.Elements
{
    public class Text : Element, IDisposable
    {
        private string _mainText;

        public string MainText
        {
            get
            {
                return _mainText;
            }
            set
            {
                _mainText = value;
            }
        }

        private Font _fnt;

        public Font Fnt
        {
            get
            {
                return _fnt;
            }
            set
            {
                _fnt = value;
            }
        }
        private Color _clr;

        public Color Clr
        {
            get
            {
                return _clr;
            }
            set
            {
                _clr = value;
            }
        }

        public override void ProcessImage(Graphics g)
        {
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            g.DrawString(MainText, Fnt, new SolidBrush(Clr), DrawAtX, DrawAtY);
        }

        public static Size MeasureText(string text, Font font)
        {
            TextFormatFlags flags
              = TextFormatFlags.Left
              | TextFormatFlags.Top
              | TextFormatFlags.NoPadding
              | TextFormatFlags.NoPrefix;
            Size szProposed = new Size(int.MaxValue, int.MaxValue);
            Size sz1 = TextRenderer.MeasureText(".", font, szProposed, flags);
            Size sz2 = TextRenderer.MeasureText(text + ".", font, szProposed, flags);
            return new Size(sz2.Width - sz1.Width, sz2.Height);
        }

        public override void Resize()
        {
            SizeF realSize = MeasureText(MainText, Fnt);
            float heightScaleRatio = Height / realSize.Height;
            float widthScaleRatio = Width / realSize.Width;
            float scaleRatio = (heightScaleRatio < widthScaleRatio) ? heightScaleRatio : widthScaleRatio;
            float scaleFontSize = Fnt.Size * scaleRatio;

            //fnt = new Font(fnt.FontFamily, Convert.ToSingle(Height / 2.5) + Convert.ToSingle(Width / 2.5), fnt.Style);
            //fnt = new Font(fnt.FontFamily, ScaleFontSize - 5, fnt.Style);
            Fnt = new Font(Fnt.FontFamily, ((scaleFontSize - 5) > 0) ? scaleFontSize - 5 : Fnt.Size , Fnt.Style);
        }

        public override void FinishResize()
        {
            
        }

        #region IDisposable Implementation

        protected bool disposed;

        protected virtual void Dispose(bool disposing)
        {
            lock (this)
            {
                // Do nothing if the object has already been disposed of.
                if (disposed)
                    return;

                if (disposing)
                {
                    // Release disposable objects used by this instance here.

                    if (_fnt != null)
                        _fnt.Dispose();
                }

                // Release unmanaged resources here. Don't access reference type fields.

                // Remember that the object has been disposed of.
                disposed = true;
            }
        }

        public virtual void Dispose()
        {
            Dispose(true);
            // Unregister object for finalization.
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
