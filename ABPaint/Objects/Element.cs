using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ABPaint
{
    /// <summary>
    /// The objects that are found within this application on the canvas. An element is one of these objects.
    /// </summary>
    public abstract class Element
    {
        /// <summary>
        /// The location of this element.
        /// </summary>
        public int X, Y;

        /// <summary>
        /// The size of this element.
        /// </summary>
        public int Width, Height;

        /// <summary>
        /// The zindex, compared to all the other elements, of this element.
        /// </summary>
        public int zindex;

        /// <summary>
        /// Whether you can see this element.
        /// </summary>
        public bool Visible = true;

        /// <summary>
        /// Gets the top of this element.
        /// </summary>
        public int Top { get { return Y; } }

        /// <summary>
        /// Gets the bottom of this element.
        /// </summary>
        public int Bottom { get { return Y + Height; } }

        /// <summary>
        /// Gets the left of this element.
        /// </summary>
        public int Left { get { return X; } }

        /// <summary>
        /// Gets the right of this element.
        /// </summary>
        public int Right { get { return X + Width; } }

        /// <summary>
        /// What gets ran when the element is drawn.
        /// </summary>
        /// <returns>An image of the result of this element.</returns>
        public abstract Bitmap ProcessImage();

        /// <summary>
        /// Changes certain values within the element to adjust to a new size.
        /// </summary>
        /// <param name="newWidth">The new width.</param>
        /// <param name="newHeight">The new height.</param>
        public abstract void Resize(int newWidth, int newHeight);
    }
}
