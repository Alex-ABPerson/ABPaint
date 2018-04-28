// ***********************************************************************
// Assembly         : ABPaint
// Author           : Alex
// Created          : 11-25-2017
//
// Last Modified By : Alex
// Last Modified On : 03-16-2018
// ***********************************************************************
// <copyright file="Sizer.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Globalization;

namespace ABPaint
{
    public partial class Sizer : Form
    {
        private Size _returnSize;

        public Size ReturnSize
        {
            get
            {
                return _returnSize;
            }
            set
            {
                _returnSize = value;
            }
        }
        private bool _cancelled = true;

        public bool Cancelled
        {
            get
            {
                return _cancelled;
            }
            set
            {
                _cancelled = value;
            }
        }

        public Sizer()
        {
            InitializeComponent();
        }

        public void StartSizer(bool forNew, Size oldSize)
        {
            if (forNew)
            {
                // Change the UI for "new"
                Text = "New";
                button1.Text = "New";

                Size clipboardSize = new Size(0, 0);
                if (Clipboard.ContainsImage())
                {
                    Image getSize;
                    getSize = Clipboard.GetImage();

                    clipboardSize = getSize.Size;
                }

                txtWidth.Text = (clipboardSize.Width == 0) ? "800" : clipboardSize.Width.ToString(CultureInfo.CurrentCulture);
                txtHeight.Text = (clipboardSize.Width == 0) ? "600" : clipboardSize.Height.ToString(CultureInfo.CurrentCulture);               
            } else {
                Text = "Resize";
                button1.Text = "Resize";

                txtWidth.Text = oldSize.Width.ToString(CultureInfo.CurrentCulture) ?? "800";
                txtHeight.Text = oldSize.Height.ToString(CultureInfo.CurrentCulture) ?? "600";               
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ReturnSize = new Size(Convert.ToInt32(txtWidth.Text, CultureInfo.CurrentCulture), Convert.ToInt32(txtHeight.Text, CultureInfo.CurrentCulture));
            Cancelled = false;
            this.Close();
        }

        private void NumbersOnlyKeyPress(Object sender, KeyPressEventArgs e)
        {
            if (((TextBox)sender).Text.Length > 3)
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

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
