// ***********************************************************************
// Assembly         : ABPaint
// Author           : Alex
// Created          : 10-08-2017
//
// Last Modified By : Alex
// Last Modified On : 03-02-2018
// ***********************************************************************
// <copyright file="Ellipse.cs" company="">
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

namespace ABPaint.Objects.Elements
{
    public class Ellipse : Element
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
        private Color _borderColor;

        public Color BorderColor
        {
            get
            {
                return _borderColor;
            }
            set
            {
                _borderColor = value;
            }
        }
        private int _borderSize;

        public int BorderSize
        {
            get
            {
                return _borderSize;
            }
            set
            {
                _borderSize = value;
            }
        }
        private bool _isFilled;

        public bool IsFilled
        {
            get
            {
                return _isFilled;
            }
            set
            {
                _isFilled = value;
            }
        }

        private bool isEllipse = true; // Temp Variable

        public override void ProcessImage(Graphics g)
        {
            if (IsFilled) g.FillEllipse(new SolidBrush(FillColor), DrawAtX, DrawAtY, Math.Abs(Width), Math.Abs(Height)); // Fill

            g.DrawEllipse(new Pen(BorderColor, BorderSize), (BorderSize / 2) + DrawAtX, (BorderSize / 2) + DrawAtY, Math.Abs(Width - (BorderSize)), Math.Abs(Height - (BorderSize)));
        }

        public override void Resize()
        {
        }

        public override void FinishResize()
        {
        }
    }
}
