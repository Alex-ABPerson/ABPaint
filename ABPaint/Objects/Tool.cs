using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPaint.Objects
{
    /// <summary>
    /// The tools within the toolbox - commonly used for knowing what tool is selected.
    /// </summary>
    public enum Tool
    {
        /// <summary>
        /// The selection tool. Used for selecting elements.
        /// </summary>
        Selection = 0,
        /// <summary>
        /// The bitmap selection tool. Used for selecting areas of bitmaps.
        /// </summary>
        BitmapSelection = 1,
        /// <summary>
        /// The pencil tool. Used to draw using a "pencil"
        /// </summary>
        Pencil = 2,
        /// <summary>
        /// The brush tool. Used to draw using a "brush"
        /// </summary>
        Brush = 3,
        /// <summary>
        /// The rectangle tool. Used to draw a rectangle.
        /// </summary>
        Rectangle = 4,
        /// <summary>
        /// The ellipse tool. Used to draw an ellipse.
        /// </summary>
        Ellipse = 5,
        /// <summary>
        /// The line tool. Used to draw a line.
        /// </summary>
        Line = 6,
        /// <summary>
        /// The fill tool. Used to fill in a certain color.
        /// </summary>
        Fill = 7,
        /// <summary>
        /// The text tool. Used to write text and draw it.
        /// </summary>
        Text = 8
    }
}
