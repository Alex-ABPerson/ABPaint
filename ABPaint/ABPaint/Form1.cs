using ABPaint.Elements;
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

        public Point mousePoint = new Point(0, 0);

        // The image + Extra SolidBrush
        public Bitmap endImage;
        public Size imageSize = new Size(800, 600);
        public SolidBrush sb101 = new SolidBrush(Color.FromArgb(1, 0, 1));

        // Magnification level.
        public int MagnificationLevel = 1;

        // All the things needed for pencil + brush + mouseup
        public System.Drawing.Drawing2D.GraphicsPath grph = new System.Drawing.Drawing2D.GraphicsPath();
        public Element currentDrawingElement;
        public Graphics currentDrawingGraphics;
        public Point DrawingMin;
        public Point DrawingMax;
        public bool MouseDownOnCanvas = false;
        public Point lastMousePoint;
        public Point startPoint;
        Task<Image> tskPP = null;
        public int LastX;
        public int LastY;

        // Fill?
        Task<Bitmap> fill;
        // The Selection tool stuff

        public Element selectedElement;
        public Size IsMovingGap;
        public Point IsMovingOld;
        public bool IsMoving = false;

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
                bool intersectsX = false;
                bool intersectsY = false;

                if (x > ele.X) if (x < ele.X + Math.Abs(ele.Width)) intersectsX = true;
                if (y > ele.Y) if (y < ele.Y + Math.Abs(ele.Height)) intersectsY = true;
                if (intersectsX && intersectsY)
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
            if (selectedTool == 0)
            {
                if (selectedElement != null)
                {
                    if (IsMoving)
                    {
                        selectedElement.X = e.Location.X - IsMovingGap.Width;
                        selectedElement.Y = e.Location.Y - IsMovingGap.Height;

                        if (selectedElement is RectangleE)
                        {
                            ((RectangleE)selectedElement).OriginalX = e.Location.X - IsMovingGap.Width;
                            ((RectangleE)selectedElement).OriginalY = e.Location.Y - IsMovingGap.Height;
                        }

                        if (selectedElement is Ellipse)
                        {
                            ((Ellipse)selectedElement).OriginalX = e.Location.X - IsMovingGap.Width;
                            ((Ellipse)selectedElement).OriginalY = e.Location.Y - IsMovingGap.Height;
                        }
                    }
                }
            }

            if (MouseDownOnCanvas) {
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
                    // Add more

                    canvaspre.Invalidate();

                    if (selectedElement != null)
                    {
                        bool intersectsX = false;
                        bool intersectsY = false;

                        if (e.X > selectedElement.X) if (e.X < selectedElement.X + Math.Abs(selectedElement.Width)) intersectsX = true;
                        if (e.Y > selectedElement.Y) if (e.Y < selectedElement.Y + Math.Abs(selectedElement.Height)) intersectsY = true;
                        if (intersectsX && intersectsY)
                        {
                            // The mouse is in this element!

                            // Move the element

                            IsMovingOld = new Point(selectedElement.X, selectedElement.Y);

                            IsMovingGap.Width = e.X - selectedElement.X;
                            IsMovingGap.Height = e.Y - selectedElement.Y;

                            if (selectedElement is RectangleE)
                            {
                                IsMovingGap.Width = e.X - ((RectangleE)selectedElement).OriginalX;
                                IsMovingGap.Height = e.Y - ((RectangleE)selectedElement).OriginalY;
                            }

                            if (selectedElement is Ellipse)
                            {
                                IsMovingGap.Width = e.X - ((Ellipse)selectedElement).OriginalX;
                                IsMovingGap.Height = e.Y - ((Ellipse)selectedElement).OriginalY;
                            }

                            movingRefresh.Start();
                            IsMoving = true;
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
                    fill = new Task<Bitmap>(() => {
                        return SafeFloodFill((Bitmap)PaintPreview(), e.X, e.Y, Color.FromArgb(1, 0, 1));                       
                    });

                    fill.Start();

                    ((Fill)currentDrawingElement).fillPoints = await fill;
                    ((Fill)currentDrawingElement).fillColor = clrNorm.BackColor;

                    currentDrawingElement.X = DrawingMin.X - 1; currentDrawingElement.Y = DrawingMin.Y - 1;

                    currentDrawingElement.Width = (DrawingMax.X - DrawingMin.X) + 1;
                    currentDrawingElement.Height = (DrawingMax.Y - DrawingMin.Y) + 1;
                    currentDrawingElement.zindex = topZIndex++;

                    ((Fill)currentDrawingElement).fillPoints = CropImage(((Fill)currentDrawingElement).fillPoints, currentDrawingElement.X, currentDrawingElement.Y, currentDrawingElement.Width, currentDrawingElement.Height);
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

                    try {
                        ((Text)currentDrawingElement).fnt = new Font(cmbFont.Text, Convert.ToInt32(cmbSize.Text), FontStyle.Regular);
                    } catch {
                        cmbFont.Text = "Microsoft Sans Serif";
                        cmbSize.Text = "12";
                        ((Text)currentDrawingElement).fnt = new Font(cmbFont.Text, Convert.ToInt32(cmbSize.Text), FontStyle.Regular);
                    }

                    Size widthHeight = Elements.Text.MeasureText(txtTText.Text, ((Text)currentDrawingElement).fnt);
                    currentDrawingElement.Width = widthHeight.Width;
                    currentDrawingElement.Height = widthHeight.Height;

                    if (widthHeight.Width == 0) widthHeight.Width = 1;
                    if (widthHeight.Height == 0) widthHeight.Height = 1;
                }

                MouseDownOnCanvas = true;

                if (selectedTool == 7) MouseDownOnCanvas = false; // Actually, yeah... no if you are filling!
            }
        }

        private async void canvaspre_MouseUp(object sender, MouseEventArgs e)
        {
            if (MouseDownOnCanvas)
            {
                MouseDownOnCanvas = false;

                if (selectedTool == 0)
                {
                    IsMoving = false;
                    movingRefresh.Stop();
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
                    ((Pencil)currentDrawingElement).pencilPoints = CropImage(((Pencil)currentDrawingElement).pencilPoints, DrawingMin.X, DrawingMin.Y, DrawingMax.X, DrawingMax.Y);
                    ((Pencil)currentDrawingElement).pencilColor = clrNorm.BackColor;                   
                }

                if (currentDrawingElement is Elements.Brush)
                {
                    ((Elements.Brush)currentDrawingElement).brushPoints = CropImage(((Elements.Brush)currentDrawingElement).brushPoints, DrawingMin.X, DrawingMin.Y, DrawingMax.X + Convert.ToInt32(txtBThick.Text), DrawingMax.Y + Convert.ToInt32(txtBThick.Text));
                    ((Elements.Brush)currentDrawingElement).brushColor = clrNorm.BackColor;
                }

                if (currentDrawingElement is RectangleE)
                {
                    currentDrawingElement.zindex = topZIndex++;
                    ((RectangleE)currentDrawingElement).OriginalX = startPoint.X;
                    ((RectangleE)currentDrawingElement).OriginalY = startPoint.Y;
                    currentDrawingElement.X = startPoint.X;
                    currentDrawingElement.Y = startPoint.Y;
                    currentDrawingElement.Width = (mousePoint.X - startPoint.X);
                    currentDrawingElement.Height = (mousePoint.Y - startPoint.Y);

                    imageElements.Add(currentDrawingElement);
                }

                if (currentDrawingElement is Ellipse)
                {
                    currentDrawingElement.zindex = topZIndex++;
                    ((Ellipse)currentDrawingElement).OriginalX = startPoint.X;
                    ((Ellipse)currentDrawingElement).OriginalY = startPoint.Y;
                    currentDrawingElement.X = startPoint.X;
                    currentDrawingElement.Y = startPoint.Y;
                    currentDrawingElement.Width = (mousePoint.X - startPoint.X) + (Convert.ToInt32(txtBThick.Text) * 2);
                    currentDrawingElement.Height = (mousePoint.Y - startPoint.Y) + (Convert.ToInt32(txtBThick.Text) * 2);

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

        public static Bitmap CropImage(Image source, int x, int y, int width, int height)
        {
            Rectangle crop = new Rectangle(x, y, width, height);

            if (crop.Width == 0)
                crop.Width = 1;
            if (crop.Height == 0) crop.Height = 1;

            var bmp = new Bitmap(crop.Width, crop.Height);
            using (var gr = Graphics.FromImage(bmp))
            {
                gr.DrawImage(source, new Rectangle(0, 0, bmp.Width, bmp.Height), crop, GraphicsUnit.Pixel);
            }
            return bmp;
        }

        public Bitmap CropToContent(Bitmap oldBmp)
        {
            try
            {
                Rectangle currentRect = new Rectangle();
                bool IsFirstOne = true;

                // Get a base color

                for (int y = 0; y < oldBmp.Height; y++)
                {
                    for (int x = 0; x < oldBmp.Width; x++)
                    {
                        Color debug = oldBmp.GetPixel(x, y);
                        if (oldBmp.GetPixel(x, y) != Color.Transparent)
                        {
                            // We need to interpret this!

                            // Check if it is the first one!

                            if (IsFirstOne)
                            {
                                currentRect.X = x;
                                currentRect.Y = y;
                                currentRect.Width = 1;
                                currentRect.Height = 1;
                                IsFirstOne = false;
                            }
                            else
                            {

                                if (!currentRect.Contains(new Point(x, y)))
                                {
                                    // This will run if this is out of the current rectangle

                                    if (x > (currentRect.X + currentRect.Width)) currentRect.Width = x - currentRect.X;
                                    if (x < (currentRect.X))
                                    {
                                        // Move the rectangle over there and extend it's width to make the right the same!
                                        int oldRectLeft = currentRect.Left;

                                        currentRect.X = x;
                                        currentRect.Width += oldRectLeft - x;
                                    }

                                    if (y > (currentRect.Y + currentRect.Height)) currentRect.Height = y - currentRect.Y;

                                    if (y < (currentRect.Y + currentRect.Height))
                                    {
                                        int oldRectTop = currentRect.Top;

                                        currentRect.Y = y;
                                        currentRect.Height += oldRectTop - y;
                                    }
                                }
                            }
                        }
                    }
                }
                LastX = currentRect.X;
                LastY = currentRect.Y;
                return CropImage(oldBmp, currentRect.X, currentRect.Y, currentRect.Width, currentRect.Height);
            } catch { return oldBmp; }
        }

        public void ShowProperties(string text, bool showFColor, bool showBColor, bool showColor, bool showBWidth, bool showThickness, bool showText, Color objectColor, string Text = "")
        {
            properties.Show();

            propertiesLbl.Text = text;

            // Below if the selection tool is on then set the object's color otherwise set the palette's color!

            if (showFColor)
            {
                label4.Show();
                clrFill.Show();

                clrFill.BackColor = objectColor;
            } else {
                label4.Hide();
                clrFill.Hide();
            }

            if (showBColor)
            {
                label5.Show();
                clrBord.Show();

                clrBord.BackColor = objectColor;
            } else {
                label5.Hide();
                clrBord.Hide();
            }

            if (showColor)
            {
                label6.Show();
                clrNorm.Show();

                clrNorm.BackColor = objectColor;
            } else {
                label6.Hide();
                clrNorm.Hide();
            }

            if (showBWidth)
            {
                label7.Show();
                txtBWidth.Show();
            } else {
                label7.Hide();
                txtBWidth.Hide();
            }

            if (showThickness)
            {
                label8.Show();
                txtBThick.Show();
            } else {
                label8.Hide();
                txtBThick.Hide();
            }

            if (showText)
            {
                label2.Show();
                label10.Show();
                pnlFont.Show();
                txtTText.Show();
            } else {
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

                    if (ele.IsFilled) e.Graphics.FillRectangle(new SolidBrush(ele.fillColor), startPoint.X - widthamount, startPoint.Y - heightamount, Math.Abs(width), Math.Abs(height)); // Fill

                    e.Graphics.DrawRectangle(new Pen(ele.borderColor, Convert.ToInt32(txtBWidth.Text)), startPoint.X - widthamount, startPoint.Y - heightamount, Math.Abs(width), Math.Abs(height));
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

                    if (ele.IsFilled) e.Graphics.FillEllipse(new SolidBrush(ele.fillColor), startPoint.X - widthamount, startPoint.Y - heightamount, Math.Abs(width) - (Convert.ToInt32(txtBWidth.Text) * 2), Math.Abs(height) - (Convert.ToInt32(string.IsNullOrEmpty(txtBWidth.Text) ? "0" : txtBWidth.Text) * 2)); // Fill

                    e.Graphics.DrawEllipse(new Pen(ele.borderColor, Convert.ToInt32(string.IsNullOrEmpty(txtBWidth.Text) ? "0" : txtBWidth.Text)), startPoint.X - widthamount, startPoint.Y - heightamount, Math.Abs(width) - (Convert.ToInt32(string.IsNullOrEmpty(txtBWidth.Text) ? "0" : txtBWidth.Text) * 2), Math.Abs(height) - (Convert.ToInt32(string.IsNullOrEmpty(txtBWidth.Text) ? "0" : txtBWidth.Text) * 2));
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

            if (selectedElement != null) {
                // Check if it's moved

                if (selectedElement.X == IsMovingOld.X && selectedElement.Y == IsMovingOld.Y) {
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
                }          
            }
        }

        #region Properties Panel
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // Redraw the avalible colours

            if (selectedPalette == 0)
                selectedPalette = 1;

            clL1.Invalidate();
            clL2.Invalidate();
            clL3.Invalidate();
            clL4.Invalidate();

            clR1.Invalidate();
            clR2.Invalidate();
            clR3.Invalidate();
            clR4.Invalidate();
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
                    ret = clL1.BackColor;
                    break;
                case 2:
                    ret = clL2.BackColor;
                    break;
                case 3:
                    ret = clL3.BackColor;
                    break;
                case 4:
                    ret = clL4.BackColor;
                    break;
                case 5:
                    ret = clR1.BackColor;
                    break;
                case 6:
                    ret = clR2.BackColor;
                    break;
                case 7:
                    ret = clR3.BackColor;
                    break;
                case 8:
                    ret = clR4.BackColor;
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

        private void clL1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                selectedPalette = 1;
            }
            else
            {
                if (colorDialog1.ShowDialog() == DialogResult.OK)
                {
                    clL1.BackColor = colorDialog1.Color;
                }
            }
            this.Invalidate();
        }

        private void clL2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                selectedPalette = 2;
            }
            else
            {
                if (colorDialog1.ShowDialog() == DialogResult.OK)
                {
                    clL2.BackColor = colorDialog1.Color;
                }
            }
            this.Invalidate();
        }

        private void clL3_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                selectedPalette = 3;
            }
            else
            {
                if (colorDialog1.ShowDialog() == DialogResult.OK)
                {
                    clL3.BackColor = colorDialog1.Color;
                }
            }
            this.Invalidate();
        }

        private void clL4_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                selectedPalette = 4;
            }
            else
            {
                if (colorDialog1.ShowDialog() == DialogResult.OK)
                {
                    clL4.BackColor = colorDialog1.Color;
                }
            }
            this.Invalidate();
        }

        private void clR1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                selectedPalette = 5;
            }
            else
            {
                if (colorDialog1.ShowDialog() == DialogResult.OK)
                {
                    clR1.BackColor = colorDialog1.Color;
                }
            }
            this.Invalidate();
        }

        private void clR2_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                selectedPalette = 6;
            }
            else
            {
                if (colorDialog1.ShowDialog() == DialogResult.OK)
                {
                    clR2.BackColor = colorDialog1.Color;
                }
            }
            this.Invalidate();
        }

        private void clR3_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                selectedPalette = 7;
            }
            else
            {
                if (colorDialog1.ShowDialog() == DialogResult.OK)
                {
                    clR3.BackColor = colorDialog1.Color;
                }
            }
            this.Invalidate();
        }

        private void clR4_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                selectedPalette = 8;
            }
            else
            {
                if (colorDialog1.ShowDialog() == DialogResult.OK)
                {
                    clR4.BackColor = colorDialog1.Color;
                }
            }
            this.Invalidate();
        }

        private void clL1_Paint(object sender, PaintEventArgs e)
        {
            if (selectedPalette == 1)
            {
                // Draw the outline

                if (((Control)sender).BackColor.R < 100 && ((Control)sender).BackColor.G < 100 && ((Control)sender).BackColor.B < 100)
                {
                    e.Graphics.DrawRectangle(Pens.White, 0, 0, ((Control)sender).Width - 1, ((Control)sender).Height - 1);
                }
                else
                {
                    e.Graphics.DrawRectangle(Pens.Black, 0, 0, ((Control)sender).Width - 1, ((Control)sender).Height - 1);
                }
            }
            else
            {
                clL1.BackColor = clL1.BackColor; // This refreshes it's current graphics!
            }
        }

        private void clL2_Paint(object sender, PaintEventArgs e)
        {
            if (selectedPalette == 2)
            {
                // Draw the outline

                if (((Control)sender).BackColor.R < 100 && ((Control)sender).BackColor.G < 100 && ((Control)sender).BackColor.B < 100)
                {
                    e.Graphics.DrawRectangle(Pens.White, 0, 0, ((Control)sender).Width - 1, ((Control)sender).Height - 1);
                }
                else
                {
                    e.Graphics.DrawRectangle(Pens.Black, 0, 0, ((Control)sender).Width - 1, ((Control)sender).Height - 1);
                }
            }
        }

        private void clL3_Paint(object sender, PaintEventArgs e)
        {
            if (selectedPalette == 3)
            {
                // Draw the outline

                if (((Control)sender).BackColor.R < 100 && ((Control)sender).BackColor.G < 100 && ((Control)sender).BackColor.B < 100)
                {
                    e.Graphics.DrawRectangle(Pens.White, 0, 0, ((Control)sender).Width - 1, ((Control)sender).Height - 1);
                }
                else
                {
                    e.Graphics.DrawRectangle(Pens.Black, 0, 0, ((Control)sender).Width - 1, ((Control)sender).Height - 1);
                }
            }
        }

        private void clL4_Paint(object sender, PaintEventArgs e)
        {
            if (selectedPalette == 4)
            {
                // Draw the outline

                if (((Control)sender).BackColor.R < 100 && ((Control)sender).BackColor.G < 100 && ((Control)sender).BackColor.B < 100)
                {
                    e.Graphics.DrawRectangle(Pens.White, 0, 0, ((Control)sender).Width - 1, ((Control)sender).Height - 1);
                }
                else
                {
                    e.Graphics.DrawRectangle(Pens.Black, 0, 0, ((Control)sender).Width - 1, ((Control)sender).Height - 1);
                }
            }
        }

        private void clR1_Paint(object sender, PaintEventArgs e)
        {
            if (selectedPalette == 5)
            {
                // Draw the outline

                if (((Control)sender).BackColor.R < 100 && ((Control)sender).BackColor.G < 100 && ((Control)sender).BackColor.B < 100)
                {
                    e.Graphics.DrawRectangle(Pens.White, 0, 0, ((Control)sender).Width - 1, ((Control)sender).Height - 1);
                }
                else
                {
                    e.Graphics.DrawRectangle(Pens.Black, 0, 0, ((Control)sender).Width - 1, ((Control)sender).Height - 1);
                }
            }
        }

        private void clR2_Paint(object sender, PaintEventArgs e)
        {
            if (selectedPalette == 6)
            {
                // Draw the outline

                if (((Control)sender).BackColor.R < 100 && ((Control)sender).BackColor.G < 100 && ((Control)sender).BackColor.B < 100)
                {
                    e.Graphics.DrawRectangle(Pens.White, 0, 0, ((Control)sender).Width - 1, ((Control)sender).Height - 1);
                }
                else
                {
                    e.Graphics.DrawRectangle(Pens.Black, 0, 0, ((Control)sender).Width - 1, ((Control)sender).Height - 1);
                }
            }
        }

        private void clR3_Paint(object sender, PaintEventArgs e)
        {
            if (selectedPalette == 7)
            {
                // Draw the outline

                if (((Control)sender).BackColor.R < 100 && ((Control)sender).BackColor.G < 100 && ((Control)sender).BackColor.B < 100)
                {
                    e.Graphics.DrawRectangle(Pens.White, 0, 0, ((Control)sender).Width - 1, ((Control)sender).Height - 1);
                }
                else
                {
                    e.Graphics.DrawRectangle(Pens.Black, 0, 0, ((Control)sender).Width - 1, ((Control)sender).Height - 1);
                }
            }
        }

        private void clR4_Paint(object sender, PaintEventArgs e)
        {
            if (selectedPalette == 8)
            {
                // Draw the outline

                if (((Control)sender).BackColor.R < 100 && ((Control)sender).BackColor.G < 100 && ((Control)sender).BackColor.B < 100)
                {
                    e.Graphics.DrawRectangle(Pens.White, 0, 0, ((Control)sender).Width - 1, ((Control)sender).Height - 1);
                }
                else
                {
                    e.Graphics.DrawRectangle(Pens.Black, 0, 0, ((Control)sender).Width - 1, ((Control)sender).Height - 1);
                }
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

        public Bitmap SafeFloodFill(Bitmap background, int x, int y, Color new_color)
        {
            Color old_color = background.GetPixel(x, y);
            Bitmap bm = new Bitmap(background.Width, background.Height);

            if (old_color != new_color)
            {
                Stack<Point> pts = new Stack<Point>(1000);
                pts.Push(new Point(x, y));
                background.SetPixel(x, y, new_color);
                bm.SetPixel(x, y, new_color);

                while (pts.Count > 0)
                {
                    Point pt = pts.Pop();
                    if (pt.X > 0) SafeCheckPoint(ref bm, ref background, ref pts, pt.X - 1, pt.Y, old_color, new_color);
                    if (pt.Y > 0) SafeCheckPoint(ref bm, ref background, ref pts, pt.X, pt.Y - 1, old_color, new_color);
                    if (pt.X < bm.Width - 1) SafeCheckPoint(ref bm, ref background, ref pts, pt.X + 1, pt.Y, old_color, new_color);
                    if (pt.Y < bm.Height - 1) SafeCheckPoint(ref bm, ref background, ref pts, pt.X, pt.Y + 1, old_color, new_color);
                }
            }

            GC.Collect();

            return bm;
        }

        public void SafeCheckPoint(ref Bitmap bm, ref Bitmap background, ref Stack<Point> pts, int x, int y, Color old_color, Color new_color)
        {
            Color clr = background.GetPixel(x, y);
            if (clr.Equals(old_color))
            {
                if (x < DrawingMin.X) DrawingMin.X = x;
                if (y < DrawingMin.Y) DrawingMin.Y = y;
                if (x > DrawingMax.X) DrawingMax.X = x;
                if (y > DrawingMax.Y) DrawingMax.Y = y;

                pts.Push(new Point(x, y));
                background.SetPixel(x, y, new_color);
                bm.SetPixel(x, y, new_color);
            }
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            ReloadImage();
        }
    }
}
