namespace BxImage.Tools
{
    partial class BxImagectrl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnMakeDisk = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.cbFloppyDiskSize = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtHDSize = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cbHDType = new System.Windows.Forms.ComboBox();
            this.radioFD = new System.Windows.Forms.RadioButton();
            this.radioHD = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnMakeDisk
            // 
            this.btnMakeDisk.Location = new System.Drawing.Point(243, 128);
            this.btnMakeDisk.Name = "btnMakeDisk";
            this.btnMakeDisk.Size = new System.Drawing.Size(75, 23);
            this.btnMakeDisk.TabIndex = 3;
            this.btnMakeDisk.Text = "Make Disk";
            this.btnMakeDisk.UseVisualStyleBackColor = true;
            this.btnMakeDisk.Click += new System.EventHandler(this.btnMakeDisk_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(193, 85);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(21, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "KB";
            // 
            // cbFloppyDiskSize
            // 
            this.cbFloppyDiskSize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbFloppyDiskSize.FormattingEnabled = true;
            this.cbFloppyDiskSize.Items.AddRange(new object[] {
            "160",
            "180",
            "320",
            "360",
            "720",
            "1200",
            "1440",
            "1680",
            "1720",
            "2880"});
            this.cbFloppyDiskSize.Location = new System.Drawing.Point(115, 83);
            this.cbFloppyDiskSize.Name = "cbFloppyDiskSize";
            this.cbFloppyDiskSize.Size = new System.Drawing.Size(72, 21);
            this.cbFloppyDiskSize.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtHDSize);
            this.groupBox1.Controls.Add(this.btnMakeDisk);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cbHDType);
            this.groupBox1.Controls.Add(this.cbFloppyDiskSize);
            this.groupBox1.Controls.Add(this.radioFD);
            this.groupBox1.Controls.Add(this.radioHD);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(364, 172);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Disk Feature";
            // 
            // txtHDSize
            // 
            this.txtHDSize.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtHDSize.Location = new System.Drawing.Point(217, 35);
            this.txtHDSize.Name = "txtHDSize";
            this.txtHDSize.Size = new System.Drawing.Size(70, 20);
            this.txtHDSize.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(295, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "MB";
            // 
            // cbHDType
            // 
            this.cbHDType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cbHDType.FormattingEnabled = true;
            this.cbHDType.Items.AddRange(new object[] {
            "flat",
            "sparse",
            "growing"});
            this.cbHDType.Location = new System.Drawing.Point(115, 35);
            this.cbHDType.Name = "cbHDType";
            this.cbHDType.Size = new System.Drawing.Size(72, 21);
            this.cbHDType.TabIndex = 1;
            // 
            // radioFD
            // 
            this.radioFD.AutoSize = true;
            this.radioFD.Location = new System.Drawing.Point(15, 83);
            this.radioFD.Name = "radioFD";
            this.radioFD.Size = new System.Drawing.Size(80, 17);
            this.radioFD.TabIndex = 0;
            this.radioFD.Text = "Floppy Disk";
            this.radioFD.UseVisualStyleBackColor = true;
            this.radioFD.CheckedChanged += new System.EventHandler(this.radioFD_CheckedChanged);
            // 
            // radioHD
            // 
            this.radioHD.AutoSize = true;
            this.radioHD.Checked = true;
            this.radioHD.Location = new System.Drawing.Point(15, 35);
            this.radioHD.Name = "radioHD";
            this.radioHD.Size = new System.Drawing.Size(72, 17);
            this.radioHD.TabIndex = 0;
            this.radioHD.TabStop = true;
            this.radioHD.Text = "Hard Disk";
            this.radioHD.UseVisualStyleBackColor = true;
            this.radioHD.CheckedChanged += new System.EventHandler(this.radioHD_CheckedChanged);
            // 
            // BxImage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "BxImage";
            this.Size = new System.Drawing.Size(364, 172);
            this.Load += new System.EventHandler(this.BxImage_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnMakeDisk;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cbFloppyDiskSize;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioFD;
        private System.Windows.Forms.RadioButton radioHD;
        private System.Windows.Forms.TextBox txtHDSize;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbHDType;
    }
}
