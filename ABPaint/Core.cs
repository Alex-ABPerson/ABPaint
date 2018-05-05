// ***********************************************************************
// Assembly         : ABPaint
// Author           : Alex
// Created          : 12-16-2017
//
// Last Modified By : Alex
// Last Modified On : 03-29-2018
// ***********************************************************************
// <copyright file="Core.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
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
using System.Globalization;

namespace ABPaint
{
    /// <summary>
    /// The entire core of ABPaint
    /// </summary>
    public static class Core
    {
        #region General Form Variables
        public static int OldMagnificationLevel;
        public static Graphics CanvaspreG; // The graphics of the canvas.
        public static Graphics BackBufferG; // The graphics of the back-buffer
        #endregion

        public static PowerTool CurrentTool;
        public static bool MouseDownLock; // A lock for the MouseDown - stops you from triggering MouseDown twice while it's in progress.
        public static bool MouseMoveLock; // A lock for the MouseMove - stops you from triggering MouseMove twice while it's in progress.
        public static bool MouseUpLock; // A lock for the MouseUp - stops you from triggering MouseUp twice while it's in progress.
        private static bool queueLock; // A lock for the queue - stops the queue from being looped through twice at the same time
        private static bool editLock; // A lock for anything that modifies the ImageElements. Stops it from being modified while looping etc.
        
        // All of the main variables go below, I recommend you hide them.
        #region Main Variables
        private static Bitmap endimg; // Just because you have to...

        /// <summary>
        /// The final rendered image.
        /// </summary>
        public static Bitmap EndImage
        {
            get
            {
                return endimg;
            }
            set
            {
                endimg = value;
                ActionQueue.Enqueue(new PerformAction(PerformableAction.UpdatePreview));
                ProcessQueueAsync();
            }
        }


        // ========================================================================
        // MESSAGE QUEUE (Things For The Program To Do)
        // ========================================================================

        
        /// <summary>
        /// The Queue of Actions that the paint program has to do.
        /// </summary>
        public static Queue<PerformAction> ActionQueue = new Queue<PerformAction>();


        // ========================================================================
        // GENERAL VARIABLES
        // ========================================================================

        
        /// <summary>
        /// The current SelectedTool.
        /// </summary>
        public static Tool SelectedTool = Tool.Selection;

        /// <summary>
        /// The Selected(Color)Palette
        /// </summary>
        public static int SelectedPalette = 1;

        /// <summary>
        /// The element that is currently being drawn.
        /// </summary>
        public static Element CurrentDrawingElement;

        /// <summary>
        /// The graphics for the element that is currently being drawn.
        /// </summary>
        public static Graphics CurrentDrawingGraphics;

        /// <summary>
        /// Whether the mouse is held down or not.
        /// </summary>
        public static bool MouseDownOnCanvas;

        /// <summary>
        /// The MousePoint on the canvas.
        /// </summary>
        public static Point MousePoint = new Point(0, 0);


        // ========================================================================
        // DRAG REGION
        // ========================================================================

        
        /// <summary>
        /// The location/size of the "DragRegion"
        /// </summary>
        public static Rectangle DragRegionSelect;

        /// <summary>
        /// Whether the mouse is in the drag region.
        /// </summary>
        public static bool IsInDragRegion;

        /// <summary>
        /// Whether we are drawing a drag region.
        /// </summary>
        public static bool CurrentlyDragging;


        // ========================================================================
        // SOLIDBRUSH
        // ========================================================================

        
        public static SolidBrush Sb101 = new SolidBrush(Color.FromArgb(1, 0, 1));


        // ========================================================================
        // MAGNIFICATION LEVEL
        // ========================================================================


        /// <summary>
        /// The current MagnificationLevel for zooming.
        /// </summary>
        public static int MagnificationLevel = 1;


        // ========================================================================
        // PENCIL & BRUSH VARIABLES
        // ========================================================================

        
        /// <summary>
        /// The GraphicsPath used for drawing.
        /// </summary>
        public static System.Drawing.Drawing2D.GraphicsPath Grph = new System.Drawing.Drawing2D.GraphicsPath();


        // ========================================================================
        // DRAWING (Not Painting) VARIABLES
        // ========================================================================

        
        /// <summary>
        /// The X and Y of the top-left corner of where the <see cref="CurrentDrawingElement"/> was drawn.
        /// </summary>
        public static Point DrawingMin;

        /// <summary>
        /// The X and Y of the bottom-right corner of where the <see cref="CurrentDrawingElement"/> was drawn.
        /// </summary>
        public static Point DrawingMax;

        /// <summary>
        /// The last position the mouse was in (before a MouseMove)
        /// </summary>
        public static Point LastMousePoint;

        /// <summary>
        /// Where to "start" an element from - used for the line tool mainly.
        /// </summary>
        public static Point StartPoint;

        /// <summary>
        /// Whether the program is in the process of filling.
        /// </summary>
        public static bool isFilling;


        // ==================
        // SELECTION TOOL VARIABLES
        // ==================

        
        /// <summary>
        /// The current SelectedElement.
        /// </summary>
        public static Element SelectedElement;

        /// <summary>
        /// The Offset for when moving.
        /// </summary>
        public static Size IsMovingGap;

        /// <summary>
        /// Whether the program is in the middle of moving an element.
        /// </summary>
        public static bool IsMoving;

        /// <summary>
        /// The point of the element before moving - used to check if the element has actually moved.
        /// </summary>
        public static Point IsMovingOld;

        /// <summary>
        /// Whether the mouse is within the SelectedElement.
        /// </summary>
        public static bool IsOnSelection;

        /// <summary>
        /// The Image before you started moving the element.
        /// </summary>
        public static Bitmap BeforeMove;


        // ========================================================================
        // RESIZING VARIABLES
        // ========================================================================
        
        
        /// <summary>
        /// The current corner that you have selected
        /// </summary>
        public static Corner CornerSelected;

        /// <summary>
        /// The point that the mouse was at before resizing
        /// </summary>
        public static Point BeforeResizePoint;

        /// <summary>
        /// The size of the element before resizing.
        /// </summary>
        public static Size BeforeResizeSize;

        /// <summary>
        /// Whether to limit the mouse to just diagonal.
        /// </summary>
        public static bool LimitMouse;

        /// <summary>
        /// Whether the program needs to draw the SelectedElement only repeatedly (that's what RapidRedraw is) - used for moving and resizing.
        /// </summary>
        public static bool RedrawSelectedElementOnly;


        // ========================================================================
        // TEXT VARIABLES
        // ========================================================================


        /// <summary>
        /// Whether bold is selected for text.
        /// </summary>
        public static bool BoldSelected;

        /// <summary>
        /// Whether italic is selected for text.
        /// </summary>
        public static bool ItalicSelected;

        /// <summary>
        /// Whether underline is selected for text.
        /// </summary>
        public static bool UnderlineSelected;
        #endregion

        /// <summary>
        /// Processes the main queue.
        /// </summary>
        public async static void ProcessQueueAsync()
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
                    switch (paction.Action)
                    {
                        case PerformableAction.Paint:
                            tsk = new Task<Bitmap>(PaintPreview);
                            break;
                        case PerformableAction.UpdatePreview:
                            tsk = new Task(() =>
                            {
                                try
                                {
                                    Program.MainForm.canvaspre.Image = endimg;
                                } catch { }
                            });
                            break;
                        case PerformableAction.Delete:
                            tsk = new Task(PerformDelete);
                            break;
                        case PerformableAction.AddElement:
                            tsk = new Task(() => { PerformAddElement(paction.Param1 as Element); });
                            break;
                        case PerformableAction.CancelTool:
                            tsk = new Task(PerformCancelTool);
                            break;
                        case PerformableAction.ApplyTool:
                            tsk = new Task(PerformApply);
                            break;
                        case PerformableAction.UseTool:
                            tsk = new Task(() => { PerformUseTool(paction.Param1 as PowerTool); });
                            break;                    
                        case PerformableAction.Fill:
                            tsk = new Task(() => { PerformFill((Point)paction.Param1); });
                            break;
                        //case PerformableAction.ChangeImageColor:
                        //    tsk = new Task(() => { BrushDrawing.PerformChangeGraphicsColor(paction.Param1 as Bitmap, paction.Param2 as Graphics, (Color)paction.Param3, (int)paction.Param4, (int)paction.Param5); });
                        //    break;
                        default:
                            break;
                    }

                    if (tsk != null)
                    {
                        tsk.Start();
                        await tsk;
                        Program.MainForm.canvaspre.Invalidate();
                    }
                }
                tsk.Dispose();
                queueLock = false;
            }    
        }

        public static void HandlePaint()
        {
            ActionQueue.Enqueue(new PerformAction(PerformableAction.Paint));
            ProcessQueueAsync();

        }

        /// <summary>
        /// Draws the preview. (Probably the most crucial method in the whole application!)
        /// </summary>
        /// <returns>An image for the result.</returns>
        public static Bitmap PaintPreview()
        {
            if (!editLock)
            {
                editLock = true;
                Bitmap endResult = new Bitmap(CurrentSave.ImageSize.Width, CurrentSave.ImageSize.Height);

                Graphics g = Graphics.FromImage(endResult);

                g.FillRectangle(Brushes.White, 0, 0, CurrentSave.ImageSize.Width, CurrentSave.ImageSize.Height);
                // Order them by Zindex:
                CurrentSave.ImageElements = CurrentSave.ImageElements.OrderBy(o => o.Zindex).ToList();

                // Now draw them all!

                for (int i = 0; i < CurrentSave.ImageElements.Count; i++)
                {
                    if (CurrentSave.ImageElements[i].Visible)
                        CurrentSave.ImageElements[i].ProcessImage(g);
                }

                EndImage = endResult;

                editLock = false;
                return endResult;              
            }
            return null;
        }

        /// <summary>
        /// Selects the element at the specified X and Y.
        /// </summary>
        /// <param name="x">The X to search for the element.</param>
        /// <param name="y">The Y to search for the element.</param>
        /// <returns>The element found at the location.</returns>
        public static Element SelectElementByLocation(int x, int y)
        {
            if (!editLock)
            {
                editLock = true;
                Element ret = null;

                // Order the list based on the Zindexs! But backwards so that the foreach picks up the top one!
                CurrentSave.ImageElements = CurrentSave.ImageElements.OrderBy(o => o.Zindex).Reverse().ToList();

                foreach (Element ele in CurrentSave.ImageElements)
                {
                    if (new Rectangle(ele.X - 10, ele.Y - 10, ele.Width + 20, ele.Height + 20).Contains(new Point(x, y)))
                    {
                        // The mouse is in this element!

                        ele.Zindex = CurrentSave.TopZindex++; // Brings to front
                        ret = ele;
                        break;
                    }
                }

                // Order the list based on the Zindexs!
                CurrentSave.ImageElements = CurrentSave.ImageElements.OrderBy(o => o.Zindex).ToList();

                editLock = false;
                return ret;
            }
            return null;
        }

        /// <summary>
        /// De-selects all elements.
        /// </summary>
        public static void DeselectElements()
        {
            SelectedElement = null;
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
                case Keys.Enter: // Apply PowerTool
                    HandleApply();

                    break;
                #region Arrow Keys
                case Keys.Left:
                    if (SelectedElement != null)
                        if (SelectedElement.X > 0) SelectedElement.X -= 1;

                    break;
                case Keys.Right:
                    if (SelectedElement != null)
                        if ((SelectedElement.X + SelectedElement.Width) < CurrentSave.ImageSize.Width) SelectedElement.X += 1;

                    break;
                case Keys.Up:
                    if (SelectedElement != null)
                        if (SelectedElement.Y > 0) SelectedElement.Y -= 1;

                    break;
                case Keys.Down:
                    if (SelectedElement != null)
                        if ((SelectedElement.Y + SelectedElement.Height) < CurrentSave.ImageSize.Height) SelectedElement.Y += 1;

                    break;
                case (Keys.Left | Keys.Alt):
                    if (SelectedElement != null)
                        if (SelectedElement.X > 9) SelectedElement.X -= 10;

                    break;
                case (Keys.Right | Keys.Alt):
                    if (SelectedElement != null)
                        if ((SelectedElement.X + SelectedElement.Width) < CurrentSave.ImageSize.Width - 9) SelectedElement.X += 10;

                    break;
                case (Keys.Up | Keys.Alt):
                    if (SelectedElement != null)
                        if (SelectedElement.Y > 9) SelectedElement.Y -= 10;

                    break;
                case (Keys.Down | Keys.Alt):
                    if (SelectedElement != null)
                        if ((SelectedElement.Y + SelectedElement.Height) < CurrentSave.ImageSize.Height - 9) SelectedElement.Y += 10;

                    break;
                case (Keys.Left | Keys.Control):
                    if (Program.MainForm.appcenter.HorizontalScroll.Value > 1)
                        Program.MainForm.appcenter.HorizontalScroll.Value -= 1;

                    break;
                case (Keys.Right | Keys.Control):
                    if (Program.MainForm.appcenter.HorizontalScroll.Value < Program.MainForm.appcenter.VerticalScroll.Maximum - 1)
                        Program.MainForm.appcenter.HorizontalScroll.Value += 1;

                    break;
                case (Keys.Up | Keys.Control):
                    if (Program.MainForm.appcenter.VerticalScroll.Value > 1)
                        Program.MainForm.appcenter.VerticalScroll.Value -= 1;

                    break;
                case (Keys.Down | Keys.Control):
                    if (Program.MainForm.appcenter.VerticalScroll.Value < Program.MainForm.appcenter.VerticalScroll.Maximum - 1)
                        Program.MainForm.appcenter.VerticalScroll.Value += 1;

                    break;
                case (Keys.Left | Keys.Control | Keys.Alt):
                    if (Program.MainForm.appcenter.HorizontalScroll.Value > 10)
                        Program.MainForm.appcenter.HorizontalScroll.Value -= 10;

                    break;
                case (Keys.Right | Keys.Control | Keys.Alt):
                    if (Program.MainForm.appcenter.HorizontalScroll.Value < Program.MainForm.appcenter.VerticalScroll.Maximum - 10)
                        Program.MainForm.appcenter.HorizontalScroll.Value += 10;

                    break;
                case (Keys.Up | Keys.Control | Keys.Alt):
                    if (Program.MainForm.appcenter.VerticalScroll.Value > 10)
                        Program.MainForm.appcenter.VerticalScroll.Value -= 10;

                    break;
                case (Keys.Down | Keys.Control | Keys.Alt):
                    if (Program.MainForm.appcenter.VerticalScroll.Value < Program.MainForm.appcenter.VerticalScroll.Maximum - 10)
                        Program.MainForm.appcenter.VerticalScroll.Value += 10;

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
                default:
                    break;
            }

            HandlePaint();
            Program.MainForm.canvaspre.Invalidate();
        }

        /// <summary>
        /// Starts using a PowerTool.
        /// </summary>
        /// <param name="tool">The PowerTool to use.</param>
        public static void UseTool(PowerTool tool)
        {
            ActionQueue.Enqueue(new PerformAction(PerformableAction.UseTool, tool));
            ProcessQueueAsync();
        }

        public static void PerformUseTool(PowerTool tool)
        {
            if (tool.UseRegionDrag)
                IsInDragRegion = true;

            CurrentTool = tool;

            tool.Prepare();
        }

        /// <summary>
        /// Cancels the current power tool.
        /// </summary>
        public static void CancelTool()
        {
            ActionQueue.Enqueue(new PerformAction(PerformableAction.CancelTool));
            ProcessQueueAsync();
        }

        public static void PerformCancelTool()
        {
            if (CurrentTool != null)
            {
                if (CurrentTool.UseRegionDrag)
                    IsInDragRegion = false;

                CurrentTool.Cancel();
                CurrentTool = null;

                Program.MainForm.canvaspre.Invalidate();
            }
        }

        /// <summary>
        /// Applies the current PowerTool.
        /// </summary>
        public static void HandleApply()
        {
            ActionQueue.Enqueue(new PerformAction(PerformableAction.ApplyTool));
            ProcessQueueAsync();
        }

        public static void PerformApply()
        {
            if (CurrentTool != null)
                if (CurrentTool.UseRegionDrag)
                {
                    IsInDragRegion = false;
                    CurrentTool.Apply(DragRegionSelect);
                }
                else
                {
                    IsInDragRegion = false;
                    CurrentTool.Apply(new Rectangle());
                }
            Program.MainForm.canvaspre.Invalidate();
        }

        /// <summary>
        /// Deletes the SelectedElement.
        /// </summary>
        public static void HandleDelete()
        {
            ActionQueue.Enqueue(new PerformAction(PerformableAction.Delete));
            ProcessQueueAsync();
        }

        public static void PerformDelete()
        {
            if (SelectedElement != null)
                if (SelectedTool == Tool.Selection)
                {
                    CurrentSave.ImageElements.Remove(SelectedElement);

                    DeselectElements();

                    Program.MainForm.canvaspre.Invalidate();
                }
        }

        /// <summary>
        /// Creates a new image.
        /// </summary>
        public static void HandleNew()
        {
            Sizer sz = new Sizer();
            sz.StartSizer(true, new Size());
            sz.ShowDialog();

            if (!sz.Cancelled)
            {
                CurrentSave.ImageSize = sz.ReturnSize;

                Program.MainForm.ReloadImage();

                CurrentSave.ImageElements = new List<Element>();

                PaintPreview();
            }
        }

        public static void HandleOpen()
        {
            if (Program.MainForm.openFileDialogOPEN.ShowDialog() == DialogResult.OK)
                LoadFile(Program.MainForm.openFileDialogOPEN.FileName);

            Program.MainForm.ReloadImage();
            PaintPreview();
        }

        public static void HandleSave()
        {
            if (string.IsNullOrEmpty(CurrentFile))
            {
                if (Program.MainForm.saveFileDialogSAVE.ShowDialog() == DialogResult.OK)
                    SaveFile(Program.MainForm.saveFileDialogSAVE.FileName, true);
            }
            else
                SaveFile(CurrentFile);

            Program.MainForm.ReloadImage();
            PaintPreview();
        }

        public static void HandleSaveAs()
        {
            if (Program.MainForm.saveFileDialogSAVE.ShowDialog() == DialogResult.OK)
                SaveFile(Program.MainForm.saveFileDialogSAVE.FileName, true);

            Program.MainForm.ReloadImage();
            PaintPreview();
        }

        /// <summary>
        /// Cuts the SelectedElement.
        /// </summary>
        public static void HandleCut()
        {
            HandleCopy();

            HandleDelete();

            Program.MainForm.canvaspre.Invalidate();
        }

        /// <summary>
        /// Copies the SelectedElement.
        /// </summary>
        public static void HandleCopy()
        {
            Clipboard.SetDataObject("ABPELE" + ABJson.GDISupport.JsonSerializer.Serialize("", SelectedElement, ABJson.GDISupport.JsonFormatting.Compact, 0, true).TrimEnd(','), true);
        }

        /// <summary>
        /// Pastes the SelectedElement.
        /// </summary>
        public static void HandlePaste()
        {
            IDataObject data = Clipboard.GetDataObject();

            if (data.GetDataPresent(DataFormats.Text) && data.GetData(DataFormats.Text).ToString().StartsWith("ABPELE", StringComparison.CurrentCulture))
            {
                Element ele = ABJson.GDISupport.JsonClassConverter.ConvertJsonToObject<Element>(data.GetData(DataFormats.Text).ToString().Remove(0, 6), true);

                ele.Zindex = CurrentSave.TopZindex++;
                AddElement(ele);

                SelectedElement = ele;
            }

            Program.MainForm.canvaspre.Invalidate();
        }

        /// <summary>
        /// Zooms into the image.
        /// </summary>
        public static void HandleZoomIn()
        {
            if (MagnificationLevel < 16)
            {
                OldMagnificationLevel = MagnificationLevel;
                MagnificationLevel = MagnificationLevel * 2;
                
                Program.MainForm.label11.Text = "X" + MagnificationLevel;            
                Program.MainForm.ReloadImage();
            }
        }

        /// <summary>
        /// Zooms out of the image.
        /// </summary>
        public static void HandleZoomOut()
        {
            if (MagnificationLevel > 1)
            {
                OldMagnificationLevel = MagnificationLevel;
                MagnificationLevel = MagnificationLevel / 2;

                Program.MainForm.label11.Text = "X" + MagnificationLevel;
                Program.MainForm.ReloadImage();
            }
            PaintPreview();
        }

        /// <summary>
        /// Adds an element.
        /// </summary>
        /// <param name="element">The element to add.</param>
        public static void AddElement(Element element)
        {
            ActionQueue.Enqueue(new PerformAction(PerformableAction.AddElement, element));
            ProcessQueueAsync();
        }

        public static void PerformAddElement(Element element)
        {
            Pencil asPencil = element as Pencil;
            CurrentSave.ImageElements.Add(element);

            // TODO: Add undo option!
        }

        /// <summary>
        /// Function for queuing the flood fill.
        /// </summary>
        /// <param name="mouseLoc">The mouse location (used in the flood fill).</param>
        public static void HandleFill(Point mouseLoc)
        {
            ActionQueue.Enqueue(new PerformAction(PerformableAction.Fill, mouseLoc));
            ProcessQueueAsync();
        }

        public static void PerformFill(Point mouseLoc)
        {
            if (isFilling) MessageBox.Show("That's impossible.");
            isFilling = true;

            CurrentDrawingElement = new Fill()
            {
                X = mouseLoc.X,
                Y = mouseLoc.Y,
                Width = CurrentSave.ImageSize.Width,
                Height = CurrentSave.ImageSize.Height
            };

            Fill asFill = CurrentDrawingElement as Fill;

            DrawingMin = mouseLoc;
            DrawingMax = mouseLoc;

            asFill.FillPoints = ImageFilling.SafeFloodFill(ImageFormer.ImageToByteArray(PaintPreview()), mouseLoc.X, mouseLoc.Y, Color.FromArgb(1, 0, 1));
            asFill.FillColor = Program.MainForm.clrNorm.BackColor;

            CurrentDrawingElement.X = DrawingMin.X - 1;
            CurrentDrawingElement.Y = DrawingMin.Y - 1;

            CurrentDrawingElement.Width = (DrawingMax.X - DrawingMin.X) + 1;
            CurrentDrawingElement.Height = (DrawingMax.Y - DrawingMin.Y) + 1;
            CurrentDrawingElement.Zindex = CurrentSave.TopZindex++;

            asFill.FillPoints = ImageCropping.CropImage(asFill.FillPoints, CurrentDrawingElement.X, CurrentDrawingElement.Y, CurrentDrawingElement.Width, CurrentDrawingElement.Height);
            PerformAddElement(CurrentDrawingElement);

            if (CurrentDrawingGraphics != null) CurrentDrawingGraphics.Dispose();
            CurrentDrawingElement = null;

            EndImage = PaintPreview();

            isFilling = false;
        }

        // It is recommended that you hide the lower two regions
        #region Mouse Selection Handlers

        /// <summary>
        /// Handles the selection part for the MouseDown event.
        /// </summary>
        /// <param name="mouseLoc">The Mouse Location when this event was triggered.</param>
        public static void HandleMouseDownSelection(Point mouseLoc)
        {
            SelectedElement = SelectElementByLocation(mouseLoc.X, mouseLoc.Y);

            if (SelectedElement == null) Program.MainForm.ShowProperties("Selection Tool - Nothing selected!", false, false, false, false, false, false, Program.MainForm.GetCurrentColor());
            if (SelectedElement is Pencil pencil) Program.MainForm.ShowProperties("Selection Tool - Pencil", false, false, true, false, false, false, pencil.PencilColor);
            if (SelectedElement is Elements.Brush brush) Program.MainForm.ShowProperties("Selection Tool - Brush", false, false, true, false, false, false, brush.BrushColor);
            if (SelectedElement is RectangleE rect) Program.MainForm.ShowProperties("Selection Tool - Rectangle", true, true, false, true, false, false, rect.FillColor, rect.BorderColor);
            if (SelectedElement is Ellipse elli) Program.MainForm.ShowProperties("Selection Tool - Ellipse", true, true, false, true, false, false, elli.FillColor, elli.BorderColor);
            // Line Tool Found Below
            if (SelectedElement is Fill fill) Program.MainForm.ShowProperties("Selection Tool - Fill", false, false, true, false, false, false, fill.FillColor);
            if (SelectedElement is Text text) Program.MainForm.ShowProperties("Selection Tool - Text", false, false, true, false, false, true, text.Clr, Color.Black, text.MainText, text.Fnt);
            // Add more

            Program.MainForm.canvaspre.Invalidate();

            if (SelectedElement != null)
            {
                if (new Rectangle(SelectedElement.X - (10 * MagnificationLevel), SelectedElement.Y - (10 * MagnificationLevel), SelectedElement.Width + (20 * MagnificationLevel), SelectedElement.Height + (20 * MagnificationLevel)).Contains(mouseLoc))
                {
                    IsOnSelection = true;
                    // The mouse is in this element!

                    if (SelectedElement is Line line) {
                        Program.MainForm.ShowProperties("Selection Tool - Line", false, false, true, false, true, false, line.Color);
                        // Check if the mouse is on the two points, otherwise, move the element.

                        CornerSelected = Corner.None;

                        // First point
                        if (new Rectangle(line.StartPoint.X + SelectedElement.X - 10, line.StartPoint.Y + SelectedElement.Y - 10, 20, 20).Contains(mouseLoc)) CornerSelected = Corner.TopLeft;

                        // Second point
                        if (new Rectangle(line.EndPoint.X + SelectedElement.X - 10, line.EndPoint.Y + SelectedElement.Y - 10, 20, 20).Contains(mouseLoc)) CornerSelected = Corner.TopRight;

                        if (CornerSelected == Corner.TopLeft) BeforeResizePoint = line.StartPoint;
                        else if (CornerSelected == Corner.TopRight) BeforeResizePoint = line.EndPoint;


                        SelectedElement.Visible = false;
                        BeforeMove = PaintPreview();
                        SelectedElement.Visible = true;

                        Program.MainForm.movingRefresh.Start();
                    } else {
                        if (!(SelectedElement is Pencil) && !(SelectedElement is Elements.Brush) && !(SelectedElement is Fill))
                        {
                            // Check if the mouse is on the scaling points, otherwise, move the element.

                            CornerSelected = Corner.None;

                            // Top left corner
                            if (new Rectangle(SelectedElement.X - 10, SelectedElement.Y - 10, 20, 20).Contains(mouseLoc)) CornerSelected = Corner.TopLeft;

                            // Top Right Corner
                            if (new Rectangle(SelectedElement.Right - 10, SelectedElement.Y - 10, 20, 20).Contains(mouseLoc)) CornerSelected = Corner.TopRight;

                            // Bottom Left corner
                            if (new Rectangle(SelectedElement.X - 10, SelectedElement.Bottom - 10, 20, 20).Contains(mouseLoc)) CornerSelected = Corner.BottomLeft;

                            // Bottom Right corner
                            if (new Rectangle(SelectedElement.Right - 10, SelectedElement.Bottom - 10, 20, 20).Contains(mouseLoc)) CornerSelected = Corner.BottomRight;
                        }

                        BeforeResizeSize = new Size(SelectedElement.Width, SelectedElement.Height);

                        if (CornerSelected == Corner.TopLeft) BeforeResizePoint = new Point(SelectedElement.X, SelectedElement.Y);
                        else if (CornerSelected == Corner.TopRight) BeforeResizePoint = new Point(SelectedElement.X + SelectedElement.Width, SelectedElement.Y);
                        else if (CornerSelected == Corner.BottomLeft) BeforeResizePoint = new Point(SelectedElement.X, SelectedElement.Y + SelectedElement.Height);
                        else if (CornerSelected == Corner.BottomRight) BeforeResizePoint = new Point(SelectedElement.X + SelectedElement.Width, SelectedElement.Y + SelectedElement.Height);
                    }
                }
            }
        }

        /// <summary>
        /// Handles the selection part for the MouseMove event.
        /// </summary>
        /// <param name="mouseLoc">The Mouse Location when this event was triggered.</param>
        public static void HandleMouseMoveSelection(Point mouseLoc)
        {
            if (SelectedElement != null)
            {
                if (IsMoving)
                {
                    SelectedElement.X = mouseLoc.X - IsMovingGap.Width;
                    SelectedElement.Y = mouseLoc.Y - IsMovingGap.Height;
                } else if (CornerSelected == Corner.None) {

                    if (new Rectangle(SelectedElement.X - (10 * MagnificationLevel), SelectedElement.Y - (10 * MagnificationLevel), SelectedElement.Width + (20 * MagnificationLevel), SelectedElement.Height + (20 * MagnificationLevel)).Contains(mouseLoc))
                    {
                        if (!IsOnSelection)
                        {
                            IsOnSelection = true;
                            Program.MainForm.canvaspre.Invalidate();
                        }

                        if (MouseDownOnCanvas)
                        {
                            // Move the element

                            IsMovingOld = new Point(SelectedElement.X, SelectedElement.Y);

                            IsMovingGap.Width = mouseLoc.X - SelectedElement.X;
                            IsMovingGap.Height = mouseLoc.Y - SelectedElement.Y;

                            SelectedElement.Visible = false;
                            BeforeMove = PaintPreview();
                            SelectedElement.Visible = true;

                            Program.MainForm.movingRefresh.Start();

                            IsMoving = true;
                            RedrawSelectedElementOnly = true;
                        }
                    }
                    else if (IsOnSelection)
                    {              
                        IsOnSelection = false;
                        Program.MainForm.canvaspre.Invalidate();
                    }

                } else if (SelectedElement is Line lineEle) {
                    if (CornerSelected == Corner.TopLeft)
                        lineEle.StartPoint = new Point(mouseLoc.X - SelectedElement.X, mouseLoc.Y - SelectedElement.Y);
                    else if (CornerSelected == Corner.TopRight)
                        lineEle.EndPoint = new Point(mouseLoc.X - SelectedElement.X, mouseLoc.Y - SelectedElement.Y);

                    RedrawSelectedElementOnly = true;

                    LineResizing.Resize(ref lineEle);

                } else if (!(SelectedElement is Pencil) && !(SelectedElement is Elements.Brush) && !(SelectedElement is Fill)) {
                    int proposedX = SelectedElement.X;
                    int proposedY = SelectedElement.Y;
                    int proposedWidth = SelectedElement.Width;
                    int proposedHeight = SelectedElement.Height;

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

                            if (proposedWidth < 0) proposedX = mouseLoc.X + proposedWidth;
                            else proposedX = mouseLoc.X;

                            if (proposedHeight < 0) proposedY = mouseLoc.Y;

                            break;
                        default: // Bottom-right corner
                            proposedWidth = (mouseLoc.X - BeforeResizePoint.X) + BeforeResizeSize.Width;
                            proposedHeight = (mouseLoc.Y - BeforeResizePoint.Y) + BeforeResizeSize.Height;

                            if (proposedWidth < 0) proposedX = mouseLoc.X;
                            if (proposedHeight < 0) proposedY = mouseLoc.Y;

                            break;
                    }

                    SelectedElement.X = proposedX;
                    SelectedElement.Y = proposedY;

                    SelectedElement.Width = Math.Abs(proposedWidth);
                    SelectedElement.Height = Math.Abs(proposedHeight);

                    SelectedElement.Visible = false;
                    BeforeMove = PaintPreview();
                    SelectedElement.Visible = true;

                    Program.MainForm.movingRefresh.Start();

                    RedrawSelectedElementOnly = true;
                    SelectedElement.Resize();
                }
            }
        }

        /// <summary>
        /// Handles the selection part for the MouseUp event.
        /// </summary>
        /// <param name="mouseLoc">The Mouse Location when this event was triggered.</param>
        public static void HandleMouseUpSelection()
        {
            if (CornerSelected != Corner.None)
            {
                SelectedElement.FinishResize();
                CornerSelected = Corner.None;
            }

            IsMoving = false;

            PaintPreview();
            RedrawSelectedElementOnly = false;
            Program.MainForm.movingRefresh.Stop();
        }
        #endregion

        #region Mouse Handlers
        /// <summary>
        /// Handles the MouseDown event.
        /// </summary>
        /// <param name="e">The EventArgs.</param>
        public static void HandleMouseDown(MouseEventArgs e)
        {
            Point mouseLoc = new Point(e.X / MagnificationLevel, e.Y / MagnificationLevel);
            MousePoint = mouseLoc;

            if (IsInDragRegion)
            {
                CurrentlyDragging = true;

                StartPoint = mouseLoc;
                DragRegionSelect.X = StartPoint.X;
                DragRegionSelect.Y = StartPoint.Y;
                DragRegionSelect.Width = 0;
                DragRegionSelect.Height = 0;

                Program.MainForm.canvaspre.Invalidate();
            } else {
                if (e.Button == MouseButtons.Left)
                {
                    DrawingMin.X = mouseLoc.X;
                    DrawingMin.Y = mouseLoc.Y; // This fixes a bug.
                    DrawingMax.X = 0;
                    DrawingMax.Y = 0;

                    if (SelectedTool == Tool.Selection) // Selection tool!
                        HandleMouseDownSelection(mouseLoc);

                    if (SelectedTool == Tool.Pencil)
                    {
                        LastMousePoint = new Point(mouseLoc.X, mouseLoc.Y);

                        CurrentDrawingElement = new Pencil()
                        {
                            Width = CurrentSave.ImageSize.Width,
                            Height = CurrentSave.ImageSize.Height,
                            PencilPoints = new Bitmap(CurrentSave.ImageSize.Width, CurrentSave.ImageSize.Height)
                        };

                        Grph.StartFigure();
                        CurrentDrawingGraphics = Graphics.FromImage((CurrentDrawingElement as Pencil).PencilPoints);

                        CurrentDrawingGraphics.Clear(Color.Transparent);

                        CurrentDrawingGraphics.FillRectangle(Sb101, mouseLoc.X, mouseLoc.Y, 1, 1); // (mouseLoc.X, mouseLoc.Y, Color.FromArgb(1, 0, 1));
                    }

                    if (SelectedTool == Tool.Brush)
                    {
                        LastMousePoint = new Point(mouseLoc.X, mouseLoc.Y);

                        CurrentDrawingElement = new Elements.Brush()
                        {
                            Width = CurrentSave.ImageSize.Width,
                            Height = CurrentSave.ImageSize.Height,
                            BrushPoint = new Bitmap(CurrentSave.ImageSize.Width, CurrentSave.ImageSize.Height)
                        };

                        DrawingMax = new Point(mouseLoc.X, mouseLoc.Y);

                        CurrentDrawingGraphics = Graphics.FromImage((CurrentDrawingElement as Elements.Brush).BrushPoint);

                        CurrentDrawingGraphics.Clear(Color.Transparent);
                    }

                    if (SelectedTool == Tool.Rectangle)
                    {
                        CurrentDrawingElement = new RectangleE()
                        {
                            Width = CurrentSave.ImageSize.Width,
                            Height = CurrentSave.ImageSize.Height
                        };

                        StartPoint = mouseLoc;
                    }

                    if (SelectedTool == Tool.Ellipse)
                    {
                        CurrentDrawingElement = new Ellipse()
                        {
                            Width = CurrentSave.ImageSize.Width,
                            Height = CurrentSave.ImageSize.Height
                        };

                        StartPoint = mouseLoc;
                    }

                    if (SelectedTool == Tool.Rectangle || SelectedTool == Tool.Ellipse)
                    {
                        dynamic dyn = ((dynamic)CurrentDrawingElement);
                        dyn.IsFilled = true;
                        dyn.BorderColor = Program.MainForm.clrBord.BackColor;
                        dyn.FillColor = Program.MainForm.clrFill.BackColor;
                        dyn.BorderSize = (Program.MainForm.txtBWidth.Text.Length > 0) ? int.Parse(Program.MainForm.txtBWidth.Text, CultureInfo.CurrentCulture) : 0;
                    }

                    if (SelectedTool == Tool.Line)
                    {
                        CurrentDrawingElement = new Line()
                        {
                            Width = CurrentSave.ImageSize.Width,
                            Height = CurrentSave.ImageSize.Height,
                            Color = Program.MainForm.clrNorm.BackColor,
                            Thickness = (Program.MainForm.txtBThick.Text.Length > 0) ? int.Parse(Program.MainForm.txtBThick.Text, CultureInfo.CurrentCulture) : 0
                        };

                        StartPoint = mouseLoc;
                    }


                    #region Fill + Text
                    if (SelectedTool == Tool.Fill)
                        HandleFill(mouseLoc);

                    if (SelectedTool == Tool.Text)
                    {
                        CurrentDrawingElement = new Text()
                        {
                            X = mouseLoc.X,
                            Y = mouseLoc.Y,
                        };

                        Text txt = CurrentDrawingElement as Text;

                        txt.MainText = Program.MainForm.txtTText.Text;
                        txt.Clr = Program.MainForm.clrNorm.BackColor;

                        try
                        {
                            FontStyle bold = (BoldSelected) ? FontStyle.Bold : FontStyle.Regular;
                            FontStyle italic = (ItalicSelected) ? FontStyle.Italic : FontStyle.Regular;
                            FontStyle underline = (UnderlineSelected) ? FontStyle.Underline : FontStyle.Regular;

                            txt.Fnt = new Font(Program.MainForm.cmbFont.Text, (Program.MainForm.cmbSize.Text.Length > 0) ? int.Parse(Program.MainForm.cmbSize.Text, CultureInfo.CurrentCulture) : 0, bold | italic | underline);
                        }
                        catch (ArgumentException ex)
                        {
                            Console.WriteLine("AN EXCEPTION HAS OCCURED: " + ex.Message);
                            Program.MainForm.cmbFont.Text = "Microsoft Sans Serif";
                            Program.MainForm.cmbSize.Text = "12";
                            txt.Fnt = new Font(Program.MainForm.cmbFont.Text, (Program.MainForm.cmbSize.Text.Length > 0) ? int.Parse(Program.MainForm.cmbSize.Text, CultureInfo.CurrentCulture) : 0, FontStyle.Regular);
                        }

                        Size widthHeight = Elements.Text.MeasureText(Program.MainForm.txtTText.Text, txt.Fnt);
                        CurrentDrawingElement.Width = Convert.ToInt32(Math.Ceiling(widthHeight.Width + txt.Fnt.Size));
                        CurrentDrawingElement.Height = widthHeight.Height;
                    }
                    #endregion

                    
                }
            }
            if (!isFilling) MouseDownOnCanvas = true;
        }

        /// <summary>
        /// Handles the MouseMove event.
        /// </summary>
        /// <param name="e">The EventArgs.</param>
        public static void HandleMouseMove(MouseEventArgs e)
        {
            Point mouseLoc = new Point(e.X / MagnificationLevel, e.Y / MagnificationLevel);
            if (SelectedTool == Tool.Selection)
                HandleMouseMoveSelection(mouseLoc);
            
            if (MouseDownOnCanvas)
            {
                MousePoint = mouseLoc;

                if (CurrentlyDragging)
                {
                    DragRegionSelect.Width = mouseLoc.X - StartPoint.X;
                    DragRegionSelect.Height = mouseLoc.Y - StartPoint.Y;

                    Program.MainForm.canvaspre.Invalidate();
                }
      
                if (CurrentDrawingElement is Pencil)
                {
                    Grph.AddLine(LastMousePoint.X, LastMousePoint.Y, mouseLoc.X, mouseLoc.Y);
                    CurrentDrawingGraphics.DrawPath(new Pen(Color.FromArgb(1, 0, 1)), Grph);

                    Program.MainForm.canvaspre.Invalidate();
                }

                if (CurrentDrawingElement is Elements.Brush)
                {
                    BrushDrawing.DrawLineOfEllipse((Program.MainForm.txtBThick.Text.Length > 0) ? int.Parse(Program.MainForm.txtBThick.Text, CultureInfo.CurrentCulture) : 0, CurrentDrawingGraphics, Sb101, LastMousePoint.X, LastMousePoint.Y, mouseLoc.X, mouseLoc.Y);

                    Program.MainForm.canvaspre.Invalidate();
                }

                if (CurrentDrawingElement is RectangleE || CurrentDrawingElement is Ellipse || CurrentDrawingElement is Line)
                    Program.MainForm.canvaspre.Invalidate();

                if (CurrentDrawingElement is Text)
                {
                    CurrentDrawingElement.X = mouseLoc.X;
                    CurrentDrawingElement.Y = mouseLoc.Y;

                    Program.MainForm.canvaspre.Invalidate();
                }

                if (CurrentDrawingElement is Pencil || CurrentDrawingElement is Elements.Brush || CurrentDrawingElement is Line || CurrentDrawingElement is RectangleE || CurrentDrawingElement is Ellipse)
                {
                    if (mouseLoc.X > DrawingMax.X) DrawingMax.X = mouseLoc.X;
                    if (mouseLoc.Y > DrawingMax.Y) DrawingMax.Y = mouseLoc.Y;
                    if (mouseLoc.X < DrawingMin.X) DrawingMin.X = mouseLoc.X;
                    if (mouseLoc.Y < DrawingMin.Y) DrawingMin.Y = mouseLoc.Y;
                }

                LastMousePoint = mouseLoc;
            }
        }

        /// <summary>
        /// Handles the MouseUp event.
        /// </summary>
        /// <param name="e">The EventArgs.</param>
        public static void HandleMouseUp(MouseEventArgs e)
        {
            //Point mouseLoc = new Point((int)Math.Round((decimal)((e.X - (MagnificationLevel / 2)) / MagnificationLevel)), (int)Math.Round((decimal)((e.X - (MagnificationLevel / 2)) / MagnificationLevel)));
            Point mouseLoc = new Point(e.X / MagnificationLevel, e.Y / MagnificationLevel);

            if (IsInDragRegion)
            {
                CurrentlyDragging = false;

                int widthAmount = 0, heightAmount = 0;

                if (DragRegionSelect.Width < 0)
                    widthAmount = Math.Abs(DragRegionSelect.Width);

                if (DragRegionSelect.Height < 0)
                    heightAmount = Math.Abs(DragRegionSelect.Height);

                DragRegionSelect = new Rectangle(DragRegionSelect.X - widthAmount, DragRegionSelect.Y - heightAmount, Math.Abs(DragRegionSelect.Width), Math.Abs(DragRegionSelect.Height));
            } else {
                if (MouseDownOnCanvas)
                {
                    int x = (DrawingMin.X < 0) ? 0 : DrawingMin.X; // X for Pencil/Brush
                    int y = (DrawingMin.Y < 0) ? 0 : DrawingMin.Y; // Y for Pencil/Brush

                    Program.MainForm.movingRefresh.Stop();
                    MouseDownOnCanvas = false;

                    if (SelectedTool == Tool.Selection)
                        HandleMouseUpSelection();

                    if (CurrentDrawingElement != null)
                    {
                        if (mouseLoc.X > DrawingMax.X) DrawingMax.X = mouseLoc.X;
                        if (mouseLoc.Y > DrawingMax.Y) DrawingMax.Y = mouseLoc.Y;
                        if (mouseLoc.X < DrawingMin.X) DrawingMin.X = mouseLoc.X;
                        if (mouseLoc.Y < DrawingMin.Y) DrawingMin.Y = mouseLoc.Y;

                        // Pencil
                        if (CurrentDrawingElement is Pencil asPencil)
                        {
                            Grph.Reset();
                            asPencil.PencilPoints = ImageCropping.CropImage(asPencil.PencilPoints, x, y, DrawingMax.X, DrawingMax.Y);
                            asPencil.PencilColor = Program.MainForm.clrNorm.BackColor;

                            CurrentDrawingElement.X = x;
                            CurrentDrawingElement.Y = y;

                            CurrentDrawingElement.Width = (DrawingMax.X - DrawingMin.X);
                            CurrentDrawingElement.Height = (DrawingMax.Y - DrawingMin.Y);
                        }

                        // Brush
                        if (CurrentDrawingElement is Elements.Brush asBrush)
                        {
                            CurrentDrawingElement.X = x;
                            CurrentDrawingElement.Y = y;

                            CurrentDrawingElement.Width = (DrawingMax.X - DrawingMin.X) + ((Program.MainForm.txtBThick.Text.Length > 0) ? int.Parse(Program.MainForm.txtBThick.Text, CultureInfo.CurrentCulture) : 0);
                            CurrentDrawingElement.Height = (DrawingMax.Y - DrawingMin.Y) + ((Program.MainForm.txtBThick.Text.Length > 0) ? int.Parse(Program.MainForm.txtBThick.Text, CultureInfo.CurrentCulture) : 0);

                            asBrush.BrushPoint = ImageCropping.CropImage(asBrush.BrushPoint, x, y, DrawingMax.X + ((Program.MainForm.txtBThick.Text.Length > 0) ? int.Parse(Program.MainForm.txtBThick.Text, CultureInfo.CurrentCulture) : 0), DrawingMax.Y + ((Program.MainForm.txtBThick.Text.Length > 0) ? int.Parse(Program.MainForm.txtBThick.Text, CultureInfo.CurrentCulture) : 0));
                            asBrush.BrushColor = Program.MainForm.clrNorm.BackColor;
                        }

                        // Rectangle + Ellipse
                        if (CurrentDrawingElement is RectangleE || CurrentDrawingElement is Ellipse)
                        {
                            CurrentDrawingElement.Zindex = CurrentSave.TopZindex++;

                            int borderSize = (Program.MainForm.txtBWidth.Text.Length > 0) ? int.Parse(Program.MainForm.txtBWidth.Text, CultureInfo.CurrentCulture) : 0;

                            CurrentDrawingElement.Width = mouseLoc.X - StartPoint.X + borderSize;
                            CurrentDrawingElement.Height = mouseLoc.Y - StartPoint.Y + borderSize;

                            if (CurrentDrawingElement.Width < 0) CurrentDrawingElement.Width = CurrentDrawingElement.Width - borderSize;
                            if (CurrentDrawingElement.Height < 0) CurrentDrawingElement.Height = CurrentDrawingElement.Height - borderSize;

                            if (CurrentDrawingElement.Width < 0) CurrentDrawingElement.X = StartPoint.X - Math.Abs(CurrentDrawingElement.Width);
                            else CurrentDrawingElement.X = StartPoint.X;

                            if (CurrentDrawingElement.Height < 0) CurrentDrawingElement.Y = StartPoint.Y - Math.Abs(CurrentDrawingElement.Height);
                            else CurrentDrawingElement.Y = StartPoint.Y;

                            if (CurrentDrawingElement.Width < 0) CurrentDrawingElement.Width = CurrentDrawingElement.Width - borderSize;
                            if (CurrentDrawingElement.Height < 0) CurrentDrawingElement.Height = CurrentDrawingElement.Height - borderSize;

                            CurrentDrawingElement.Width = Math.Abs(CurrentDrawingElement.Width);
                            CurrentDrawingElement.Height = Math.Abs(CurrentDrawingElement.Height);
                        }

                        // Line
                        if (CurrentDrawingElement is Line asLine)
                        {
                            CurrentDrawingElement.Zindex = CurrentSave.TopZindex++;

                            // The below code is to get the X even in minus numbers!

                            CurrentDrawingElement.X = DrawingMin.X;
                            CurrentDrawingElement.Y = DrawingMin.Y;

                            asLine.StartPoint = new Point((StartPoint.X - DrawingMin.X), (StartPoint.Y - DrawingMin.Y));
                            asLine.EndPoint = new Point((mouseLoc.X - DrawingMin.X), (mouseLoc.Y - DrawingMin.Y));

                            asLine.Color = Program.MainForm.clrNorm.BackColor;

                            LineResizing.Resize(ref asLine);

                            Program.MainForm.canvaspre.Invalidate();
                        }

                        // Text
                        if (CurrentDrawingElement is Text)
                        {
                            CurrentDrawingElement.X = e.X;
                            CurrentDrawingElement.Y = e.Y;
                        }

                        // Change some things and add the element!

                        if (CurrentDrawingElement.Width < 0) CurrentDrawingElement.Width = 1;
                        if (CurrentDrawingElement.Height < 0) CurrentDrawingElement.Height = 1;

                        CurrentDrawingElement.Zindex = CurrentSave.TopZindex++;
                        if (!isFilling) 
                        {
                            AddElement(CurrentDrawingElement);

                            // Dispose some stuff.

                        
                            if (CurrentDrawingGraphics != null) CurrentDrawingGraphics.Dispose();
                            CurrentDrawingElement = null;
                        }
                    }
                }

                if (SelectedElement != null)
                    Program.MainForm.canvaspre.Invalidate();
            }

            HandlePaint();

            // Get the garbage collector to collect all of the rubbish we made while drawing that element (MouseDown, MouseMove and MouseUp).
            GC.Collect();

            Program.MainForm.canvaspre.Invalidate();
        }
        #endregion      
    }
}
