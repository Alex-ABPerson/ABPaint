using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABPaint.Objects
{
    public abstract class PowerTool
    {
        /// <summary>
        /// If true, this tool will use the drag region to select an area.
        /// </summary>
        public abstract bool UseRegionDrag { get; }

        /// <summary>
        /// If true, you can only put a drag region on a bitmap.
        /// </summary>
        public abstract bool OnlyRegionDragBitmap { get; }

        /// <summary>
        /// The code that prepares the tool as soon at it is selected.
        /// </summary>
        public abstract void Prepare();

        /// <summary>
        /// The code that cancelled the tool when the user requests it.
        /// </summary>
        public abstract void Cancel();

        /// <summary>
        /// Once all the controls for the effect has been set this function will be run.
        /// </summary>
        /// <param name="dragRegion"></param>
        public abstract void Apply(Rectangle region);
    }
}
