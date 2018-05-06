namespace ABPaint.Windows
{
    partial class About
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(About));
            this.lblTitle = new System.Windows.Forms.Label();
            this.txtLicense = new System.Windows.Forms.TextBox();
            this.txtGeneral = new System.Windows.Forms.TextBox();
            this.lblLicense = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.LinkLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 25F);
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(692, 39);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "ABPaint";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // txtLicense
            // 
            this.txtLicense.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLicense.BackColor = System.Drawing.SystemColors.ControlDark;
            this.txtLicense.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLicense.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.txtLicense.Location = new System.Drawing.Point(0, 146);
            this.txtLicense.Multiline = true;
            this.txtLicense.Name = "txtLicense";
            this.txtLicense.ReadOnly = true;
            this.txtLicense.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLicense.Size = new System.Drawing.Size(691, 235);
            this.txtLicense.TabIndex = 2;
            this.txtLicense.Text = resources.GetString("txtLicense.Text");
            // 
            // txtGeneral
            // 
            this.txtGeneral.BackColor = System.Drawing.SystemColors.ControlDark;
            this.txtGeneral.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtGeneral.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtGeneral.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.txtGeneral.Location = new System.Drawing.Point(0, 39);
            this.txtGeneral.Multiline = true;
            this.txtGeneral.Name = "txtGeneral";
            this.txtGeneral.ReadOnly = true;
            this.txtGeneral.Size = new System.Drawing.Size(692, 90);
            this.txtGeneral.TabIndex = 3;
            this.txtGeneral.Text = "ABPaint\r\nAlpha 2\r\nCopyright © 2018 \"ABWorld Team\". All rights reserved.\r\n\r\nThis p" +
    "roduct is licensed under the MIT license.\r\n";
            // 
            // lblLicense
            // 
            this.lblLicense.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lblLicense.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLicense.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.lblLicense.ForeColor = System.Drawing.Color.White;
            this.lblLicense.Location = new System.Drawing.Point(0, 129);
            this.lblLicense.MaximumSize = new System.Drawing.Size(0, 15);
            this.lblLicense.Name = "lblLicense";
            this.lblLicense.Size = new System.Drawing.Size(692, 15);
            this.lblLicense.TabIndex = 4;
            this.lblLicense.Text = "License";
            this.lblLicense.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 381);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(692, 15);
            this.tableLayoutPanel1.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.label2.LinkColor = System.Drawing.Color.Blue;
            this.label2.Location = new System.Drawing.Point(349, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(340, 15);
            this.label2.TabIndex = 2;
            this.label2.TabStop = true;
            this.label2.Text = "abworld.ml";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.label2_LinkClicked);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.5F);
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(340, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "ABWorld";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // About
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(692, 396);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.lblLicense);
            this.Controls.Add(this.txtLicense);
            this.Controls.Add(this.txtGeneral);
            this.Controls.Add(this.lblTitle);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "About";
            this.Text = "About ABPaint";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox txtLicense;
        private System.Windows.Forms.TextBox txtGeneral;
        private System.Windows.Forms.Label lblLicense;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel label2;
    }
}