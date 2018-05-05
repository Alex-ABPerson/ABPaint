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
using System.Windows.Forms;

namespace ABPaint.Tools
{
    public class CropTool : PowerTool
    {
        public override bool UseRegionDrag { get { return true; } }
        public override bool OnlyRegionDragBitmap { get { return false; } }

        public override void Prepare()
        {
            Program.MainForm.wnd = new Windows.CropToolWnd();
            Application.Run(Program.MainForm.wnd);
        }

        public override void Apply(Rectangle region)
        {
            // Do Stuff
        }

        public override void Cancel()
        {
            // Do Stuff
        }
    }
}
