// ***********************************************************************
// Assembly         : ABPaint
// Author           : Alex
// Created          : 12-29-2017
//
// Last Modified By : Alex
// Last Modified On : 03-29-2018
// ***********************************************************************
// <copyright file="ImageE.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using ABPaint.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ABPaint.Objects.Elements
{
    public class ImageE : Element, IDisposable
    {
        private Bitmap _mainImage;

        public Bitmap MainImage
        {
            get
            {
                return _mainImage;
            }
            set
            {
                _mainImage = value;
            }
        }

        public ImageE(Bitmap img = null)
        {
            MainImage = img;
        }

        public override void ProcessImage(Graphics g)
        {
            g.DrawImage(MainImage, DrawAtX, DrawAtY);
        }

        public override void Resize()
        {
            //MainImage = ResizeImage.Resize(MainImage, new Size(Width, Height));
            // TODO: Add Resizing On Pencil, Brush, Fill and Images!
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

                    if (_mainImage != null)
                        _mainImage.Dispose();
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
