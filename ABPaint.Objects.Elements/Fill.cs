// ***********************************************************************
// Assembly         : ABPaint
// Author           : Alex
// Created          : 10-08-2017
//
// Last Modified By : Alex
// Last Modified On : 03-25-2018
// ***********************************************************************
// <copyright file="Fill.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using ABPaint.Engine;
using ABPaint.Objects;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPaint.Objects.Elements
{
    public class Fill : Element, IDisposable
    {
        private Color _fillColor;

        public Color FillColor
        {
            get
            {
                return _fillColor;
            }
            set
            {
                _fillColor = value;
            }
        }
        private Bitmap _fillPoints;

        public Bitmap FillPoints
        {
            get
            {
                return _fillPoints;
            }
            set
            {
                _fillPoints = value;
            }
        }

        public override void ProcessImage(Graphics g)
        {
            BrushDrawing.ChangeGraphicsColor(FillPoints, g, FillColor, DrawAtX, DrawAtY);
        }

        public override void Resize()
        {
            FillPoints = ResizeImage.ResizeNoAntiAlias(FillPoints, new Size(Width, Height));
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

                    if (FillPoints != null)
                        FillPoints.Dispose();
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
