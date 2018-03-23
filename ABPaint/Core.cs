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
        public static PowerTool currentTool;
        internal static bool queueLock = false; // A lock for the queue - stops the queue from being looped through twice at the same time

        // All of the main variables go below, I recommend you hide them.
        #region Main Variables
        // ==================
        // The Final Renderered Image
        // ==================
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
                ActionQueue.Enqueue(new PerformAction(PerformableAction.UpdatePreview));
                ProcessQueue();
            }
        }

        // ==================
        // Message Queue (Things For The Program To Do)
        // ==================
        private static Queue<PerformAction> ActionQueue = new Queue<PerformAction>();
        // ==================
        // General Variables
        // ==================
        public static Tool selectedTool = Tool.Selection;

        public static int selectedPalette = 1;
        public static Element currentDrawingElement;
        public static Graphics currentDrawingGraphics;
        public static bool MouseDownOnCanvas = false;
        public static Task<Bitmap> tskPP = null;

        public static Point mousePoint = new Point(0, 0);

        // ==================
        // Drag Region
        // ==================
        public static Rectangle dragRegionSelect;
        public static bool IsInDragRegion;
        public static bool CurrentlyDragging;

        // ==================
        // SolidBrush
        // ==================

        public static SolidBrush sb101 = new SolidBrush(Color.FromArgb(1, 0, 1));

        // ==================
        // Magnification level
        // ==================
        public static int MagnificationLevel = 1;

        // ==================
        // Pencil & Brush Variables
        // ==================
        public static System.Drawing.Drawing2D.GraphicsPath grph = new System.Drawing.Drawing2D.GraphicsPath();
        public static int LastX;
        public static int LastY;

        // ==================
        // Drawing (Not Painting) Variables
        // ==================
        public static Point DrawingMin;
        public static Point DrawingMax;
        public static Point lastMousePoint;
        public static Point startPoint;

        // ==================
        // Task For Fill (For Async)
        // ==================
        static Task<Bitmap> fill;

        // ==================
        // Selection Tool Variables
        // ==================
        public static Element selectedElement;
        public static Size IsMovingGap;
        public static Point IsMovingOld;
        public static bool IsMoving = false;
        public static bool IsOnSelection = false;

        // ==================
        // Resizing Variables
        // ==================
        public static Corner CornerSelected = 0;
        public static Point BeforeResizePoint;
        public static Size BeforeResizeSize;
        public static bool Resized = false;
        public static bool LimitMouse;

        // ==================
        // Text Variables
        // ==================
        public static bool BoldSelected = false;
        public static bool ItalicSelected = false;
        public static bool UnderlineSelected = false;
        #endregion

        public async static void ProcessQueue()
        {   
            Task tsk = null;
            PerformAction paction;
            if (!queueLock)
            {
                queueLock = true;
                ActionQueue.Enqueue(new PerformAction(PerformableAction.Paint));
                while (ActionQueue.Count > 0)
                {
                    paction = ActionQueue.Dequeue();
                    switch (paction.action)
                    {
                        case PerformableAction.Paint:
                            tsk = new Task<Bitmap>(PaintPreview);
                            break;
                        case PerformableAction.UpdatePreview:
                            tsk = new Task(() =>
                            {
                                if (Program.mainForm.canvaspre.Image != null)
                                {
                                    if (Monitor.TryEnter(Program.mainForm.canvaspre.Image))
                                        Program.mainForm.canvaspre.Image = endimg;
                                }
                                else Program.mainForm.canvaspre.Image = endimg;
                            });
                            break;
                        case PerformableAction.SelectionToolSelect:
                            tsk = new Task(() => { selectElementByLocation((int)paction.param1, (int)paction.param2); });
                            break;
                        case PerformableAction.Delete:
                            tsk = new Task(PerformDelete);
                            break;
                        case PerformableAction.AddElement:
                            tsk = new Task(() => { PerformAddElement(paction.param1 as Element); });
                            break;
                        case PerformableAction.CancelTool:
                            tsk = new Task(PerformCancelTool);
                            break;
                        case PerformableAction.ApplyTool:
                            tsk = new Task(PerformApply);
                            break;
                        case PerformableAction.UseTool:
                            tsk = new Task(() => { PerformUseTool(paction.param1 as PowerTool); });
                            break;                    
                        case PerformableAction.Cut:
                            tsk = new Task(PerformCut);
                            break;
                        case PerformableAction.Copy:
                            tsk = new Task(PerformCopy);
                            break;
                        case PerformableAction.Paste:
                            tsk = new Task(PerformPaste);
                            break;
                        case PerformableAction.Fill:
                            tsk = new Task(() => { PerformFill((Point)paction.param1); });
                            break;
                        case PerformableAction.Text:
                            tsk = new Task(() => { PerformText((Point)paction.param1); });
                            break;
                    }
                    if (tsk != null)
                    {
                        tsk.Start();
                        await tsk;
                        Program.mainForm.canvaspre.Invalidate();
                    }
                }
                tsk.Dispose();
                queueLock = false;              
            }    
        }

        public static void HandlePaint()
        {
            ActionQueue.Enqueue(new PerformAction(PerformableAction.Paint));
            ProcessQueue();
        }

        /// <summary>
        /// Draws the preview. (Probably the most crucial method in the whole application!)
        /// </summary>
        /// <returns>An image for the result.</returns>
        public static Bitmap PaintPreview()
        {
            Bitmap endResult = new Bitmap(savedata.imageSize.Width, savedata.imageSize.Height);

            //try
            //{
            // Draw the elements in order


            Graphics g = Graphics.FromImage(endResult);

            g.FillRectangle(Brushes.White, 0, 0, savedata.imageSize.Width, savedata.imageSize.Height);
            // Order them by zindex:
            savedata.imageElements = savedata.imageElements.OrderBy(o => o.zindex).ToList();

            // Now draw them all!

            for (int i = 0; i < savedata.imageElements.Count; i++)
            {
                if (savedata.imageElements[i].Visible)
                    savedata.imageElements[i].ProcessImage(g);
            }

            endImage = endResult;

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

                    ele.zindex = savedata.topZIndex++; // Brings to front

                    ret = ele;

                    break;
                }
            }

            // Order the list based on zindex!
            savedata.imageElements = savedata.imageElements.OrderBy(o => o.zindex).ToList();

            return ret;
        }

        /// <summary>
        /// Deselects all elements.
        /// </summary>
        public static void DeselectElements()
        {
            selectedElement = null;
        }

        /// <summary>
        /// Handles a key press.
        /// </summary>
        /// <param name="key">The key pressed.</param>
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

            HandlePaint();
            Program.mainForm.canvaspre.Invalidate();
        }

        /// <summary>
        /// Starts using a PowerTool.
        /// </summary>
        /// <param name="tool">The PowerTool to use.</param>
        public static void UseTool(PowerTool tool)
        {
            ActionQueue.Enqueue(new PerformAction(PerformableAction.CancelTool, tool));
            ProcessQueue();
        }

        public static void PerformUseTool(PowerTool tool)
        {
            if (tool.UseRegionDrag)
                IsInDragRegion = true;

            currentTool = tool;

            tool.Prepare();
        }

        /// <summary>
        /// Cancels the current power tool.
        /// </summary>
        public static void CancelTool()
        {
            ActionQueue.Enqueue(new PerformAction(PerformableAction.CancelTool));
            ProcessQueue();
        }

        public static void PerformCancelTool()
        {
            if (currentTool.UseRegionDrag)
                IsInDragRegion = false;

            currentTool.Cancel();
            currentTool = null;

            Program.mainForm.canvaspre.Invalidate();
        }

        public static void HandleApply()
        {
            ActionQueue.Enqueue(new PerformAction(PerformableAction.ApplyTool));
            ProcessQueue();
        }

        public static void PerformApply()
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
            Program.mainForm.canvaspre.Invalidate();
        }

        public static void HandleDelete()
        {
            ActionQueue.Enqueue(new PerformAction(PerformableAction.Delete));
            ProcessQueue();
        }

        public static void PerformDelete()
        {
            if (selectedElement != null)
                if (selectedTool == Tool.Selection)
                {
                    savedata.imageElements.Remove(selectedElement);

                    DeselectElements();

                    Program.mainForm.canvaspre.Invalidate();
                }
        }

        public static void HandleCut()
        {
            ActionQueue.Enqueue(new PerformAction(PerformableAction.Cut));
            ProcessQueue();
        }

        public static void PerformCut()
        {
            PerformCopy();

            savedata.imageElements.Remove(selectedElement);
            selectedElement = null;

            Program.mainForm.canvaspre.Invalidate();
        }

        public static void HandleCopy()
        {
            ActionQueue.Enqueue(new PerformAction(PerformableAction.Copy));
            ProcessQueue();
        }

        public static void PerformCopy()
        {
            Clipboard.SetDataObject("ABPELE" + ABJson.GDISupport.JsonSerializer.Serialize("", selectedElement, ABJson.GDISupport.JsonFormatting.Compact, 0, true).TrimEnd(','), true);
        }

        public static void HandlePaste()
        {
            ActionQueue.Enqueue(new PerformAction(PerformableAction.Paste));
            ProcessQueue();
        }

        public static void PerformPaste()
        {
            IDataObject data = Clipboard.GetDataObject();

            if (data.GetDataPresent(DataFormats.Text) && data.GetData(DataFormats.Text).ToString().StartsWith("ABPELE"))
            {
                Element ele = ABJson.GDISupport.JsonClassConverter.ConvertJsonToObject<Element>(data.GetData(DataFormats.Text).ToString().Remove(0, 6), true);

                ele.zindex = savedata.topZIndex++;
                AddElement(ele);

                selectedElement = ele;
            }

            Program.mainForm.canvaspre.Invalidate();
        }

        public static void HandleZoomIn()
        {
            if (MagnificationLevel < 16)
            {
                Program.mainForm.OldMagnificationLevel = MagnificationLevel;
                MagnificationLevel = MagnificationLevel * 2;
                
                Program.mainForm.label11.Text = "X" + MagnificationLevel;            
                Program.mainForm.ReloadImage();
            }
        }

        public static void HandleZoomOut()
        {
            if (MagnificationLevel > 1)
            {
                Program.mainForm.OldMagnificationLevel = MagnificationLevel;
                MagnificationLevel = MagnificationLevel / 2;

                Program.mainForm.label11.Text = "X" + MagnificationLevel;
                Program.mainForm.ReloadImage();
            }
            PaintPreview();
        }

        public static void AddElement(Element element)
        {
            ActionQueue.Enqueue(new PerformAction(PerformableAction.AddElement, element));
            ProcessQueue();
        }

        public static void PerformAddElement(Element element)
        {
            savedata.imageElements.Add(element);

            // TODO: Add undo option!
        }

        public static void HandleFill(Point mouseLoc)
        {
            ActionQueue.Enqueue(new PerformAction(PerformableAction.Fill, mouseLoc));
            ProcessQueue();
        }

        public static void PerformFill(Point mouseLoc)
        {
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

            ((Fill)currentDrawingElement).fillPoints = ImageFilling.SafeFloodFill(ImageFormer.ImageToByteArray(Core.PaintPreview()), mouseLoc.X, mouseLoc.Y, Color.FromArgb(1, 0, 1));
            ((Fill)currentDrawingElement).fillColor = Program.mainForm.clrNorm.BackColor;

            currentDrawingElement.X = DrawingMin.X - 1; currentDrawingElement.Y = DrawingMin.Y - 1;

            currentDrawingElement.Width = (DrawingMax.X - DrawingMin.X) + 1;
            currentDrawingElement.Height = (DrawingMax.Y - DrawingMin.Y) + 1;
            currentDrawingElement.zindex = savedata.topZIndex++;

            ((Fill)currentDrawingElement).fillPoints = ImageCropping.CropImage(((Fill)currentDrawingElement).fillPoints, currentDrawingElement.X, currentDrawingElement.Y, currentDrawingElement.Width, currentDrawingElement.Height);
            AddElement(currentDrawingElement);

            if (currentDrawingGraphics != null) currentDrawingGraphics.Dispose();
            currentDrawingElement = null;

            endImage = PaintPreview();
        }

        public static void HandleText(Point mouseLoc)
        {
            ActionQueue.Enqueue(new PerformAction(PerformableAction.Text, mouseLoc));
            ProcessQueue();
        }

        public static void PerformText(Point mouseLoc)
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

                ((Text)currentDrawingElement).fnt = new Font(Program.mainForm.cmbFont.Text, (Program.mainForm.cmbSize.Text.Length > 0) ? int.Parse(Program.mainForm.cmbSize.Text) : 0, bold | italic | underline);
            }
            catch
            {
                Program.mainForm.cmbFont.Text = "Microsoft Sans Serif";
                Program.mainForm.cmbSize.Text = "12";
                ((Text)currentDrawingElement).fnt = new Font(Program.mainForm.cmbFont.Text, (Program.mainForm.cmbSize.Text.Length > 0) ? int.Parse(Program.mainForm.cmbSize.Text) : 0, FontStyle.Regular);
            }

            Size widthHeight = Elements.Text.MeasureText(Program.mainForm.txtTText.Text, ((Text)currentDrawingElement).fnt);
            currentDrawingElement.Width = Convert.ToInt32(Math.Ceiling(widthHeight.Width + ((Text)currentDrawingElement).fnt.Size));
            currentDrawingElement.Height = widthHeight.Height;
        }

        // It is recommended that you hide the lower two regions
        #region Mouse Selection Handlers
        public static void HandleMouseDownSelection(Point mouseLoc)
        {
            selectedElement = selectElementByLocation(mouseLoc.X, mouseLoc.Y);

            if (selectedElement == null) Program.mainForm.ShowProperties("Selection Tool - Nothing selected!", false, false, false, false, false, false, Program.mainForm.GetCurrentColor());
            if (selectedElement is Pencil) Program.mainForm.ShowProperties("Selection Tool - Pencil", false, false, true, false, false, false, ((Pencil)selectedElement).pencilColor);
            if (selectedElement is Elements.Brush) Program.mainForm.ShowProperties("Selection Tool - Brush", false, false, true, false, false, false, ((Elements.Brush)selectedElement).brushColor);
            if (selectedElement is RectangleE) Program.mainForm.ShowProperties("Selection Tool - Rectangle", true, true, false, true, false, false, ((RectangleE)selectedElement).FillColor);
            if (selectedElement is Ellipse) Program.mainForm.ShowProperties("Selection Tool - Ellipse", true, true, false, true, false, false, ((Ellipse)selectedElement).FillColor);
            if (selectedElement is Line) Program.mainForm.ShowProperties("Selection Tool - Line", false, false, true, false, true, false, ((Line)selectedElement).color);
            if (selectedElement is Fill) Program.mainForm.ShowProperties("Selection Tool - Fill", false, false, true, false, false, false, ((Fill)selectedElement).fillColor);
            if (selectedElement is Text) Program.mainForm.ShowProperties("Selection Tool - Text", false, false, true, false, false, true, ((Text)selectedElement).clr, ((Text)selectedElement).mainText, ((Text)selectedElement).fnt);
            // Add more

            Program.mainForm.canvaspre.Invalidate();

            if (selectedElement != null)
            {
                if (new Rectangle(selectedElement.X - (10 * MagnificationLevel), selectedElement.Y - (10 * MagnificationLevel), selectedElement.Width + (20 * MagnificationLevel), selectedElement.Height + (20 * MagnificationLevel)).Contains(mouseLoc))
                {
                    // The mouse is in this element!

                    // I know it's bad to hard code certain elements but the thing is that line is completely different to ANY other element I could think of in resizing so I have to do this for it...

                    if (selectedElement is Line)
                    {
                        // Check if the mouse is on the two points otherwise move the element.

                        CornerSelected = Corner.None;

                        // First point
                        if (new Rectangle(((Line)selectedElement).StartPoint.X + selectedElement.X - 10, ((Line)selectedElement).StartPoint.Y + selectedElement.Y - 10, 20, 20).Contains(mouseLoc)) CornerSelected = Corner.TopLeft;

                        // Second point
                        if (new Rectangle(((Line)selectedElement).EndPoint.X + selectedElement.X - 10, ((Line)selectedElement).EndPoint.Y + selectedElement.Y - 10, 20, 20).Contains(mouseLoc)) CornerSelected = Corner.TopRight;
                    }
                    else
                    {
                        if (!(selectedElement is Pencil) && !(selectedElement is Elements.Brush) && !(selectedElement is Fill))
                        {
                            // Check if the mouse is on the scaling points otherwise move the element.

                            CornerSelected = Corner.None;

                            // Top left corner
                            if (new Rectangle(selectedElement.X - 10, selectedElement.Y - 10, 20, 20).Contains(mouseLoc)) CornerSelected = Corner.TopLeft;

                            // Top Right Corner
                            if (new Rectangle(selectedElement.Right - 10, selectedElement.Y - 10, 20, 20).Contains(mouseLoc)) CornerSelected = Corner.TopRight;

                            // Bottom Left corner
                            if (new Rectangle(selectedElement.X - 10, selectedElement.Bottom - 10, 20, 20).Contains(mouseLoc)) CornerSelected = Corner.BottomLeft;

                            // Bottom Right corner
                            if (new Rectangle(selectedElement.Right - 10, selectedElement.Bottom - 10, 20, 20).Contains(mouseLoc)) CornerSelected = Corner.BottomRight;
                        }
                    }

                    if (CornerSelected == Corner.None)
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
                            if (CornerSelected == Corner.TopLeft) BeforeResizePoint = new Point(((Line)selectedElement).StartPoint.X, ((Line)selectedElement).StartPoint.Y);
                            else if (CornerSelected == Corner.TopRight) BeforeResizePoint = new Point(((Line)selectedElement).EndPoint.X, ((Line)selectedElement).EndPoint.Y);
                        }
                        else
                        {
                            if (CornerSelected == Corner.TopLeft) BeforeResizePoint = new Point(selectedElement.X, selectedElement.Y);
                            else if (CornerSelected == Corner.TopRight) BeforeResizePoint = new Point(selectedElement.X + selectedElement.Width, selectedElement.Y);
                            else if (CornerSelected == Corner.BottomLeft) BeforeResizePoint = new Point(selectedElement.X, selectedElement.Y + selectedElement.Height);
                            else if (CornerSelected == Corner.BottomRight) BeforeResizePoint = new Point(selectedElement.X + selectedElement.Width, selectedElement.Y + selectedElement.Height);
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

                            Line lineEle = ((Line)selectedElement);

                            if (CornerSelected == Corner.TopLeft)
                                lineEle.StartPoint = new Point(mouseLoc.X - selectedElement.X, mouseLoc.Y - selectedElement.Y);                        
                            else if (CornerSelected == Corner.TopRight)
                                lineEle.EndPoint = new Point(mouseLoc.X - selectedElement.X, mouseLoc.Y - selectedElement.Y);

                            Program.mainForm.movingRefresh.Start();

                            LineResizing.Resize(ref lineEle);
                        }
                        else
                        {
                            if (!(selectedElement is Pencil) && !(selectedElement is Elements.Brush) && !(selectedElement is Fill)) {
                                int proposedX = selectedElement.X;
                                int proposedY = selectedElement.Y;
                                int proposedWidth = selectedElement.Width;
                                int proposedHeight = selectedElement.Height;                               

                                switch (CornerSelected)
                                {
                                    case Corner.TopLeft:   
                                        proposedWidth = ((mouseLoc.X - BeforeResizePoint.X) * -1) + BeforeResizeSize.Width;
                                        proposedHeight = ((mouseLoc.Y - BeforeResizePoint.Y) * -1) + BeforeResizeSize.Height;

                                        if (proposedWidth < 0) proposedX = mouseLoc.X + proposedWidth; else proposedX = mouseLoc.X;
                                        if (proposedHeight < 0) proposedY = mouseLoc.Y + proposedHeight; else proposedY = mouseLoc.Y;

                                        break;
                                    case Corner.TopRight: // Top-right corner
                                        proposedWidth = (mouseLoc.X - BeforeResizePoint.X) + BeforeResizeSize.Width;
                                        proposedHeight = ((mouseLoc.Y - BeforeResizePoint.Y) * -1) + BeforeResizeSize.Height;

                                        if (proposedWidth < 0) proposedX = mouseLoc.X;
                                        if (proposedHeight < 0) proposedY = mouseLoc.Y + proposedHeight; else proposedY = mouseLoc.Y;

                                        break;
                                    case Corner.BottomLeft: // Bottom-left corner
                                        proposedWidth = ((mouseLoc.X - BeforeResizePoint.X) * -1) + BeforeResizeSize.Width;
                                        proposedHeight = (mouseLoc.Y - BeforeResizePoint.Y) + BeforeResizeSize.Height;

                                        if (proposedWidth < 0) proposedX = mouseLoc.X + proposedWidth; else proposedX = mouseLoc.X;
                                        if (proposedHeight < 0) proposedY = mouseLoc.Y;

                                        break;
                                    case Corner.BottomRight: // Bottom-right corner
                                        proposedWidth = (mouseLoc.X - BeforeResizePoint.X) + BeforeResizeSize.Width;
                                        proposedHeight = (mouseLoc.Y - BeforeResizePoint.Y) + BeforeResizeSize.Height;

                                        if (proposedWidth < 0) proposedX = mouseLoc.X;
                                        if (proposedHeight < 0) proposedY = mouseLoc.Y;

                                        break;
                                }

                                selectedElement.X = proposedX;
                                selectedElement.Y = proposedY;

                                selectedElement.Width = Math.Abs(proposedWidth);
                                selectedElement.Height = Math.Abs(proposedHeight);

                                Program.mainForm.movingRefresh.Start();
                                selectedElement.Resize();
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

            if (CornerSelected != Corner.None)
            {
                selectedElement.FinishResize();
                CornerSelected = Corner.None;
            }
        }
        #endregion

        #region Mouse Handlers
        public static void HandleMouseDown(MouseEventArgs e)
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

                    if (selectedTool == Tool.Selection)
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
                        ((dynamic)currentDrawingElement).BorderSize = (Program.mainForm.txtBWidth.Text.Length > 0) ? int.Parse(Program.mainForm.txtBWidth.Text) : 0;
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
                        ((Line)currentDrawingElement).Thickness = (Program.mainForm.txtBThick.Text.Length > 0) ? int.Parse(Program.mainForm.txtBThick.Text) : 0;
                    }


                    #region Fill + Text
                    if (selectedTool == Tool.Fill)
                        HandleFill(mouseLoc);

                    if (selectedTool == Tool.Text)
                    {
                        
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
                if (selectedTool == Tool.Selection)
                    HandleMouseMoveSelection(mouseLoc);
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

                    BrushDrawing.DrawLineOfEllipse((Program.mainForm.txtBThick.Text.Length > 0) ? int.Parse(Program.mainForm.txtBThick.Text) : 0, currentDrawingGraphics, sb101, lastMousePoint.X, lastMousePoint.Y, mouseLoc.X, mouseLoc.Y);
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
                    int x = (DrawingMin.X < 0) ? 0 : DrawingMin.X; // X for Pencil/Brush
                    int y = (DrawingMin.Y < 0) ? 0 : DrawingMin.Y; // Y for Pencil/Brush

                    Program.mainForm.movingRefresh.Stop();
                    MouseDownOnCanvas = false;

                    if (selectedTool == Tool.Selection)
                        HandleMouseUpSelection(mouseLoc);

                    if (currentDrawingElement != null)
                    {
                        if (selectedTool == Tool.Pencil) grph.Reset();

                        var asPencil = currentDrawingElement as Pencil;
                        var asBrush = currentDrawingElement as Elements.Brush;
                        var asLine = currentDrawingElement as Line;
                        var asRectangle = currentDrawingElement as RectangleE;
                        var asEllipse = currentDrawingElement as Ellipse;
                        var asText = currentDrawingElement as Text;

                        if (asPencil != null || asBrush != null || asLine != null || asRectangle != null || asEllipse != null)
                        {
                            if (mouseLoc.X > DrawingMax.X) DrawingMax.X = mouseLoc.X;
                            if (mouseLoc.Y > DrawingMax.Y) DrawingMax.Y = mouseLoc.Y;
                            if (mouseLoc.X < DrawingMin.X) DrawingMin.X = mouseLoc.X;
                            if (mouseLoc.Y < DrawingMin.Y) DrawingMin.Y = mouseLoc.Y;                           
                        }

                        // Apply the data

                        // Pencil
                        if (asPencil != null)
                        {
                            ((Pencil)currentDrawingElement).pencilPoints = ImageCropping.CropImage(((Pencil)currentDrawingElement).pencilPoints, x, y, DrawingMax.X, DrawingMax.Y);
                            ((Pencil)currentDrawingElement).pencilColor = Program.mainForm.clrNorm.BackColor;

                            currentDrawingElement.X = x;
                            currentDrawingElement.Y = y;

                            currentDrawingElement.Width = (DrawingMax.X - DrawingMin.X);
                            currentDrawingElement.Height = (DrawingMax.Y - DrawingMin.Y);
                        }

                        // Brush
                        if (asBrush != null)
                        {
                            currentDrawingElement.X = x;
                            currentDrawingElement.Y = y;

                            currentDrawingElement.Width = (DrawingMax.X - DrawingMin.X) + ((Program.mainForm.txtBThick.Text.Length > 0) ? int.Parse(Program.mainForm.txtBThick.Text) : 0);
                            currentDrawingElement.Height = (DrawingMax.Y - DrawingMin.Y) + ((Program.mainForm.txtBThick.Text.Length > 0) ? int.Parse(Program.mainForm.txtBThick.Text) : 0);

                            ((Elements.Brush)currentDrawingElement).brushPoints = ImageCropping.CropImage(((Elements.Brush)currentDrawingElement).brushPoints, x, y, DrawingMax.X + ((Program.mainForm.txtBThick.Text.Length > 0) ? int.Parse(Program.mainForm.txtBThick.Text) : 0), DrawingMax.Y + ((Program.mainForm.txtBThick.Text.Length > 0) ? int.Parse(Program.mainForm.txtBThick.Text) : 0));
                            ((Elements.Brush)currentDrawingElement).brushColor = Program.mainForm.clrNorm.BackColor;
                        }

                        // Rectangle + Ellipse
                        if (asRectangle != null || asEllipse != null)
                        {
                            currentDrawingElement.zindex = savedata.topZIndex++;

                            int borderSize = (Program.mainForm.txtBWidth.Text.Length > 0) ? int.Parse(Program.mainForm.txtBWidth.Text) : 0;

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
                        }

                        // Line
                        if (asLine != null)
                        {
                            currentDrawingElement.zindex = savedata.topZIndex++;

                            int thickness = (Program.mainForm.txtBWidth.Text.Length > 0) ? int.Parse(Program.mainForm.txtBWidth.Text) : 0;

                            // The below code is to get the X even in minus numbers!

                            //if (width < 0) currentDrawingElement.Width = 1;
                            //if (height < 0) currentDrawingElement.Height = 1;


                            currentDrawingElement.X = DrawingMin.X;
                            currentDrawingElement.Y = DrawingMin.Y;

                            ((Line)currentDrawingElement).StartPoint = new Point((startPoint.X - DrawingMin.X), (startPoint.Y - DrawingMin.Y));
                            ((Line)currentDrawingElement).EndPoint = new Point((mouseLoc.X - DrawingMin.X), (mouseLoc.Y - DrawingMin.Y));

                            ((Line)currentDrawingElement).color = Program.mainForm.clrNorm.BackColor;

                            Line lineEle = ((Line)currentDrawingElement);
                            LineResizing.Resize(ref lineEle);

                            Program.mainForm.canvaspre.Invalidate();
                        }

                        // Text
                        if (asText != null)
                        {
                            currentDrawingElement.X = e.X;
                            currentDrawingElement.Y = e.Y;
                        }

                        // Change some things and add the element!

                        if (currentDrawingElement.Width < 0) currentDrawingElement.Width = 1;
                        if (currentDrawingElement.Height < 0) currentDrawingElement.Height = 1;

                        currentDrawingElement.zindex = savedata.topZIndex++;
                        AddElement(currentDrawingElement);

                        // Dispose some stuff.

                        if (currentDrawingGraphics != null) currentDrawingGraphics.Dispose();
                        currentDrawingElement = null;            
                    }
                }

                if (selectedElement != null)
                {
                    IsMovingOld = new Point(selectedElement.X, selectedElement.Y);
                    Program.mainForm.canvaspre.Invalidate();
                }
            }

            HandlePaint();

            GC.Collect();
        }
        #endregion      
    }
}
