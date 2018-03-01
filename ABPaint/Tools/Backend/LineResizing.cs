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
            if (lineEle.StartPoint.X < lineEle.EndPoint.X)
            {
                lineEle.X += lineEle.StartPoint.X - lineEle.Thickness;
                lineEle.Width = lineEle.EndPoint.X - lineEle.StartPoint.X + lineEle.Thickness * 2;
                lineEle.StartPoint.X = lineEle.Thickness;
                lineEle.EndPoint.X = lineEle.Width - lineEle.Thickness;

                //lineEle.X -= lineEle.Thickness;
                //lineEle.Width += lineEle.Thickness * 2;

                //lineEle.StartPoint.X = lineEle.Thickness;
                //lineEle.EndPoint.X = lineEle.Width - lineEle.Thickness;
            }
            else
            {
                lineEle.X += lineEle.EndPoint.X - lineEle.Thickness;
                lineEle.Width = lineEle.StartPoint.X - lineEle.EndPoint.X + lineEle.Thickness * 2;
                lineEle.EndPoint.X = lineEle.Thickness;
                lineEle.StartPoint.X = lineEle.Width - lineEle.Thickness;
            }

            if (lineEle.StartPoint.Y < lineEle.EndPoint.Y)
            {
                lineEle.Y += lineEle.StartPoint.Y - lineEle.Thickness;
                lineEle.Height = lineEle.EndPoint.Y - lineEle.StartPoint.Y + lineEle.Thickness * 2;
                lineEle.StartPoint.Y = lineEle.Thickness;
                lineEle.EndPoint.Y = lineEle.Height - lineEle.Thickness;

                //lineEle.Y -= lineEle.Thickness;
                //lineEle.Height += lineEle.Thickness * 2;
            }
            else
            {
                lineEle.Y += lineEle.EndPoint.Y - lineEle.Thickness;
                lineEle.Height = lineEle.StartPoint.Y - lineEle.EndPoint.Y + lineEle.Thickness * 2;
                lineEle.EndPoint.Y = lineEle.Thickness;
                lineEle.StartPoint.Y = lineEle.Height - lineEle.Thickness;
            }



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
