// ***********************************************************************
// Assembly         : ABPaint
// Author           : Alex
// Created          : 01-13-2018
//
// Last Modified By : Alex
// Last Modified On : 03-29-2018
// ***********************************************************************
// <copyright file="CropTool.cs" company="">
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

namespace ABPaint.Tools
{
    public class CropTool : PowerTool, IDisposable
    {
        public override bool UseRegionDrag { get { return true; } }
        public override bool OnlyRegionDragBitmap { get { return false; } }
        Windows.CropTool wnd;

        public override void Prepare()
        {
            wnd = new Windows.CropTool();

            wnd.Show();
        }

        public override void Apply(Rectangle region)
        {
            wnd.Close();
            // Do Stuff
        }

        public override void Cancel()
        {
            wnd.Close();
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

                    if (wnd != null)
                        wnd.Dispose();
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
