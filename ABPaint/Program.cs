// ***********************************************************************
// Assembly         : ABPaint
// Author           : Alex
// Created          : 10-08-2017
//
// Last Modified By : Alex
// Last Modified On : 12-16-2017
// ***********************************************************************
// <copyright file="Program.cs" company="">
//     . All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Windows.Forms;

namespace ABPaint
{
    public static class Program
    {
        public static Form1 MainForm = null;
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainForm = new Form1();
            Application.Run(MainForm);
        }
    }
}
