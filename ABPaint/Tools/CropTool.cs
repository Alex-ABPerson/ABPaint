using ABPaint.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace ABPaint.Tools
{
    public class CropTool : PowerTool
    {
        public override bool UseRegionDrag { get { return true; } }
        public override bool OnlyRegionDragBitmap { get { return false; } }
        Windows.CropTool wnd;

        public override void Prepare()
        {
            wnd = new Windows.CropTool();

            wnd.Show();
        }

        public override void Apply(Rectangle dragRegion)
        {
            wnd.Close();
            // Do Stuff
        }
    }
}
