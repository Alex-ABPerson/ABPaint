// ***********************************************************************
// Assembly         : ABPaint
// Author           : Alex
// Created          : 12-02-2017
//
// Last Modified By : Alex
// Last Modified On : 02-24-2018
// ***********************************************************************
// <copyright file="About.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ABPaint.Windows
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void label2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.abworld.ml");
        }
    }
}
