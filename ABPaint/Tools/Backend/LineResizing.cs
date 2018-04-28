// ***********************************************************************
// Assembly         : ABPaint
// Author           : Alex
// Created          : 02-11-2018
//
// Last Modified By : Alex
// Last Modified On : 02-28-2018
// ***********************************************************************
// <copyright file="LineResizing.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using ABPaint.Elements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPaint.Tools.Backend
{
    public static class LineResizing
    {
        public static void Resize(ref Line lineEle)
        {
            Point startPoint = lineEle.StartPoint;
            Point endPoint = lineEle.EndPoint;

            if (lineEle.StartPoint.X < lineEle.EndPoint.X)
            {
                lineEle.X += lineEle.StartPoint.X - lineEle.Thickness;
                lineEle.Width = lineEle.EndPoint.X - lineEle.StartPoint.X + lineEle.Thickness * 2;
                startPoint.X = lineEle.Thickness;
                endPoint.X = lineEle.Width - lineEle.Thickness;
            }
            else
            {
                lineEle.X += lineEle.EndPoint.X - lineEle.Thickness;
                lineEle.Width = lineEle.StartPoint.X - lineEle.EndPoint.X + lineEle.Thickness * 2;
                endPoint.X = lineEle.Thickness;
                startPoint.X = lineEle.Width - lineEle.Thickness;
            }

            if (lineEle.StartPoint.Y < lineEle.EndPoint.Y)
            {
                lineEle.Y += lineEle.StartPoint.Y - lineEle.Thickness;
                lineEle.Height = lineEle.EndPoint.Y - lineEle.StartPoint.Y + lineEle.Thickness * 2;
                startPoint.Y = lineEle.Thickness;
                endPoint.Y = lineEle.Height - lineEle.Thickness;
            }
            else
            {
                lineEle.Y += lineEle.EndPoint.Y - lineEle.Thickness;
                lineEle.Height = lineEle.StartPoint.Y - lineEle.EndPoint.Y + lineEle.Thickness * 2;
                endPoint.Y = lineEle.Thickness;
                startPoint.Y = lineEle.Height - lineEle.Thickness;
            }

            lineEle.StartPoint = startPoint;
            lineEle.EndPoint = endPoint;

            //if (lineEle.StartPoint.X < lineEle.EndPoint.X)
            //{

            //} else {
            //lineEle.EndPoint.X = lineEle.Thickness;
            //lineEle.StartPoint.X = lineEle.Width - lineEle.Thickness;
            //}

            // lineEle.Height - lineEle.Thickness
        }
    }
}
