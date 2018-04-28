// ***********************************************************************
// Assembly         : ABPaint
// Author           : Alex
// Created          : 10-08-2017
//
// Last Modified By : Alex
// Last Modified On : 03-25-2018
// ***********************************************************************
// <copyright file="Pencil.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
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
    public class Pencil : Element, IDisposable
    {
        private Bitmap _pencilPoints;

        public Bitmap PencilPoints
        {
            get
            {
                return _pencilPoints;
            }
            set
            {
                _pencilPoints = value;
            }
        }
        private Color _pencilColor = Color.Blue;

        public Color PencilColor
        {
            get
            {
                return _pencilColor;
            }
            set
            {
                _pencilColor = value;
            }
        }

        public override void ProcessImage(Graphics g)
        {
            if (!disposed)
                BrushDrawing.ChangeGraphicsColor(PencilPoints, g, PencilColor, DrawAtX, DrawAtY);
        }

        public override void Resize()
        {
            PencilPoints = ResizeImage.ResizeNoAntiAlias(PencilPoints, new Size(Width, Height));
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

                    if (PencilPoints != null)
                        PencilPoints.Dispose();
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
