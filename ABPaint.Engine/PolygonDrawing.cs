// ***********************************************************************
// Assembly         : ABPaint
// Author           : Alex
// Created          : 12-31-2017
//
// Last Modified By : Alex
// Last Modified On : 12-31-2017
// ***********************************************************************
// <copyright file="PolygonDrawing.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPaint.Engine
{
    // ===================================
    // WARNING!
    // ===================================
    // THIS CLASS WAS SCRAPPED DUE TO THERE ALREADY BEING POLYGON DRAWING FUNCTION!!! (But I'm keeping it here because I'm proud of literally making the function FIRST TIME!)
    /// <summary>
    /// Adds control for drawing custom shapes, with just a list of points!
    /// </summary>
    public static class PolygonDrawing
    {
        /// <summary>
        /// Draws a shape to a GraphicsPath (where you can do whatever you want with it).
        /// </summary>
        /// <param name="points">The points to draw.</param>
        public static GraphicsPath DrawPolygon(List<Point> points)
        {
            GraphicsPath grph = new GraphicsPath();

            grph.StartFigure();

            for (int i = 1; i < points.Count; i++)
                grph.AddLine(points[i - 1], points[i]);

            return grph;
        }
    }
}
