// ***********************************************************************
// Assembly         : ABPaint
// Author           : Alex
// Created          : 11-25-2017
//
// Last Modified By : Alex
// Last Modified On : 03-29-2018
// ***********************************************************************
// <copyright file="ColorBox.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ABPaint.RightMenu
{
    public partial class ColorBox : UserControl
    {
        public int ToSelect { get; set; }

        public ColorBox()
        {
            InitializeComponent();
        }

        private void ColorBox_Paint(object sender, PaintEventArgs e)
        {
            if (Program.MainForm != null)
                if (Core.SelectedPalette == ToSelect)
                {
                    // Draw the outline

                    if (BackColor.R < 100 && BackColor.G < 100 && BackColor.B < 100)
                        e.Graphics.DrawRectangle(Pens.White, 0, 0, ((Control)sender).Width - 1, ((Control)sender).Height - 1);
                    else
                        e.Graphics.DrawRectangle(Pens.Black, 0, 0, ((Control)sender).Width - 1, ((Control)sender).Height - 1);
                }
                else
                {
                    BackColor = BackColor; // This refreshes it's current graphics!
                }
        }

        private void ColorBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
                Core.SelectedPalette = ToSelect;
            else
            {
                if (Program.MainForm.colorDialog1.ShowDialog() == DialogResult.OK)
                {
                    BackColor = Program.MainForm.colorDialog1.Color;
                }
            }
            Program.MainForm.Refresh();
        }
    }
}
