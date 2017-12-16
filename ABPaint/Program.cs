using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ABPaint
{
    public static class Program
    {
        public static Form1 mainForm = null;
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

            mainForm = new Form1();
            Application.Run(mainForm);
        }
    }
}
