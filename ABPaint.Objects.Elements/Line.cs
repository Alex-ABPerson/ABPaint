// ***********************************************************************
// Assembly         : ABPaint
// Author           : Alex
// Created          : 10-08-2017
//
// Last Modified By : Alex
// Last Modified On : 03-02-2018
// ***********************************************************************
// <copyright file="Line.cs" company="">
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
    public class Line : Element
    {
        private Color _color;

        public Color Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
            }
        }
        private Point _endPoint;

        public Point EndPoint
        {
            get
            {
                return _endPoint;
            }
            set
            {
                _endPoint = value;
            }
        }
        private Point _startPoint;

        public Point StartPoint
        {
            get
            {
                return _startPoint;
            }
            set
            {
                _startPoint = value;
            }
        }
        private int _thickness;

        public int Thickness
        {
            get
            {
                return _thickness;
            }
            set
            {
                _thickness = value;
            }
        }

        public override void ProcessImage(Graphics g)
        {
            g.DrawLine(new Pen(Color, Thickness), StartPoint.X + DrawAtX, StartPoint.Y + DrawAtY, EndPoint.X + DrawAtX, EndPoint.Y + DrawAtY);
        }

        public override void Resize()
        {
        }

        public override void FinishResize()
        {
        }
    }
}
