using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ABPaint.Tools.Backend.SaveSystem;

namespace ABPaint
{
    public static class Core
    {
        /// <summary>
        /// Draws the preview. (Probably the most crucial method in the whole application!)
        /// </summary>
        /// <returns>An image for the result.</returns>
        public static Bitmap PaintPreview()
        {
            //try
            //{
            // Draw the elements in order

            Bitmap endResult = new Bitmap(savedata.imageSize.Width, savedata.imageSize.Height);
            Graphics g = Graphics.FromImage(endResult);

            // Order them by zindex:
            savedata.imageElements = savedata.imageElements.OrderBy(o => o.zindex).ToList();

            // Now draw them all!

            foreach (Element ele in savedata.imageElements)
            {
                if (ele.Visible)
                {
                    Bitmap bmp = ele.ProcessImage();
                    g.DrawImage(bmp, ele.X, ele.Y);
                    bmp.Dispose();
                }
            }

            Program.mainForm.endImage = endResult;
            return endResult;
            //} catch { return endImage; }
        }

        /// <summary>
        /// Selects the element at the specified X and Y.
        /// </summary>
        /// <param name="x">The X to search for the element.</param>
        /// <param name="y">The Y to search for the element.</param>
        /// <returns>The element found at the location.</returns>
        public static Element selectElementByLocation(int x, int y)
        {
            Element ret = null;
            // Order the list based on zindex!
            savedata.imageElements = savedata.imageElements.OrderBy(o => o.zindex).ToList();

            foreach (Element ele in savedata.imageElements)
            {
                if (new Rectangle(ele.X - 10, ele.Y - 10, ele.Width + 20, ele.Height + 20).Contains(new Point(x, y)))
                {
                    // The mouse is in this element!

                    ele.zindex = Program.mainForm.topZIndex++; // Brings to front

                    ret = ele;

                    continue;
                }
            }

            return ret;
        }

        public static void HandleKeyPress(Keys key)
        {
            switch (key)
            {
                case Keys.Delete:
                    if (Program.mainForm.selectedElement != null)
                        if (Program.mainForm.selectedTool == Objects.Tool.Selection)
                        {
                            savedata.imageElements.Remove(Program.mainForm.selectedElement);
                            Program.mainForm.selectedElement = null;
                            Core.selectElementByLocation(0, 0); // Deselects everything.
                            Program.mainForm.canvaspre.Invalidate();
                            Program.mainForm.endImage = Core.PaintPreview();
                        }

                    break;
                    //case Keys.ShiftKey: THIS FEATURE IS CURRENTLY UN-NEEDED AND ISN'T REALLY NECESSARY, MAYBE IN BETA 1 OR SOMETHING! 
                    //    if (selectedElement != null)
                    //        if (CornerSelected != 0)
                    //            LimitMouse = true;

                    //    canvaspre.Invalidate();

                    //    break;
            }
        }
    }
}
