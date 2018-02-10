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
    /// <summary>
    /// The entire core of ABPaint
    /// </summary>
    public static class Core
    {
        public static bool InOperation = false;
        public static PowerTool currentTool;
        private static Object paintLock = new Object(); // A lock for a painting
        private static Object editLock = new Object(); // A lock for editing imageElements
        private static Object actionLock = new Object(); // A lock for doing big processing that involve changing variables like "selectedElement"

        #region Main Variables
        // General Variables

        public static Tool selectedTool = Tool.Selection;

        public static int selectedPalette = 1;
        public static Element currentDrawingElement;
        public static Graphics currentDrawingGraphics;
        public static bool MouseDownOnCanvas = false;
        public static Task<Bitmap> tskPP = null;

        public static Point mousePoint = new Point(0, 0);

        // Drag Region
        public static Rectangle dragRegionSelect;
        public static bool IsInDragRegion;
        public static bool CurrentlyDragging;

        // The image + Extra SolidBrush

        private static Bitmap endimg; // Just because you have to...

        public static Bitmap endImage
        {
            get
            {
                return endimg;
            }
            set
            {
                endimg = value;
                Program.mainForm.canvaspre.Invalidate();
            }
        }


        public static SolidBrush sb101 = new SolidBrush(Color.FromArgb(1, 0, 1));

        // Magnification level.
        public static int MagnificationLevel = 1;

        // All the things needed for pencil + brush
        public static System.Drawing.Drawing2D.GraphicsPath grph = new System.Drawing.Drawing2D.GraphicsPath();
        public static Point DrawingMin;
        public static Point DrawingMax;
        public static Point lastMousePoint;
        public static Point startPoint;

        public static int LastX;
        public static int LastY;

        // Fill?
        static Task<Bitmap> fill;

        // The Selection tool stuff
        public static Element selectedElement;
        public static Size IsMovingGap;
        public static Point IsMovingOld;
        public static bool IsMoving = false;
        public static bool IsOnSelection = false;

        // Resizing 
        public static int CornerSelected = 0;
        public static Point BeforeResizePoint;
        public static Size BeforeResizeSize;
        public static bool Resized = false;
        public static bool LimitMouse;

        // Text
        public static bool BoldSelected = false;
        public static bool ItalicSelected = false;
        public static bool UnderlineSelected = false;
        #endregion

        public async static void PaintPreviewAsync()
        {
            while (InOperation)
                await Task.Delay(10);

            tskPP = new Task<Bitmap>(Core.PaintPreview);
            tskPP.Start();

            endImage = await tskPP;
        }

        /// <summary>
        /// Draws the preview. (Probably the most crucial method in the whole application!)
        /// </summary>
        /// <returns>An image for the result.</returns>
        public static Bitmap PaintPreview()
        {
            lock (paintLock)
            {
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

                endImage = endResult;

                return endResult;
            }
               
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

            lock (editLock)
            {
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
            }

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
                #region Arrow Keys
                case Keys.Left:
                    if (selectedElement != null)
                        if (selectedElement.X > 0) selectedElement.X -= 1;

                    break;
                case Keys.Right:
                    if (selectedElement != null)
                        if ((selectedElement.X + selectedElement.Width) < savedata.imageSize.Width) selectedElement.X += 1;

                    break;
                case Keys.Up:
                    if (selectedElement != null)
                        if (selectedElement.Y > 0) selectedElement.Y -= 1;

                    break;
                case Keys.Down:
                    if (selectedElement != null)
                        if ((selectedElement.Y + selectedElement.Height) < savedata.imageSize.Height) selectedElement.Y += 1;

                    break;
                case (Keys.Left | Keys.Alt):
                    if (selectedElement != null)
                        if (selectedElement.X > 9) selectedElement.X -= 10;

                    break;
                case (Keys.Right | Keys.Alt):
                    if (selectedElement != null)
                        if ((selectedElement.X + selectedElement.Width) < savedata.imageSize.Width - 9) selectedElement.X += 10;

                    break;
                case (Keys.Up | Keys.Alt):
                    if (selectedElement != null)
                        if (selectedElement.Y > 9) selectedElement.Y -= 10;

                    break;
                case (Keys.Down | Keys.Alt):
                    if (selectedElement != null)
                        if ((selectedElement.Y + selectedElement.Height) < savedata.imageSize.Height - 9) selectedElement.Y += 10;

                    break;
                case (Keys.Left | Keys.Control):
                    if (Program.mainForm.appcenter.HorizontalScroll.Value > 1)
                        Program.mainForm.appcenter.HorizontalScroll.Value -= 1;

                    break;
                case (Keys.Right | Keys.Control):
                    if (Program.mainForm.appcenter.HorizontalScroll.Value < Program.mainForm.appcenter.VerticalScroll.Maximum - 1)
                        Program.mainForm.appcenter.HorizontalScroll.Value += 1;

                    break;
                case (Keys.Up | Keys.Control):
                    if (Program.mainForm.appcenter.VerticalScroll.Value > 1)
                        Program.mainForm.appcenter.VerticalScroll.Value -= 1;

                    break;
                case (Keys.Down | Keys.Control):
                    if (Program.mainForm.appcenter.VerticalScroll.Value < Program.mainForm.appcenter.VerticalScroll.Maximum - 1)
                        Program.mainForm.appcenter.VerticalScroll.Value += 1;

                    break;
                case (Keys.Left | Keys.Control | Keys.Alt):
                    if (Program.mainForm.appcenter.HorizontalScroll.Value > 10)
                        Program.mainForm.appcenter.HorizontalScroll.Value -= 10;

                    break;
                case (Keys.Right | Keys.Control | Keys.Alt):
                    if (Program.mainForm.appcenter.HorizontalScroll.Value < Program.mainForm.appcenter.VerticalScroll.Maximum - 10)
                        Program.mainForm.appcenter.HorizontalScroll.Value += 10;

                    break;
                case (Keys.Up | Keys.Control | Keys.Alt):
                    if (Program.mainForm.appcenter.VerticalScroll.Value > 10)
                        Program.mainForm.appcenter.VerticalScroll.Value -= 10;

                    break;
                case (Keys.Down | Keys.Control | Keys.Alt):
                    if (Program.mainForm.appcenter.VerticalScroll.Value < Program.mainForm.appcenter.VerticalScroll.Maximum - 10)
                        Program.mainForm.appcenter.VerticalScroll.Value += 10;

                    break;
                #endregion
                case (Keys.Control | Keys.OemMinus):
                case (Keys.Control | Keys.Subtract):             
                    HandleZoomOut();

                    break;
                case (Keys.Control | Keys.Oemplus):
                case (Keys.Control | Keys.Add):          
                    HandleZoomIn();

                    break;
            }

            PaintPreviewAsync();
            Program.mainForm.canvaspre.Invalidate();
        }

        public static void UseTool(PowerTool tool)
        {
            lock (actionLock)
            {
                if (tool.UseRegionDrag)
                    IsInDragRegion = true;

                currentTool = tool;

                tool.Prepare();
            }
        }

        public static void HandleApply()
        {
            lock (actionLock)
            {
                if (currentTool != null)
                    if (currentTool.UseRegionDrag)
                    {
                        IsInDragRegion = false;
                        currentTool.Apply(dragRegionSelect);
                    }
                    else
                    {
                        IsInDragRegion = false;
                        currentTool.Apply(new Rectangle());
                    }
            }
        }

        public static void HandleDelete()
        {
            lock (actionLock)
            {
                if (selectedElement != null)
                    if (selectedTool == Tool.Selection)
                    {
                        savedata.imageElements.Remove(selectedElement);

                       selectedElement = null;

                        Program.mainForm.canvaspre.Invalidate();
                        endImage = PaintPreview();
                    }
            }
        }

        public static void HandleCut()
        {
            lock (actionLock)
            {
                HandleCopy();

                savedata.imageElements.Remove(selectedElement);
                selectedElement = null;

                Program.mainForm.canvaspre.Invalidate();
                endImage = Core.PaintPreview();
            }
        }

        public static void HandleCopy()
        {
            lock (actionLock)
            {
                Clipboard.SetDataObject("ABPELE" + ABJson.GDISupport.JsonSerializer.Serialize("", selectedElement, ABJson.GDISupport.JsonFormatting.Compact, 0, true).TrimEnd(','), true);
            }
            //Clipboard.SetDataObject(Program.mainForm.selectedElement, true);
        }

        public static void HandlePaste()
        {
            lock (actionLock)
            {
                //Clipboard.SetDataObject("ABPAINTELEMENT" + ABJson.GDISupport.JsonSerializer.Serialize("", Program.mainForm.selectedElement, ABJson.GDISupport.JsonFormatting.Compact, 0, true).TrimEnd(','), true);
                IDataObject data = Clipboard.GetDataObject();

                if (data.GetDataPresent(DataFormats.Text) && data.GetData(DataFormats.Text).ToString().StartsWith("ABPELE"))
                {            
                    Element ele = ABJson.GDISupport.JsonClassConverter.ConvertJsonToObject<Element>(data.GetData(DataFormats.Text).ToString().Remove(0, 6), true);

                    ele.zindex = savedata.topZIndex++;
                    AddElement(ele);

                    selectedElement = ele;
                }

                Program.mainForm.canvaspre.Invalidate();
                endImage = Core.PaintPreview();
            }
        }

        public static void HandleZoomIn()
        {
            if (MagnificationLevel < 16)
            {
                MagnificationLevel = MagnificationLevel * 2;
                Program.mainForm.label11.Text = "X" + MagnificationLevel;
                Program.mainForm.ReloadImage();
            }
        }

        public static void HandleZoomOut()
        {
            if (MagnificationLevel > 1)
            {
                MagnificationLevel = MagnificationLevel / 2;
                Program.mainForm.label11.Text = "X" + MagnificationLevel;
                Program.mainForm.ReloadImage();
            }
        }

        public static void AddElement(Element element)
        {
            lock (editLock)
            {
                savedata.imageElements.Add(element);
            }

            // TODO: Add undo option!
        }

        // It is recommended that you hide the lower two - however, if you don't... at least hide the "Fill + Text" region in the MouseDown...
        #region Mouse Selection Handlers
        public static void HandleMouseDownSelection(Point mouseLoc)
        {
            selectedElement = Core.selectElementByLocation(mouseLoc.X, mouseLoc.Y);

            if (selectedElement == null) Program.mainForm.ShowProperties("Selection Tool - Nothing selected!", false, false, false, false, false, false, Program.mainForm.GetCurrentColor());
            if (selectedElement is Pencil) Program.mainForm.ShowProperties("Selection Tool - Pencil", false, false, true, false, false, false, ((Pencil)selectedElement).pencilColor);
            if (selectedElement is Elements.Brush) Program.mainForm.ShowProperties("Selection Tool - Brush", false, false, true, false, false, false, ((Elements.Brush)selectedElement).brushColor);
            if (selectedElement is RectangleE) Program.mainForm.ShowProperties("Selection Tool - Rectangle", true, true, false, true, false, false, ((RectangleE)selectedElement).fillColor);
            if (selectedElement is Ellipse) Program.mainForm.ShowProperties("Selection Tool - Ellipse", true, true, false, true, false, false, ((Ellipse)selectedElement).FillColor);
            if (selectedElement is Line) Program.mainForm.ShowProperties("Selection Tool - Line", false, false, true, false, true, false, ((Line)selectedElement).color);
            if (selectedElement is Fill) Program.mainForm.ShowProperties("Selection Tool - Fill", false, false, true, false, false, false, ((Fill)selectedElement).fillColor);
            if (selectedElement is Text) Program.mainForm.ShowProperties("Selection Tool - Text", false, false, true, false, false, true, ((Text)selectedElement).clr, ((Text)selectedElement).mainText, ((Text)selectedElement).fnt);
            // Add more

            Program.mainForm.canvaspre.Invalidate();

            if (selectedElement != null)
            {
                if (new Rectangle(selectedElement.X - (10 / MagnificationLevel), selectedElement.Y - (10 / MagnificationLevel), selectedElement.Width + (20 / MagnificationLevel), selectedElement.Height + (20 / MagnificationLevel)).Contains(mouseLoc))
                {
                    // The mouse is in this element!

                    // I know it's bad to hard code certain elements but the thing is that line is completely different to ANY other element I could think of in resizing so I have to do this for it...

                    if (selectedElement is Line)
                    {
                        // Check if the mouse is on the two points otherwise move the element.

                        CornerSelected = 0;

                        // First point
                        if (new Rectangle(((Line)selectedElement).StartPoint.X + selectedElement.X - (10 / MagnificationLevel), ((Line)selectedElement).StartPoint.Y + selectedElement.Y - (10 / MagnificationLevel), (20 / MagnificationLevel), (20 / MagnificationLevel)).Contains(mouseLoc)) CornerSelected = 1;

                        // Second point
                        if (new Rectangle(((Line)selectedElement).EndPoint.X + selectedElement.X - (10 / MagnificationLevel), ((Line)selectedElement).EndPoint.Y + selectedElement.Y - (10 / MagnificationLevel), (20 / MagnificationLevel), (20 / MagnificationLevel)).Contains(mouseLoc)) CornerSelected = 2;
                    }
                    else
                    {
                        if (!(selectedElement is Pencil) && !(selectedElement is Elements.Brush) && !(selectedElement is Fill))
                        {
                            // Check if the mouse is on the scaling points otherwise move the element.

                            CornerSelected = 0;

                            // Top left corner
                            if (new Rectangle(selectedElement.X - (10 / MagnificationLevel), selectedElement.Y - (10 / MagnificationLevel), (20 / MagnificationLevel), (20 / MagnificationLevel)).Contains(mouseLoc)) CornerSelected = 1;

                            // Top Right Corner
                            if (new Rectangle(selectedElement.Right - (10 / MagnificationLevel), selectedElement.Y - (10 / MagnificationLevel), (20 / MagnificationLevel), (20 / MagnificationLevel)).Contains(mouseLoc)) CornerSelected = 2;

                            // Bottom Left corner
                            if (new Rectangle(selectedElement.X - (10 / MagnificationLevel), selectedElement.Bottom - (10 / MagnificationLevel), (20 / MagnificationLevel), (20 / MagnificationLevel)).Contains(mouseLoc)) CornerSelected = 3;

                            // Bottom Right corner
                            if (new Rectangle(selectedElement.Right - (10 / MagnificationLevel), selectedElement.Bottom - (10 / MagnificationLevel), (20 / MagnificationLevel), (20 / MagnificationLevel)).Contains(mouseLoc)) CornerSelected = 4;
                        }
                    }

                    if (CornerSelected == 0)
                    {
                        // Move the element

                        IsMovingOld = new Point(selectedElement.X, selectedElement.Y);

                        IsMovingGap.Width = mouseLoc.X - selectedElement.X;
                        IsMovingGap.Height = mouseLoc.Y - selectedElement.Y;

                        Program.mainForm.movingRefresh.Start();
                        IsMoving = true;
                    }
                    else
                    {
                        if (selectedElement is Line)
                        {
                            if (CornerSelected == 1) BeforeResizePoint = new Point(((Line)selectedElement).StartPoint.X, ((Line)selectedElement).StartPoint.Y);
                            else if (CornerSelected == 2) BeforeResizePoint = new Point(((Line)selectedElement).EndPoint.X, ((Line)selectedElement).EndPoint.Y);
                        }
                        else
                        {
                            if (CornerSelected == 1) BeforeResizePoint = new Point(selectedElement.X, selectedElement.Y);
                            else if (CornerSelected == 2) BeforeResizePoint = new Point(selectedElement.X + selectedElement.Width, selectedElement.Y);
                            else if (CornerSelected == 3) BeforeResizePoint = new Point(selectedElement.X, selectedElement.Y + selectedElement.Height);
                            else if (CornerSelected == 4) BeforeResizePoint = new Point(selectedElement.X + selectedElement.Width, selectedElement.Y + selectedElement.Height);
                        }
                        BeforeResizeSize = new Size(selectedElement.Width, selectedElement.Height);
                    }

                }
            }
        }

        public static void HandleMouseMoveSelection(Point mouseLoc)
        {
            if (selectedElement != null)
            {
                if (IsMoving)
                {
                    selectedElement.X = mouseLoc.X - IsMovingGap.Width;
                    selectedElement.Y = mouseLoc.Y - IsMovingGap.Height;
                }
                else
                {
                    if (CornerSelected == 0)
                    {
                        if (new Rectangle(selectedElement.X - 10, selectedElement.Y - 10, selectedElement.Width + 20, selectedElement.Height + 20).Contains(mouseLoc))
                            IsOnSelection = true;
                        else
                            IsOnSelection = false;
                    }
                    else
                    {
                        if (selectedElement is Line)
                        {

                            //selectedElement.X = BeforeResizePoint.X + ((((Line)selectedElement).StartPoint.X < ((Line)selectedElement).EndPoint.X) ? ((Line)selectedElement).StartPoint.X : ((Line)selectedElement).EndPoint.X);
                            //selectedElement.Y = BeforeResizePoint.X + ((((Line)selectedElement).StartPoint.Y < ((Line)selectedElement).EndPoint.Y) ? ((Line)selectedElement).StartPoint.Y : ((Line)selectedElement).EndPoint.Y);

                            //int proposedX = (((Line)selectedElement).StartPoint.X < ((Line)selectedElement).EndPoint.X) ? ((Line)selectedElement).StartPoint.X + ((Line)selectedElement).BeforeResizeX : ((Line)selectedElement).EndPoint.X + ((Line)selectedElement).BeforeResizeX;
                            //int proposedY = (((Line)selectedElement).StartPoint.Y < ((Line)selectedElement).EndPoint.Y) ? ((Line)selectedElement).StartPoint.Y + ((Line)selectedElement).BeforeResizeY : ((Line)selectedElement).EndPoint.Y + ((Line)selectedElement).BeforeResizeY;

                            if (CornerSelected == 1)
                            {
                                ((Line)selectedElement).StartPoint = new Point(mouseLoc.X - ((Line)selectedElement).BeforeResizeX, mouseLoc.Y - ((Line)selectedElement).BeforeResizeY);

                                Program.mainForm.movingRefresh.Start();
                            }
                            else if (CornerSelected == 2)
                            {
                                ((Line)selectedElement).EndPoint = new Point(mouseLoc.X - ((Line)selectedElement).BeforeResizeX, mouseLoc.Y - ((Line)selectedElement).BeforeResizeY);

                                Program.mainForm.movingRefresh.Start();
                            }

                            int proposedX = 0, proposedY = 0;

                            int proposedWidth = (((Line)selectedElement).StartPoint.X > ((Line)selectedElement).EndPoint.X) ? ((Line)selectedElement).StartPoint.X : ((Line)selectedElement).EndPoint.X;
                            int proposedHeight = (((Line)selectedElement).StartPoint.Y > ((Line)selectedElement).EndPoint.Y) ? ((Line)selectedElement).StartPoint.Y : ((Line)selectedElement).EndPoint.Y;

                            //if (proposedX > 0) selectedElement.X = proposedX - Convert.ToInt32(string.IsNullOrEmpty(txtBThick.Text) ? "0" : txtBThick.Text);
                            //if (proposedY > 0) selectedElement.Y = proposedY - Convert.ToInt32(string.IsNullOrEmpty(txtBThick.Text) ? "0" : txtBThick.Text);

                            if (((Line)selectedElement).StartPoint.X < 0) proposedX = selectedElement.X + (((Line)selectedElement).StartPoint.X - ((Line)selectedElement).ResizeFilledX);
                            if (((Line)selectedElement).StartPoint.Y < 0) proposedY = selectedElement.Y + (((Line)selectedElement).StartPoint.Y - ((Line)selectedElement).ResizeFilledY);

                            if (proposedWidth > 0) selectedElement.Width = proposedWidth + Convert.ToInt32(string.IsNullOrEmpty(Program.mainForm.txtBThick.Text) ? "0" : Program.mainForm.txtBThick.Text);
                            if (proposedHeight > 0) selectedElement.Height = proposedHeight + Convert.ToInt32(string.IsNullOrEmpty(Program.mainForm.txtBThick.Text) ? "0" : Program.mainForm.txtBThick.Text);

                            if (proposedX != 0) selectedElement.X = proposedX - Convert.ToInt32(string.IsNullOrEmpty(Program.mainForm.txtBThick.Text) ? "0" : Program.mainForm.txtBThick.Text);
                            if (proposedY != 0) selectedElement.Y = proposedY - Convert.ToInt32(string.IsNullOrEmpty(Program.mainForm.txtBThick.Text) ? "0" : Program.mainForm.txtBThick.Text);

                            ((Line)selectedElement).ResizeFilledX = ((Line)selectedElement).StartPoint.X + Convert.ToInt32(string.IsNullOrEmpty(Program.mainForm.txtBThick.Text) ? "0" : Program.mainForm.txtBThick.Text);
                            ((Line)selectedElement).ResizeFilledY = ((Line)selectedElement).StartPoint.Y + Convert.ToInt32(string.IsNullOrEmpty(Program.mainForm.txtBThick.Text) ? "0" : Program.mainForm.txtBThick.Text);

                            //if (((Line)selectedElement).StartPoint.X > 0) ((Line)selectedElement).StartPoint.X = 0;
                            //if (((Line)selectedElement).StartPoint.Y > 0) ((Line)selectedElement).StartPoint.Y = 0;

                            //if (((Line)selectedElement).EndPoint.X > 0) ((Line)selectedElement).EndPoint.X = 0;
                            //if (((Line)selectedElement).EndPoint.Y > 0) ((Line)selectedElement).EndPoint.Y = 0;
                        }
                        else
                        {
                            if (!(selectedElement is Pencil) && !(selectedElement is Elements.Brush) && !(selectedElement is Fill))
                                switch (CornerSelected)
                                {
                                    case 1: // Top-left corner
                                        selectedElement.X = mouseLoc.X;
                                        selectedElement.Y = mouseLoc.Y;

                                        int proposedWidth = ((mouseLoc.X - BeforeResizePoint.X) * -1) + BeforeResizeSize.Width;
                                        int proposedHeight = ((mouseLoc.Y - BeforeResizePoint.Y) * -1) + BeforeResizeSize.Height;
                                        if (proposedWidth > 0) selectedElement.Width = proposedWidth;
                                        if (proposedHeight > 0) selectedElement.Height = proposedHeight;

                                        Program.mainForm.movingRefresh.Start();
                                        selectedElement.Resize(selectedElement.Width, selectedElement.Height);
                                        break;
                                    case 2: // Top-right corner
                                        selectedElement.Y = mouseLoc.Y;

                                        int proposedWidth2 = ((mouseLoc.X - BeforeResizePoint.X)) + BeforeResizeSize.Width;
                                        int proposedHeight2 = ((mouseLoc.Y - BeforeResizePoint.Y) * -1) + BeforeResizeSize.Height;
                                        if (proposedWidth2 > 0) selectedElement.Width = proposedWidth2;
                                        if (proposedHeight2 > 0) selectedElement.Height = proposedHeight2;

                                        Program.mainForm.movingRefresh.Start();
                                        selectedElement.Resize(selectedElement.Width, selectedElement.Height);
                                        break;
                                    case 3: // Bottom-left corner
                                        selectedElement.X = mouseLoc.X;

                                        int proposedWidth3 = ((mouseLoc.X - BeforeResizePoint.X) * -1) + BeforeResizeSize.Width;
                                        int proposedHeight3 = ((mouseLoc.Y - BeforeResizePoint.Y)) + BeforeResizeSize.Height;
                                        if (proposedWidth3 > 0) selectedElement.Width = proposedWidth3;
                                        if (proposedHeight3 > 0) selectedElement.Height = proposedHeight3;

                                        Program.mainForm.movingRefresh.Start();
                                        selectedElement.Resize(selectedElement.Width, selectedElement.Height);
                                        break;
                                    case 4: // Bottom-right corner
                                        int proposedWidth4 = ((mouseLoc.X - BeforeResizePoint.X)) + BeforeResizeSize.Width;
                                        int proposedHeight4 = ((mouseLoc.Y - BeforeResizePoint.Y)) + BeforeResizeSize.Height;
                                        if (proposedWidth4 > 0) selectedElement.Width = proposedWidth4;
                                        if (proposedHeight4 > 0) selectedElement.Height = proposedHeight4;

                                        Program.mainForm.movingRefresh.Start();
                                        selectedElement.Resize(selectedElement.Width, selectedElement.Height);
                                        break;
                                }
                        }
                    }

                    Program.mainForm.canvaspre.Invalidate();
                }
            }
        }

        public static void HandleMouseUpSelection(Point mouseLoc)
        {
            IsMoving = false;
            Program.mainForm.movingRefresh.Stop();

            if (CornerSelected != 0)
                CornerSelected = 0;

            if (selectedElement is Line)
            {
                ((Line)selectedElement).BeforeResizeX = selectedElement.X;
                ((Line)selectedElement).BeforeResizeY = selectedElement.Y;

                ((Line)selectedElement).ResizeFilledX = ((Line)selectedElement).StartPoint.X;
                ((Line)selectedElement).ResizeFilledY = ((Line)selectedElement).StartPoint.Y;
            }
        }
        #endregion

        #region Mouse Handlers
        public async static void HandleMouseDown(MouseEventArgs e)
        {
            Point mouseLoc = new Point(e.X / MagnificationLevel, e.Y / MagnificationLevel);
            mousePoint = mouseLoc;

            if (IsInDragRegion)
            {
                CurrentlyDragging = true;

                startPoint = mouseLoc;
                dragRegionSelect.X = startPoint.X;
                dragRegionSelect.Y = startPoint.Y;
                dragRegionSelect.Width = 0;
                dragRegionSelect.Height = 0;

                Program.mainForm.canvaspre.Invalidate();
            }
            else
            {
                if (e.Button == MouseButtons.Left)
                {
                    DrawingMin.X = mouseLoc.X;
                    DrawingMin.Y = mouseLoc.Y; // This fixes a bug.
                    DrawingMax.X = 0;
                    DrawingMax.Y = 0;

                    if (selectedTool == 0)
                    { // Selection tool!
                        HandleMouseDownSelection(mouseLoc);
                    }

                    if (selectedTool == Tool.Pencil)
                    {
                        lastMousePoint = new Point(mouseLoc.X, mouseLoc.Y);

                        currentDrawingElement = new Pencil()
                        {
                            Width = savedata.imageSize.Width,
                            Height = savedata.imageSize.Height
                        };

                        grph.StartFigure();
                        ((Pencil)currentDrawingElement).pencilPoints = new Bitmap(currentDrawingElement.Width, currentDrawingElement.Height);
                        currentDrawingGraphics = Graphics.FromImage(((Pencil)currentDrawingElement).pencilPoints);

                        currentDrawingGraphics.Clear(Color.Transparent);

                        currentDrawingGraphics.FillRectangle(sb101, mouseLoc.X, mouseLoc.Y, 1, 1); // (mouseLoc.X, mouseLoc.Y, Color.FromArgb(1, 0, 1));
                    }

                    if (selectedTool == Tool.Brush)
                    {
                        lastMousePoint = new Point(mouseLoc.X, mouseLoc.Y);

                        currentDrawingElement = new Elements.Brush()
                        {
                            Width = savedata.imageSize.Width,
                            Height = savedata.imageSize.Height
                        };

                        ((Elements.Brush)currentDrawingElement).brushPoints = new Bitmap(currentDrawingElement.Width, currentDrawingElement.Height);

                        DrawingMin = new Point(mouseLoc.X, mouseLoc.Y);
                        DrawingMax = new Point(mouseLoc.X, mouseLoc.Y);

                        currentDrawingGraphics = Graphics.FromImage(((Elements.Brush)currentDrawingElement).brushPoints);

                        currentDrawingGraphics.Clear(Color.Transparent);
                    }

                    if (selectedTool == Tool.Rectangle)
                    {
                        currentDrawingElement = new RectangleE()
                        {
                            Width = savedata.imageSize.Width,
                            Height = savedata.imageSize.Height
                        };

                        DrawingMin.X = mouseLoc.X;
                        DrawingMin.Y = mouseLoc.Y;

                        startPoint = mouseLoc;
                    }

                    if (selectedTool == Tool.Ellipse)
                    {

                        currentDrawingElement = new Ellipse()
                        {
                            Width = savedata.imageSize.Width,
                            Height = savedata.imageSize.Height
                        };

                        DrawingMin.X = mouseLoc.X;
                        DrawingMin.Y = mouseLoc.Y;

                        startPoint = mouseLoc;
                    }

                    if (selectedTool == Tool.Rectangle || selectedTool == Tool.Ellipse)
                    {
                        ((dynamic)currentDrawingElement).IsFilled = true;
                        ((dynamic)currentDrawingElement).BorderColor = Program.mainForm.clrBord.BackColor;
                        ((dynamic)currentDrawingElement).FillColor = Program.mainForm.clrFill.BackColor;
                        ((dynamic)currentDrawingElement).BorderSize = Convert.ToInt32(string.IsNullOrEmpty(Program.mainForm.txtBWidth.Text) ? "0" : Program.mainForm.txtBWidth.Text);
                    }

                    if (selectedTool == Tool.Line)
                    {

                        currentDrawingElement = new Line()
                        {
                            Width = savedata.imageSize.Width,
                            Height = savedata.imageSize.Height
                        };

                        DrawingMin.X = mouseLoc.X;
                        DrawingMin.Y = mouseLoc.Y;

                        startPoint = mouseLoc;

                        ((Line)currentDrawingElement).color = Program.mainForm.clrNorm.BackColor;
                        ((Line)currentDrawingElement).Thickness = Convert.ToInt32(string.IsNullOrEmpty(Program.mainForm.txtBThick.Text) ? "0" : Program.mainForm.txtBThick.Text);
                    }

                    #region Fill + Text
                    if (selectedTool == Tool.Fill) // I would hide this function, it's quite long because it runs async which causes all sorts of problems!
                    {
                        try
                        {
                            if (!Core.InOperation)
                            {
                                Core.InOperation = true;

                                startPoint = new Point(mouseLoc.X, mouseLoc.Y);

                                currentDrawingElement = new Fill()
                                {
                                    X = mouseLoc.X,
                                    Y = mouseLoc.Y,
                                    Width = savedata.imageSize.Width,
                                    Height = savedata.imageSize.Height
                                };

                                DrawingMin.X = mouseLoc.X;
                                DrawingMin.Y = mouseLoc.Y;
                                DrawingMax.X = mouseLoc.X;
                                DrawingMax.Y = mouseLoc.Y;

                                Program.mainForm.lblProcess.Show();
                                fill = new Task<Bitmap>(() =>
                                {
                                    return ImageFilling.SafeFloodFill(ImageFormer.ImageToByteArray(Core.PaintPreview()), mouseLoc.X, mouseLoc.Y, Color.FromArgb(1, 0, 1));
                                });

                                fill.Start();

                                ((Fill)currentDrawingElement).fillPoints = await fill;
                                ((Fill)currentDrawingElement).fillColor = Program.mainForm.clrNorm.BackColor;

                                currentDrawingElement.X = DrawingMin.X - 1; currentDrawingElement.Y = DrawingMin.Y - 1;

                                currentDrawingElement.Width = (DrawingMax.X - DrawingMin.X) + 1;
                                currentDrawingElement.Height = (DrawingMax.Y - DrawingMin.Y) + 1;
                                currentDrawingElement.zindex = savedata.topZIndex++;

                                ((Fill)currentDrawingElement).fillPoints = ImageCropping.CropImage(((Fill)currentDrawingElement).fillPoints, currentDrawingElement.X, currentDrawingElement.Y, currentDrawingElement.Width, currentDrawingElement.Height);
                                Core.AddElement(currentDrawingElement);

                                if (currentDrawingGraphics != null) currentDrawingGraphics.Dispose();
                                currentDrawingElement = null;

                                endImage = Core.PaintPreview();

                                Program.mainForm.lblProcess.Hide();

                                Core.InOperation = false;
                            }
                        }
                        catch { Program.mainForm.lblProcess.Hide(); }
                    }

                    if (selectedTool == Tool.Text)
                    {
                        currentDrawingElement = new Elements.Text()
                        {
                            X = mouseLoc.X,
                            Y = mouseLoc.Y,
                        };

                        ((Text)currentDrawingElement).mainText = Program.mainForm.txtTText.Text;
                        ((Text)currentDrawingElement).clr = Program.mainForm.clrNorm.BackColor;

                        try
                        {
                            FontStyle bold = (BoldSelected) ? FontStyle.Bold : FontStyle.Regular;
                            FontStyle italic = (ItalicSelected) ? FontStyle.Italic : FontStyle.Regular;
                            FontStyle underline = (UnderlineSelected) ? FontStyle.Underline : FontStyle.Regular;

                            ((Text)currentDrawingElement).fnt = new Font(Program.mainForm.cmbFont.Text, Convert.ToInt32(string.IsNullOrEmpty(Program.mainForm.cmbSize.Text) ? "0" : Program.mainForm.cmbSize.Text), bold | italic | underline);
                        }
                        catch
                        {
                            Program.mainForm.cmbFont.Text = "Microsoft Sans Serif";
                            Program.mainForm.cmbSize.Text = "12";
                            ((Text)currentDrawingElement).fnt = new Font(Program.mainForm.cmbFont.Text, Convert.ToInt32(string.IsNullOrEmpty(Program.mainForm.cmbSize.Text) ? "0" : Program.mainForm.cmbSize.Text), FontStyle.Regular);
                        }

                        Size widthHeight = Elements.Text.MeasureText(Program.mainForm.txtTText.Text, ((Text)currentDrawingElement).fnt);
                        currentDrawingElement.Width = Convert.ToInt32(Math.Ceiling(widthHeight.Width + ((Text)currentDrawingElement).fnt.Size));
                        currentDrawingElement.Height = widthHeight.Height;
                    }
                    #endregion

                    if (selectedTool != Tool.Fill) MouseDownOnCanvas = true;
                }
            }
        }

        public static void HandleMouseMove(MouseEventArgs e)
        {
            Point mouseLoc = new Point(e.X / MagnificationLevel, e.Y / MagnificationLevel);
            mousePoint = mouseLoc;

            if (CurrentlyDragging)
            {
                dragRegionSelect.Width = mouseLoc.X - startPoint.X;
                dragRegionSelect.Height = mouseLoc.Y - startPoint.Y;

                Program.mainForm.canvaspre.Invalidate();
            }
            else
            {
                if (selectedTool == 0)
                {
                    HandleMouseMoveSelection(mouseLoc);
                }
            }

            if (MouseDownOnCanvas)
            {
                if (currentDrawingElement is Pencil)
                {
                    grph.AddLine(lastMousePoint.X, lastMousePoint.Y, mouseLoc.X, mouseLoc.Y);

                    // We now need to use the element

                    currentDrawingGraphics.DrawPath(new Pen(Color.FromArgb(1, 0, 1)), grph);

                    Program.mainForm.canvaspre.Invalidate();
                }

                if (currentDrawingElement is Elements.Brush)
                {
                    //grph.AddLine(lastMousePoint.X, lastMousePoint.Y, mouseLoc.X, mouseLoc.Y);
                    //BrushDrawing.DrawLineOfEllipse(Convert.ToInt32(string.IsNullOrEmpty(txtBThick.Text) ? "0" : txtBThick.Text), currentDrawingGraphics, sb101, lastMousePoint.X, lastMousePoint.Y, mouseLoc.X, mouseLoc.Y);

                    BrushDrawing.DrawLineOfEllipse(Convert.ToInt32(string.IsNullOrEmpty(Program.mainForm.txtBThick.Text) ? "0" : Program.mainForm.txtBThick.Text), currentDrawingGraphics, sb101, lastMousePoint.X, lastMousePoint.Y, mouseLoc.X, mouseLoc.Y);
                    //currentDrawingGraphics.DrawPath(new Pen(Color.FromArgb(1, 0, 1)), grph);
                    //currentDrawingGraphics.DrawPath(new Pen(Color.FromArgb(1, 0, 1), Convert.ToInt32(string.IsNullOrEmpty(txtBThick.Text) ? "0" : txtBThick.Text)), grph);
                    //currentDrawingGraphics.DrawLine(new Pen(Color.FromArgb(1, 0, 1), Convert.ToInt32(string.IsNullOrEmpty(txtBThick.Text) ? "0" : txtBThick.Text)), lastMousePoint, mouseLoc);
                    //currentDrawingGraphics.FillEllipse(sb101, mouseLoc.X, mouseLoc.Y, Convert.ToInt32(string.IsNullOrEmpty(txtBThick.Text) ? "0" : txtBThick.Text), Convert.ToInt32(txtBThick.Text));

                    Program.mainForm.canvaspre.Invalidate();
                }

                if (currentDrawingElement is RectangleE || currentDrawingElement is Ellipse)
                {
                    Program.mainForm.canvaspre.Invalidate();
                }

                if (currentDrawingElement is Line)
                {
                    Program.mainForm.canvaspre.Invalidate();
                }

                if (currentDrawingElement is Elements.Text)
                {
                    ((Elements.Text)currentDrawingElement).X = mouseLoc.X;
                    ((Elements.Text)currentDrawingElement).Y = mouseLoc.Y;

                    Program.mainForm.canvaspre.Invalidate();
                }

                if (currentDrawingElement is Pencil || currentDrawingElement is Elements.Brush || currentDrawingElement is Line || currentDrawingElement is RectangleE || currentDrawingElement is Ellipse)
                {
                    if (mouseLoc.X > DrawingMax.X) DrawingMax.X = mouseLoc.X;
                    if (mouseLoc.Y > DrawingMax.Y) DrawingMax.Y = mouseLoc.Y;
                    if (mouseLoc.X < DrawingMin.X) DrawingMin.X = mouseLoc.X;
                    if (mouseLoc.Y < DrawingMin.Y) DrawingMin.Y = mouseLoc.Y;
                }

                lastMousePoint = mouseLoc;
            }
        }

        public static void HandleMouseUp(MouseEventArgs e)
        {
            //Point mouseLoc = new Point((int)Math.Round((decimal)((e.X - (MagnificationLevel / 2)) / MagnificationLevel)), (int)Math.Round((decimal)((e.X - (MagnificationLevel / 2)) / MagnificationLevel)));
            Point mouseLoc = new Point(e.X / MagnificationLevel, e.Y / MagnificationLevel);

            if (IsInDragRegion)
            {
                CurrentlyDragging = false;

                int widthamount = 0, heightamount = 0;

                if (dragRegionSelect.Width < 0)
                    widthamount = Math.Abs(dragRegionSelect.Width);

                if (dragRegionSelect.Height < 0)
                    heightamount = Math.Abs(dragRegionSelect.Height);

                dragRegionSelect = new Rectangle(dragRegionSelect.X - widthamount, dragRegionSelect.Y - heightamount, Math.Abs(dragRegionSelect.Width), Math.Abs(dragRegionSelect.Height));
            }
            else
            {
                if (MouseDownOnCanvas)
                {
                    Program.mainForm.movingRefresh.Stop();
                    MouseDownOnCanvas = false;

                    if (selectedTool == 0)
                    {
                        HandleMouseUpSelection(mouseLoc);
                    }

                    if (selectedTool == Tool.Pencil) grph.Reset();

                    if (currentDrawingElement is Pencil || currentDrawingElement is Elements.Brush || currentDrawingElement is Line || currentDrawingElement is RectangleE || currentDrawingElement is Ellipse)
                    {
                        if (mouseLoc.X > DrawingMax.X) DrawingMax.X = mouseLoc.X;
                        if (mouseLoc.Y > DrawingMax.Y) DrawingMax.Y = mouseLoc.Y;
                        if (mouseLoc.X < DrawingMin.X) DrawingMin.X = mouseLoc.X;
                        if (mouseLoc.Y < DrawingMin.Y) DrawingMin.Y = mouseLoc.Y;
                    }

                    // Apply the data

                    if (currentDrawingElement is Pencil)
                    {
                        int x = (DrawingMin.X < 0) ? 0 : DrawingMin.X;
                        int y = (DrawingMin.Y < 0) ? 0 : DrawingMin.Y;

                        ((Pencil)currentDrawingElement).pencilPoints = ImageCropping.CropImage(((Pencil)currentDrawingElement).pencilPoints, x, y, DrawingMax.X, DrawingMax.Y);
                        ((Pencil)currentDrawingElement).pencilColor = Program.mainForm.clrNorm.BackColor;
                    }

                    if (currentDrawingElement is Elements.Brush)
                    {
                        int x = (DrawingMin.X < 0) ? 0 : DrawingMin.X;
                        int y = (DrawingMin.Y < 0) ? 0 : DrawingMin.Y;

                        ((Elements.Brush)currentDrawingElement).brushPoints = ImageCropping.CropImage(((Elements.Brush)currentDrawingElement).brushPoints, x, y, DrawingMax.X + Convert.ToInt32(string.IsNullOrEmpty(Program.mainForm.txtBThick.Text) ? "0" : Program.mainForm.txtBThick.Text), DrawingMax.Y + Convert.ToInt32(string.IsNullOrEmpty(Program.mainForm.txtBThick.Text) ? "0" : Program.mainForm.txtBThick.Text));
                        ((Elements.Brush)currentDrawingElement).brushColor = Program.mainForm.clrNorm.BackColor;
                    }

                    if (currentDrawingElement is RectangleE || currentDrawingElement is Ellipse)
                    {
                        currentDrawingElement.zindex = savedata.topZIndex++;

                        int borderSize = Convert.ToInt32(string.IsNullOrEmpty(Program.mainForm.txtBWidth.Text) ? "0" : Program.mainForm.txtBWidth.Text);

                        currentDrawingElement.Width = mouseLoc.X - startPoint.X + borderSize;
                        currentDrawingElement.Height = mouseLoc.Y - startPoint.Y + borderSize;

                        if (currentDrawingElement.Width < 0) currentDrawingElement.Width = currentDrawingElement.Width - borderSize;
                        if (currentDrawingElement.Height < 0) currentDrawingElement.Height = currentDrawingElement.Height - borderSize;

                        if (currentDrawingElement.Width < 0) currentDrawingElement.X = startPoint.X - Math.Abs(currentDrawingElement.Width); else currentDrawingElement.X = startPoint.X;
                        if (currentDrawingElement.Height < 0) currentDrawingElement.Y = startPoint.Y - Math.Abs(currentDrawingElement.Height); else currentDrawingElement.Y = startPoint.Y;

                        if (currentDrawingElement.Width < 0) currentDrawingElement.Width = currentDrawingElement.Width - borderSize;
                        if (currentDrawingElement.Height < 0) currentDrawingElement.Height = currentDrawingElement.Height - borderSize;

                        currentDrawingElement.Width = Math.Abs(currentDrawingElement.Width);
                        currentDrawingElement.Height = Math.Abs(currentDrawingElement.Height);

                        Core.AddElement(currentDrawingElement);
                    }

                    if (currentDrawingElement is Line)
                    {
                        currentDrawingElement.zindex = savedata.topZIndex++;

                        int thickness = Convert.ToInt32(string.IsNullOrEmpty(Program.mainForm.txtBWidth.Text) ? "0" : Program.mainForm.txtBWidth.Text);
                        currentDrawingElement.Width = (DrawingMax.X - DrawingMin.X) + (thickness * 3);
                        currentDrawingElement.Height = (DrawingMax.Y - DrawingMin.Y) + (thickness * 3);

                        // The below code is to get the X even in minus numbers!

                        //if (width < 0) currentDrawingElement.Width = 1;
                        //if (height < 0) currentDrawingElement.Height = 1;

                        currentDrawingElement.X = DrawingMin.X - (thickness * 2);
                        currentDrawingElement.Y = DrawingMin.Y - (thickness * 2);

                        ((Line)currentDrawingElement).BeforeResizeX = currentDrawingElement.X;
                        ((Line)currentDrawingElement).BeforeResizeY = currentDrawingElement.Y;

                        ((Line)currentDrawingElement).BeforeResizeWidth = currentDrawingElement.Width;
                        ((Line)currentDrawingElement).BeforeResizeHeight = currentDrawingElement.Height;

                        ((Line)currentDrawingElement).StartPoint = new Point((startPoint.X - DrawingMin.X) + (thickness * 2), (startPoint.Y - DrawingMin.Y) + (thickness * 2));
                        ((Line)currentDrawingElement).EndPoint = new Point((mouseLoc.X - DrawingMin.X) + (thickness * 2), (mouseLoc.Y - DrawingMin.Y) + (thickness * 2));

                        ((Line)currentDrawingElement).BeforeResizeStart = ((Line)currentDrawingElement).StartPoint;
                        ((Line)currentDrawingElement).BeforeResizeEnd = ((Line)currentDrawingElement).EndPoint;

                        ((Line)currentDrawingElement).color = Program.mainForm.clrNorm.BackColor;

                        Core.AddElement(currentDrawingElement);

                        Program.mainForm.canvaspre.Invalidate();
                    }

                    if (currentDrawingElement is Elements.Text)
                    {
                        currentDrawingElement.zindex = savedata.topZIndex++;

                        currentDrawingElement.X = e.X;
                        currentDrawingElement.Y = e.Y;

                        Core.AddElement(currentDrawingElement);
                    }

                    if (currentDrawingElement is Pencil || currentDrawingElement is Elements.Brush)
                    {
                        // Reset everything back

                        currentDrawingElement.zindex = savedata.topZIndex++;
                        currentDrawingElement.X = (DrawingMin.X < 0) ? 0 : DrawingMin.X;
                        currentDrawingElement.Y = (DrawingMin.Y < 0) ? 0 : DrawingMin.Y;
                        currentDrawingElement.Width = (DrawingMax.X - DrawingMin.X) + Convert.ToInt32(string.IsNullOrEmpty(Program.mainForm.txtBThick.Text) ? "0" : Program.mainForm.txtBThick.Text);
                        currentDrawingElement.Height = (DrawingMax.Y - DrawingMin.Y) + Convert.ToInt32(string.IsNullOrEmpty(Program.mainForm.txtBThick.Text) ? "0" : Program.mainForm.txtBThick.Text);

                        if (currentDrawingElement.Width < 0) currentDrawingElement.Width = 1;
                        if (currentDrawingElement.Height < 0) currentDrawingElement.Height = 1;

                        Core.AddElement(currentDrawingElement);
                    }

                    if (currentDrawingGraphics != null) currentDrawingGraphics.Dispose();
                    currentDrawingElement = null;
                }

                if (selectedElement != null)
                {
                    IsMovingOld = new Point(selectedElement.X, selectedElement.Y);
                    Program.mainForm.canvaspre.Invalidate();
                }
            }

            Core.PaintPreviewAsync();

            GC.Collect();
        }
        #endregion      
    }
}
