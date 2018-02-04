using ABPaint.Elements;
using ABPaint.Objects;
using ABPaint.Tools.Backend;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ABPaint.Tools.Backend.SaveSystem;

namespace ABPaint
{
    public static class Core
    {
        public static bool InOperation = false;
        public static MagicTool currentTool;

        public async static void PaintPreviewAsync()
        {
            while (InOperation)
                await Task.Delay(10);

            Program.mainForm.tskPP = new Task<Bitmap>(Core.PaintPreview);
            Program.mainForm.tskPP.Start();

            Program.mainForm.endImage = await Program.mainForm.tskPP;
        }

        /// <summary>
        /// Draws the preview. (Probably the most crucial method in the whole application!)
        /// </summary>
        /// <returns>An image for the result.</returns>
        public static Bitmap PaintPreview()
        {
            InOperation = true;

            Bitmap endResult = new Bitmap(savedata.imageSize.Width, savedata.imageSize.Height);

            //try
            //{
            // Draw the elements in order

                
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

                    if (!(ele is ImageE)) bmp.Dispose(); // Why don't we dispose it if it's an image? Because, for some reason Bitmaps actually reference where they came from so I dispose it here
                    // then it will dispose it in the ImageE as well, however, this doesn't affect other elements since their image is just for the preview and usually created from the data.
                }
            }

            Program.mainForm.endImage = endResult;

            InOperation = false;

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

            // Order the list based on zindex! But backwards so that the foreach picks up the top one!
            savedata.imageElements = savedata.imageElements.OrderBy(o => o.zindex).Reverse().ToList();

            foreach (Element ele in savedata.imageElements)
            {
                if (new Rectangle(ele.X - 10, ele.Y - 10, ele.Width + 20, ele.Height + 20).Contains(new Point(x, y)))
                {
                    // The mouse is in this element!

                    ele.zindex = SaveSystem.savedata.topZIndex++; // Brings to front

                    ret = ele;

                    break;
                }
            }

            // Order the list based on zindex!
            savedata.imageElements = savedata.imageElements.OrderBy(o => o.zindex).ToList();

            return ret;
        }

        public static void HandleKeyPress(Keys key)
        {
            switch (key)
            {
                case Keys.Delete:
                    HandleDelete();

                    break;
                case Keys.X:
                    if (Control.ModifierKeys == Keys.Control)
                        HandleCut();

                    break;
                case Keys.C:
                    if (Control.ModifierKeys == Keys.Control)
                        HandleCopy();

                    break;
                case Keys.V:
                    if (Control.ModifierKeys == Keys.Control)
                        HandlePaste();

                    break;
                case Keys.Enter:
                    HandleApply();

                    break;
                case (Keys.Control | Keys.OemMinus):
                case (Keys.Control | Keys.Subtract):             
                    HandleZoomOut();

                    break;
                case (Keys.Control | Keys.Oemplus):
                case (Keys.Control | Keys.Add):
                    
                    HandleZoomIn();

                    break;
            }
        }

        public static void UseTool(MagicTool tool)
        {
            if (tool.UseRegionDrag)
                Program.mainForm.IsInDragRegion = true;

            currentTool = tool;

            tool.Prepare();
        }

        public static void HandleApply()
        {
            if (currentTool != null)
                if (currentTool.UseRegionDrag)
                {
                    Program.mainForm.IsInDragRegion = false;
                    currentTool.Apply(Program.mainForm.dragRegionSelect);
                } else {
                    Program.mainForm.IsInDragRegion = false;
                    currentTool.Apply(new Rectangle());
                }
        }

        public static void HandleDelete()
        {
            if (!InOperation)
            {
                InOperation = true;
                if (Program.mainForm.selectedElement != null)
                    if (Program.mainForm.selectedTool == Tool.Selection)
                    {
                        savedata.imageElements.Remove(Program.mainForm.selectedElement);

                        Program.mainForm.selectedElement = null;

                        Program.mainForm.canvaspre.Invalidate();
                        Program.mainForm.endImage = PaintPreview();
                    }

                InOperation = false;
            }
        }

        public static void HandleCut()
        {
            if (!InOperation)
            {
                InOperation = true;

                HandleCopy();

                savedata.imageElements.Remove(Program.mainForm.selectedElement);
                Program.mainForm.selectedElement = null;

                Program.mainForm.canvaspre.Invalidate();
                Program.mainForm.endImage = Core.PaintPreview();

                InOperation = false;
            }
        }

        public static void HandleCopy()
        {
            if (!InOperation)
            {
                InOperation = true;
                Clipboard.SetDataObject("ABPAINTELEMENT" + ABJson.GDISupport.JsonSerializer.Serialize("", Program.mainForm.selectedElement, ABJson.GDISupport.JsonFormatting.Compact, 0, true).TrimEnd(','), true);

                InOperation = false;
            }
            //Clipboard.SetDataObject(Program.mainForm.selectedElement, true);
        }

        public static void HandlePaste()
        {
            if (!InOperation)
            {
                InOperation = true;
                //Clipboard.SetDataObject("ABPAINTELEMENT" + ABJson.GDISupport.JsonSerializer.Serialize("", Program.mainForm.selectedElement, ABJson.GDISupport.JsonFormatting.Compact, 0, true).TrimEnd(','), true);
                IDataObject data = Clipboard.GetDataObject();

                if (Clipboard.GetDataObject().GetDataPresent(DataFormats.Text) && Clipboard.GetDataObject().GetData(DataFormats.Text).ToString().StartsWith("ABPAINTELEMENT"))
                {
                
                    Element ele = ABJson.GDISupport.JsonClassConverter.ConvertJsonToObject<Element>(Clipboard.GetDataObject().GetData(DataFormats.Text).ToString().Remove(0, 14), true);

                    ele.zindex = savedata.topZIndex++;
                    savedata.imageElements.Add(ele);

                    Program.mainForm.selectedElement = ele;
                }

                Program.mainForm.canvaspre.Invalidate();
                Program.mainForm.endImage = Core.PaintPreview();
                InOperation = false;
            }
        }

        public static void HandleZoomIn()
        {
            if (Program.mainForm.MagnificationLevel < 16)
            {
                Program.mainForm.MagnificationLevel = Program.mainForm.MagnificationLevel * 2;
                Program.mainForm.label11.Text = "X" + Program.mainForm.MagnificationLevel;
                Program.mainForm.ReloadImage();
            }
        }

        public static void HandleZoomOut()
        {
            if (Program.mainForm.MagnificationLevel > 1)
            {
                Program.mainForm.MagnificationLevel = Program.mainForm.MagnificationLevel / 2;
                Program.mainForm.label11.Text = "X" + Program.mainForm.MagnificationLevel;
                Program.mainForm.ReloadImage();
            }
        }
    }
}
