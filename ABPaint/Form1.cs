using ABPaint.Elements;
using ABPaint.Objects;
using ABPaint.Tools.Backend;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static ABPaint.Tools.Backend.SaveSystem;

namespace ABPaint
{
    public partial class Form1 : Form
    {
        // General Variables
        public Tool selectedTool = Tool.Selection;
        public int topZIndex = 0;
        public int selectedPalette = 1;
        public Element currentDrawingElement;
        public Graphics currentDrawingGraphics;
        public bool MouseDownOnCanvas = false;
        Task<Bitmap> tskPP = null;

        public Point mousePoint = new Point(0, 0);

        // The image + Extra SolidBrush

        private Bitmap endimg; // Just because you have to...

        public Bitmap endImage
        {
            get
            {
                return endimg;
            }
            set
            {
                endimg = value;
                canvaspre.Invalidate();
            }
        }

        
        public SolidBrush sb101 = new SolidBrush(Color.FromArgb(1, 0, 1));

        // Magnification level.
        public int MagnificationLevel = 1;

        // All the things needed for pencil + brush
        public System.Drawing.Drawing2D.GraphicsPath grph = new System.Drawing.Drawing2D.GraphicsPath();
        public Point DrawingMin;
        public Point DrawingMax;
        public Point lastMousePoint;
        public Point startPoint;

        public int LastX;
        public int LastY;

        // Fill?
        Task<Bitmap> fill;

        // The Selection tool stuff
        public Element selectedElement;
        public Size IsMovingGap;
        public Point IsMovingOld;
        public bool IsMoving = false;
        public bool IsOnSelection = false;

        // Resizing 
        public int CornerSelected = 0;
        public Point BeforeResizePoint;
        public Size BeforeResizeSize;
        public bool Resized = false;
        public bool LimitMouse;

        // Text
        public bool BoldSelected = false;
        public bool ItalicSelected = false;
        public bool UnderlineSelected = false;

        //public List<Element> savedata.imageElements = new List<Element>();

        public Form1()
        {
            InitializeComponent();

            //canvaspre.PreviewKeyDown += new PreviewKeyDownEventHandler((sender, e) => {
            //    Core.HandleKeyPress(e.KeyCode); });
            //this.KeyDown += new KeyEventHandler((sender, e) => {
            //    Core.HandleKeyPress(e.KeyCode); });

            ReloadImage();
        }
        #region General Code

        private void Form1_Load(object sender, EventArgs e)
        {
            properties.Hide();
            toolCursorS.Hide();

            System.Drawing.Text.InstalledFontCollection allFonts = new System.Drawing.Text.InstalledFontCollection();

            // Get an array of the system's font familiies.
            FontFamily[] fontFamilies = allFonts.Families;

            // Display the font families.
            foreach (FontFamily myFont in fontFamilies)
            {
                cmbFont.Items.Add(myFont.Name);
                //font_family

            }

            savedata.imageElements = new List<Element>();
            endImage = new Bitmap(savedata.imageSize.Width, savedata.imageSize.Height);
        }

        public void SelectTool(ref PictureBox toSelect)
        {
            toSelect.BackColor = Color.Black;
            try
            {
                if (toSelect.Tag.ToString() != "selected") // If it isn't selected already
                    toSelect.Image = ImageInverting.InvertImage((Bitmap)toSelect.Image);
            }
            catch { toSelect.Image = ImageInverting.InvertImage((Bitmap)toSelect.Image); }
            toSelect.Tag = "selected";
        }

        private void NumbersOnlyKeyPress(Object sender, KeyPressEventArgs e)
        {
            if (((TextBox)sender).Text.Length > 2)
            {
                if ((int)e.KeyChar != 8)
                {
                    e.Handled = true;
                    return;
                }
            }
            if ((int)e.KeyChar != 8)
            {
                if ((int)e.KeyChar < 48 | (int)e.KeyChar > 57)
                {
                    e.Handled = true;
                }
            }

        }

        public void DeSelectTool(ref PictureBox toSelect)
        {
            toSelect.BackColor = Color.White;
            try
            {
                if (toSelect.Tag.ToString() == "selected") // If it isn't selected already
                    toSelect.Image = ImageInverting.InvertImage((Bitmap)toSelect.Image);
            }
            catch { }
            toSelect.Tag = "";
        }

        private void timgCursorN_Click(object sender, EventArgs e)
        {
            // Select this tool

            SelectTool(ref timgCursorN);
            DeSelectTool(ref timgCursorS);
            DeSelectTool(ref timgPencil);
            DeSelectTool(ref timgBrush);
            DeSelectTool(ref timgRect);
            DeSelectTool(ref timgElli);
            DeSelectTool(ref timgLine);
            DeSelectTool(ref timgFill);
            DeSelectTool(ref timgText);
            selectedTool = 0;

            ShowProperties("Selection Tool - Nothing Selected!", false, false, false, false, false, false, GetCurrentColor());
        }

        private void timgCursorS_Click(object sender, EventArgs e)
        {
            DeSelectTool(ref timgCursorN);
            SelectTool(ref timgCursorS);
            DeSelectTool(ref timgPencil);
            DeSelectTool(ref timgBrush);
            DeSelectTool(ref timgRect);
            DeSelectTool(ref timgElli);
            DeSelectTool(ref timgLine);
            DeSelectTool(ref timgFill);
            DeSelectTool(ref timgText);

            selectedTool = Tool.BitmapSelection;
        }

        private void timgPencil_Click(object sender, EventArgs e)
        {
            selectedElement = null;

            DeSelectTool(ref timgCursorN);
            DeSelectTool(ref timgCursorS);
            SelectTool(ref timgPencil);
            DeSelectTool(ref timgBrush);
            DeSelectTool(ref timgRect);
            DeSelectTool(ref timgElli);
            DeSelectTool(ref timgLine);
            DeSelectTool(ref timgFill);
            DeSelectTool(ref timgText);

            selectedTool = Tool.Pencil;

            ShowProperties("Pencil Tool Settings", false, false, true, false, false, false, GetCurrentColor());
        }

        private void timgBrush_Click(object sender, EventArgs e)
        {
            selectedElement = null;

            DeSelectTool(ref timgCursorN);
            DeSelectTool(ref timgCursorS);
            DeSelectTool(ref timgPencil);
            SelectTool(ref timgBrush);
            DeSelectTool(ref timgRect);
            DeSelectTool(ref timgElli);
            DeSelectTool(ref timgLine);
            DeSelectTool(ref timgFill);
            DeSelectTool(ref timgText);

            selectedTool = Tool.Brush;

            ShowProperties("Brush Tool Settings", false, false, true, false, true, false, GetCurrentColor());
        }

        private void timgRect_Click(object sender, EventArgs e)
        {
            selectedElement = null;

            DeSelectTool(ref timgCursorN);
            DeSelectTool(ref timgCursorS);
            DeSelectTool(ref timgPencil);
            DeSelectTool(ref timgBrush);
            SelectTool(ref timgRect);
            DeSelectTool(ref timgElli);
            DeSelectTool(ref timgLine);
            DeSelectTool(ref timgFill);
            DeSelectTool(ref timgText);

            selectedTool = Tool.Rectangle;

            ShowProperties("Rectangle Tool Settings", true, true, false, true, false, false, GetCurrentColor());
        }

        private void timgElli_Click(object sender, EventArgs e)
        {
            selectedElement = null;

            DeSelectTool(ref timgCursorN);
            DeSelectTool(ref timgCursorS);
            DeSelectTool(ref timgPencil);
            DeSelectTool(ref timgBrush);
            DeSelectTool(ref timgRect);
            SelectTool(ref timgElli);
            DeSelectTool(ref timgLine);
            DeSelectTool(ref timgFill);
            DeSelectTool(ref timgText);

            selectedTool = Tool.Ellipse;

            ShowProperties("Ellipse Tool Settings", true, true, false, true, false, false, GetCurrentColor());
        }

        private void timgLine_Click(object sender, EventArgs e)
        {
            selectedElement = null;

            DeSelectTool(ref timgCursorN);
            DeSelectTool(ref timgCursorS);
            DeSelectTool(ref timgPencil);
            DeSelectTool(ref timgBrush);
            DeSelectTool(ref timgRect);
            DeSelectTool(ref timgElli);
            SelectTool(ref timgLine);
            DeSelectTool(ref timgFill);
            DeSelectTool(ref timgText);

            selectedTool = Tool.Line;

            ShowProperties("Line Tool Settings", false, false, true, false, true, false, GetCurrentColor());
        }

        private void timgFill_Click(object sender, EventArgs e)
        {
            selectedElement = null;

            DeSelectTool(ref timgCursorN);
            DeSelectTool(ref timgCursorS);
            DeSelectTool(ref timgPencil);
            DeSelectTool(ref timgBrush);
            DeSelectTool(ref timgRect);
            DeSelectTool(ref timgElli);
            DeSelectTool(ref timgLine);
            SelectTool(ref timgFill);
            DeSelectTool(ref timgText);

            selectedTool = Tool.Fill;

            ShowProperties("Fill Tool Settings", false, false, true, false, false, false, GetCurrentColor());
        }

        private void timgText_Click(object sender, EventArgs e)
        {
            selectedElement = null;

            DeSelectTool(ref timgCursorN);
            DeSelectTool(ref timgCursorS);
            DeSelectTool(ref timgPencil);
            DeSelectTool(ref timgBrush);
            DeSelectTool(ref timgRect);
            DeSelectTool(ref timgElli);
            DeSelectTool(ref timgLine);
            DeSelectTool(ref timgFill);
            SelectTool(ref timgText);

            selectedTool = Tool.Text;

            ShowProperties("Text Tool Settings", false, false, true, false, false, true, GetCurrentColor(), "");
        }
        #endregion

        /// <summary>
        /// Resizes and repositions the canvaspre.
        /// </summary>
        public void ReloadImage()
        {
            canvaspre.Width = (savedata.imageSize.Width * MagnificationLevel) + 2;
            canvaspre.Height = (savedata.imageSize.Height * MagnificationLevel) + 2;

            int ProposedLeft = appcenter.Width / 2 - canvaspre.Width / 2;
            int ProposedTop = appcenter.Height / 2 - canvaspre.Height / 2;

            canvaspre.Left = (ProposedLeft < 0) ? 0 : ProposedLeft;
            canvaspre.Top = (ProposedTop < 0) ? 0 : ProposedTop;

            //if (canvaspre.Width + canvaspre.Left > appcenter.Width) appcenter.HorizontalScroll.Visible = true;
            //else appcenter.HorizontalScroll.Visible = false;

            //if (canvaspre.Height + canvaspre.Top > appcenter.Height) appcenter.VerticalScroll.Visible = true;
            //else appcenter.VerticalScroll.Visible = false;
        }

        

        private void canvaspre_MouseMove(object sender, MouseEventArgs e)
        {
            //Point mouseLoc = new Point((int)Math.Round((decimal)((e.X - (MagnificationLevel / 2)) / MagnificationLevel)), (int)Math.Round((decimal)((e.X - (MagnificationLevel / 2)) / MagnificationLevel)));
            Point mouseLoc = new Point(e.X / MagnificationLevel, e.Y / MagnificationLevel);
            mousePoint = mouseLoc;

            if (selectedTool == 0)
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
                                int proposedWidth = (((Line)selectedElement).StartPoint.X > ((Line)selectedElement).EndPoint.X) ? ((Line)selectedElement).StartPoint.X : ((Line)selectedElement).EndPoint.X;
                                int proposedHeight = (((Line)selectedElement).StartPoint.Y > ((Line)selectedElement).EndPoint.Y) ? ((Line)selectedElement).StartPoint.Y : ((Line)selectedElement).EndPoint.Y;
                                if (proposedWidth > 0) selectedElement.Width = proposedWidth + Convert.ToInt32(string.IsNullOrEmpty(txtBThick.Text) ? "0" : txtBThick.Text);
                                if (proposedHeight > 0) selectedElement.Height = proposedHeight + Convert.ToInt32(string.IsNullOrEmpty(txtBThick.Text) ? "0" : txtBThick.Text);

                                //selectedElement.X = BeforeResizePoint.X + ((((Line)selectedElement).StartPoint.X < ((Line)selectedElement).EndPoint.X) ? ((Line)selectedElement).StartPoint.X : ((Line)selectedElement).EndPoint.X);
                                //selectedElement.Y = BeforeResizePoint.X + ((((Line)selectedElement).StartPoint.Y < ((Line)selectedElement).EndPoint.Y) ? ((Line)selectedElement).StartPoint.Y : ((Line)selectedElement).EndPoint.Y);

                                if (CornerSelected == 1) {                                   
                                    ((Line)selectedElement).StartPoint = new Point(mouseLoc.X - selectedElement.X, mouseLoc.Y - selectedElement.Y);
                                    
                                    movingRefresh.Start();
                                } else if (CornerSelected == 2) {                                 
                                    ((Line)selectedElement).EndPoint = new Point(mouseLoc.X - selectedElement.X, mouseLoc.Y - selectedElement.Y);

                                    movingRefresh.Start();
                                }
                            } else {
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

                                            movingRefresh.Start();
                                            selectedElement.Resize(selectedElement.Width, selectedElement.Height);
                                            break;
                                        case 2: // Top-right corner
                                            selectedElement.Y = mouseLoc.Y;

                                            int proposedWidth2 = ((mouseLoc.X - BeforeResizePoint.X)) + BeforeResizeSize.Width;
                                            int proposedHeight2 = ((mouseLoc.Y - BeforeResizePoint.Y) * -1) + BeforeResizeSize.Height;
                                            if (proposedWidth2 > 0) selectedElement.Width = proposedWidth2;
                                            if (proposedHeight2 > 0) selectedElement.Height = proposedHeight2;

                                            movingRefresh.Start();
                                            selectedElement.Resize(selectedElement.Width, selectedElement.Height);
                                            break;
                                        case 3: // Bottom-left corner
                                            selectedElement.X = mouseLoc.X;

                                            int proposedWidth3 = ((mouseLoc.X - BeforeResizePoint.X) * -1) + BeforeResizeSize.Width;
                                            int proposedHeight3 = ((mouseLoc.Y - BeforeResizePoint.Y)) + BeforeResizeSize.Height;
                                            if (proposedWidth3 > 0) selectedElement.Width = proposedWidth3;
                                            if (proposedHeight3 > 0) selectedElement.Height = proposedHeight3;

                                            movingRefresh.Start();
                                            selectedElement.Resize(selectedElement.Width, selectedElement.Height);
                                            break;
                                        case 4: // Bottom-right corner
                                            int proposedWidth4 = ((mouseLoc.X - BeforeResizePoint.X)) + BeforeResizeSize.Width;
                                            int proposedHeight4 = ((mouseLoc.Y - BeforeResizePoint.Y)) + BeforeResizeSize.Height;
                                            if (proposedWidth4 > 0) selectedElement.Width = proposedWidth4;
                                            if (proposedHeight4 > 0) selectedElement.Height = proposedHeight4;

                                            movingRefresh.Start();
                                            selectedElement.Resize(selectedElement.Width, selectedElement.Height);
                                            break;
                                    }
                            }
                        }

                        canvaspre.Invalidate();
                    }
                }
            }

            if (MouseDownOnCanvas)
            {
                if (currentDrawingElement is Pencil)
                {
                    grph.AddLine(lastMousePoint.X, lastMousePoint.Y, mouseLoc.X, mouseLoc.Y);

                    // We now need to use the element

                    currentDrawingGraphics.DrawPath(new Pen(Color.FromArgb(1, 0, 1)), grph);

                    if (mouseLoc.X > DrawingMax.X) DrawingMax.X = mouseLoc.X;
                    if (mouseLoc.Y > DrawingMax.Y) DrawingMax.Y = mouseLoc.Y;
                    if (mouseLoc.X < DrawingMin.X) DrawingMin.X = mouseLoc.X;
                    if (mouseLoc.Y < DrawingMin.Y) DrawingMin.Y = mouseLoc.Y;

                    canvaspre.Invalidate();
                }

                if (currentDrawingElement is Elements.Brush)
                {
                    //grph.AddLine(lastMousePoint.X, lastMousePoint.Y, mouseLoc.X, mouseLoc.Y);
                    //BrushDrawing.DrawLineOfEllipse(Convert.ToInt32(string.IsNullOrEmpty(txtBThick.Text) ? "0" : txtBThick.Text), currentDrawingGraphics, sb101, lastMousePoint.X, lastMousePoint.Y, mouseLoc.X, mouseLoc.Y);

                    BrushDrawing.DrawLineOfEllipse(Convert.ToInt32(string.IsNullOrEmpty(txtBThick.Text) ? "0" : txtBThick.Text), currentDrawingGraphics, sb101, lastMousePoint.X, lastMousePoint.Y, mouseLoc.X, mouseLoc.Y);
                    //currentDrawingGraphics.DrawPath(new Pen(Color.FromArgb(1, 0, 1)), grph);
                    //currentDrawingGraphics.DrawPath(new Pen(Color.FromArgb(1, 0, 1), Convert.ToInt32(string.IsNullOrEmpty(txtBThick.Text) ? "0" : txtBThick.Text)), grph);
                    //currentDrawingGraphics.DrawLine(new Pen(Color.FromArgb(1, 0, 1), Convert.ToInt32(string.IsNullOrEmpty(txtBThick.Text) ? "0" : txtBThick.Text)), lastMousePoint, mouseLoc);
                    //currentDrawingGraphics.FillEllipse(sb101, mouseLoc.X, mouseLoc.Y, Convert.ToInt32(string.IsNullOrEmpty(txtBThick.Text) ? "0" : txtBThick.Text), Convert.ToInt32(txtBThick.Text));

                    if (mouseLoc.X > DrawingMax.X) DrawingMax.X = mouseLoc.X;
                    if (mouseLoc.Y > DrawingMax.Y) DrawingMax.Y = mouseLoc.Y;
                    if (mouseLoc.X < DrawingMin.X) DrawingMin.X = mouseLoc.X;
                    if (mouseLoc.Y < DrawingMin.Y) DrawingMin.Y = mouseLoc.Y;

                    canvaspre.Invalidate();
                }

                if (currentDrawingElement is RectangleE || currentDrawingElement is Ellipse)
                {
                    DrawingMax.X = mouseLoc.X;
                    DrawingMax.Y = mouseLoc.Y;

                    if (mouseLoc.X < DrawingMin.X) DrawingMin.X = mouseLoc.X;
                    if (mouseLoc.Y < DrawingMin.Y) DrawingMin.Y = mouseLoc.Y;

                    canvaspre.Invalidate();
                }

                if (currentDrawingElement is Line)
                {
                    if (mouseLoc.X > DrawingMax.X) DrawingMax.X = mouseLoc.X;
                    if (mouseLoc.Y > DrawingMax.Y) DrawingMax.Y = mouseLoc.Y;
                    if (mouseLoc.X < DrawingMin.X) DrawingMin.X = mouseLoc.X;
                    if (mouseLoc.Y < DrawingMin.Y) DrawingMin.Y = mouseLoc.Y;

                    canvaspre.Invalidate();
                }

                if (currentDrawingElement is Elements.Text)
                {
                    ((Elements.Text)currentDrawingElement).X = mouseLoc.X;
                    ((Elements.Text)currentDrawingElement).Y = mouseLoc.Y;

                    canvaspre.Invalidate();
                }

                lastMousePoint = mouseLoc;
            }
        }

        private async void canvaspre_MouseDown(object sender, MouseEventArgs e)
        {
            //Point mouseLoc = new Point((int)Math.Round((decimal)((e.X - (MagnificationLevel / 2)) / MagnificationLevel)), (int)Math.Round((decimal)((e.X - (MagnificationLevel / 2)) / MagnificationLevel)));
            Point mouseLoc = new Point(e.X / MagnificationLevel, e.Y / MagnificationLevel);
            mousePoint = mouseLoc;

            if (e.Button == MouseButtons.Left)
            {
                DrawingMin.X = mouseLoc.X;
                DrawingMin.Y = mouseLoc.Y; // This fixes a bug.
                DrawingMax.X = 0;
                DrawingMax.Y = 0;

                if (selectedTool == 0)
                { // Selection tool! This one is really complex!                        

                    selectedElement = Core.selectElementByLocation(mouseLoc.X, mouseLoc.Y);

                    if (selectedElement == null) ShowProperties("Selection Tool - Nothing selected!", false, false, false, false, false, false, GetCurrentColor());
                    if (selectedElement is Pencil) ShowProperties("Selection Tool - Pencil", false, false, true, false, false, false, ((Pencil)selectedElement).pencilColor);
                    if (selectedElement is Elements.Brush) ShowProperties("Selection Tool - Brush", false, false, true, false, false, false, ((Elements.Brush)selectedElement).brushColor);
                    if (selectedElement is RectangleE) ShowProperties("Selection Tool - Rectangle", true, true, false, true, false, false, ((RectangleE)selectedElement).fillColor);
                    if (selectedElement is Ellipse) ShowProperties("Selection Tool - Ellipse", true, true, false, true, false, false, ((Ellipse)selectedElement).FillColor);
                    if (selectedElement is Line) ShowProperties("Selection Tool - Line", false, false, true, false, true, false, ((Line)selectedElement).color);
                    if (selectedElement is Fill) ShowProperties("Selection Tool - Fill", false, false, true, false, false, false, ((Fill)selectedElement).fillColor);
                    if (selectedElement is Text) ShowProperties("Selection Tool - Text", false, false, true, false, false, true, ((Text)selectedElement).clr, ((Text)selectedElement).mainText, ((Text)selectedElement).fnt);
                    // Add more

                    canvaspre.Invalidate();

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

                                movingRefresh.Start();
                                IsMoving = true;
                            }
                            else
                            {
                                if (selectedElement is Line)
                                {
                                    if (CornerSelected == 1) BeforeResizePoint = new Point(((Line)selectedElement).StartPoint.X, ((Line)selectedElement).StartPoint.Y);
                                    else if (CornerSelected == 2) BeforeResizePoint = new Point(((Line)selectedElement).EndPoint.X, ((Line)selectedElement).EndPoint.Y);
                                } else {
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

                    ((RectangleE)currentDrawingElement).IsFilled = true;
                    ((RectangleE)currentDrawingElement).borderColor = clrBord.BackColor;
                    ((RectangleE)currentDrawingElement).fillColor = clrFill.BackColor;
                    ((RectangleE)currentDrawingElement).BorderSize = Convert.ToInt32(string.IsNullOrEmpty(txtBWidth.Text) ? "0" : txtBWidth.Text);
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

                    ((Ellipse)currentDrawingElement).IsFilled = true;
                    ((Ellipse)currentDrawingElement).BorderColor = clrBord.BackColor;
                    ((Ellipse)currentDrawingElement).FillColor = clrFill.BackColor;
                    ((Ellipse)currentDrawingElement).BorderSize = Convert.ToInt32(string.IsNullOrEmpty(txtBWidth.Text) ? "0" : txtBWidth.Text);
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

                    ((Line)currentDrawingElement).color = clrNorm.BackColor;
                    ((Line)currentDrawingElement).Thickness = Convert.ToInt32(string.IsNullOrEmpty(txtBThick.Text) ? "0" : txtBThick.Text);
                }

                if (selectedTool == Tool.Fill) // I would hide this function, it's quite long because it runs async which causes all sorts of problems!
                {
                    try
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

                        lblProcess.Show();
                        fill = new Task<Bitmap>(() =>
                        {
                            return ImageFilling.SafeFloodFill(ImageFormer.ImageToByteArray(Core.PaintPreview()), mouseLoc.X, mouseLoc.Y, Color.FromArgb(1, 0, 1));
                        });

                        fill.Start();

                        ((Fill)currentDrawingElement).fillPoints = await fill;
                        ((Fill)currentDrawingElement).fillColor = clrNorm.BackColor;

                        currentDrawingElement.X = DrawingMin.X - 1; currentDrawingElement.Y = DrawingMin.Y - 1;

                        currentDrawingElement.Width = (DrawingMax.X - DrawingMin.X) + 1;
                        currentDrawingElement.Height = (DrawingMax.Y - DrawingMin.Y) + 1;
                        currentDrawingElement.zindex = topZIndex++;

                        ((Fill)currentDrawingElement).fillPoints = ImageCropping.CropImage(((Fill)currentDrawingElement).fillPoints, currentDrawingElement.X, currentDrawingElement.Y, currentDrawingElement.Width, currentDrawingElement.Height);
                        savedata.imageElements.Add(currentDrawingElement);

                        if (currentDrawingGraphics != null) currentDrawingGraphics.Dispose();
                        currentDrawingElement = null;

                        endImage = Core.PaintPreview();

                        lblProcess.Hide();
                    } catch { lblProcess.Hide(); }
                }

                if (selectedTool == Tool.Text)
                {
                    currentDrawingElement = new Elements.Text()
                    {
                        X = mouseLoc.X,
                        Y = mouseLoc.Y,
                    };

                    ((Text)currentDrawingElement).mainText = txtTText.Text;
                    ((Text)currentDrawingElement).clr = clrNorm.BackColor;

                    try
                    {
                        FontStyle bold = (BoldSelected) ? FontStyle.Bold : FontStyle.Regular;
                        FontStyle italic = (ItalicSelected) ? FontStyle.Italic : FontStyle.Regular;
                        FontStyle underline = (UnderlineSelected) ? FontStyle.Underline : FontStyle.Regular;

                        ((Text)currentDrawingElement).fnt = new Font(cmbFont.Text, Convert.ToInt32(string.IsNullOrEmpty(cmbSize.Text) ? "0" : cmbSize.Text), bold | italic | underline);
                    }
                    catch
                    {
                        cmbFont.Text = "Microsoft Sans Serif";
                        cmbSize.Text = "12";
                        ((Text)currentDrawingElement).fnt = new Font(cmbFont.Text, Convert.ToInt32(string.IsNullOrEmpty(cmbSize.Text) ? "0" : cmbSize.Text), FontStyle.Regular);
                    }

                    Size widthHeight = Elements.Text.MeasureText(txtTText.Text, ((Text)currentDrawingElement).fnt);
                    currentDrawingElement.Width = Convert.ToInt32(Math.Ceiling(widthHeight.Width + ((Text)currentDrawingElement).fnt.Size));
                    currentDrawingElement.Height = widthHeight.Height;
                }

                MouseDownOnCanvas = true;

                if (selectedTool == Tool.Fill) MouseDownOnCanvas = false; // Actually, yeah... no if you are filling!
            }
        }

        private async void canvaspre_MouseUp(object sender, MouseEventArgs e)
        {
            //Point mouseLoc = new Point((int)Math.Round((decimal)((e.X - (MagnificationLevel / 2)) / MagnificationLevel)), (int)Math.Round((decimal)((e.X - (MagnificationLevel / 2)) / MagnificationLevel)));
            Point mouseLoc = new Point(e.X / MagnificationLevel, e.Y / MagnificationLevel);

            if (MouseDownOnCanvas)
            {
                movingRefresh.Stop();
                MouseDownOnCanvas = false;

                if (selectedTool == 0)
                {
                    IsMoving = false;
                    movingRefresh.Stop();

                    if (CornerSelected != 0)
                        CornerSelected = 0;
                    //selectedElement.Width += BeforeResizeSize.Width;
                    //selectedElement.Height += BeforeResizeSize.Height;
                    //if (IsMovingSelectionInPlace)
                    //{
                    //    selectedElement.X = NewMovingX;
                    //    selectedElement.Y = NewMovingY;
                    //    selectedElement.Visible = true;

                    //    var i = savedata.imageElements.FindIndex(x => x == selectedElement);
                    //    savedata.imageElements[i] = selectedElement;

                    //    IsMovingSelectionInPlace = false;
                    //}

                }

                if (selectedTool == Tool.Pencil) grph.Reset();

                // Apply the data

                if (currentDrawingElement is Pencil)
                {
                    ((Pencil)currentDrawingElement).pencilPoints = ImageCropping.CropImage(((Pencil)currentDrawingElement).pencilPoints, DrawingMin.X, DrawingMin.Y, DrawingMax.X, DrawingMax.Y);
                    ((Pencil)currentDrawingElement).pencilColor = clrNorm.BackColor;
                }

                if (currentDrawingElement is Elements.Brush)
                {
                    ((Elements.Brush)currentDrawingElement).brushPoints = ImageCropping.CropImage(((Elements.Brush)currentDrawingElement).brushPoints, DrawingMin.X, DrawingMin.Y, DrawingMax.X + Convert.ToInt32(string.IsNullOrEmpty(txtBThick.Text) ? "0" : txtBThick.Text), DrawingMax.Y + Convert.ToInt32(string.IsNullOrEmpty(txtBThick.Text) ? "0" : txtBThick.Text));
                    ((Elements.Brush)currentDrawingElement).brushColor = clrNorm.BackColor;
                }

                if (currentDrawingElement is RectangleE || currentDrawingElement is Ellipse)
                {
                    currentDrawingElement.zindex = topZIndex++;
                    currentDrawingElement.Width = mouseLoc.X - startPoint.X + Convert.ToInt32(string.IsNullOrEmpty(txtBWidth.Text) ? "0" : txtBWidth.Text);
                    currentDrawingElement.Height = mouseLoc.Y - startPoint.Y + Convert.ToInt32(string.IsNullOrEmpty(txtBWidth.Text) ? "0" : txtBWidth.Text);
                    if (currentDrawingElement.Width < 0) currentDrawingElement.X = startPoint.X - Math.Abs(currentDrawingElement.Width); else currentDrawingElement.X = startPoint.X;
                    if (currentDrawingElement.Height < 0) currentDrawingElement.Y = startPoint.Y - Math.Abs(currentDrawingElement.Height); else currentDrawingElement.Y = startPoint.Y;
                    currentDrawingElement.Width = Math.Abs(currentDrawingElement.Width);
                    currentDrawingElement.Height = Math.Abs(currentDrawingElement.Height);

                    savedata.imageElements.Add(currentDrawingElement);
                }

                if (currentDrawingElement is Line)
                {
                    currentDrawingElement.zindex = topZIndex++;

                    int thickness = Convert.ToInt32(string.IsNullOrEmpty(txtBWidth.Text) ? "0" : txtBWidth.Text);
                    currentDrawingElement.Width = (DrawingMax.X - DrawingMin.X) + (thickness * 3);
                    currentDrawingElement.Height = (DrawingMax.Y - DrawingMin.Y) + (thickness * 3);

                    // The below code is to get the X even in minus numbers!

                    //if (width < 0) currentDrawingElement.Width = 1;
                    //if (height < 0) currentDrawingElement.Height = 1;

                    currentDrawingElement.X = DrawingMin.X - (thickness * 2);
                    currentDrawingElement.Y = DrawingMin.Y - (thickness * 2);

                    ((Line)currentDrawingElement).BeforeResizeWidth = currentDrawingElement.Width;
                    ((Line)currentDrawingElement).BeforeResizeHeight = currentDrawingElement.Height;
                    
                    ((Line)currentDrawingElement).StartPoint = new Point((startPoint.X - DrawingMin.X) + (thickness * 2), (startPoint.Y - DrawingMin.Y) + (thickness * 2));
                    ((Line)currentDrawingElement).EndPoint = new Point((mouseLoc.X - DrawingMin.X) + (thickness * 2), (mouseLoc.Y - DrawingMin.Y) + (thickness * 2));

                    ((Line)currentDrawingElement).BeforeResizeStart = ((Line)currentDrawingElement).StartPoint;
                    ((Line)currentDrawingElement).BeforeResizeEnd = ((Line)currentDrawingElement).EndPoint;

                    ((Line)currentDrawingElement).color = clrNorm.BackColor;

                    savedata.imageElements.Add(currentDrawingElement);

                    canvaspre.Invalidate();
                }

                if (currentDrawingElement is Elements.Text)
                {
                    currentDrawingElement.zindex = topZIndex++;

                    currentDrawingElement.X = e.X;
                    currentDrawingElement.Y = e.Y;

                    savedata.imageElements.Add(currentDrawingElement);
                }

                if (currentDrawingElement is Pencil || currentDrawingElement is Elements.Brush)
                {
                    // Reset everything back

                    currentDrawingElement.zindex = topZIndex++;
                    currentDrawingElement.X = DrawingMin.X;
                    currentDrawingElement.Y = DrawingMin.Y;
                    currentDrawingElement.Width = (DrawingMax.X - DrawingMin.X) + Convert.ToInt32(string.IsNullOrEmpty(txtBThick.Text) ? "0" : txtBThick.Text);
                    currentDrawingElement.Height = (DrawingMax.Y - DrawingMin.Y) + Convert.ToInt32(string.IsNullOrEmpty(txtBThick.Text) ? "0" : txtBThick.Text);

                    if (currentDrawingElement.Width < 0) currentDrawingElement.Width = 1;
                    if (currentDrawingElement.Height < 0) currentDrawingElement.Height = 1;

                    savedata.imageElements.Add(currentDrawingElement);
                }

                if (currentDrawingGraphics != null) currentDrawingGraphics.Dispose();
                currentDrawingElement = null;
            }

            tskPP = new Task<Bitmap>(Core.PaintPreview);
            tskPP.Start();

            endImage = await tskPP;

            if (selectedElement != null)
            {
                IsMovingOld = new Point(selectedElement.X, selectedElement.Y);
                canvaspre.Invalidate();
            }

            GC.Collect();
        }

        /// <summary>
        /// Shows the properties panel with the specified options.
        /// </summary>
        /// <param name="text">The text to put as the label.</param>
        /// <param name="showFColor">Show the Fill Color.</param>
        /// <param name="showBColor">Show the Border Color.</param>
        /// <param name="showColor">Show the Color.</param>
        /// <param name="showBWidth">Show the Border Size.</param>
        /// <param name="showThickness">Show the Thickness.</param>
        /// <param name="showText">Show the Text tools.</param>
        /// <param name="objectColor">The color of this object (if this is a selection)</param>
        /// <param name="Text">The text to put into the "Text" box.</param>
        /// <param name="fnt">The font to put into the "Font" box.</param>
        public void ShowProperties(string text, bool showFColor, bool showBColor, bool showColor, bool showBWidth, bool showThickness, bool showText, Color objectColor, string Text = "", Font fnt = null)
        {
            properties.Show();

            propertiesLbl.Text = text;

            // Below if the selection tool is on then set the object's color otherwise set the palette's color!

            if (showFColor)
            {
                label4.Show();
                clrFill.Show();

                clrFill.BackColor = objectColor;
            }
            else
            {
                label4.Hide();
                clrFill.Hide();
            }

            if (showBColor)
            {
                label5.Show();
                clrBord.Show();

                clrBord.BackColor = objectColor;
            }
            else
            {
                label5.Hide();
                clrBord.Hide();
            }

            if (showColor)
            {
                label6.Show();
                clrNorm.Show();

                clrNorm.BackColor = objectColor;
            }
            else
            {
                label6.Hide();
                clrNorm.Hide();
            }

            if (showBWidth)
            {
                label7.Show();
                txtBWidth.Show();
            }
            else
            {
                label7.Hide();
                txtBWidth.Hide();
            }

            if (showThickness)
            {
                label8.Show();
                txtBThick.Show();
            }
            else
            {
                label8.Hide();
                txtBThick.Hide();
            }

            if (showText)
            {
                label2.Show();
                label10.Show();
                pnlFont.Show();
                txtTText.Show();

                txtTText.Text = Text;
                if (fnt != null)
                {
                    cmbFont.Text = fnt.FontFamily.Name;
                    cmbSize.Text = fnt.Size.ToString();

                    if (fnt.Bold)
                    {
                        BoldSelected = true;
                        btnBold.BackColor = Color.Black;
                        btnBold.Image = ImageInverting.InvertImage((Bitmap)btnBold.Image);
                    }
                    else
                    {
                        BoldSelected = false;
                        btnBold.BackColor = SystemColors.Control;
                        btnBold.Image = Properties.Resources.bold;
                    }

                    if (fnt.Italic)
                    {
                        ItalicSelected = true;
                        btnItl.BackColor = Color.Black;
                        btnItl.Image = ImageInverting.InvertImage((Bitmap)btnItl.Image);
                    }
                    else
                    {
                        ItalicSelected = false;
                        btnItl.BackColor = SystemColors.Control;
                        btnItl.Image = Properties.Resources.italic;
                    }

                    if (fnt.Underline)
                    {
                        UnderlineSelected = true;
                        btnUline.BackColor = Color.Black;
                        btnUline.Image = ImageInverting.InvertImage((Bitmap)btnUline.Image);
                    }
                    else
                    {
                        UnderlineSelected = false;
                        btnUline.BackColor = SystemColors.Control;
                        btnUline.Image = Properties.Resources.underline;
                    }
                }
            }
            else
            {
                label2.Hide();
                label10.Hide();
                pnlFont.Hide();
                txtTText.Hide();
            }
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            Task<Bitmap> tskPP = new Task<Bitmap>(Core.PaintPreview);
            tskPP.Start();

            endImage = await tskPP;
        }

        private void canvaspre_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;
            e.Graphics.ScaleTransform(MagnificationLevel, MagnificationLevel); // Transform anything drawn to the zoom!

            try { e.Graphics.DrawImage(endImage, 0, 0); } catch { }

            //try { e.Graphics.DrawImage(canvaspre.Image, 0, 0, canvaspre.Width, canvaspre.Height); } catch { }
            //if (canvaspre.Image != null) canvaspre.Image = null;

            // This is to preview what you are drawing!

            if (MouseDownOnCanvas)
            {
                if (currentDrawingElement is Pencil)
                    e.Graphics.DrawPath(new Pen(clrNorm.BackColor), grph);

                if (currentDrawingElement is Elements.Brush)
                    e.Graphics.DrawImage(BrushDrawing.ChangeImageColor(((Elements.Brush)currentDrawingElement).brushPoints, clrNorm.BackColor), 0, 0);

                if (currentDrawingElement is RectangleE)
                {
                    // Now let's draw this rectangle!

                    RectangleE ele = ((RectangleE)currentDrawingElement);
                    int width = (mousePoint.X - startPoint.X);
                    int height = (mousePoint.Y - startPoint.Y);

                    int heightamount = 0;
                    int widthamount = 0;
                    if (width < 0)
                        widthamount = Math.Abs(width);
                    else
                        widthamount = 0;

                    if (height < 0)
                        heightamount = Math.Abs(height);
                    else
                        heightamount = 0;

                    //if (width < 0) currentDrawingElement.Width = 1;
                    //if (height < 0) currentDrawingElement.Height = 1;

                    int borderSize = Convert.ToInt32(string.IsNullOrEmpty(txtBWidth.Text) ? "0" : txtBWidth.Text);

                    if (ele.IsFilled) e.Graphics.FillRectangle(new SolidBrush(ele.fillColor), startPoint.X - widthamount + (borderSize / 2), startPoint.Y - heightamount + (borderSize / 2), Math.Abs(width), Math.Abs(height)); // Fill

                    e.Graphics.DrawRectangle(new Pen(ele.borderColor, borderSize), startPoint.X - widthamount + (borderSize / 2), startPoint.Y - heightamount + (borderSize / 2), Math.Abs(width), Math.Abs(height));
                    //e.Graphics.FillRectangle(new SolidBrush(ele.borderColor), DrawingMin.X, DrawingMin.Y, ele.BorderSize, height); // Left border
                    //e.Graphics.FillRectangle(new SolidBrush(ele.borderColor), DrawingMin.X, DrawingMin.Y, width, ele.BorderSize); // Top border
                    //e.Graphics.FillRectangle(new SolidBrush(ele.borderColor), (ele.Width - ele.BorderSize) + DrawingMin.X, DrawingMin.Y, ele.BorderSize, Height); // Right border
                    //e.Graphics.FillRectangle(new SolidBrush(ele.borderColor), DrawingMin.X, (ele.Height - ele.BorderSize) + DrawingMin.Y, ele.Width, ele.BorderSize); // Bottom border
                }

                if (currentDrawingElement is Ellipse)
                {
                    // Now let's draw this ellipse! and yes this is pratically the same code as the rectangle one - both of them use the same code for things

                    Ellipse ele = ((Ellipse)currentDrawingElement);
                    int width = (mousePoint.X - startPoint.X);
                    int height = (mousePoint.Y - startPoint.Y);

                    int heightamount = 0;
                    int widthamount = 0;
                    if (width < 0)
                        widthamount = Math.Abs(width);
                    else
                        widthamount = 0;

                    if (height < 0)
                        heightamount = Math.Abs(height);
                    else
                        heightamount = 0;

                    //if (width < 0) currentDrawingElement.Width = 1;
                    //if (height < 0) currentDrawingElement.Height = 1;

                    int borderSize = Convert.ToInt32(string.IsNullOrEmpty(txtBWidth.Text) ? "0" : txtBWidth.Text);
                    if (ele.IsFilled) e.Graphics.FillEllipse(new SolidBrush(ele.FillColor), startPoint.X - widthamount + (borderSize / 2), startPoint.Y - heightamount + (borderSize / 2), Math.Abs(width), Math.Abs(height)); // Fill

                    e.Graphics.DrawEllipse(new Pen(ele.BorderColor, borderSize), startPoint.X - widthamount + (borderSize / 2), startPoint.Y - heightamount + (borderSize / 2), Math.Abs(width), Math.Abs(height));
                    //e.Graphics.FillRectangle(new SolidBrush(ele.borderColor), DrawingMin.X, DrawingMin.Y, ele.BorderSize, height); // Left border
                    //e.Graphics.FillRectangle(new SolidBrush(ele.borderColor), DrawingMin.X, DrawingMin.Y, width, ele.BorderSize); // Top border
                    //e.Graphics.FillRectangle(new SolidBrush(ele.borderColor), (ele.Width - ele.BorderSize) + DrawingMin.X, DrawingMin.Y, ele.BorderSize, Height); // Right border
                    //e.Graphics.FillRectangle(new SolidBrush(ele.borderColor), DrawingMin.X, (ele.Height - ele.BorderSize) + DrawingMin.Y, ele.Width, ele.BorderSize); // Bottom border
                }

                if (currentDrawingElement is Line)
                    e.Graphics.DrawLine(new Pen(clrNorm.BackColor, Convert.ToInt32(string.IsNullOrEmpty(txtBThick.Text) ? "0" : txtBThick.Text)), startPoint.X, startPoint.Y, mousePoint.X, mousePoint.Y);

                if (currentDrawingElement is Elements.Text)
                    e.Graphics.DrawString(((Elements.Text)currentDrawingElement).mainText, ((Elements.Text)currentDrawingElement).fnt, new SolidBrush(((Elements.Text)currentDrawingElement).clr), mousePoint.X, mousePoint.Y);
            }

            // ...or to draw the overlay of the selection tool!

            if (selectedElement != null)
            {
                // Check if it hasn't moved

                if (selectedElement.X == IsMovingOld.X && selectedElement.Y == IsMovingOld.Y)
                {
                    int width = Math.Abs(selectedElement.Width);
                    int height = Math.Abs(selectedElement.Height);

                    int heightamount = 0;
                    int widthamount = 0;
                    if (width < 0)
                        widthamount = Math.Abs(width);
                    else
                        widthamount = 0;

                    if (height < 0)
                        heightamount = Math.Abs(height);
                    else
                        heightamount = 0;

                    e.Graphics.DrawRectangle(new Pen(Color.Gray), selectedElement.X - widthamount, selectedElement.Y - heightamount, width, height);

                    // The points for scaling

                    if (IsOnSelection)
                    {
                        if (selectedElement is Line)
                        {
                            if (CornerSelected == 1) e.Graphics.FillEllipse(new SolidBrush(Color.Gray), ((Line)selectedElement).StartPoint.X + selectedElement.X - (10 / MagnificationLevel), ((Line)selectedElement).StartPoint.Y + selectedElement.Y - (10 / MagnificationLevel), 20 / MagnificationLevel, 20 / MagnificationLevel); else e.Graphics.DrawEllipse(new Pen(Color.Gray), ((Line)selectedElement).StartPoint.X + selectedElement.X - (10 / MagnificationLevel), ((Line)selectedElement).StartPoint.Y + selectedElement.Y - (10 / MagnificationLevel), 20 / MagnificationLevel, 20 / MagnificationLevel);
                            if (CornerSelected == 2) e.Graphics.FillEllipse(new SolidBrush(Color.Gray), ((Line)selectedElement).EndPoint.X + selectedElement.X - (10 / MagnificationLevel), ((Line)selectedElement).EndPoint.Y + selectedElement.Y - (10 / MagnificationLevel), 20 / MagnificationLevel, 20 / MagnificationLevel); else e.Graphics.DrawEllipse(new Pen(Color.Gray), ((Line)selectedElement).EndPoint.X + selectedElement.X - (10 / MagnificationLevel), ((Line)selectedElement).EndPoint.Y + selectedElement.Y - (10 / MagnificationLevel), 20 / MagnificationLevel, 20 / MagnificationLevel);
                        } else {
                            if (!(selectedElement is Pencil) && !(selectedElement is Elements.Brush) && !(selectedElement is Fill))
                            {
                                if (CornerSelected == 1) e.Graphics.FillEllipse(new SolidBrush(Color.Gray), selectedElement.X - widthamount - (10 / MagnificationLevel), selectedElement.Y - heightamount - (10 / MagnificationLevel), 20 / MagnificationLevel, 20 / MagnificationLevel); else e.Graphics.DrawEllipse(new Pen(Color.Gray), selectedElement.X - widthamount - (10 / MagnificationLevel), selectedElement.Y - heightamount - (10 / MagnificationLevel), 20 / MagnificationLevel, 20 / MagnificationLevel);
                                if (CornerSelected == 2) e.Graphics.FillEllipse(new SolidBrush(Color.Gray), ((selectedElement.X - widthamount) + selectedElement.Width) - (10 / MagnificationLevel), selectedElement.Y - heightamount - (10 / MagnificationLevel), 20 / MagnificationLevel, 20 / MagnificationLevel); else e.Graphics.DrawEllipse(new Pen(Color.Gray), ((selectedElement.X - widthamount) + selectedElement.Width) - (10 / MagnificationLevel), selectedElement.Y - heightamount - (10 / MagnificationLevel), 20 / MagnificationLevel, 20 / MagnificationLevel);
                                if (CornerSelected == 3) e.Graphics.FillEllipse(new SolidBrush(Color.Gray), selectedElement.X - widthamount - (10 / MagnificationLevel), ((selectedElement.Y - heightamount) + selectedElement.Height) - (10 / MagnificationLevel), 20 / MagnificationLevel, 20 / MagnificationLevel); else e.Graphics.DrawEllipse(new Pen(Color.Gray), selectedElement.X - widthamount - (10 / MagnificationLevel), ((selectedElement.Y - heightamount) + selectedElement.Height) - (10 / MagnificationLevel), 20 / MagnificationLevel, 20 / MagnificationLevel);
                                if (CornerSelected == 4) e.Graphics.FillEllipse(new SolidBrush(Color.Gray), ((selectedElement.X - widthamount) + selectedElement.Width) - (10 / MagnificationLevel), ((selectedElement.Y - heightamount) + selectedElement.Height) - (10 / MagnificationLevel), 20 / MagnificationLevel, 20 / MagnificationLevel); else e.Graphics.DrawEllipse(new Pen(Color.Gray), ((selectedElement.X - widthamount) + selectedElement.Width) - (10 / MagnificationLevel), ((selectedElement.Y - heightamount) + selectedElement.Height) - (10 / MagnificationLevel), 20 / MagnificationLevel, 20 / MagnificationLevel);
                            }
                        }                        
                    }
                }
            }
        }

        #region Properties Panel
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // Redraw the avalible colours

            if (selectedPalette == 0)
                selectedPalette = 1;

            cl1.Invalidate();
            cl2.Invalidate();
            cl3.Invalidate();
            cl4.Invalidate();

            cl5.Invalidate();
            cl6.Invalidate();
            cl7.Invalidate();
            cl8.Invalidate();
        }

        public Color GetCurrentColor()
        {
            Color ret = Color.Black;

            switch (selectedPalette)
            {
                case 0:
                    ret = Color.Black;
                    break;
                case 1:
                    ret = cl1.BackColor;
                    break;
                case 2:
                    ret = cl2.BackColor;
                    break;
                case 3:
                    ret = cl3.BackColor;
                    break;
                case 4:
                    ret = cl4.BackColor;
                    break;
                case 5:
                    ret = cl5.BackColor;
                    break;
                case 6:
                    ret = cl6.BackColor;
                    break;
                case 7:
                    ret = cl7.BackColor;
                    break;
                case 8:
                    ret = cl8.BackColor;
                    break;
            }

            return ret;
        }

        private async void clrNorm_MouseClick(object sender, MouseEventArgs e)
        {
            clrNorm.BackColor = GetCurrentColor();

            if (selectedElement != null)
            {
                if (selectedElement is Elements.Brush) ((Elements.Brush)selectedElement).brushColor = clrNorm.BackColor;
                if (selectedElement is Elements.Pencil) ((Elements.Pencil)selectedElement).pencilColor = clrNorm.BackColor;
                if (selectedElement is Elements.Line) ((Elements.Line)selectedElement).color = clrNorm.BackColor;
                if (selectedElement is Elements.Fill) ((Elements.Fill)selectedElement).fillColor = clrNorm.BackColor;
                if (selectedElement is Elements.Text) ((Elements.Text)selectedElement).clr = clrNorm.BackColor;

                Task<Bitmap> tskPP = new Task<Bitmap>(Core.PaintPreview);
                tskPP.Start();

                endImage = await tskPP;
            }
        }

        private async void clrFill_MouseClick(object sender, MouseEventArgs e)
        {
            clrFill.BackColor = GetCurrentColor();

            if (selectedElement != null)
            {
                if (selectedElement is RectangleE) ((RectangleE)selectedElement).fillColor = clrFill.BackColor;
                if (selectedElement is Ellipse) ((Ellipse)selectedElement).FillColor = clrFill.BackColor;

                Task<Bitmap> tskPP = new Task<Bitmap>(Core.PaintPreview);
                tskPP.Start();

                endImage = await tskPP;
            }
        }

        private async void clrBord_MouseClick(object sender, MouseEventArgs e)
        {
            clrBord.BackColor = GetCurrentColor();

            if (selectedElement != null)
            {
                if (selectedElement is RectangleE) ((RectangleE)selectedElement).borderColor = clrBord.BackColor;
                if (selectedElement is Ellipse) ((Ellipse)selectedElement).BorderColor = clrBord.BackColor;

                Task<Bitmap> tskPP = new Task<Bitmap>(Core.PaintPreview);
                tskPP.Start();

                endImage = await tskPP;
            }
        }

        private async void txtBWidth_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBWidth.Text))
                txtBWidth.Text = "0";

            if (selectedElement != null)
            {
                if (selectedElement is RectangleE) ((RectangleE)selectedElement).BorderSize = Convert.ToInt32(string.IsNullOrEmpty(txtBWidth.Text) ? "0" : txtBWidth.Text);
                if (selectedElement is Ellipse) ((Ellipse)selectedElement).BorderSize = Convert.ToInt32(string.IsNullOrEmpty(txtBWidth.Text) ? "0" : txtBWidth.Text);

                Task<Bitmap> tskPP = new Task<Bitmap>(Core.PaintPreview);
                tskPP.Start();

                endImage = await tskPP;
            }
        }

        private async void btnBold_Click(object sender, EventArgs e)
        {
            if (selectedElement != null)
            {
                Font currentFont = ((Text)selectedElement).fnt;
                if (selectedElement is Text)
                {
                    if (((Text)selectedElement).fnt.Bold)
                        ((Text)selectedElement).fnt = new Font(currentFont.FontFamily, currentFont.Size, currentFont.Style & ~FontStyle.Bold);
                    else
                        ((Text)selectedElement).fnt = new Font(currentFont.FontFamily, currentFont.Size, FontStyle.Bold | currentFont.Style);
                }
                Task<Bitmap> tskPP = new Task<Bitmap>(Core.PaintPreview);
                tskPP.Start();

                endImage = await tskPP;
            }

            if (BoldSelected)
            {
                BoldSelected = false;
                btnBold.BackColor = SystemColors.Control;
                btnBold.Image = Properties.Resources.bold;
            }
            else
            {
                BoldSelected = true;
                btnBold.BackColor = Color.Black;
                btnBold.Image = ImageInverting.InvertImage((Bitmap)btnBold.Image);
            }
        }

        private async void txtTText_TextChanged(object sender, EventArgs e)
        {
            if (selectedElement != null)
                ((Text)selectedElement).mainText = txtTText.Text;

            Task<Bitmap> tskPP = new Task<Bitmap>(Core.PaintPreview);
            tskPP.Start();

            endImage = await tskPP;
        }

        private async void btnItl_Click(object sender, EventArgs e)
        {
            if (selectedElement != null)
            {
                Font currentFont = ((Text)selectedElement).fnt;
                if (selectedElement is Text)
                {
                    if (((Text)selectedElement).fnt.Italic)
                        ((Text)selectedElement).fnt = new Font(currentFont.FontFamily, currentFont.Size, currentFont.Style & ~FontStyle.Italic);
                    else
                        ((Text)selectedElement).fnt = new Font(currentFont.FontFamily, currentFont.Size, FontStyle.Italic | currentFont.Style);
                }
                Task<Bitmap> tskPP = new Task<Bitmap>(Core.PaintPreview);
                tskPP.Start();

                endImage = await tskPP;
            }

            if (ItalicSelected)
            {
                ItalicSelected = false;
                btnItl.BackColor = SystemColors.Control;
                btnItl.Image = Properties.Resources.italic;
            }
            else
            {
                ItalicSelected = true;
                btnItl.BackColor = Color.Black;
                btnItl.Image = ImageInverting.InvertImage((Bitmap)btnItl.Image);
            }
        }

        private async void btnUline_Click(object sender, EventArgs e)
        {
            if (selectedElement != null)
            {
                Font currentFont = ((Text)selectedElement).fnt;
                if (selectedElement is Text)
                {
                    if (((Text)selectedElement).fnt.Underline)
                        ((Text)selectedElement).fnt = new Font(currentFont.FontFamily, currentFont.Size, currentFont.Style & ~FontStyle.Underline);
                    else
                        ((Text)selectedElement).fnt = new Font(currentFont.FontFamily, currentFont.Size, FontStyle.Underline | currentFont.Style);
                }
                Task<Bitmap> tskPP = new Task<Bitmap>(Core.PaintPreview);
                tskPP.Start();

                endImage = await tskPP;
            }

            if (UnderlineSelected)
            {
                UnderlineSelected = false;
                btnUline.BackColor = SystemColors.Control;
                btnUline.Image = Properties.Resources.underline;
            }
            else
            {
                UnderlineSelected = true;
                btnUline.BackColor = Color.Black;
                btnUline.Image = ImageInverting.InvertImage((Bitmap)btnUline.Image);
            }
        }

        private async void cmbFont_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectedElement != null)
                if (selectedElement is Text)
                    try
                    {
                        ((Text)selectedElement).fnt = new Font(cmbFont.Text, ((Text)selectedElement).fnt.Size, ((Text)selectedElement).fnt.Style);
                    }
                    catch
                    {
                        cmbFont.Text = "Microsoft Sans Serif";
                        ((Text)selectedElement).fnt = new Font(cmbFont.Text, ((Text)selectedElement).fnt.Size, ((Text)selectedElement).fnt.Style);
                    }

            Task<Bitmap> tskPP = new Task<Bitmap>(Core.PaintPreview);
            tskPP.Start();

            endImage = await tskPP;
        }

        private async void cmbSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectedElement != null)
                if (selectedElement is Text)
                    try
                    {
                        ((Text)selectedElement).fnt = new Font(((Text)selectedElement).fnt.FontFamily, float.Parse(cmbSize.Text), ((Text)selectedElement).fnt.Style);

                        SizeF TextSize = Elements.Text.MeasureText(((Text)selectedElement).mainText, ((Text)selectedElement).fnt);
                        ((Text)selectedElement).Width = (int)Math.Round(TextSize.Width);
                        ((Text)selectedElement).Height = (int)Math.Round(TextSize.Height);
                    }
                    catch
                    {
                        cmbSize.Text = "12";
                        ((Text)selectedElement).fnt = new Font(((Text)selectedElement).fnt.FontFamily, float.Parse(cmbSize.Text), ((Text)selectedElement).fnt.Style);

                        SizeF TextSize = Elements.Text.MeasureText(((Text)selectedElement).mainText, ((Text)selectedElement).fnt);
                        ((Text)selectedElement).Width = (int)Math.Round(TextSize.Width);
                        ((Text)selectedElement).Height = (int)Math.Round(TextSize.Height);
                    }

            Task<Bitmap> tskPP = new Task<Bitmap>(Core.PaintPreview);
            tskPP.Start();

            endImage = await tskPP;
        }

        private async void txtBThick_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBThick.Text))
                txtBThick.Text = "0";

            if (selectedElement != null)
            {
                if (selectedElement is Elements.Brush) ((Elements.Brush)selectedElement).Width = Convert.ToInt32(string.IsNullOrEmpty(txtBThick.Text) ? "0" : txtBThick.Text);
                if (selectedElement is Line)
                {
                    int beforeThickness = ((Line)selectedElement).Thickness;
                    ((Line)selectedElement).Thickness = Convert.ToInt32(string.IsNullOrEmpty(txtBThick.Text) ? "0" : txtBThick.Text);
                    selectedElement.Width += ((Line)selectedElement).Thickness - beforeThickness;
                    selectedElement.Height += ((Line)selectedElement).Thickness - beforeThickness;
                }

                Task<Bitmap> tskPP = new Task<Bitmap>(Core.PaintPreview);
                tskPP.Start();

                endImage = await tskPP;
            }
        }
        #endregion

        private void movingRefresh_Tick(object sender, EventArgs e)
        {
            endImage = Core.PaintPreview();
        }

        private void CanvasAnywhereClick(object sender, EventArgs e)
        {
            welcomeScreen.Hide();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            ReloadImage();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            (new Windows.About()).Show();
        }

        private void zoomDown_Click(object sender, EventArgs e)
        {
            
        }

        private void zoomUp_Click(object sender, EventArgs e)
        {
            Core.HandleZoomIn();
        }

        #region MenuStrip
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sizer sz = new Sizer();
            sz.ShowDialog();

            if (!sz.Cancelled)
            {
                savedata.imageSize = sz.returnSize;

                ReloadImage();

                savedata.imageElements = new List<Element>();

                Core.PaintPreview();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialogOPEN.ShowDialog() == DialogResult.OK)
                LoadFile(openFileDialogOPEN.FileName);

            ReloadImage();
            Core.PaintPreview();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialogSAVE.ShowDialog() == DialogResult.OK)
                SaveFile(saveFileDialogSAVE.FileName);

            ReloadImage();
            Core.PaintPreview();
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialogIMPORT.ShowDialog() == DialogResult.OK)
                ImportFile(openFileDialogIMPORT.FileName);

            ReloadImage();
            Core.PaintPreview();
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialogEXPORT.ShowDialog() == DialogResult.OK)
                ExportFile(saveFileDialogEXPORT.FileName);

            ReloadImage();
            Core.PaintPreview();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentFile == "")
                if (saveFileDialogSAVE.ShowDialog() == DialogResult.OK)
                    SaveFile(saveFileDialogSAVE.FileName);
                else
                    SaveFile(currentFile);

            ReloadImage();
            Core.PaintPreview();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Core.HandleCut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Core.HandleCopy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Core.HandlePaste();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Core.HandleDelete();
        }
        #endregion

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Core.HandleKeyPress(keyData);
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void redrawToolStripMenuItem_Click(object sender, EventArgs e)
        {
            endImage = Core.PaintPreview();
        }
    }
}