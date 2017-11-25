using ABPaint.Elements;
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

namespace ABPaint
{
    public partial class Form1 : Form
    {
        // General Variables
        public int selectedTool = 0;
        public int topZIndex = 0;
        public int selectedPalette = 1;
        public Element currentDrawingElement;
        public Graphics currentDrawingGraphics;
        public bool MouseDownOnCanvas = false;
        Task<Image> tskPP = null;

        public Point mousePoint = new Point(0, 0);

        // The image + Extra SolidBrush
        public Bitmap endImage;
        public Size imageSize = new Size(800, 600);
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

        // Text
        public bool BoldSelected = false;
        public bool ItalicSelected = false;
        public bool UnderlineSelected = false;

        public List<Element> imageElements = new List<Element>();

        public Form1()
        {
            InitializeComponent();
            ReloadImage();
        }

        public static Bitmap InvertImage(Bitmap bitmapImage)
        {
            var bitmapRead = bitmapImage.LockBits(new Rectangle(0, 0, bitmapImage.Width, bitmapImage.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppPArgb);
            var bitmapLength = bitmapRead.Stride * bitmapRead.Height;
            var bitmapBGRA = new byte[bitmapLength];
            Marshal.Copy(bitmapRead.Scan0, bitmapBGRA, 0, bitmapLength);
            bitmapImage.UnlockBits(bitmapRead);

            for (int i = 0; i < bitmapLength; i += 4)
            {
                bitmapBGRA[i] = (byte)(255 - bitmapBGRA[i]);
                bitmapBGRA[i + 1] = (byte)(255 - bitmapBGRA[i + 1]);
                bitmapBGRA[i + 2] = (byte)(255 - bitmapBGRA[i + 2]);
                //        [i + 3] = ALPHA.
            }

            var bitmapWrite = bitmapImage.LockBits(new Rectangle(0, 0, bitmapImage.Width, bitmapImage.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
            Marshal.Copy(bitmapBGRA, 0, bitmapWrite.Scan0, bitmapLength);
            bitmapImage.UnlockBits(bitmapWrite);

            return bitmapImage;
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

            imageElements = new List<Element>();
        }

        public void SelectTool(ref PictureBox toSelect)
        {
            toSelect.BackColor = Color.Black;
            try
            {
                if (toSelect.Tag.ToString() != "selected") // If it isn't selected already
                    toSelect.Image = InvertImage((Bitmap)toSelect.Image);
            }
            catch { toSelect.Image = InvertImage((Bitmap)toSelect.Image); }
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
                    toSelect.Image = InvertImage((Bitmap)toSelect.Image);
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

            selectedTool = 1;
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

            selectedTool = 2;

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

            selectedTool = 3;

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

            selectedTool = 4;

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

            selectedTool = 5;

            ShowProperties("Oval Tool Settings", true, true, false, true, false, false, GetCurrentColor());
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

            selectedTool = 6;

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

            selectedTool = 7;

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

            selectedTool = 8;

            ShowProperties("Text Tool Settings", false, false, true, false, false, true, GetCurrentColor(), "");
        }
        #endregion
        private void picNew_Click(object sender, EventArgs e)
        {
            Sizer sz = new Sizer();
            sz.ShowDialog();

            imageSize = sz.returnSize;

            ReloadImage();

            imageElements = new List<Element>();

            PaintPreview();
        }

        void ReloadImage()
        {
            canvaspre.Width = (imageSize.Width * MagnificationLevel) + 2;
            canvaspre.Height = (imageSize.Height * MagnificationLevel) + 2;
            canvaspre.Left = appcenter.Width / 2 - canvaspre.Width / 2;
            canvaspre.Top = appcenter.Height / 2 - canvaspre.Height / 2;

            //if (canvaspre.Width + canvaspre.Left > appcenter.Width) appcenter.HorizontalScroll.Visible = true;
            //else appcenter.HorizontalScroll.Visible = false;

            //if (canvaspre.Height + canvaspre.Top > appcenter.Height) appcenter.VerticalScroll.Visible = true;
            //else appcenter.VerticalScroll.Visible = false;
        }

        public Image PaintPreview()
        {
            //try
            //{
            // Draw the elements in order

            Bitmap endResult = new Bitmap(imageSize.Width, imageSize.Height);
            Graphics g = Graphics.FromImage(endResult);

            // Order them by zindex:
            imageElements = imageElements.OrderBy(o => o.zindex).ToList();

            // Now draw them all!

            foreach (Element ele in imageElements)
            {
                if (ele.Visible)
                {
                    Bitmap bmp = ele.ProcessImage();
                    g.DrawImage(bmp, ele.X, ele.Y);
                    bmp.Dispose();
                }
            }

            endImage = endResult;
            return endResult;
            //} catch { return endImage; }
        }

        public Element selectElementByLocation(int x, int y)
        {
            Element ret = null;
            // Order the list based on zindex!
            imageElements = imageElements.OrderBy(o => o.zindex).ToList();

            foreach (Element ele in imageElements)
            {
                if (new Rectangle(ele.X - 10, ele.Y - 10, ele.Width + 20, ele.Height + 20).Contains(new Point(x, y)))
                {
                    // The mouse is in this element!

                    ele.zindex = topZIndex++; // Brings to front

                    ret = ele;

                    continue;
                }
            }

            return ret;
        }

        private void canvaspre_MouseMove(object sender, MouseEventArgs e)
        {
            Point mouseLoc = e.Location;
            if (selectedTool == 0)
            {
                if (selectedElement != null)
                {
                    if (IsMoving)
                    {
                        selectedElement.X = e.Location.X - IsMovingGap.Width;
                        selectedElement.Y = e.Location.Y - IsMovingGap.Height;
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

                        canvaspre.Invalidate();
                    }
                }
            }

            if (MouseDownOnCanvas)
            {
                mousePoint = new Point(e.Location.X, e.Location.Y);

                if (currentDrawingElement is Pencil)
                {
                    grph.AddLine(lastMousePoint.X, lastMousePoint.Y, mousePoint.X, mousePoint.Y);

                    // We now need to use the element

                    currentDrawingGraphics.DrawPath(new Pen(Color.FromArgb(1, 0, 1)), grph);

                    if (mousePoint.X > DrawingMax.X) DrawingMax.X = mousePoint.X;
                    if (mousePoint.Y > DrawingMax.Y) DrawingMax.Y = mousePoint.Y;
                    if (mousePoint.X < DrawingMin.X) DrawingMin.X = mousePoint.X;
                    if (mousePoint.Y < DrawingMin.Y) DrawingMin.Y = mousePoint.Y;

                    canvaspre.Invalidate();
                }

                if (currentDrawingElement is Elements.Brush)
                {
                    currentDrawingGraphics.FillEllipse(sb101, mousePoint.X, mousePoint.Y, Convert.ToInt32(txtBThick.Text), Convert.ToInt32(txtBThick.Text));

                    if (mousePoint.X > DrawingMax.X) DrawingMax.X = mousePoint.X;
                    if (mousePoint.Y > DrawingMax.Y) DrawingMax.Y = mousePoint.Y;
                    if (mousePoint.X < DrawingMin.X) DrawingMin.X = mousePoint.X;
                    if (mousePoint.Y < DrawingMin.Y) DrawingMin.Y = mousePoint.Y;

                    canvaspre.Invalidate();
                }

                if (currentDrawingElement is RectangleE || currentDrawingElement is Ellipse)
                {
                    DrawingMax.X = mousePoint.X;
                    DrawingMax.Y = mousePoint.Y;

                    if (mousePoint.X < DrawingMin.X) DrawingMin.X = mousePoint.X;
                    if (mousePoint.Y < DrawingMin.Y) DrawingMin.Y = mousePoint.Y;

                    canvaspre.Invalidate();
                }

                if (currentDrawingElement is Line)
                {
                    if (mousePoint.X > DrawingMax.X) DrawingMax.X = mousePoint.X;
                    if (mousePoint.Y > DrawingMax.Y) DrawingMax.Y = mousePoint.Y;
                    if (mousePoint.X < DrawingMin.X) DrawingMin.X = mousePoint.X;
                    if (mousePoint.Y < DrawingMin.Y) DrawingMin.Y = mousePoint.Y;

                    canvaspre.Invalidate();
                }

                if (currentDrawingElement is Elements.Text)
                {
                    ((Elements.Text)currentDrawingElement).X = mousePoint.X;
                    ((Elements.Text)currentDrawingElement).Y = mousePoint.Y;

                    canvaspre.Invalidate();
                }

                lastMousePoint = mousePoint;
            }
        }

        private async void canvaspre_MouseDown(object sender, MouseEventArgs e)
        {
            Point mouseLoc = e.Location;

            if (e.Button == MouseButtons.Left)
            {
                DrawingMin.X = e.Location.X;
                DrawingMin.Y = e.Location.Y; // This fixes a bug.
                DrawingMax.X = 0;
                DrawingMax.Y = 0;

                if (selectedTool == 0)
                { // Selection tool! This one is really complex!                        

                    selectedElement = selectElementByLocation(e.Location.X, e.Location.Y);

                    if (selectedElement == null) ShowProperties("Selection Tool - Nothing selected!", false, false, false, false, false, false, GetCurrentColor());
                    if (selectedElement is Pencil) ShowProperties("Selection Tool - Pencil", false, false, true, false, false, false, ((Pencil)selectedElement).pencilColor);
                    if (selectedElement is Elements.Brush) ShowProperties("Selection Tool - Brush", false, false, true, false, false, false, ((Elements.Brush)selectedElement).brushColor);
                    if (selectedElement is RectangleE) ShowProperties("Selection Tool - Rectangle", true, true, false, true, false, false, ((RectangleE)selectedElement).fillColor);
                    if (selectedElement is Ellipse) ShowProperties("Selection Tool - Ellipse", true, true, false, true, false, false, ((Ellipse)selectedElement).fillColor);
                    if (selectedElement is Line) ShowProperties("Selection Tool - Line", false, false, true, false, true, false, ((Line)selectedElement).color);
                    if (selectedElement is Fill) ShowProperties("Selection Tool - Fill", false, false, true, false, false, false, ((Fill)selectedElement).fillColor);
                    if (selectedElement is Text) ShowProperties("Selection Tool - Text", false, false, true, false, false, true, ((Text)selectedElement).clr, ((Text)selectedElement).mainText, ((Text)selectedElement).fnt);
                    // Add more

                    canvaspre.Invalidate();

                    if (selectedElement != null)
                    {
                        if (new Rectangle(selectedElement.X - 10, selectedElement.Y - 10, selectedElement.Width + 20, selectedElement.Height + 20).Contains(mouseLoc))
                        {
                            // The mouse is in this element!


                            // Check if the mouse is on the scaling points otherwise move the element.

                            CornerSelected = 0;

                            // Top left corner
                            if (new Rectangle(selectedElement.X - 10, selectedElement.Y - 10, 20, 20).Contains(mouseLoc)) CornerSelected = 1;

                            // Top Right Corner
                            if (new Rectangle(selectedElement.Right - 10, selectedElement.Y - 10, 20, 20).Contains(mouseLoc)) CornerSelected = 2;

                            // Bottom Left corner
                            if (new Rectangle(selectedElement.X - 10, selectedElement.Bottom - 10, 20, 20).Contains(mouseLoc)) CornerSelected = 3;

                            // Bottom Right corner
                            if (new Rectangle(selectedElement.Right - 10, selectedElement.Bottom - 10, 20, 20).Contains(mouseLoc)) CornerSelected = 4;

                            if (CornerSelected == 0)
                            {
                                // Move the element

                                IsMovingOld = new Point(selectedElement.X, selectedElement.Y);

                                IsMovingGap.Width = e.X - selectedElement.X;
                                IsMovingGap.Height = e.Y - selectedElement.Y;

                                movingRefresh.Start();
                                IsMoving = true;
                            }
                            else
                            {
                                if (CornerSelected == 1) BeforeResizePoint = new Point(selectedElement.X, selectedElement.Y);
                                else if (CornerSelected == 2) BeforeResizePoint = new Point(selectedElement.X + selectedElement.Width, selectedElement.Y);
                                else if (CornerSelected == 3) BeforeResizePoint = new Point(selectedElement.X, selectedElement.Y + selectedElement.Height);
                                else if (CornerSelected == 4) BeforeResizePoint = new Point(selectedElement.X + selectedElement.Width, selectedElement.Y + selectedElement.Height);

                                BeforeResizeSize = new Size(selectedElement.Width, selectedElement.Height);
                            }

                        }
                    }
                }

                if (selectedTool == 2)
                { // Pencil
                    lastMousePoint = new Point(e.Location.X, e.Location.Y);

                    currentDrawingElement = new Pencil()
                    {
                        Width = imageSize.Width,
                        Height = imageSize.Height
                    };

                    grph.StartFigure();
                    ((Pencil)currentDrawingElement).pencilPoints = new Bitmap(currentDrawingElement.Width, currentDrawingElement.Height);
                    currentDrawingGraphics = Graphics.FromImage(((Pencil)currentDrawingElement).pencilPoints);

                    currentDrawingGraphics.Clear(Color.Transparent);
                }

                if (selectedTool == 3) // Brush
                {
                    lastMousePoint = new Point(e.Location.X, e.Location.Y);

                    currentDrawingElement = new Elements.Brush()
                    {
                        Width = imageSize.Width,
                        Height = imageSize.Height
                    };

                    ((Elements.Brush)currentDrawingElement).brushPoints = new Bitmap(currentDrawingElement.Width, currentDrawingElement.Height);

                    currentDrawingGraphics = Graphics.FromImage(((Elements.Brush)currentDrawingElement).brushPoints);

                    currentDrawingGraphics.Clear(Color.Transparent);
                }

                if (selectedTool == 4) // Rectangle
                {

                    currentDrawingElement = new RectangleE()
                    {
                        Width = imageSize.Width,
                        Height = imageSize.Height
                    };

                    DrawingMin.X = e.X;
                    DrawingMin.Y = e.Y;

                    startPoint = e.Location;

                    ((RectangleE)currentDrawingElement).IsFilled = true;
                    ((RectangleE)currentDrawingElement).borderColor = clrBord.BackColor;
                    ((RectangleE)currentDrawingElement).fillColor = clrFill.BackColor;
                    ((RectangleE)currentDrawingElement).BorderSize = Convert.ToInt32(txtBWidth.Text);
                }

                if (selectedTool == 5) // Ellipse
                {

                    currentDrawingElement = new Ellipse()
                    {
                        Width = imageSize.Width,
                        Height = imageSize.Height
                    };

                    DrawingMin.X = e.X;
                    DrawingMin.Y = e.Y;

                    startPoint = e.Location;

                    ((Ellipse)currentDrawingElement).IsFilled = true;
                    ((Ellipse)currentDrawingElement).borderColor = clrBord.BackColor;
                    ((Ellipse)currentDrawingElement).fillColor = clrFill.BackColor;
                    ((Ellipse)currentDrawingElement).BorderSize = Convert.ToInt32(txtBWidth.Text);
                }

                if (selectedTool == 6) // Line
                {

                    currentDrawingElement = new Line()
                    {
                        Width = imageSize.Width,
                        Height = imageSize.Height
                    };

                    DrawingMin.X = e.X;
                    DrawingMin.Y = e.Y;

                    startPoint = e.Location;

                    ((Line)currentDrawingElement).color = clrNorm.BackColor;
                    ((Line)currentDrawingElement).Thickness = Convert.ToInt32(txtBThick.Text);
                }

                if (selectedTool == 7) // Fill - I would hide this function, it's quite long because it runs async which causes all sorts of problems!
                {
                    startPoint = new Point(e.Location.X, e.Location.Y);

                    currentDrawingElement = new Fill()
                    {
                        X = e.X,
                        Y = e.Y,
                        Width = imageSize.Width,
                        Height = imageSize.Height
                    };

                    DrawingMin.X = e.X;
                    DrawingMin.Y = e.Y;
                    DrawingMax.X = e.X;
                    DrawingMax.Y = e.Y;

                    lblProcess.Show();
                    fill = new Task<Bitmap>(() =>
                    {
                        return ImageFilling.SafeFloodFill((Bitmap)PaintPreview(), e.X, e.Y, Color.FromArgb(1, 0, 1));
                    });

                    fill.Start();

                    ((Fill)currentDrawingElement).fillPoints = await fill;
                    ((Fill)currentDrawingElement).fillColor = clrNorm.BackColor;

                    currentDrawingElement.X = DrawingMin.X - 1; currentDrawingElement.Y = DrawingMin.Y - 1;

                    currentDrawingElement.Width = (DrawingMax.X - DrawingMin.X) + 1;
                    currentDrawingElement.Height = (DrawingMax.Y - DrawingMin.Y) + 1;
                    currentDrawingElement.zindex = topZIndex++;

                    ((Fill)currentDrawingElement).fillPoints = ImageCropping.CropImage(((Fill)currentDrawingElement).fillPoints, currentDrawingElement.X, currentDrawingElement.Y, currentDrawingElement.Width, currentDrawingElement.Height);
                    imageElements.Add(currentDrawingElement);

                    if (currentDrawingGraphics != null) currentDrawingGraphics.Dispose();
                    currentDrawingElement = null;

                    canvaspre.Image = PaintPreview();

                    lblProcess.Hide();
                }

                if (selectedTool == 8) // Text
                {
                    currentDrawingElement = new Elements.Text()
                    {
                        X = e.X,
                        Y = e.Y,
                    };

                    ((Text)currentDrawingElement).mainText = txtTText.Text;
                    ((Text)currentDrawingElement).clr = clrNorm.BackColor;

                    try
                    {
                        FontStyle bold = (BoldSelected) ? FontStyle.Bold : FontStyle.Regular;
                        FontStyle italic = (ItalicSelected) ? FontStyle.Italic : FontStyle.Regular;
                        FontStyle underline = (UnderlineSelected) ? FontStyle.Underline : FontStyle.Regular;

                        ((Text)currentDrawingElement).fnt = new Font(cmbFont.Text, Convert.ToInt32(cmbSize.Text), bold | italic | underline);
                    }
                    catch
                    {
                        cmbFont.Text = "Microsoft Sans Serif";
                        cmbSize.Text = "12";
                        ((Text)currentDrawingElement).fnt = new Font(cmbFont.Text, Convert.ToInt32(cmbSize.Text), FontStyle.Regular);
                    }

                    Size widthHeight = Elements.Text.MeasureText(txtTText.Text, ((Text)currentDrawingElement).fnt);
                    currentDrawingElement.Width = Convert.ToInt32(Math.Ceiling(widthHeight.Width + ((Text)currentDrawingElement).fnt.Size));
                    currentDrawingElement.Height = widthHeight.Height;
                }

                MouseDownOnCanvas = true;

                if (selectedTool == 7) MouseDownOnCanvas = false; // Actually, yeah... no if you are filling!
            }
        }

        private async void canvaspre_MouseUp(object sender, MouseEventArgs e)
        {
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

                    //    var i = imageElements.FindIndex(x => x == selectedElement);
                    //    imageElements[i] = selectedElement;

                    //    IsMovingSelectionInPlace = false;
                    //}

                }

                if (selectedTool == 2) grph.Reset();

                // Apply the data

                if (currentDrawingElement is Pencil)
                {
                    ((Pencil)currentDrawingElement).pencilPoints = ImageCropping.CropImage(((Pencil)currentDrawingElement).pencilPoints, DrawingMin.X, DrawingMin.Y, DrawingMax.X, DrawingMax.Y);
                    ((Pencil)currentDrawingElement).pencilColor = clrNorm.BackColor;
                }

                if (currentDrawingElement is Elements.Brush)
                {
                    ((Elements.Brush)currentDrawingElement).brushPoints = ImageCropping.CropImage(((Elements.Brush)currentDrawingElement).brushPoints, DrawingMin.X, DrawingMin.Y, DrawingMax.X + Convert.ToInt32(txtBThick.Text), DrawingMax.Y + Convert.ToInt32(txtBThick.Text));
                    ((Elements.Brush)currentDrawingElement).brushColor = clrNorm.BackColor;
                }

                if (currentDrawingElement is RectangleE || currentDrawingElement is Ellipse)
                {
                    currentDrawingElement.zindex = topZIndex++;
                    currentDrawingElement.Width = mousePoint.X - startPoint.X;
                    currentDrawingElement.Height = mousePoint.Y - startPoint.Y;
                    if (currentDrawingElement.Width < 0) currentDrawingElement.X = startPoint.X - Math.Abs(currentDrawingElement.Width); else currentDrawingElement.X = startPoint.X;
                    if (currentDrawingElement.Height < 0) currentDrawingElement.Y = startPoint.Y - Math.Abs(currentDrawingElement.Height); else currentDrawingElement.Y = startPoint.Y;
                    currentDrawingElement.Width = Math.Abs(currentDrawingElement.Width);
                    currentDrawingElement.Height = Math.Abs(currentDrawingElement.Height);

                    imageElements.Add(currentDrawingElement);
                }

                if (currentDrawingElement is Line)
                {
                    currentDrawingElement.zindex = topZIndex++;

                    currentDrawingElement.Width = (DrawingMax.X - DrawingMin.X) + (Convert.ToInt32(txtBThick.Text) * 3);
                    currentDrawingElement.Height = (DrawingMax.Y - DrawingMin.Y) + (Convert.ToInt32(txtBThick.Text) * 3);

                    // The below code is to get the X even in minus numbers!

                    //if (width < 0) currentDrawingElement.Width = 1;
                    //if (height < 0) currentDrawingElement.Height = 1;

                    currentDrawingElement.X = DrawingMin.X - (Convert.ToInt32(txtBThick.Text) * 2);
                    currentDrawingElement.Y = DrawingMin.Y - (Convert.ToInt32(txtBThick.Text) * 2);

                    ((Line)currentDrawingElement).StartPoint = new Point((startPoint.X - DrawingMin.X) + (Convert.ToInt32(txtBThick.Text) * 2), (startPoint.Y - DrawingMin.Y) + (Convert.ToInt32(txtBThick.Text) * 2));
                    ((Line)currentDrawingElement).EndPoint = new Point((mousePoint.X - DrawingMin.X) + (Convert.ToInt32(txtBThick.Text) * 2), (mousePoint.Y - DrawingMin.Y) + (Convert.ToInt32(txtBThick.Text) * 2));
                    ((Line)currentDrawingElement).color = clrNorm.BackColor;

                    imageElements.Add(currentDrawingElement);

                    canvaspre.Invalidate();
                }

                if (currentDrawingElement is Elements.Text)
                {
                    currentDrawingElement.zindex = topZIndex++;

                    currentDrawingElement.X = e.X;
                    currentDrawingElement.Y = e.Y;

                    imageElements.Add(currentDrawingElement);
                }

                if (selectedTool == 2 || selectedTool == 3)
                {
                    // Reset everything back

                    currentDrawingElement.zindex = topZIndex++;
                    currentDrawingElement.X = DrawingMin.X;
                    currentDrawingElement.Y = DrawingMin.Y;
                    currentDrawingElement.Width = (DrawingMax.X - DrawingMin.X) + Convert.ToInt32(txtBThick.Text);
                    currentDrawingElement.Height = (DrawingMax.Y - DrawingMin.Y) + Convert.ToInt32(txtBThick.Text);

                    if (currentDrawingElement.Width < 0) currentDrawingElement.Width = 1;
                    if (currentDrawingElement.Height < 0) currentDrawingElement.Height = 1;

                    imageElements.Add(currentDrawingElement);
                }

                if (currentDrawingGraphics != null) currentDrawingGraphics.Dispose();
                currentDrawingElement = null;
            }

            tskPP = new Task<Image>(PaintPreview);
            tskPP.Start();

            canvaspre.Image = await tskPP;

            if (selectedElement != null)
            {
                IsMovingOld = new Point(selectedElement.X, selectedElement.Y);
                canvaspre.Invalidate();
            }

            GC.Collect();
        }

        

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
                        btnBold.Image = InvertImage((Bitmap)btnBold.Image);
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
                        btnItl.Image = InvertImage((Bitmap)btnItl.Image);
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
                        btnUline.Image = InvertImage((Bitmap)btnUline.Image);
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
            Task<Image> tskPP = new Task<Image>(PaintPreview);
            tskPP.Start();

            canvaspre.Image = await tskPP;
        }

        private void canvaspre_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            // This is to preview what you are drawing!

            if (MouseDownOnCanvas)
            {
                if (currentDrawingElement is Pencil)
                {
                    e.Graphics.DrawPath(new Pen(Color.FromArgb(0, 0, 0)), grph);
                }

                if (currentDrawingElement is Elements.Brush)
                {
                    e.Graphics.DrawImage(((Elements.Brush)currentDrawingElement).brushPoints, 0, 0);
                }

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

                    if (ele.IsFilled) e.Graphics.FillRectangle(new SolidBrush(ele.fillColor), startPoint.X - widthamount, startPoint.Y - heightamount, Math.Abs(width), Math.Abs(height)); // Fill

                    e.Graphics.DrawRectangle(new Pen(ele.borderColor, borderSize), startPoint.X - widthamount, startPoint.Y - heightamount, Math.Abs(width), Math.Abs(height));
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
                    if (ele.IsFilled) e.Graphics.FillEllipse(new SolidBrush(ele.fillColor), startPoint.X - widthamount + borderSize, startPoint.Y - heightamount + borderSize, Math.Abs(width) - (borderSize * 2), Math.Abs(height) - (borderSize * 2)); // Fill

                    e.Graphics.DrawEllipse(new Pen(ele.borderColor, borderSize), startPoint.X - widthamount + borderSize, startPoint.Y - heightamount + borderSize, Math.Abs(width) - (borderSize * 2), Math.Abs(height) - (borderSize * 2));
                    //e.Graphics.FillRectangle(new SolidBrush(ele.borderColor), DrawingMin.X, DrawingMin.Y, ele.BorderSize, height); // Left border
                    //e.Graphics.FillRectangle(new SolidBrush(ele.borderColor), DrawingMin.X, DrawingMin.Y, width, ele.BorderSize); // Top border
                    //e.Graphics.FillRectangle(new SolidBrush(ele.borderColor), (ele.Width - ele.BorderSize) + DrawingMin.X, DrawingMin.Y, ele.BorderSize, Height); // Right border
                    //e.Graphics.FillRectangle(new SolidBrush(ele.borderColor), DrawingMin.X, (ele.Height - ele.BorderSize) + DrawingMin.Y, ele.Width, ele.BorderSize); // Bottom border
                }

                if (currentDrawingElement is Line)
                    e.Graphics.DrawLine(new Pen(clrNorm.BackColor, Convert.ToInt32(txtBThick.Text)), startPoint.X, startPoint.Y, mousePoint.X, mousePoint.Y);

                if (currentDrawingElement is Elements.Text)
                    e.Graphics.DrawString(((Elements.Text)currentDrawingElement).mainText, ((Elements.Text)currentDrawingElement).fnt, new SolidBrush(((Elements.Text)currentDrawingElement).clr), mousePoint.X, mousePoint.Y);
            }

            // ...or to draw the overlay of the selection tool!

            if (selectedElement != null)
            {
                // Check if it's moved

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
                        if (CornerSelected == 1) e.Graphics.FillEllipse(new SolidBrush(Color.Gray), selectedElement.X - widthamount - 10, selectedElement.Y - heightamount - 10, 20, 20); else e.Graphics.DrawEllipse(new Pen(Color.Gray), selectedElement.X - widthamount - 10, selectedElement.Y - heightamount - 10, 20, 20);
                        if (CornerSelected == 2) e.Graphics.FillEllipse(new SolidBrush(Color.Gray), ((selectedElement.X - widthamount) + selectedElement.Width) - 10, selectedElement.Y - heightamount - 10, 20, 20); else e.Graphics.DrawEllipse(new Pen(Color.Gray), ((selectedElement.X - widthamount) + selectedElement.Width) - 10, selectedElement.Y - heightamount - 10, 20, 20);
                        if (CornerSelected == 3) e.Graphics.FillEllipse(new SolidBrush(Color.Gray), selectedElement.X - widthamount - 10, ((selectedElement.Y - heightamount) + selectedElement.Height) - 10, 20, 20); else e.Graphics.DrawEllipse(new Pen(Color.Gray), selectedElement.X - widthamount - 10, ((selectedElement.Y - heightamount) + selectedElement.Height) - 10, 20, 20);
                        if (CornerSelected == 4) e.Graphics.FillEllipse(new SolidBrush(Color.Gray), ((selectedElement.X - widthamount) + selectedElement.Width) - 10, ((selectedElement.Y - heightamount) + selectedElement.Height) - 10, 20, 20); else e.Graphics.DrawEllipse(new Pen(Color.Gray), ((selectedElement.X - widthamount) + selectedElement.Width) - 10, ((selectedElement.Y - heightamount) + selectedElement.Height) - 10, 20, 20);
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

                Task<Image> tskPP = new Task<Image>(PaintPreview);
                tskPP.Start();

                canvaspre.Image = await tskPP;
            }
        }

        private async void clrFill_MouseClick(object sender, MouseEventArgs e)
        {
            clrFill.BackColor = GetCurrentColor();

            if (selectedElement != null)
            {
                if (selectedElement is RectangleE) ((RectangleE)selectedElement).fillColor = clrFill.BackColor;
                if (selectedElement is Ellipse) ((Ellipse)selectedElement).fillColor = clrFill.BackColor;

                Task<Image> tskPP = new Task<Image>(PaintPreview);
                tskPP.Start();

                canvaspre.Image = await tskPP;
            }
        }

        private async void clrBord_MouseClick(object sender, MouseEventArgs e)
        {
            clrBord.BackColor = GetCurrentColor();

            if (selectedElement != null)
            {
                if (selectedElement is RectangleE) ((RectangleE)selectedElement).borderColor = clrBord.BackColor;
                if (selectedElement is Ellipse) ((Ellipse)selectedElement).borderColor = clrBord.BackColor;

                Task<Image> tskPP = new Task<Image>(PaintPreview);
                tskPP.Start();

                canvaspre.Image = await tskPP;
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

                Task<Image> tskPP = new Task<Image>(PaintPreview);
                tskPP.Start();

                canvaspre.Image = await tskPP;
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
                Task<Image> tskPP = new Task<Image>(PaintPreview);
                tskPP.Start();

                canvaspre.Image = await tskPP;
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
                btnBold.Image = InvertImage((Bitmap)btnBold.Image);
            }
        }

        private void txtTText_TextChanged(object sender, EventArgs e)
        {
            if (selectedElement != null)
                ((Text)selectedElement).mainText = txtTText.Text;
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
                Task<Image> tskPP = new Task<Image>(PaintPreview);
                tskPP.Start();

                canvaspre.Image = await tskPP;
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
                btnItl.Image = InvertImage((Bitmap)btnItl.Image);
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
                Task<Image> tskPP = new Task<Image>(PaintPreview);
                tskPP.Start();

                canvaspre.Image = await tskPP;
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
                btnUline.Image = InvertImage((Bitmap)btnUline.Image);
            }
        }

        private void cmbFont_SelectedIndexChanged(object sender, EventArgs e)
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
        }

        private void cmbSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (selectedElement != null)
                if (selectedElement is Text)
                    try
                    {
                        ((Text)selectedElement).fnt = new Font(((Text)selectedElement).fnt.FontFamily, float.Parse(cmbSize.Text), ((Text)selectedElement).fnt.Style);
                    }
                    catch
                    {
                        cmbSize.Text = "12";
                        ((Text)selectedElement).fnt = new Font(((Text)selectedElement).fnt.FontFamily, float.Parse(cmbSize.Text), ((Text)selectedElement).fnt.Style);
                    }
        }
        #endregion

        private void movingRefresh_Tick(object sender, EventArgs e)
        {
            canvaspre.Image = PaintPreview();
        }

        private async void txtBThick_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBThick.Text))
                txtBThick.Text = "0";

            if (selectedElement != null)
            {
                if (selectedElement is Elements.Brush) ((Elements.Brush)selectedElement).Width = Convert.ToInt32(txtBThick.Text);
                if (selectedElement is Line) ((Line)selectedElement).Thickness = Convert.ToInt32(txtBThick.Text);

                Task<Image> tskPP = new Task<Image>(PaintPreview);
                tskPP.Start();

                canvaspre.Image = await tskPP;
            }
        }

        private void CanvasAnywhereClick(object sender, EventArgs e)
        {
            welcomeScreen.Hide();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Delete:
                    if (selectedElement != null)
                    {
                        if (selectedTool == 0)
                        {
                            imageElements.Remove(selectedElement);
                            selectedElement = null;
                            selectElementByLocation(0, 0); // Deselects everything.
                            canvaspre.Invalidate();
                            canvaspre.Image = PaintPreview();
                        }
                    }

                    break;
            }
        }

        

        private void Form1_Resize(object sender, EventArgs e)
        {
            ReloadImage();
        }
    }
}