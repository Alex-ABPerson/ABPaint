using ABPaint.Elements;
using ABPaint.Objects;
using ABPaint.Tools.Backend;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using static ABPaint.Tools.Backend.SaveSystem;
using static ABPaint.Core;

namespace ABPaint
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();

            //canvaspre.PreviewKeyDown += new PreviewKeyDownEventHandler((sender, e) => {
            //    HandleKeyPress(e.KeyCode); });
            //this.KeyDown += new KeyEventHandler((sender, e) => {
            //    HandleKeyPress(e.KeyCode); });

            ReloadImage();
        }
        #region General Code

        private void Form1_Load(object sender, EventArgs e)
        {
            System.Windows.Automation.AutomationElement.FromHandle(Handle);

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

            DeselectElements();

            //if (canvaspre.Width + canvaspre.Left > appcenter.Width) appcenter.HorizontalScroll.Visible = true;
            //else appcenter.HorizontalScroll.Visible = false;

            //if (canvaspre.Height + canvaspre.Top > appcenter.Height) appcenter.VerticalScroll.Visible = true;
            //else appcenter.VerticalScroll.Visible = false;
        }

        

        private void canvaspre_MouseMove(object sender, MouseEventArgs e)
        {
            //Point mouseLoc = new Point((int)Math.Round((decimal)((e.X - (MagnificationLevel / 2)) / MagnificationLevel)), (int)Math.Round((decimal)((e.X - (MagnificationLevel / 2)) / MagnificationLevel)));

            if (!eventLock)
            {
                eventLock = true;
                HandleMouseMove(e);
            }
        }

        private void canvaspre_MouseDown(object sender, MouseEventArgs e)
        {
            //Point mouseLoc = new Point((int)Math.Round((decimal)((e.X - (MagnificationLevel / 2)) / MagnificationLevel)), (int)Math.Round((decimal)((e.X - (MagnificationLevel / 2)) / MagnificationLevel)));

            if (!eventLock)
            {
                eventLock = true;
                HandleMouseDown(e);              
            }
        }

        private void canvaspre_MouseUp(object sender, MouseEventArgs e)
        {
            if (!eventLock)
            {
                eventLock = true;
                HandleMouseUp(e);
            }
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

        private void timer1_Tick(object sender, EventArgs e)
        {
           PaintPreviewAsync();
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

                    if (ele.IsFilled) e.Graphics.FillRectangle(new SolidBrush(ele.FillColor), startPoint.X - widthamount + (borderSize / 2), startPoint.Y - heightamount + (borderSize / 2), Math.Abs(width), Math.Abs(height)); // Fill

                    e.Graphics.DrawRectangle(new Pen(ele.BorderColor, borderSize), startPoint.X - widthamount + (borderSize / 2), startPoint.Y - heightamount + (borderSize / 2), Math.Abs(width), Math.Abs(height));
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

                if (currentDrawingElement is Line) {
                    int thickness = Convert.ToInt32(string.IsNullOrEmpty(txtBThick.Text) ? "0" : txtBThick.Text);
                    e.Graphics.DrawLine(new Pen(clrNorm.BackColor, thickness), startPoint.X, startPoint.Y, mousePoint.X, mousePoint.Y);
                }

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
                    
                    e.Graphics.DrawRectangle(new Pen(Color.Gray, 3), selectedElement.X - 1, selectedElement.Y - 1, width + 1, height + 1);
                    e.Graphics.DrawRectangle(new Pen(Color.Blue), selectedElement.X - 1, selectedElement.Y - 1, width + 1, height + 1);

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
                                if (CornerSelected == 1) e.Graphics.FillEllipse(new SolidBrush(Color.Gray), selectedElement.X - (10 / MagnificationLevel), selectedElement.Y  - (10 / MagnificationLevel), 20 / MagnificationLevel, 20 / MagnificationLevel); else e.Graphics.DrawEllipse(new Pen(Color.Gray), selectedElement.X  - (10 / MagnificationLevel), selectedElement.Y  - (10 / MagnificationLevel), 20 / MagnificationLevel, 20 / MagnificationLevel);
                                if (CornerSelected == 2) e.Graphics.FillEllipse(new SolidBrush(Color.Gray), ((selectedElement.X ) + selectedElement.Width) - (10 / MagnificationLevel), selectedElement.Y  - (10 / MagnificationLevel), 20 / MagnificationLevel, 20 / MagnificationLevel); else e.Graphics.DrawEllipse(new Pen(Color.Gray), ((selectedElement.X ) + selectedElement.Width) - (10 / MagnificationLevel), selectedElement.Y  - (10 / MagnificationLevel), 20 / MagnificationLevel, 20 / MagnificationLevel);
                                if (CornerSelected == 3) e.Graphics.FillEllipse(new SolidBrush(Color.Gray), selectedElement.X  - (10 / MagnificationLevel), ((selectedElement.Y ) + selectedElement.Height) - (10 / MagnificationLevel), 20 / MagnificationLevel, 20 / MagnificationLevel); else e.Graphics.DrawEllipse(new Pen(Color.Gray), selectedElement.X  - (10 / MagnificationLevel), ((selectedElement.Y ) + selectedElement.Height) - (10 / MagnificationLevel), 20 / MagnificationLevel, 20 / MagnificationLevel);
                                if (CornerSelected == 4) e.Graphics.FillEllipse(new SolidBrush(Color.Gray), ((selectedElement.X ) + selectedElement.Width) - (10 / MagnificationLevel), ((selectedElement.Y ) + selectedElement.Height) - (10 / MagnificationLevel), 20 / MagnificationLevel, 20 / MagnificationLevel); else e.Graphics.DrawEllipse(new Pen(Color.Gray), ((selectedElement.X ) + selectedElement.Width) - (10 / MagnificationLevel), ((selectedElement.Y ) + selectedElement.Height) - (10 / MagnificationLevel), 20 / MagnificationLevel, 20 / MagnificationLevel);
                            }
                        }                        
                    }
                }
            }

            // Drag region overlay

            if (IsInDragRegion)
            {
                int heightamount = 0;
                int widthamount = 0;
                if (dragRegionSelect.Width < 0)
                    widthamount = Math.Abs(dragRegionSelect.Width);
                else
                    widthamount = 0;

                if (dragRegionSelect.Height < 0)
                    heightamount = Math.Abs(dragRegionSelect.Height);
                else
                    heightamount = 0;

                e.Graphics.DrawRectangle(new Pen(Color.Gray, 3), dragRegionSelect.X - widthamount, dragRegionSelect.Y - heightamount, Math.Abs(dragRegionSelect.Width) + 1, Math.Abs(dragRegionSelect.Height) + 1);
                e.Graphics.DrawRectangle(new Pen(Color.Green), dragRegionSelect.X - widthamount, dragRegionSelect.Y - heightamount, Math.Abs(dragRegionSelect.Width) + 1, Math.Abs(dragRegionSelect.Height) + 1);
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

        private void clrNorm_MouseClick(object sender, MouseEventArgs e)
        {
            clrNorm.BackColor = GetCurrentColor();

            if (selectedElement != null)
            {
                if (selectedElement is Elements.Brush) ((Elements.Brush)selectedElement).brushColor = clrNorm.BackColor;
                if (selectedElement is Elements.Pencil) ((Elements.Pencil)selectedElement).pencilColor = clrNorm.BackColor;
                if (selectedElement is Elements.Line) ((Elements.Line)selectedElement).color = clrNorm.BackColor;
                if (selectedElement is Elements.Fill) ((Elements.Fill)selectedElement).fillColor = clrNorm.BackColor;
                if (selectedElement is Elements.Text) ((Elements.Text)selectedElement).clr = clrNorm.BackColor;

               PaintPreviewAsync();
            }
        }

        private void clrFill_MouseClick(object sender, MouseEventArgs e)
        {
            clrFill.BackColor = GetCurrentColor();

            if (selectedElement != null)
            {
                if (selectedElement is RectangleE) ((RectangleE)selectedElement).FillColor = clrFill.BackColor;
                if (selectedElement is Ellipse) ((Ellipse)selectedElement).FillColor = clrFill.BackColor;

                PaintPreviewAsync();
            }
        }

        private void clrBord_MouseClick(object sender, MouseEventArgs e)
        {
            clrBord.BackColor = GetCurrentColor();

            if (selectedElement != null)
            {
                if (selectedElement is RectangleE) ((RectangleE)selectedElement).BorderColor = clrBord.BackColor;
                if (selectedElement is Ellipse) ((Ellipse)selectedElement).BorderColor = clrBord.BackColor;

                PaintPreviewAsync();
            }
        }

        private void txtBWidth_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBWidth.Text))
                txtBWidth.Text = "0";

            if (selectedElement != null)
            {
                if (selectedElement is RectangleE) ((RectangleE)selectedElement).BorderSize = Convert.ToInt32(string.IsNullOrEmpty(txtBWidth.Text) ? "0" : txtBWidth.Text);
                if (selectedElement is Ellipse) ((Ellipse)selectedElement).BorderSize = Convert.ToInt32(string.IsNullOrEmpty(txtBWidth.Text) ? "0" : txtBWidth.Text);

                PaintPreviewAsync();
            }
        }

        private void btnBold_Click(object sender, EventArgs e)
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

                PaintPreviewAsync();
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

        private void txtTText_TextChanged(object sender, EventArgs e)
        {
            if (selectedElement != null)
                ((Text)selectedElement).mainText = txtTText.Text;

            PaintPreviewAsync();
        }

        private void btnItl_Click(object sender, EventArgs e)
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

                PaintPreviewAsync();
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

        private void btnUline_Click(object sender, EventArgs e)
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

                PaintPreviewAsync();
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

            PaintPreviewAsync();
        }

        private void cmbSize_SelectedIndexChanged(object sender, EventArgs e)
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

            PaintPreviewAsync();
        }

        private void txtBThick_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBThick.Text))
                txtBThick.Text = "0";

            if (selectedElement != null)
            {
                if (selectedElement is Elements.Brush) ((Elements.Brush)selectedElement).Width = Convert.ToInt32(string.IsNullOrEmpty(txtBThick.Text) ? "0" : txtBThick.Text);
                if (selectedElement is Line)
                {
                    Line lineEle = ((Line)selectedElement);

                    lineEle.Thickness = Convert.ToInt32(string.IsNullOrEmpty(txtBThick.Text) ? "0" : txtBThick.Text);
                    
                    LineResizing.Resize(ref lineEle);
                }

                PaintPreview();
                canvaspre.Invalidate();
            }
        }
        #endregion

        private void movingRefresh_Tick(object sender, EventArgs e)
        {
            endImage = PaintPreview();
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
            HandleZoomOut();
        }

        private void zoomUp_Click(object sender, EventArgs e)
        {
            HandleZoomIn();
        }

        #region MenuStrip
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Sizer sz = new Sizer();
            sz.StartSizer(true, new Size());
            sz.ShowDialog();

            if (!sz.Cancelled)
            {
                savedata.imageSize = sz.returnSize;

                ReloadImage();

                savedata.imageElements = new List<Element>();

                PaintPreview();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialogOPEN.ShowDialog() == DialogResult.OK)
                LoadFile(openFileDialogOPEN.FileName);

            ReloadImage();
            PaintPreview();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialogSAVE.ShowDialog() == DialogResult.OK)
                SaveFile(saveFileDialogSAVE.FileName, true);

            ReloadImage();
            PaintPreview();
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialogIMPORT.ShowDialog() == DialogResult.OK)
                ImportFile(openFileDialogIMPORT.FileName);

            ReloadImage();
            PaintPreview();
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialogEXPORT.ShowDialog() == DialogResult.OK)
                ExportFile(saveFileDialogEXPORT.FileName);

            ReloadImage();
            PaintPreview();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (currentFile == "")
            {
                if (saveFileDialogSAVE.ShowDialog() == DialogResult.OK)
                    SaveFile(saveFileDialogSAVE.FileName, true);
            }
            else
                SaveFile(currentFile);

            ReloadImage();
            PaintPreview();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleCut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleCopy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandlePaste();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HandleDelete();
        }

        private void redrawToolStripMenuItem_Click(object sender, EventArgs e)
        {
            endImage = PaintPreview();
        }
        #endregion

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            HandleKeyPress(keyData);
            canvaspre.Invalidate();

            if ((currentTool == null || !currentTool.UseRegionDrag) && keyData != Keys.Enter)
                return base.ProcessCmdKey(ref msg, keyData);
            return true;
        }

        private void polygonTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Form2().Show();
        }

        private void cropToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UseTool(new Tools.CropTool());
        }
    }
}