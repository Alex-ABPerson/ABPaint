using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ABPaint
{
    public partial class Sizer : Form
    {
        public Size returnSize;
        public bool Cancelled = true;

        public Sizer()
        {
            InitializeComponent();
        }

        public void StartSizer(bool ForNew, Size oldSize)
        {
            if (ForNew)
            {
                Size clipboardSize = new Size(0, 0);
                if (Clipboard.ContainsImage())
                {
                    Image getSize;
                    getSize = Clipboard.GetImage();

                    clipboardSize = getSize.Size;
                }

                txtWidth.Text = clipboardSize.Width.ToString() ?? 800.ToString();
                txtHeight.Text = clipboardSize.Height.ToString() ?? 600.ToString();

                button1.Text = "New";
            } else {
                txtWidth.Text = oldSize.Width.ToString() ?? 800.ToString();
                txtHeight.Text = oldSize.Height.ToString() ?? 600.ToString();

                button1.Text = "Resize";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            returnSize = new Size(Convert.ToInt32(txtWidth.Text), Convert.ToInt32(txtHeight.Text));
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
