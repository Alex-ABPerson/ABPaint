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
            // General WinForms code
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Initialize the core
            Core.Core.InitCore();

            // Store the main form in a variable for easy access.
            MainForm = new Form1();

            // Run the form
            Application.Run(MainForm);
        }
    }
}
