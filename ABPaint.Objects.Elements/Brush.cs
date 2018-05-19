// ***********************************************************************
// Assembly         : ABPaint
// Author           : Alex
// Created          : 10-08-2017
//
// Last Modified By : Alex
// Last Modified On : 03-25-2018
// ***********************************************************************
// <copyright file="Brush.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Drawing;
using ABPaint.Engine;

namespace ABPaint.Objects.Elements
{
    public class Brush : Element, IDisposable
    {
        private Bitmap _brushPoint;

        public Bitmap BrushPoint
        {
            get
            {
                return _brushPoint;
            }
            set
            {
                _brushPoint = value;
            }
        }
        private Color _brushColor = Color.Black;

        public Color BrushColor
        {
            get
            {
                return _brushColor;
            }
            set
            {
                _brushColor = value;
            }
        }

        public override void ProcessImage(Graphics g)
        {
            if (!disposed)
                BrushDrawing.ChangeGraphicsColor(BrushPoint, g, BrushColor, DrawAtX, DrawAtY);
        }

        public override void Resize()
        {
            BrushPoint = ResizeImage.ResizeNoAntiAlias(BrushPoint, new Size(Width, Height));
        }

        public override void FinishResize()
        {
        }

        #region IDisposable Implementation

        public bool disposed;

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

                    if (BrushPoint != null)
                        BrushPoint.Dispose();
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
