// ***********************************************************************
// Assembly         : ABPaint
// Author           : Alex
// Created          : 10-08-2017
//
// Last Modified By : Alex
// Last Modified On : 03-29-2018
// ***********************************************************************
// <copyright file="Form1.cs" company="">
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
using System.Windows.Forms;

using static ABPaint.Tools.Backend.SaveSystem;
using static ABPaint.Core;
using System.Globalization;

namespace ABPaint
{
    public partial class Form1 : Form
    {
        private Bitmap _backBuffer;
        private bool paintLock;

        public Bitmap BackBuffer
        { 
            get { return _backBuffer; }
            set
            {
                _backBuffer = value;

            }
        } // The back-buffer

        public Form1()
        {
            InitializeComponent();

            //canvaspre.PreviewKeyDown += new PreviewKeyDownEventHandler((sender, e) => {
            //    HandleKeyPress(e.KeyCode); });
            //this.KeyDown += new KeyEventHandler((sender, e) => {
            //    HandleKeyPress(e.KeyCode); });

            ReloadImage();

            CanvaspreG = canvaspre.CreateGraphics();
            BackBufferG = Graphics.FromImage(BackBuffer);

            canvaspre.Image = new Bitmap(CurrentSave.ImageSize.Width, CurrentSave.ImageSize.Height);
        }
        #region General Code

        private void Form1_Load(object sender, EventArgs e)
        {
            System.Windows.Automation.AutomationElement.FromHandle(Handle);

            properties.Hide();
            toolCursorS.Hide();

            System.Drawing.Text.InstalledFontCollection allFonts = new System.Drawing.Text.InstalledFontCollection();

            // Get an array of the system's font families.
            FontFamily[] fontFamilies = allFonts.Families;

            // Display the font families.
            foreach (FontFamily myFont in fontFamilies)
            {
                cmbFont.Items.Add(myFont.Name);
                //font_family

            }

            CurrentSave.ImageElements = new List<Element>();
            EndImage = new Bitmap(CurrentSave.ImageSize.Width, CurrentSave.ImageSize.Height);
        }

        public void SelectTool(ref PictureBox toSelect)
        {
            toSelect.BackColor = Color.Black;
                if (toSelect.Tag != null && toSelect.Tag.ToString() != "selected") // If it isn't selected already
                    toSelect.Image = ImageInverting.InvertImage((Bitmap)toSelect.Image);
            toSelect.Tag = "selected";
        }

        private void NumbersOnlyKeyPress(Object sender, KeyPressEventArgs e)
        {
            if (((dynamic)sender).Text.Length > 2)
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

        public void DESelectTool(ref PictureBox toSelect)
        {
            toSelect.BackColor = Color.White;
            if (toSelect.Tag != null && toSelect.Tag.ToString() == "selected") // If it isn't selected already
                    toSelect.Image = ImageInverting.InvertImage((Bitmap)toSelect.Image);
            toSelect.Tag = "";
        }

        private void timgCursorN_Click(object sender, EventArgs e)
        {
            // Select this tool

            SelectTool(ref timgCursorN);
            DESelectTool(ref timgCursorS);
            DESelectTool(ref timgPencil);
            DESelectTool(ref timgBrush);
            DESelectTool(ref timgRect);
            DESelectTool(ref timgElli);
            DESelectTool(ref timgLine);
            DESelectTool(ref timgFill);
            DESelectTool(ref timgText);
            SelectedTool = 0;

            ShowProperties("Selection Tool - Nothing Selected!", false, false, false, false, false, false, GetCurrentColor(), GetCurrentColor());
        }

        private void timgCursorS_Click(object sender, EventArgs e)
        {
            DESelectTool(ref timgCursorN);
            SelectTool(ref timgCursorS);
            DESelectTool(ref timgPencil);
            DESelectTool(ref timgBrush);
            DESelectTool(ref timgRect);
            DESelectTool(ref timgElli);
            DESelectTool(ref timgLine);
            DESelectTool(ref timgFill);
            DESelectTool(ref timgText);

            SelectedTool = Tool.BitmapSelection;
        }

        private void timgPencil_Click(object sender, EventArgs e)
        {
            SelectedElement = null;

            DESelectTool(ref timgCursorN);
            DESelectTool(ref timgCursorS);
            SelectTool(ref timgPencil);
            DESelectTool(ref timgBrush);
            DESelectTool(ref timgRect);
            DESelectTool(ref timgElli);
            DESelectTool(ref timgLine);
            DESelectTool(ref timgFill);
            DESelectTool(ref timgText);

            SelectedTool = Tool.Pencil;

            ShowProperties("Pencil Tool Settings", false, false, true, false, false, false, GetCurrentColor(), GetCurrentColor());
        }

        private void timgBrush_Click(object sender, EventArgs e)
        {
            SelectedElement = null;

            DESelectTool(ref timgCursorN);
            DESelectTool(ref timgCursorS);
            DESelectTool(ref timgPencil);
            SelectTool(ref timgBrush);
            DESelectTool(ref timgRect);
            DESelectTool(ref timgElli);
            DESelectTool(ref timgLine);
            DESelectTool(ref timgFill);
            DESelectTool(ref timgText);

            SelectedTool = Tool.Brush;

            ShowProperties("Brush Tool Settings", false, false, true, false, true, false, GetCurrentColor(), GetCurrentColor());
        }

        private void timgRect_Click(object sender, EventArgs e)
        {
            SelectedElement = null;

            DESelectTool(ref timgCursorN);
            DESelectTool(ref timgCursorS);
            DESelectTool(ref timgPencil);
            DESelectTool(ref timgBrush);
            SelectTool(ref timgRect);
            DESelectTool(ref timgElli);
            DESelectTool(ref timgLine);
            DESelectTool(ref timgFill);
            DESelectTool(ref timgText);

            SelectedTool = Tool.Rectangle;

            ShowProperties("Rectangle Tool Settings", true, true, false, true, false, false, GetCurrentColor(), GetCurrentColor());
        }

        private void timgElli_Click(object sender, EventArgs e)
        {
            SelectedElement = null;

            DESelectTool(ref timgCursorN);
            DESelectTool(ref timgCursorS);
            DESelectTool(ref timgPencil);
            DESelectTool(ref timgBrush);
            DESelectTool(ref timgRect);
            SelectTool(ref timgElli);
            DESelectTool(ref timgLine);
            DESelectTool(ref timgFill);
            DESelectTool(ref timgText);

            SelectedTool = Tool.Ellipse;

            ShowProperties("Ellipse Tool Settings", true, true, false, true, false, false, GetCurrentColor(), GetCurrentColor());
        }

        private void timgLine_Click(object sender, EventArgs e)
        {
            SelectedElement = null;

            DESelectTool(ref timgCursorN);
            DESelectTool(ref timgCursorS);
            DESelectTool(ref timgPencil);
            DESelectTool(ref timgBrush);
            DESelectTool(ref timgRect);
            DESelectTool(ref timgElli);
            SelectTool(ref timgLine);
            DESelectTool(ref timgFill);
            DESelectTool(ref timgText);

            SelectedTool = Tool.Line;

            ShowProperties("Line Tool Settings", false, false, true, false, true, false, GetCurrentColor(), GetCurrentColor());
        }

        private void timgFill_Click(object sender, EventArgs e)
        {
            SelectedElement = null;

            DESelectTool(ref timgCursorN);
            DESelectTool(ref timgCursorS);
            DESelectTool(ref timgPencil);
            DESelectTool(ref timgBrush);
            DESelectTool(ref timgRect);
            DESelectTool(ref timgElli);
            DESelectTool(ref timgLine);
            SelectTool(ref timgFill);
            DESelectTool(ref timgText);

            SelectedTool = Tool.Fill;

            ShowProperties("Fill Tool Settings", false, false, true, false, false, false, GetCurrentColor(), GetCurrentColor());
        }

        private void timgText_Click(object sender, EventArgs e)
        {
            SelectedElement = null;

            DESelectTool(ref timgCursorN);
            DESelectTool(ref timgCursorS);
            DESelectTool(ref timgPencil);
            DESelectTool(ref timgBrush);
            DESelectTool(ref timgRect);
            DESelectTool(ref timgElli);
            DESelectTool(ref timgLine);
            DESelectTool(ref timgFill);
            SelectTool(ref timgText);

            SelectedTool = Tool.Text;

            ShowProperties("Text Tool Settings", false, false, true, false, false, true, GetCurrentColor(), GetCurrentColor(), "");
        }
        #endregion

        /// <summary>
        /// Resizes and repositions the canvaspre.
        /// </summary>
        public void ReloadImage()
        {
            Point oldScrollOffset = appcenter.AutoScrollOffset;
        
            canvaspre.Width = (CurrentSave.ImageSize.Width * MagnificationLevel);
            canvaspre.Height = (CurrentSave.ImageSize.Height * MagnificationLevel);

            canvaspre.Left = 25;
            canvaspre.Top = 25;

            if (canvaspre.Width < appcenter.Width)
                canvaspre.Left = (appcenter.Width / 2 - canvaspre.Width / 2);

            if (canvaspre.Height < appcenter.Height)
                canvaspre.Top = (appcenter.Height / 2 - canvaspre.Height / 2);

            canvaspre.Invalidate();

            if (MagnificationLevel > OldMagnificationLevel)
                appcenter.AutoScrollOffset = new Point((oldScrollOffset.X * (MagnificationLevel - OldMagnificationLevel)) + (25 * MagnificationLevel), (oldScrollOffset.Y * (MagnificationLevel - OldMagnificationLevel)) + (25 * MagnificationLevel));
            else
                appcenter.AutoScrollOffset = new Point((oldScrollOffset.X / (MagnificationLevel - OldMagnificationLevel)) + (25 * MagnificationLevel), (oldScrollOffset.Y / (MagnificationLevel - OldMagnificationLevel)) + (25 * MagnificationLevel));

            appcenter.AutoScrollMinSize = new Size(canvaspre.Width + 50, canvaspre.Height + 50);

            BackBuffer = new Bitmap(canvaspre.Width, canvaspre.Height);

            //if (canvaspre.Width + canvaspre.Left > appcenter.Width) appcenter.HorizontalScroll.Visible = true;
            //else appcenter.HorizontalScroll.Visible = false;

                //if (canvaspre.Height + canvaspre.Top > appcenter.Height) appcenter.VerticalScroll.Visible = true;
                //else appcenter.VerticalScroll.Visible = false;
        }

        public void UpdateCanvaspreWithBackBuffer()
        {
            CanvaspreG.DrawImage(BackBuffer, 0, 0);
        }

        private void canvaspre_MouseMove(object sender, MouseEventArgs e)
        {
            //Point mouseLoc = new Point((int)Math.Round((decimal)((e.X - (MagnificationLevel / 2)) / MagnificationLevel)), (int)Math.Round((decimal)((e.X - (MagnificationLevel / 2)) / MagnificationLevel)));

            if (!MouseMoveLock)
            {
                MouseMoveLock = true;
                HandleMouseMove(e);
                MouseMoveLock = false;
            }
        }

        private void canvaspre_MouseDown(object sender, MouseEventArgs e)
        {
            //Point mouseLoc = new Point((int)Math.Round((decimal)((e.X - (MagnificationLevel / 2)) / MagnificationLevel)), (int)Math.Round((decimal)((e.X - (MagnificationLevel / 2)) / MagnificationLevel)));

            if (!MouseDownLock)
            {
                MouseDownLock = true;
                HandleMouseDown(e);
                MouseDownLock = false;
            }
        }

        private void canvaspre_MouseUp(object sender, MouseEventArgs e)
        {
            if (!MouseUpLock)
            {
                MouseUpLock = true;
                HandleMouseUp(e);
                MouseUpLock = false;
            }
        }

        /// <summary>
        /// Shows the properties panel with the specified options.
        /// </summary>
        /// <param name="title">The text to put as the label.</param>
        /// <param name="showFColor">Show the Fill Color.</param>
        /// <param name="showBColor">Show the Border Color.</param>
        /// <param name="showColor">Show the Color.</param>
        /// <param name="showBWidth">Show the Border Size.</param>
        /// <param name="showThickness">Show the Thickness.</param>
        /// <param name="showText">Show the Text tools.</param>
        /// <param name="objectColor">The color of this object (if this is a selection)</param>
        /// <param name="text">The text to put into the "Text" box.</param>
        /// <param name="fnt">The font to put into the "Font" box.</param>
        public void ShowProperties(string title, bool showFColor, bool showBColor, bool showColor, bool showBWidth, bool showThickness, bool showText, Color objectColor, Color borderColor = new Color(), string text = "", Font fnt = null)
        {
            properties.Show();

            propertiesLbl.Text = title;

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

                clrBord.BackColor = borderColor;
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

                txtTText.Text = text;
                if (fnt != null)
                {
                    cmbFont.Text = fnt.FontFamily.Name;
                    cmbSize.Text = fnt.Size.ToString(CultureInfo.CurrentCulture);

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
            HandlePaint();
        }

        private void canvaspre_Paint(object sender, PaintEventArgs e)
        {
            if (!paintLock)
            {
                paintLock = true;
                //if (!actionLock)
                //    if (!fillLock)
                //        if (!editLock)
                //            try { e.Graphics.DrawImage(endImage, 0, 0); } catch { MessageBox.Show("Canvaspre paint error! Check locks!"); } // The try + catch is only there in case of a case where I haven't checked my locks.

                //try { e.Graphics.DrawImage(canvaspre.Image, 0, 0, canvaspre.Width, canvaspre.Height); } catch { }
                //if (canvaspre.Image != null) canvaspre.Image = null;

                // This is to preview what you are drawing!

                if (MouseDownOnCanvas)
                {
                    if (CurrentDrawingElement is Pencil)
                        e.Graphics.DrawPath(new Pen(clrNorm.BackColor), Grph);

                    if (CurrentDrawingElement is Elements.Brush asBrush)
                        BrushDrawing.ChangeGraphicsColor(asBrush.BrushPoint, e.Graphics, clrNorm.BackColor);

                    if (CurrentDrawingElement is RectangleE asRectangle)
                    {
                        // Now let's draw this rectangle!

                        int width = (MousePoint.X - StartPoint.X);
                        int height = (MousePoint.Y - StartPoint.Y);

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

                        //if (width < 0) CurrentDrawingElement.Width = 1;
                        //if (height < 0) CurrentDrawingElement.Height = 1;

                        int borderSize = Convert.ToInt32(string.IsNullOrEmpty(txtBWidth.Text) ? "0" : txtBWidth.Text);

                        if (asRectangle.IsFilled) e.Graphics.FillRectangle(new SolidBrush(asRectangle.FillColor), StartPoint.X - widthamount + (borderSize / 2), StartPoint.Y - heightamount + (borderSize / 2), Math.Abs(width), Math.Abs(height)); // Fill

                        e.Graphics.DrawRectangle(new Pen(asRectangle.BorderColor, borderSize), StartPoint.X - widthamount + (borderSize / 2), StartPoint.Y - heightamount + (borderSize / 2), Math.Abs(width), Math.Abs(height));
                        //e.Graphics.FillRectangle(new SolidBrush(ele.borderColor), DrawingMin.X, DrawingMin.Y, ele.BorderSize, height); // Left border
                        //e.Graphics.FillRectangle(new SolidBrush(ele.borderColor), DrawingMin.X, DrawingMin.Y, width, ele.BorderSize); // Top border
                        //e.Graphics.FillRectangle(new SolidBrush(ele.borderColor), (ele.Width - ele.BorderSize) + DrawingMin.X, DrawingMin.Y, ele.BorderSize, Height); // Right border
                        //e.Graphics.FillRectangle(new SolidBrush(ele.borderColor), DrawingMin.X, (ele.Height - ele.BorderSize) + DrawingMin.Y, ele.Width, ele.BorderSize); // Bottom border
                    }

                    if (CurrentDrawingElement is Ellipse asEllipse)
                    {
                        // Now let's draw this ellipse! and yes this is practically the same code as the rectangle one - both of them use the same code for things

                        int width = (MousePoint.X - StartPoint.X);
                        int height = (MousePoint.Y - StartPoint.Y);

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

                        //if (width < 0) CurrentDrawingElement.Width = 1;
                        //if (height < 0) CurrentDrawingElement.Height = 1;

                        int borderSize = Convert.ToInt32(string.IsNullOrEmpty(txtBWidth.Text) ? "0" : txtBWidth.Text);
                        if (asEllipse.IsFilled) e.Graphics.FillEllipse(new SolidBrush(asEllipse.FillColor), StartPoint.X - widthamount + (borderSize / 2), StartPoint.Y - heightamount + (borderSize / 2), Math.Abs(width), Math.Abs(height)); // Fill

                        e.Graphics.DrawEllipse(new Pen(asEllipse.BorderColor, borderSize), StartPoint.X - widthamount + (borderSize / 2), StartPoint.Y - heightamount + (borderSize / 2), Math.Abs(width), Math.Abs(height));
                        //e.Graphics.FillRectangle(new SolidBrush(ele.borderColor), DrawingMin.X, DrawingMin.Y, ele.BorderSize, height); // Left border
                        //e.Graphics.FillRectangle(new SolidBrush(ele.borderColor), DrawingMin.X, DrawingMin.Y, width, ele.BorderSize); // Top border
                        //e.Graphics.FillRectangle(new SolidBrush(ele.borderColor), (ele.Width - ele.BorderSize) + DrawingMin.X, DrawingMin.Y, ele.BorderSize, Height); // Right border
                        //e.Graphics.FillRectangle(new SolidBrush(ele.borderColor), DrawingMin.X, (ele.Height - ele.BorderSize) + DrawingMin.Y, ele.Width, ele.BorderSize); // Bottom border
                    }

                    if (CurrentDrawingElement is Line)
                    {
                        int thickness = (txtBThick.Text.Length > 0) ? int.Parse(txtBThick.Text, CultureInfo.CurrentCulture) : 0;
                        e.Graphics.DrawLine(new Pen(clrNorm.BackColor, thickness), StartPoint.X, StartPoint.Y, MousePoint.X, MousePoint.Y);
                    }

                    if (CurrentDrawingElement is Text txt)
                        e.Graphics.DrawString(txt.MainText, txt.Fnt, new SolidBrush(txt.Clr), MousePoint.X, MousePoint.Y);
                }

                // ...or to draw the overlay of the selection tool...

                if (SelectedElement != null)
                {
                    int width = Math.Abs(SelectedElement.Width);
                    int height = Math.Abs(SelectedElement.Height);

                    e.Graphics.DrawRectangle(new Pen(Color.Gray, 3), SelectedElement.X - 1, SelectedElement.Y - 1, width + 1, height + 1);
                    e.Graphics.DrawRectangle(new Pen(Color.Blue), SelectedElement.X - 1, SelectedElement.Y - 1, width + 1, height + 1);

                    // The points for scaling

                    if (IsOnSelection)
                    {
                        if (SelectedElement is Line lineEle)
                        {
                            if (CornerSelected == Corner.TopLeft) e.Graphics.FillEllipse(new SolidBrush(Color.Gray), lineEle.StartPoint.X + SelectedElement.X - 10, lineEle.StartPoint.Y + SelectedElement.Y - 10, 20, 20);
                            else e.Graphics.DrawEllipse(new Pen(Color.Gray), lineEle.StartPoint.X + SelectedElement.X - 10, lineEle.StartPoint.Y + SelectedElement.Y - 10, 20, 20);
                            if (CornerSelected == Corner.TopRight) e.Graphics.FillEllipse(new SolidBrush(Color.Gray), lineEle.EndPoint.X + SelectedElement.X - 10, lineEle.EndPoint.Y + SelectedElement.Y - 10, 20, 20);
                            else e.Graphics.DrawEllipse(new Pen(Color.Gray), lineEle.EndPoint.X + SelectedElement.X - 10, lineEle.EndPoint.Y + SelectedElement.Y - 10, 20, 20);
                        }
                        else
                        {
                            if (!(SelectedElement is Pencil) && !(SelectedElement is Elements.Brush) && !(SelectedElement is Fill))
                            {
                                if (CornerSelected == Corner.TopLeft) e.Graphics.FillEllipse(new SolidBrush(Color.Gray), SelectedElement.X - 10, SelectedElement.Y - 10, 20, 20);
                                else e.Graphics.DrawEllipse(new Pen(Color.Gray), SelectedElement.X - 10, SelectedElement.Y - 10, 20, 20);
                                if (CornerSelected == Corner.TopRight) e.Graphics.FillEllipse(new SolidBrush(Color.Gray), ((SelectedElement.X) + SelectedElement.Width) - 10, SelectedElement.Y - 10, 20, 20);
                                else e.Graphics.DrawEllipse(new Pen(Color.Gray), ((SelectedElement.X) + SelectedElement.Width) - 10, SelectedElement.Y - 10, 20, 20);
                                if (CornerSelected == Corner.BottomLeft) e.Graphics.FillEllipse(new SolidBrush(Color.Gray), SelectedElement.X - 10, ((SelectedElement.Y) + SelectedElement.Height) - 10, 20, 20);
                                else e.Graphics.DrawEllipse(new Pen(Color.Gray), SelectedElement.X - 10, ((SelectedElement.Y) + SelectedElement.Height) - 10, 20, 20);
                                if (CornerSelected == Corner.BottomRight) e.Graphics.FillEllipse(new SolidBrush(Color.Gray), ((SelectedElement.X) + SelectedElement.Width) - 10, ((SelectedElement.Y) + SelectedElement.Height) - 10, 20, 20);
                                else e.Graphics.DrawEllipse(new Pen(Color.Gray), ((SelectedElement.X) + SelectedElement.Width) - 10, ((SelectedElement.Y) + SelectedElement.Height) - 10, 20, 20);
                            }
                        }
                    }
                }

                // ...or even for the drag region overlay...

                if (IsInDragRegion)
                {
                    int heightamount = 0;
                    int widthamount = 0;
                    if (DragRegionSelect.Width < 0)
                        widthamount = Math.Abs(DragRegionSelect.Width);
                    else
                        widthamount = 0;

                    if (DragRegionSelect.Height < 0)
                        heightamount = Math.Abs(DragRegionSelect.Height);
                    else
                        heightamount = 0;

                    e.Graphics.DrawRectangle(new Pen(Color.Gray, 3), DragRegionSelect.X - widthamount, DragRegionSelect.Y - heightamount, Math.Abs(DragRegionSelect.Width) + 1, Math.Abs(DragRegionSelect.Height) + 1);
                    e.Graphics.DrawRectangle(new Pen(Color.Green), DragRegionSelect.X - widthamount, DragRegionSelect.Y - heightamount, Math.Abs(DragRegionSelect.Width) + 1, Math.Abs(DragRegionSelect.Height) + 1);
                }

                // ...or for just drawing the element that's moving.

                if (RedrawSelectedElementOnly)
                {
                    e.Graphics.DrawImage(BeforeMove, 0, 0);
                    SelectedElement.ProcessImage(e.Graphics);
                }

                paintLock = false;
            }

            
        }
        
        #region Properties Panel
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // Redraw the available colors

            if (SelectedPalette == 0)
                SelectedPalette = 1;

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

            switch (SelectedPalette)
            {               
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
                default:
                    ret = Color.Black;
                    break;
            }

            return ret;
        }

        private void clrNorm_MouseClick(object sender, MouseEventArgs e)
        {
            clrNorm.BackColor = GetCurrentColor();

            if (SelectedElement != null)
            {
                if (SelectedElement is Elements.Brush brush) brush.BrushColor = clrNorm.BackColor;
                if (SelectedElement is Elements.Pencil pencil) pencil.PencilColor = clrNorm.BackColor;
                if (SelectedElement is Elements.Line line) line.Color = clrNorm.BackColor;
                if (SelectedElement is Elements.Fill fill) fill.FillColor = clrNorm.BackColor;
                if (SelectedElement is Elements.Text text) text.Clr = clrNorm.BackColor;

               PaintPreview();
            }
        }

        private void clrFill_MouseClick(object sender, MouseEventArgs e)
        {
            clrFill.BackColor = GetCurrentColor();

            if (SelectedElement != null)
            {
                if (SelectedElement is RectangleE rect) rect.FillColor = clrFill.BackColor;
                if (SelectedElement is Ellipse elli) elli.FillColor = clrFill.BackColor;

                PaintPreview();
            }
        }

        private void clrBord_MouseClick(object sender, MouseEventArgs e)
        {
            clrBord.BackColor = GetCurrentColor();

            if (SelectedElement != null)
            {
                if (SelectedElement is RectangleE rect) rect.BorderColor = clrBord.BackColor;
                if (SelectedElement is Ellipse elli) elli.BorderColor = clrBord.BackColor;

                PaintPreview();
            }
        }

        private void txtBWidth_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBWidth.Text))
                txtBWidth.Text = "0";

            if (SelectedElement != null)
            {
                if (SelectedElement is RectangleE rect) rect.BorderSize = (txtBWidth.Text.Length > 0) ? int.Parse(txtBWidth.Text, CultureInfo.CurrentCulture) : 0;
                if (SelectedElement is Ellipse elli) elli.BorderSize = (txtBWidth.Text.Length > 0) ? int.Parse(txtBWidth.Text, CultureInfo.CurrentCulture) : 0;

                PaintPreview();
            }
        }

        private void btnBold_Click(object sender, EventArgs e)
        {
            if (SelectedElement != null)
            {      
                if (SelectedElement is Text txt)
                {
                    Font currentFont = txt.Fnt;
                    if (txt.Fnt.Bold)
                        txt.Fnt = new Font(currentFont.FontFamily, currentFont.Size, currentFont.Style & ~FontStyle.Bold);
                    else
                        txt.Fnt = new Font(currentFont.FontFamily, currentFont.Size, FontStyle.Bold | currentFont.Style);
                }

                PaintPreview();
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
            if (SelectedElement != null)
                ((Text)SelectedElement).MainText = txtTText.Text;

            PaintPreview();
        }

        private void btnItl_Click(object sender, EventArgs e)
        {
            if (SelectedElement != null)
            {           
                if (SelectedElement is Text txt)
                {
                    Font currentFont = txt.Fnt;
                    if (txt.Fnt.Italic)
                        txt.Fnt = new Font(currentFont.FontFamily, currentFont.Size, currentFont.Style & ~FontStyle.Italic);
                    else
                        txt.Fnt = new Font(currentFont.FontFamily, currentFont.Size, FontStyle.Italic | currentFont.Style);
                }

                PaintPreview();
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
            if (SelectedElement != null)
            {        
                if (SelectedElement is Text txt)
                {
                    Font currentFont = txt.Fnt;
                    if (txt.Fnt.Underline)
                        txt.Fnt = new Font(currentFont.FontFamily, currentFont.Size, currentFont.Style & ~FontStyle.Underline);
                    else
                        txt.Fnt = new Font(currentFont.FontFamily, currentFont.Size, FontStyle.Underline | currentFont.Style);
                }

                PaintPreview();
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
            if (SelectedElement != null)
                if (SelectedElement is Text txt)
                    try
                    {
                        txt.Fnt = new Font(cmbFont.Text, txt.Fnt.Size, txt.Fnt.Style);
                    }
                    catch (ArgumentException ex)
                    {
                        cmbFont.Text = "Microsoft Sans Serif";
                        txt.Fnt = new Font(cmbFont.Text, txt.Fnt.Size, txt.Fnt.Style);
                        Console.WriteLine("AN EXCEPTION OCCURED: " + ex.Message);
                    }

            PaintPreview();
        }

        private void cmbSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            HandleChangeTextSize();
        }

        private void txtBThick_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBThick.Text))
                txtBThick.Text = "0";

            if (SelectedElement != null)
            {
                if (SelectedElement is Elements.Brush asBrush) asBrush.Width = (txtBThick.Text.Length > 0) ? int.Parse(txtBThick.Text, CultureInfo.CurrentCulture) : 0;
                if (SelectedElement is Line asLine)
                {
                    asLine.Thickness = (txtBThick.Text.Length > 0) ? int.Parse(txtBThick.Text, CultureInfo.CurrentCulture) : 0;
                    
                    LineResizing.Resize(ref asLine);
                }

                PaintPreview();
                canvaspre.Invalidate();
            }
        }
        #endregion

        private void movingRefresh_Tick(object sender, EventArgs e)
        {
            canvaspre.Invalidate();
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
                CurrentSave.ImageSize = sz.ReturnSize;

                ReloadImage();

                CurrentSave.ImageElements = new List<Element>();

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
            if (string.IsNullOrEmpty(CurrentFile))
            {
                if (saveFileDialogSAVE.ShowDialog() == DialogResult.OK)
                    SaveFile(saveFileDialogSAVE.FileName, true);
            }
            else
                SaveFile(CurrentFile);

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
            EndImage = PaintPreview();
        }
        #endregion

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            HandleKeyPress(keyData);
            canvaspre.Invalidate();

            if ((CurrentTool == null || !CurrentTool.UseRegionDrag) && keyData != Keys.Enter)
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

        public void HandleChangeTextSize()
        {
            if (SelectedElement != null)
                if (SelectedElement is Text txt)
                    try
                    {
                        txt.Fnt = new Font(txt.Fnt.FontFamily, (float.Parse(cmbSize.Text, CultureInfo.CurrentCulture) > 999) ? float.Parse(cmbSize.Text, CultureInfo.CurrentCulture) : 12, txt.Fnt.Style);
                        SizeF realSize = Elements.Text.MeasureText(txt.MainText, txt.Fnt);
                        SelectedElement.Width = Convert.ToInt32(realSize.Width);
                        SelectedElement.Height = Convert.ToInt32(realSize.Height);
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine(ex.Message);
                        cmbSize.Text = "12";
                        txt.Fnt = new Font(txt.Fnt.FontFamily, float.Parse(cmbSize.Text, CultureInfo.CurrentCulture), txt.Fnt.Style);
                        SizeF realSize = Elements.Text.MeasureText(txt.MainText, txt.Fnt);
                        SelectedElement.Width = Convert.ToInt32(realSize.Width);
                        SelectedElement.Height = Convert.ToInt32(realSize.Height);
                    }

            PaintPreview();
        }

        private void cmbSize_TextUpdate(object sender, EventArgs e)
        {
            HandleChangeTextSize();
        }
    }
}