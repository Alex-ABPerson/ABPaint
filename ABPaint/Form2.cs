// ***********************************************************************
// Assembly         : ABPaint
// Author           : Alex
// Created          : 12-31-2017
//
// Last Modified By : Alex
// Last Modified On : 02-23-2018
// ***********************************************************************
// <copyright file="Form2.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using ABPaint.Tools.Backend;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ABPaint
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            


        }

        private void Form2_Paint(object sender, PaintEventArgs e)
        {
            Point[] pointArray = new Point[]
            {
                new Point(10, 10),
                new Point(80, 10),
                new Point(100, 40),
                new Point(100, 80),
                new Point(80, 120),
                new Point(10, 120)
            };

            e.Graphics.DrawPolygon(new Pen(Color.Black, 3), pointArray);
            e.Graphics.DrawPolygon(new Pen(Color.Red), pointArray);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Hello world!");
        }
    }
}
