using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ABPaint.Tools.Windows
{
    public partial class CropTool : Form
    {
        public CropTool()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Core.HandleApply();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Core.CancelTool();
        }

        private void CropTool_FormClosing(object sender, FormClosingEventArgs e)
        {
            Core.CancelTool();
        }
    }
}
