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

            if (!System.IO.File.Exists("ABJson.GDISupport.dll"))
                System.IO.File.WriteAllBytes("ABJson.GDISupport.dll", Properties.Resources.ABJson_GDISupport);

            MainForm = new Form1();
            Application.Run(MainForm);
        }
    }
}
